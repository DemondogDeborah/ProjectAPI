using ErrorOr;
using LoginMongoAPI.Models;

namespace LoginMongoAPI.Interfaces
{
    public interface IUserService
    {
        public Task<ErrorOr<User>> CreateUser(string name,
                                              string email,
                                              string username,
                                              string password,
                                              string rol);
        public Task<ErrorOr<User>> UpdateUser(string id,
                                              string name,
                                              string email,
                                              string username,
                                              string password,
                                              string rol);

        public Task<ErrorOr<string>> DeleteUser(string id);

        public Task<ErrorOr<User>> GetUser(string id);

        public Task<List<User>> GetAll();
        public Task SetSecret(string email, string secret);

        public Task<string> GetSecret(string email);
        public Task<bool> ValidateCode(string email, string code);
    }
}
