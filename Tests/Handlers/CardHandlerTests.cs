using Gridly.Command;
using Gridly.Dtos;
using Gridly.Models;
using Gridly.Repositories;
using Gridly.Services;
using Gridly.Tests.Infrastructure;
using BackendCardHandler = Gridly.Handlers.CardHandler;

namespace Gridly.Tests.Handlers;

public class CardHandlerTests
{
    [Fact]
    public async Task HandleDelete_WhenDeletedCardWasLastInRow_DeletesEmptyRowAndRenumbersRows()
    {
        var columnRowRepository = new FakeColumnRowRepository
        {
            Rows =
            [
                new ColumnRowModel { Id = 1, RowPosition = 1, Cards = [] },
                new ColumnRowModel { Id = 2, RowPosition = 2, Cards = [] },
            ],
        };
        var cardRepository = new FakeCardRepository
        {
            Cards =
            [
                new CardModel { Id = 10, RowColumnId = 1, IndexPosition = 1, Name = "Delete", Url = "https://delete.example" },
                new CardModel { Id = 20, RowColumnId = 2, IndexPosition = 1, Name = "Keep", Url = "https://keep.example" },
            ],
        };
        var handler = CreateHandler(columnRowRepository, cardRepository);

        var result = await handler.Handle(new DeleteCardCommand { Id = 10 }, CancellationToken.None);

        ResultAssertions.AssertStatusCode(result, StatusCodes.Status200OK);
        Assert.Collection(columnRowRepository.DeletedRows, row => Assert.Equal(1, row.Id));
        Assert.Collection(columnRowRepository.BatchEditedRows, row =>
        {
            Assert.Equal(2, row.Id);
            Assert.Equal(1, row.RowPosition);
        });
        Assert.Single(cardRepository.BatchEditedCards);
    }

    [Fact]
    public async Task HandleDelete_WhenRowStillHasCards_DoesNotDeleteRow()
    {
        var columnRowRepository = new FakeColumnRowRepository
        {
            Rows =
            [
                new ColumnRowModel { Id = 1, RowPosition = 1, Cards = [] },
            ],
        };
        var cardRepository = new FakeCardRepository
        {
            Cards =
            [
                new CardModel { Id = 10, RowColumnId = 1, IndexPosition = 1, Name = "Delete", Url = "https://delete.example" },
                new CardModel { Id = 11, RowColumnId = 1, IndexPosition = 2, Name = "Keep", Url = "https://keep.example" },
            ],
        };
        var handler = CreateHandler(columnRowRepository, cardRepository);

        var result = await handler.Handle(new DeleteCardCommand { Id = 10 }, CancellationToken.None);

        ResultAssertions.AssertStatusCode(result, StatusCodes.Status200OK);
        Assert.Empty(columnRowRepository.DeletedRows);
        Assert.Empty(columnRowRepository.BatchEditedRows);
        Assert.Collection(cardRepository.BatchEditedCards, card =>
        {
            Assert.Equal(11, card.Id);
            Assert.Equal(1, card.IndexPosition);
        });
    }

    [Fact]
    public async Task HandleDelete_WhenDeletingOnlyCard_SkipsCardBatchEditAndDeletesRow()
    {
        var columnRowRepository = new FakeColumnRowRepository
        {
            Rows =
            [
                new ColumnRowModel { Id = 1, RowPosition = 1, Cards = [] },
            ],
        };
        var cardRepository = new FakeCardRepository
        {
            Cards =
            [
                new CardModel { Id = 10, RowColumnId = 1, IndexPosition = 1, Name = "Delete", Url = "https://delete.example" },
            ],
        };
        var handler = CreateHandler(columnRowRepository, cardRepository);

        var result = await handler.Handle(new DeleteCardCommand { Id = 10 }, CancellationToken.None);

        ResultAssertions.AssertStatusCode(result, StatusCodes.Status200OK);
        Assert.Collection(columnRowRepository.DeletedRows, row => Assert.Equal(1, row.Id));
        Assert.Empty(cardRepository.BatchEditedCards);
    }

    private static BackendCardHandler CreateHandler(
        IColumnRowRepository columnRowRepository,
        ICardRepository cardRepository) =>
        new(
            columnRowRepository,
            cardRepository,
            new FakeSettingsRepository(),
            new FakeIconRepository(),
            new FakeIconConnectedRepository(),
            new FakeFileService());

    private sealed class FakeColumnRowRepository : IColumnRowRepository
    {
        public List<ColumnRowModel> Rows { get; set; } = [];
        public List<ColumnRowModel> DeletedRows { get; private set; } = [];
        public List<ColumnRowModel> BatchEditedRows { get; private set; } = [];

        public Task<ColumnRowModel> Insert(ColumnRowModel columnRow)
        {
            var insertedRow = Clone(columnRow);
            insertedRow.Id = Rows.Count == 0 ? 1 : Rows.Max(row => row.Id) + 1;
            Rows.Add(insertedRow);
            return Task.FromResult(Clone(insertedRow));
        }

        public Task<IEnumerable<ColumnRowModel>> Get() =>
            Task.FromResult<IEnumerable<ColumnRowModel>>(Rows.Select(Clone).ToList());

        public Task<bool> BatchDelete(IEnumerable<ColumnRowModel> columnRows)
        {
            DeletedRows = columnRows.Select(Clone).ToList();
            var deletedIds = DeletedRows.Select(row => row.Id).ToHashSet();
            Rows = Rows.Where(row => !deletedIds.Contains(row.Id)).ToList();
            return Task.FromResult(true);
        }

        public Task<bool> BatchEdit(IEnumerable<ColumnRowModel> columnRows)
        {
            BatchEditedRows = columnRows.Select(Clone).ToList();
            foreach (var editedRow in BatchEditedRows)
            {
                var row = Rows.FirstOrDefault(row => row.Id == editedRow.Id);
                if (row is not null)
                    row.RowPosition = editedRow.RowPosition;
            }

            return Task.FromResult(true);
        }

        private static ColumnRowModel Clone(ColumnRowModel row) =>
            new()
            {
                Id = row.Id,
                RowPosition = row.RowPosition,
                RowWidth = row.RowWidth,
                Cards = row.Cards?.Select(Clone).ToList() ?? [],
            };

        private static CardModel Clone(CardModel card) =>
            new()
            {
                Id = card.Id,
                IndexPosition = card.IndexPosition,
                RowColumnId = card.RowColumnId,
                Name = card.Name,
                Url = card.Url,
                IconUrl = card.IconUrl,
                IconData = card.IconData,
                Settings = card.Settings,
            };
    }

    private sealed class FakeCardRepository : ICardRepository
    {
        public List<CardModel> Cards { get; set; } = [];
        public List<CardModel> BatchEditedCards { get; private set; } = [];

        public Task<CardModel> Insert(CardModel Card) => Task.FromResult(Clone(Card));

        public Task<bool> Edit(CardModel Card) => Task.FromResult(true);

        public Task<bool> BatchEdit(IEnumerable<CardModel>? cards)
        {
            BatchEditedCards = cards?.Select(Clone).ToList() ?? [];
            return Task.FromResult(true);
        }

        public Task<IEnumerable<CardModel>?> Get() =>
            Task.FromResult<IEnumerable<CardModel>?>(Cards.Select(Clone).ToList());

        public Task<CardModel> GetById(int Id) =>
            Task.FromResult(Clone(Cards.Single(card => card.Id == Id)));

        public Task<bool> Delete(int Id)
        {
            Cards = Cards.Where(card => card.Id != Id).ToList();
            return Task.FromResult(true);
        }

        private static CardModel Clone(CardModel card) =>
            new()
            {
                Id = card.Id,
                IndexPosition = card.IndexPosition,
                RowColumnId = card.RowColumnId,
                Name = card.Name,
                Url = card.Url,
                IconUrl = card.IconUrl,
                IconData = card.IconData,
                Settings = card.Settings,
            };
    }

    private sealed class FakeSettingsRepository : ISettingsRepository
    {
        public Task<SettingsModel> Insert(SettingsModel settings) => Task.FromResult(settings);
        public Task<SettingsModel> Edit(SettingsModel settings) => Task.FromResult(settings);
        public Task<bool> Delete(int Id) => Task.FromResult(true);
    }

    private sealed class FakeIconRepository : IIconRepository
    {
        public Task<IconModel> Insert(IconModel icon) => Task.FromResult(icon);
        public Task<IconModel> Edit(IconModel icon) => Task.FromResult(icon);
        public Task<IconModel> GetById(int Id) => Task.FromResult(new IconModel { Id = Id });
        public Task<IconModel> GetByFullName(IconModel icon) => Task.FromResult(icon);
        public List<string> FindUnusedIcons(IEnumerable<CardModel> cards) => [];
        public Task<bool> Delete(int Id) => Task.FromResult(true);
    }

    private sealed class FakeIconConnectedRepository : IIconConnectedRepository
    {
        public Task<IEnumerable<IconConnectedDtoModel>> GetManyById(int? cardId, int? iconId) =>
            Task.FromResult<IEnumerable<IconConnectedDtoModel>>([]);

        public Task<IconConnectedDtoModel> Insert(IconConnectedDtoModel model) => Task.FromResult(model);
        public Task<bool> Delete(int cardId) => Task.FromResult(true);
    }
}
