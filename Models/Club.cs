using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;



namespace WebApplication1.Models
{
    [BsonIgnoreExtraElements]
    public class Club
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = String.Empty;

        [BsonElement("name")]
        public string Name { get; set; } = String.Empty;

        [BsonElement("country")]
        public string Country { get; set; } = String.Empty;

        [BsonElement("photo")]
        public string Photo { get; set; } = String.Empty;


    }
}
