using System.Collections.Generic;

namespace AzureFunction.UniqueCode.Entities
{
    using Newtonsoft.Json;
    using System.ComponentModel.DataAnnotations;

    [JsonObject(Title = "Boxes")]
    public record Box
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [Key]
        [JsonProperty(PropertyName = "usda")]
        public string Usda { get; set; }

        [JsonProperty(PropertyName = "masterGuideId")]
        public int? MasterGuideId { get; set; }

        [JsonProperty(PropertyName = "awbId")]
        public string AWBId { get; set; }

        [JsonProperty(PropertyName = "masterGuide")]
        public string MasterGuide { get; set; }

        [JsonProperty(PropertyName = "dispatchId")]
        public int DispatchId { get; set; }

        [JsonProperty(PropertyName = "importer")]
        public string Importer { get; set; }

        [JsonProperty(PropertyName = "orderRef")]
        public string OrderRef { get; set; }

        [JsonProperty(PropertyName = "item")]
        public string Item { get; set; }

        [JsonProperty(PropertyName = "grower")]
        public string Grower { get; set; }

        [JsonProperty(PropertyName = "events")]
        public List<Event> Events { get; set; }

        [JsonProperty(PropertyName = "properties")]
        public List<Property> Properties { get; set; }

        [JsonProperty(PropertyName = "cheksum")]
        public string Cheksum { get; set; }
    }
}
