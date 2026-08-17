namespace Gridly.Models;

public class ColumnRowModel
{
    public int Id { get; set; }
    public int RowPosition { get; set; }
    public required List<CardModel> Cards { get; set; }
    public int RowWidth { get; set; }
}