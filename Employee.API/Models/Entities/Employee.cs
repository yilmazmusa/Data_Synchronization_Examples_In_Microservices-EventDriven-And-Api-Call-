﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Employee.API.Models.Entities
{
    public class Employee
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement(Order = 0)]
        public ObjectId Id { get; set; }


        [BsonRepresentation(BsonType.String)]
        [BsonElement(Order =1)] 
        public string PersonId { get; set; }


        [BsonRepresentation(BsonType.String)]
        [BsonElement(Order =2)]
        public string Name { get; set; }


        [BsonRepresentation(BsonType.String)]
        [BsonElement(Order =3)]
        public string Department { get; set; }


        [BsonRepresentation(BsonType.Decimal128)]
        [BsonElement(Order =4)]
        public decimal Salary { get; set; }


    }

    
}
