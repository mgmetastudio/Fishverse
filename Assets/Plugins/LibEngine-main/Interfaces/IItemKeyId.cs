namespace LibEngine.Items
{
    public interface IItemKeyId
    {
        int ItemKeyId { get; set; }
    }

    public interface IItem : IItemKeyId
    {
        string Name { get; set; }
        string Description { get; set; }
        string Category { get; set; }
        string Image { get; set; }
    }

    public interface IRarity
    {
        string Rarity { get; set; }
    }
}
