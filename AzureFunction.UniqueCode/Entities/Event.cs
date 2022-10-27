using System;

namespace AzureFunction.UniqueCode.Entities
{
    public record Event
    {
        public string Code { get; set; }
        public string Date { get; set; }
        public string Area { get; set; }
        public string Obsv { get; set; }
    }
}
