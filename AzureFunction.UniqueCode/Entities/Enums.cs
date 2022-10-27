using System.Collections.Generic;
using static Microsoft.Azure.Amqp.Serialization.SerializableType;

namespace AzureFunction.UniqueCode.Entities
{
    /// <summary>
    /// Event types in unique code
    /// </summary>
    public static class EnumUniqueCodeEventType
    {
        public const string BRU = "BRU"; // Born USDA
        public const string RCI = "RCI"; // Receiving Importer
        public const string SHI = "SHI"; // Shipping Importer
        public const string RTS = "RTS"; // Return Shipping Importer
        public const string RCB = "RCB"; // Receiving Bouquets Maker 
        public const string MIB = "MIB"; // Move Inventory Bouquets Maker
        public const string RTB = "RTB"; // Return Inventory Bouquetera
        public const string BRP = "BRP"; // Born Pre-Marked
        public const string IFP = "IFP"; // Incoming Finished Product Farm
        public const string OFP = "OFP"; // Output Finished Product Farm
        public const string WTM = "WTM"; // Wiga Temperature
        public const string QAI = "QAI"; // Quality Inspection
        public const string TCS = "TCS"; // Truck Container Shipment 
        public const string BOL = "BOL"; // Bill of Lading Invoice
        public const string AAW = "AAW"; // Accepted AWB
        public const string PKI = "PKI"; // Picking Invoice
    }

    /// <summary>
    /// BRU event enumerators
    /// </summary>
    public static class EnumEventBRU
    {
        public const string SERIALIZE = "SERIALIZE"; // BRU SERIALIZE
        public const string DESERIALIZE = "DESERIALIZE"; // BRU DESERIALIZE
        public const string AWB = "AWB"; // BRU AWB
        public const string HAWB = "HAWB"; // BRU HAWB
        public static IEnumerable<string> GetListTypeBRU() => new[] { SERIALIZE, DESERIALIZE };
        public static IEnumerable<string> GetListSerializationType() => new[] { AWB, HAWB };
    }

    /// <summary>
    /// Type of checked exceptions enumerators
    /// </summary>
    public static class EnumTypeExecptions
    {
        public const string NODATA = "NODATA"; // There is no data in the source
        public const string FLUENTVALIDATION = "FLUENTVALIDATION"; // Fluentvalidation validation fails
        public static IEnumerable<string> GetListCheckedExceptions() => new[] { NODATA, FLUENTVALIDATION };
    }
}
