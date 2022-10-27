namespace AzureFunction.UniqueCode.Entities
{
    public record EventBRU
    {
        public string Type { get; set; }
        public string SerializationType { get; set; }
        public string Parameter1 { get; set; }
        public string Parameter2 { get; set; }
        public Event Event { get; set; }
    }
}
