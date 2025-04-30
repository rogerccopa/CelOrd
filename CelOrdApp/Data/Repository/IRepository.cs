using CelOrdApp.Models;
using Domain;

namespace CelOrdApp.Data.Repository;

public interface IRepository
{
	public Result<CelOrdAccount> SetupNewAccount(LoginViewModel signupRequest);
	public Company GetCompany(string subdomain);
	public User GetUser(Company company, string username, string password);
}
