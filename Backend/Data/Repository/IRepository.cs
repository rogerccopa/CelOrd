using Backend.Models;

namespace Backend.Data.Repository;

public interface IRepository
{
	public Result<CelOrdenAccount> SetupNewAccount(string companyName, string adminEmail, string password);
	public Company GetCompany(string subdomain);
	public User GetUser(Company company, string username, string password);
}
