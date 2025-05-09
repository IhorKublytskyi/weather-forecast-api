using WeatherApp.DataAccess.Postgres.Models;

namespace WeatherApp.DataAccess.Postgres.Interfaces
{
    public interface IUsersRepository
    {
        Task<bool> Exists(string email);
        Task Add(Guid id, string email, string password, DateTime createdAt);
        Task Delete(Guid id);
        Task<List<UserEntity>> Get();
        Task<List<UserEntity>> GetByFilter(DateTime createdAt);
        Task<UserEntity?> GetById(Guid id);
        Task<UserEntity?> GetByEmail(string email);
        Task<List<UserEntity>> GetByPage(int page, int pageSize);
        Task<List<UserEntity>> GetWithRequests();
        Task<UserEntity> GetWithRequests(string email);
        Task Update(Guid id, string email, string password, DateTime createdAt);
    }
}