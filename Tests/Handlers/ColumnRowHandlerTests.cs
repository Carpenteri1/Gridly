using Gridly.Commands;
using Gridly.Handlers;
using Gridly.Models;
using Gridly.Repositories;
using Gridly.Services;
using Gridly.Dtos;
using Gridly.Tests.Infrastructure;

namespace Gridly.Tests.Handlers;

public class ColumnRowHandlerTests
{
    [Fact]
    public async Task Handle_WhenRowIsRemoved_AttachesStoredCardsToMissingRowsBeforeDelete()
    {
        var operations = new List<string>();
        var columnRowRepository = new FakeColumnRowRepository(operations)
        {
            Rows =
            [
                new ColumnRowModel { Id = 1, RowPosition = 1, Cards = [] },
                new ColumnRowModel { Id = 2, RowPosition = 2, Cards = [] },
            ],
        };
        var cardRepository = new FakeCardRepository(operations)
        {
            Cards =
            [
                new CardModel { Id = 10, RowColumnId = 1, IndexPosition = 1, Name = "Moved", Url = "https://moved.example" },
                new CardModel { Id = 20, RowColumnId = 2, IndexPosition = 1, Name = "Kept", Url = "https://kept.example" },
            ],
        };
        var handler = new ColumnRowHandler(
            columnRowRepository,
            cardRepository,
            new FakeSettingsRepository(),
            new FakeIconRepository(),
            new FakeIconConnectedRepository(),
            new FakeWeatherDataConnectionRepository(),
            new FakeFileService());
        var command = new BatchSaveColumnRowCommands
        {
            new()
            {
                Id = 2,
                RowPosition = 1,
                Cards =
                [
                    new CardModel { Id = 10, RowColumnId = 1, IndexPosition = 1, Name = "Moved", Url = "https://moved.example" },
                    new CardModel { Id = 20, RowColumnId = 2, IndexPosition = 2, Name = "Kept", Url = "https://kept.example" },
                ],
            },
        };

        await handler.Handle(command, CancellationToken.None);

        Assert.Collection(columnRowRepository.DeletedRows, row =>
        {
            Assert.Equal(1, row.Id);
        });
        Assert.Equal(["batch-edit-cards", "delete-rows"], operations);
        Assert.All(cardRepository.BatchEditedCards, card => Assert.Equal(2, card.RowColumnId));
    }

    [Fact]
    public async Task Handle_WhenDeletingACardWithoutAWeatherConnection_NeverCallsDeleteIfOrphaned()
    {
        var operations = new List<string>();
        var columnRowRepository = new FakeColumnRowRepository(operations)
        {
            Rows = [new ColumnRowModel { Id = 1, RowPosition = 1, Cards = [] }],
        };
        var cardRepository = new FakeCardRepository(operations)
        {
            Cards = [new CardModel { Id = 10, RowColumnId = 1, IndexPosition = 1, Name = "Plain", Url = "https://plain.example" }],
        };
        var weatherDataConnectionRepository = new FakeWeatherDataConnectionRepository();
        var handler = new ColumnRowHandler(
            columnRowRepository,
            cardRepository,
            new FakeSettingsRepository(),
            new FakeIconRepository(),
            new FakeIconConnectedRepository(),
            weatherDataConnectionRepository,
            new FakeFileService());
        var command = new BatchSaveColumnRowCommands
        {
            new() { Id = 1, RowPosition = 1, Cards = [] },
        };

        await handler.Handle(command, CancellationToken.None);

        Assert.Equal([10], weatherDataConnectionRepository.DeletedCardIds);
    }

    private sealed class FakeColumnRowRepository(List<string> operations) : IColumnRowRepository
    {
        public List<ColumnRowModel> Rows { get; set; } = [];
        public List<ColumnRowModel> DeletedRows { get; private set; } = [];

        public Task<ColumnRowModel> Insert(ColumnRowModel columnRow)
        {
            var insertedRow = Clone(columnRow);
            insertedRow.Id = Rows.Count == 0 ? 1 : Rows.Max(row => row.Id) + 1;
            Rows.Add(insertedRow);
            return Task.FromResult(Clone(insertedRow));
        }

        public Task<IEnumerable<ColumnRowModel>?> Get() =>
            Task.FromResult<IEnumerable<ColumnRowModel>?>(Rows.Select(Clone).ToList());

        public Task<bool> BatchDelete(IEnumerable<ColumnRowModel> columnRows)
        {
            operations.Add("delete-rows");
            DeletedRows = columnRows.Select(Clone).ToList();
            var deletedIds = DeletedRows.Select(row => row.Id).ToHashSet();
            Rows = Rows.Where(row => !deletedIds.Contains(row.Id)).ToList();
            return Task.FromResult(true);
        }

        public Task<bool> BatchEdit(IEnumerable<ColumnRowModel> columnRows) =>
            Task.FromResult(true);

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

    private sealed class FakeCardRepository(List<string> operations) : ICardRepository
    {
        public List<CardModel> Cards { get; set; } = [];
        public List<CardModel> BatchEditedCards { get; private set; } = [];

        public Task<CardModel> Insert(CardModel Card) => Task.FromResult(Card);

        public Task<bool> Edit(CardModel Card) => Task.FromResult(true);

        public Task<bool> BatchEdit(IEnumerable<CardModel>? cards)
        {
            operations.Add("batch-edit-cards");
            BatchEditedCards = cards?.Select(Clone).ToList() ?? [];
            return Task.FromResult(true);
        }

        public Task<IEnumerable<CardModel>?> Get() =>
            Task.FromResult<IEnumerable<CardModel>?>(Cards.Select(Clone).ToList());

        public Task<bool> Delete(int Id) => Task.FromResult(true);

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
        public Task<IconModel> GetById(int Id) => Task.FromResult(new IconModel { Id = Id, Name = "", Type = "", Base64Data = "", MaterialIcon = "" });
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

    private sealed class FakeFileService : IFileService
    {
        public bool FileExist(string filePath) => true;
        public bool DeletedFile(string filePath) => true;
        public bool WriteAllBitesToFile(string filePath, string content) => true;
        public bool WriteToFile(string filePath, string content) => true;
        public Task<string> ReadAllFromFileAsync(string filePath) => Task.FromResult(string.Empty);
        public IEnumerable<FileInfo> GetAllIcons() => [];
        public bool UploadIcon(IconModel iconModel) => true;
        public bool DeleteIcon(string fileName, string fileType) => true;
    }
}
