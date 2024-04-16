using ErrorOr;
using LoginMongoAPI.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using MongoDB.Driver;
using TodoApiMongo.StaticClasses;
using LoginMongoAPI.helpers;
using System.Data;

namespace LoginMongoAPI.Models
{
    public class UserService : IUserService
    {
        public async Task<ErrorOr<User>> CreateUser(string name,
                                                    string email,
                                                    string username,
                                                    string password,
                                                    string rol)

        {
            try
            {
                string hashedPassword = AuthenticationHelper.GeneratePasswordHash(password);

                var user = new User
                {
                    Name = name,
                    Email = email,
                    Username = username,
                    Password = hashedPassword,
                    Rol = rol
                };

                await DBCollections.userCollection.InsertOneAsync(user);

                return user;
            }
            catch (Exception ex)
            {
                return new ErrorOr<User>();
            }
        }

        public async Task<ErrorOr<string>> DeleteUser(string id)
        {
            try
            {
                var user = await DBCollections.userCollection.Find(u => u.Id == id).FirstOrDefaultAsync();
                //DBCollections.tareaCollection.DeleteOne(tarea.Id);
                if (user == null)
                {
                    return new ErrorOr<string>();
                }
                var result = await DBCollections.userCollection.DeleteOneAsync(u => u.Id == id);
                if (result.DeletedCount == 0)
                {
                    return new ErrorOr<string>();
                }
                return "User sucessfully deleted";
            }
            catch (Exception ex)
            {
                throw new Exception("Error to delete user.", ex);
            }
        }


        public async Task<List<User>> GetAll() =>
        await DBCollections.userCollection.Find(_ => true).ToListAsync();


        public async Task<ErrorOr<User>> GetUser(string id)
        {
            try
            {
                var user = await DBCollections.userCollection.Find(u => u.Id == id).FirstOrDefaultAsync();

                if (user == null)
                    return new ErrorOr<User>();

                return user;
            }
            catch (Exception ex)
            {
                return new ErrorOr<User>();
            }
        }



        public async Task<ErrorOr<User>> UpdateUser(string id,
                                                    string name,
                                                     string email,
                                                     string username,
                                                    string password,
                                                    string rol)
        {
            try
            {
                var filter = Builders<User>.Filter.Eq(u => u.Id, id);
                var update = Builders<User>.Update
                    .Set(u => u.Name, name)
                     .Set(u => u.Email, email)
                      .Set(u => u.Username, username);


                var user = await DBCollections.userCollection.FindOneAndUpdateAsync(filter, update);

                return user;
            }
            catch (Exception ex)
            {
                return new ErrorOr<User>();
            }
        }



        public async Task SetSecret(string email, string code)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Email, email);
            var update = Builders<User>.Update.Set(u => u.TwoFactorSecret, code);

            await DBCollections.userCollection.UpdateOneAsync(filter, update);
        }

        public async Task<string> GetSecret(string email)
        {
            try
            {
                var user = await DBCollections.userCollection.Find(u => u.Email == email).FirstOrDefaultAsync();

                if (user != null)
                {
                    return user.TwoFactorSecret ?? "";
                }
                else
                {
                    return "";
                }
            }
            catch (Exception ex)
            {
                return ""; 
            }
        }




    }
}
