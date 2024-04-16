using LoginMongoAPI.Models;
using MongoDB.Driver;

namespace TodoApiMongo.StaticClasses
{
    public class DBCollections
    {
        private static MongoClient mongoClient = new MongoClient("mongodb://localhost:27017");


        private static IMongoDatabase dataBase = mongoClient.GetDatabase("LoginMongo");

        public static IMongoCollection<User> userCollection = dataBase.GetCollection<User>("User");
        public static IMongoCollection<Client> clientCollection = dataBase.GetCollection<Client>("Client");
    }
}
