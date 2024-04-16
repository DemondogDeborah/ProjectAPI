using ErrorOr;
using LoginMongoAPI.Interfaces;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using TodoApiMongo.StaticClasses;

namespace LoginMongoAPI.Models
{
    public class ClientService : IClientService
    {
        public async Task<ErrorOr<Client>> CreateClient(string clientName,
                                              string email)
        {
            try
            {
                var client = new Client
                {
                    ClientName = clientName,
                    Email = email
                };

                await DBCollections.clientCollection.InsertOneAsync(client);

                return client;
            }
            catch (Exception ex)
            {
                return new ErrorOr<Client>();
            }
        }

        public async Task<ErrorOr<string>> DeleteClient(string id)
        {
            try
            {
                var client = await DBCollections.clientCollection.Find(u => u.Id == id).FirstOrDefaultAsync();
                //DBCollections.tareaCollection.DeleteOne(tarea.Id);
                if (client == null)
                {
                    return new ErrorOr<string>();
                }
                var result = await DBCollections.clientCollection.DeleteOneAsync(u => u.Id == id);
                if (result.DeletedCount == 0)
                {
                    return new ErrorOr<string>();
                }
                return "Client sucessfully deleted";
            }
            catch (Exception ex)
            {
                throw new Exception("Error to delete client.", ex);
            }
        }


        public async Task<List<Client>> GetAll() =>
        await DBCollections.clientCollection.Find(_ => true).ToListAsync();


        public async Task<ErrorOr<Client>> GetClient(string id)
        {
            try
            {
                var client = await DBCollections.clientCollection.Find(u => u.Id == id).FirstOrDefaultAsync();

                if (client == null)
                    return new ErrorOr<Client>();

                return client;
            }
            catch (Exception ex)
            {
                return new ErrorOr<Client>();
            }
        }



        public async Task<ErrorOr<Client>> UpdateClient(string id,
                                                    string clientName,
                                                     string email)
        {
            try
            {
                var filter = Builders<Client>.Filter.Eq(u => u.Id, id);
                var update = Builders<Client>.Update
                    .Set(u => u.ClientName, clientName)
                     .Set(u => u.Email, email);


                var client = await DBCollections.clientCollection.FindOneAndUpdateAsync(filter, update);

                return client;
            }
            catch (Exception ex)
            {
                return new ErrorOr<Client>();
            }
        }

    }
}
