namespace AzureFunction.UniqueCode.Entities
{
    public record Property
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string Area { get; set; }
    }
}
