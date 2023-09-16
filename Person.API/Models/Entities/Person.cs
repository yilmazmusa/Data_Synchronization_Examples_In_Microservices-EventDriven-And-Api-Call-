using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Person.API.Models.Entities
{
    public class Person
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement(Order = 1 )]
        public ObjectId Id { get; set; }


        [BsonRepresentation(BsonType.String)]
        [BsonElement(Order = 1)]
        public string Name { get; set; } // Burdaki Name değeri değiştiriğinde biz Service Employee.API deki Name değerinide değiştirmek zorundayız.Veri tutarlılığı için bu önemli buna Veri Senkronizasyonu denir.
    }
}
