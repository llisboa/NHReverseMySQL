public class Field
{
    public string? Name { get; set; }
    public bool IsNullable { get; set; }
    public string? DataType { get; set; }
    public long CharacterMaximumLength { get; set; }
    public int NumericPrecision { get; set; }
    public int NumericScale { get; set; }
}