using AspNetCore.Identity.MongoDbCore.Models;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using MongoDbGenericRepository.Attributes;

namespace LoginMongoAPI.Models
{
    [CollectionName("Users")]
    public class User //: MongoIdentityUser<Guid>
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
       
        [BsonElement("Name")]
        public string? Name { get; set; }

        [BsonElement("Email")]
        public string? Email { get; set; }

        [BsonElement("Username")]
        public string? Username { get; set; }

        [BsonElement("Password")]
        public string? Password { get; set; }

        [BsonElement("Rol")]
        public string? Rol { get; set; }

        [BsonElement("TwoFactorSecret")]
        public string? TwoFactorSecret { get; set; }
    }
   
}
