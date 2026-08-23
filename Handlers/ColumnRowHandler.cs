using Gridly.Commands;
using Gridly.Querys;
using Gridly.Factories;
using Gridly.helpers;
using Gridly.Models;
using Gridly.Repositories;
using Gridly.Repositories.Interfaces;
using Gridly.Services;
using MediatR;

namespace Gridly.Handlers;
public class ColumnRowHandler(
    IColumnRowRepository columnRowRepository,
    ICardRepository cardRepository,
    ISettingsRepository settingsRepository,
    IIconRepository iconRepository,
    IIconConnectedRepository iconConnectedRepository,
    IWeatherDataConnectionRepository weatherDataConnectionRepository):
    IRequestHandler<GetAllRowColumnsQuery, IResult>,
    IRequestHandler<BatchSaveColumnRowCommands, IResult>
{
    public async Task<IResult> Handle(GetAllRowColumnsQuery query, CancellationToken cancellationToken)
    {
        var storedRowColumns = (await columnRowRepository.Get())?.ToList() ?? new List<ColumnRowModel>();
        var cards = (await cardRepository.Get())?.ToList() ?? new List<CardModel>();
        
        AttachCardsToRows(storedRowColumns, cards);
        
        return storedRowColumns.Any() ? Results.Ok(storedRowColumns.ToList()) : Results.NoContent();    
    }

    public async Task<IResult> Handle(BatchSaveColumnRowCommands commands, CancellationToken cancellationToken)
    {
        var storedRowColumns = (await columnRowRepository.Get())?.ToList() ?? new List<ColumnRowModel>();
        var storedCards = (await cardRepository.Get())?.ToList() ?? new List<CardModel>();
        var rowsToSave = commands
            .Where(row => row.Cards is not null)
            .OrderBy(row => row.RowPosition)
            .ToList();

        await UpsertRows(rowsToSave, storedRowColumns);
        await UpsertCards(rowsToSave);

        await columnRowRepository.BatchEdit(rowsToSave);

        var cardsToSave = rowsToSave.SelectMany(row => row.Cards).ToList();
        if (cardsToSave.Count > 0)
            await cardRepository.BatchEdit(cardsToSave);

        await DeleteMissingCards(rowsToSave, storedCards);
        await DeleteMissingRows(rowsToSave, storedRowColumns);

        var savedRows = (await columnRowRepository.Get())?.ToList() ?? new List<ColumnRowModel>();
        var savedCards = (await cardRepository.Get())?.ToList() ?? new List<CardModel>();
        AttachCardsToRows(savedRows, savedCards);

        return savedRows.Any() ? Results.Ok(savedRows.ToList()) : Results.NoContent();
        
        async Task UpsertRows(List<ColumnRowModel> submittedRows, List<ColumnRowModel> existingRows)
        {
            var existingRowsById = existingRows.ToDictionary(row => row.Id);
            for (var rowIndex = 0; rowIndex < submittedRows.Count; rowIndex++)
            {
                var row = submittedRows[rowIndex];
                row.RowPosition = rowIndex + 1;

                existingRowsById.TryGetValue(row.Id, out var existingRow);
                row.RowWidth = existingRow?.RowWidth ?? 0;

                if (row.Id == 0 || existingRow is null)
                {
                    var insertedRow = await columnRowRepository.Insert(new ColumnRowModel
                    {
                        RowPosition = row.RowPosition,
                        RowWidth = 0,
                        Cards = []
                    });
                    row.Id = insertedRow.Id;
                    row.RowWidth = insertedRow.RowWidth;
                }
            }
        }

        async Task UpsertCards(List<ColumnRowModel> submittedRows)
        {
            foreach (var row in submittedRows)
            {
                for (var cardIndex = 0; cardIndex < row.Cards.Count; cardIndex++)
                {
                    var card = row.Cards[cardIndex];
                    card.RowColumnId = row.Id;
                    card.IndexPosition = cardIndex + 1;
                    card.Settings ??= SettingsFactory.Create((SettingsModel?)null);

                    if (card.Id != 0)
                        continue;

                    var insertedCard = await cardRepository.Insert(card);
                    card.Id = insertedCard.Id;

                    var settings = SettingsFactory.Create(card.Settings);
                    settings.CardId = insertedCard.Id;
                    card.Settings = await settingsRepository.Insert(settings);

                    var icon = IconFactory.Create(card.IconData);
                    card.IconData = await iconRepository.Insert(icon);
                    await iconConnectedRepository.Insert(IconConnectedFactory.Create(insertedCard.Id, card.IconData.Id));
                }
            }
        }

        async Task DeleteMissingCards(List<ColumnRowModel> submittedRows, List<CardModel> existingCards)
        {
            var submittedCardIds = submittedRows
                .SelectMany(row => row.Cards)
                .Where(card => card.Id > 0)
                .Select(card => card.Id)
                .ToHashSet();

            var cardsToDelete = existingCards
                .Where(card => !submittedCardIds.Contains(card.Id))
                .ToList();

            foreach (var card in cardsToDelete)
                await DeleteCard(card);
        }

        async Task DeleteMissingRows(List<ColumnRowModel> submittedRows, List<ColumnRowModel> existingRows)
        {
            var submittedRowIds = submittedRows
                .Where(row => row.Id > 0)
                .Select(row => row.Id)
                .ToHashSet();

            var rowsToDelete = existingRows
                .Where(row => !submittedRowIds.Contains(row.Id))
                .ToList();

            if (rowsToDelete.Count > 0)
                await columnRowRepository.BatchDelete(rowsToDelete);
        }

        async Task DeleteCard(CardModel card)
        {
            if (card.Settings is not null)
                await settingsRepository.Delete(card.Id);

            if (card.IconData is not null)
                await iconConnectedRepository.Delete(card.Id);

            await weatherDataConnectionRepository.Delete(card.Id);
            await cardRepository.Delete(card.Id);

            if (card.IconData is null)
                return;

            var remainingConnections = await iconConnectedRepository.GetManyById(null, card.IconData.Id);
            if (remainingConnections.Any())
                return;

            await iconRepository.Delete(card.IconData.Id);
            
        }
    }

    private static void AttachCardsToRows(List<ColumnRowModel> rows, IEnumerable<CardModel> cards)
    {
        foreach (var row in rows)
        {
            row.Cards = cards
                .Where(card => card.RowColumnId == row.Id)
                .OrderBy(card => card.IndexPosition)
                .ToList();
        }
    }
}
