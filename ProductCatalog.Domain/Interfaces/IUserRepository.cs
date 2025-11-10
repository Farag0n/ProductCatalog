using ProductCatalog.Domain.Entities;

namespace ProductCatalog.Domain.Interfaces;

public interface IUserRepository
{
    public Task<IEnumerable<User>> GetAllUser();
    public Task<User?> GetUserById(int id); 
    public Task<User> AddUser(User user); 
    public Task<User?> UpdateUser(User user); 
    public Task<User?> DeleteUser(int id);
}