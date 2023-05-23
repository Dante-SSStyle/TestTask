namespace TestTask.Domain;

/// <summary>
/// Model that stores left and right byte data. 
/// </summary>
public class DataModel
{
    public int Id { get; set; }
    public byte[]? Left { get; set; }
    public byte[]? Right { get; set; }

    public DataModel(byte[]? left = null, byte[]? right = null)
    {
        if (left != null)
            Left = left;
        if (right != null)
            Right = right;
    }

    public DataModel Update(byte[]? left = null, byte[]? right = null)
    {
        if (left != null)
            Left = left;
        if (right != null)
            Right = right;

        return this;
    }
}