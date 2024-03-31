namespace EzioLearning.Domain.Entities.System
{
    public class Culture
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;

        public bool IsDefault { get; set; }
        public int SortOrder { get; set; }
    }
}
