using AspNetCore.Identity.MongoDbCore.Models;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using MongoDbGenericRepository.Attributes;

namespace LoginMongoAPI.Models
{
    [CollectionName("Clients")]
    public class Client 
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("ClientName")]
        public string? ClientName { get; set; }

        [BsonElement("Email")]
        public string? Email { get; set; }
    }

}
