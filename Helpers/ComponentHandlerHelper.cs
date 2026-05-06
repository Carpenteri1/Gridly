using Gridly.Models;
using Gridly.Services;

namespace Gridly.helpers;

public class CardHandlerHelper(IFileService fileService)
{
    public bool IconDataHasValue(IconModel iconModel) =>
        iconModel != null &&
        !string.IsNullOrEmpty(iconModel.Name) &&
        !string.IsNullOrEmpty(iconModel.Type) &&
        !string.IsNullOrEmpty(iconModel.Base64Data);
    
    public bool UploadIcon(CardModel Card) 
        =>  fileService.UploadIcon(Card.IconData);
            
    public bool DeleteIcon(CardModel Card) =>
        fileService.DeleteIcon(Card.IconData.Name, Card.IconData.Type);

    public IEnumerable<CardModel> SetIndexValues(List<CardModel> cards)
    {
        for (int i = 0; i < cards.Count(); i++)
            cards[i].IndexPosition = i +1;
        
        return cards;
    }
}