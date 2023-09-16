using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ServiceA.Models.Entities
{
    public class Person
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        [BsonElement(Order = 0)]
        public ObjectId Id { get; set; }

        [BsonRepresentation(BsonType.String)]
        [BsonElement(Order = 1)]
        public string Name { get; set; }
    }
}
