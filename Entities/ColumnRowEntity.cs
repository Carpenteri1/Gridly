namespace Gridly.Entities;

public class ColumnRowEntity
{
    public int Id { get; set; }
    public int RowPosition { get; set; }
    public int RowWidth { get; set; }
    public ICollection<CardEntity>? Cards { get; set; }
}
