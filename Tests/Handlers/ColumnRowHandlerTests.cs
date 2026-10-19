using Gridly.Commands;
using Gridly.Handlers;
using Gridly.Models;
using Gridly.Repositories;
using Gridly.Services;
using Gridly.Dtos;
using Gridly.Repositories.Interfaces;
using Gridly.Tests.Infrastructure;

namespace Gridly.Tests.Handlers;

public class ColumnRowHandlerTests
{
    /*[Fact]
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
            new FakeWeatherDataConnectionRepository());
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
    }*/

    /*[Fact]
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
        var handler = new ColumnRowHandler;
        var command = new BatchSaveColumnRowCommands
        {
            new() { Id = 1, RowPosition = 1, Cards = [] },
        };

        await handler.Handle(command, CancellationToken.None);

        Assert.Equal([10], weatherDataConnectionRepository.DeletedCardIds);
    }*/
    
}
