using ErrorOr;
using LoginMongoAPI.Models;

namespace LoginMongoAPI.Interfaces
{
    public interface IClientService
    {
        public Task<ErrorOr<Client>> CreateClient(string clientname,
                                              string email);
        public Task<ErrorOr<Client>> UpdateClient(string id,
                                              string clientname,
                                              string email);

        public Task<ErrorOr<string>> DeleteClient(string id);

        public Task<ErrorOr<Client>> GetClient(string id);

        public Task<List<Client>> GetAll();
    }
}
