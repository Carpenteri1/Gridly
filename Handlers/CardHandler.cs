using Gridly.Command;
using Gridly.Dtos;
using Gridly.Factories;
using Gridly.helpers;
using Gridly.Models;
using Gridly.Repositories;
using Gridly.Services;
using MediatR;

namespace Gridly.Handlers;
public class CardHandler(
    ICardRepository cardRepository, 
    ISettingsRepository settingsRepository,
    IIconRepository iconRepository,
    IIconConnectedRepository iconConnectedRepository,
    IFileService fileService) : 
    IRequestHandler<SaveCardCommand, IResult>,
    IRequestHandler<GetAllCardCommand, IResult>,
    IRequestHandler<GetCardByIdCommands, CardModel>,
    IRequestHandler<DeleteCardCommand, IResult>,
    IRequestHandler<EditCardCommand, IResult>,
    IRequestHandler<BatchEditCardCommand, IResult>
{
    private readonly CardHandlerHelper handlerHelper = new(fileService);

    public async Task<IResult> Handle(SaveCardCommand command, CancellationToken cancellationToken)
    {
        var storedCards = await cardRepository.Get();
        
        if (storedCards == null || storedCards.Count() == 0 ) command.IndexPosition = 1;
        else command.IndexPosition = storedCards.Count() + 1;
        
        var Card = await cardRepository.Insert(command);

        command.Settings ??= SettingsFactory.Create(null);
        command.Settings.CardId = Card.Id;
        await settingsRepository.Insert(command.Settings);

        Card.IconData ??= IconFactory.Create(command.IconData);
        Card.IconData = await iconRepository.Insert(Card.IconData);

        await iconConnectedRepository.Insert(IconConnectedFactory.Create(Card.Id, Card.IconData.Id));

        return Results.Ok();
    }
    
    public async Task<IResult> Handle(DeleteCardCommand command, CancellationToken cancellationToken)
    {
        var cards = await cardRepository.Get();
   
        
        if (await settingsRepository.Delete(command.Id) is false ||
            await cardRepository.Delete(command.Id) is false)
            return Results.StatusCode(500);
        
         var Card = cards.FirstOrDefault(x => x.Id == command.Id);
        if (Card != null &&
         Card.IconData != null)
        {
            var iconsConnected = 
                await iconConnectedRepository.GetManyById(null,Card.IconData.Id);
            
            if (!iconsConnected.Any())
            {
                await iconRepository.Delete(Card.IconData.Id);
                await iconConnectedRepository.Delete(command.Id);
                fileService.DeleteIcon(Card.IconData.Name, Card.IconData.Type);
            }   
        }
        
        cards = await cardRepository.Get();
        var indexedCards = handlerHelper.SetIndexValues(cards.ToList());
        return await cardRepository.BatchEdit(indexedCards) == false ? Results.StatusCode(500) : Results.Ok();
    }
    
    public async Task<IResult> Handle(EditCardCommand command, CancellationToken cancellationToken)
    {
        var cards = await cardRepository.Get();
        if (cards == null) return Results.StatusCode(500);
        
        var Card = cards.FirstOrDefault(x => x.Id == command.EditCard.Id);
        if (Card is null) return Results.BadRequest();
        
        var connectionModel = new IconConnectedDtoModel { CardId = Card.Id };
        var editHasIcon = handlerHelper.IconDataHasValue(command.EditCard.IconData);
        var currentHasIcon = handlerHelper.IconDataHasValue(Card.IconData);


         //switch (command.SelectedDropDownIconValue)
        // {
           //  case 1:
        Card.IconUrl = string.Empty;
                 /*if (editHasIcon)
                 {
                     if (currentHasIcon)
                     {
                         var icon = await iconRepository.GetByFullName(Card.IconData);
                         if (icon != null)
                         {
                             var connections = await iconConnectedRepository.GetManyById(null, icon.Id);
                             if (connections.Count(x => x.IconId == icon.Id) <= 1)
                             {
                                 await iconConnectedRepository.Delete(Card.Id);
                                 await iconRepository.Delete(icon.Id);
                                 handlerHelper.DeleteIcon(Card);
                             }
                         }
                     }
                     var editIcon = await iconRepository.GetByFullName(command.EditCard.IconData);
                     if (editIcon == null)
                     {
                         editIcon = await iconRepository.Insert(command.EditCard.IconData);
                         Card.IconData = editIcon;
                         connectionModel.IconId = editIcon.Id;
                         await iconConnectedRepository.Insert(connectionModel);
                         handlerHelper.UploadIcon(Card);
                     }
                     else
                     {
                         Card.IconData = editIcon;
                         var connections = await iconConnectedRepository.GetManyById(null, editIcon.Id);
                         if (connections.Count(x => x.IconId == editIcon.Id) == 0)
                         {
                             connectionModel.IconId = editIcon.Id;
                             if (await iconConnectedRepository.Insert(connectionModel) == null) return Results.StatusCode(500);   
                         }
                     }
                 }
                 else
                 {
                     connectionModel.CardId = Card.Id;
                     Card.IconData = await iconRepository.Insert(command.EditCard.IconData);
                     if (Card.IconData != null) connectionModel.IconId = Card.IconData.Id;
                     if(handlerHelper.UploadIcon(Card) && 
                        await iconConnectedRepository.Insert(connectionModel) == null)
                         return Results.StatusCode(500);   
                 }*/

             //    break;
           //  case 2:
        if (currentHasIcon)
        {
            var connections = await iconConnectedRepository.GetManyById(null, Card.IconData.Id);
            var connectionsToIcon = connections.Count(x => x.IconId == Card.IconData.Id);
            if (connectionsToIcon == 1)
            {
                if(handlerHelper.DeleteIcon(Card) is false &&
                   await iconRepository.Delete(Card.IconData.Id) is false)
                    return Results.StatusCode(500);  
            }
            if (await iconConnectedRepository.Delete(Card.Id) is false) return Results.StatusCode(500);
               //Card.IconUrl = command.EditCard.IconUrl;
               Card.IconData.MaterialIcon = command.EditCard.IconData.MaterialIcon;
        }
        // break;
        // }

        //TODO EditCard.IconData.MaterialIcon has no value 
        if (Card.IconData is null && command.EditCard.IconData is not null)
            Card.IconData = await iconRepository.GetById(command.EditCard.IconData.Id);

        if (Card.IconData is not null)
            Card.IconData.MaterialIcon = command.EditCard.IconData?.MaterialIcon ?? Card.IconData.MaterialIcon;

        if (Card != command.EditCard)
        {
            Card.Name = command.EditCard.Name;
            if(command.EditCard.Url != null && 
                command.EditCard.Url!= string.Empty &&
                !command.EditCard.Url.Contains("http"))
            {
                Card.Url =  "https://" + command.EditCard.Url;
            }
            else
            {
                Card.Url = command.EditCard.Url;
            }
        }

        if (Card.Settings is not null && command.EditCard.Settings is not null &&
            Card.Settings != command.EditCard.Settings)
        {
            Card.Settings.ImageHidden = command.EditCard.Settings.ImageHidden;
            Card.Settings.TitleHidden = command.EditCard.Settings.TitleHidden;
            Card.Settings.Height = command.EditCard.Settings.Height;
            Card.Settings.Width = command.EditCard.Settings.Width;
        }

        var result = Card.Settings is null || await settingsRepository.Edit(Card.Settings) != null;
        result = await cardRepository.Edit(Card);
        
        if (Card.IconData is null)
            return result ? Results.Ok() : Results.StatusCode(500);
        
        await iconRepository.Edit(Card.IconData);
        var s = await iconConnectedRepository.GetManyById(Card.Id, Card.IconData.Id);

        if(s.Count() == 0)
        {
            await iconConnectedRepository.Insert(
               new IconConnectedDtoModel
            {
                CardId = Card.Id,
                IconId = Card.IconData.Id
            });
        }
        /*if (Card.IconData.Id > 0)
        {
            result = await iconConnectedRepository.Insert(new IconConnectedDtoModel
            {
                CardId = Card.Id,
                IconId = Card.IconData.Id
            }) != null;
        }*/
        return result ? Results.Ok(): Results.StatusCode(500);
    }

    public async Task<IResult> Handle(BatchEditCardCommand commands, CancellationToken cancellationToken)
    {
        var sortedCards = handlerHelper.SetIndexValues(commands);
        if(sortedCards == null) return Results.StatusCode(500);
        
        return await cardRepository.BatchEdit(sortedCards) ? 
            Results.Ok() : Results.StatusCode(500);                                         
    }

    public async Task<IResult> Handle(GetAllCardCommand command, CancellationToken cancellationToken)
    {
        var cards = await cardRepository.Get();
        return Results.Ok(cards.OrderBy(c => c.IndexPosition));
    }
    
    public async Task<CardModel?> Handle(GetCardByIdCommands command, CancellationToken cancellationToken) => 
        await cardRepository.GetById(command.Id);
}
