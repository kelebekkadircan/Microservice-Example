using MongoDB.Bson.Serialization.Attributes;

namespace STOCK.API.Models.Entities
{
    public class Stock
    {
        [BsonId]
        [BsonGuidRepresentation(MongoDB.Bson.GuidRepresentation.CSharpLegacy)]
        [BsonElement(Order = 0)]
        public Guid StockID { get; set; }

        [BsonGuidRepresentation(MongoDB.Bson.GuidRepresentation.CSharpLegacy)]
        [BsonElement(Order = 1)]
        public Guid ProductID { get; set; }

        [BsonRepresentation(MongoDB.Bson.BsonType.Int32)]
        [BsonElement(Order = 2)]
        public int Count { get; set; }


    }
}
