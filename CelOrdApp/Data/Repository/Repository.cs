using CelOrdApp.Models;
using Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using static Domain.EntityTypes;

namespace CelOrdApp.Data.Repository;

public class Repository(AdminDbContext adminDbcontext) : IRepository
{
	private readonly AdminDbContext _adminDbCtx = adminDbcontext;

	public Result<CelOrdAccount> SetupNewAccount(LoginViewModel signupRequest)
	{
		if (_adminDbCtx.Companies.Any(c => c.Subdomain == signupRequest.Subdomain))
		{
			return Result<CelOrdAccount>.Failure($"[{signupRequest.CompanyName}]  ya existe.");
		}

		// CONITNUE HERE... use table Company ID to se new db name

		string newDbName = _adminDbCtx.GetNextDbName();

		var newCompany = new Company()
		{
			CompanyName = signupRequest.CompanyName,
			Subdomain = signupRequest.Subdomain,
			DbName = newDbName
		};

		_adminDbCtx.Add(newCompany);
		_adminDbCtx.SaveChanges();

		string newAcctSqlScript_filePath = $"{Environment.CurrentDirectory}/Files/ClientDbBase.sql";
		string newAcctSqlScript = File.ReadAllText(newAcctSqlScript_filePath).Replace("[co100]", $"[{newDbName}]");

		string newDbConnStr = _adminDbCtx.CreateDatabase(newDbName, newAcctSqlScript);

		User user = new() { Username = signupRequest.Username, Password = signupRequest.Password};
		var hasher = new PasswordHasher<User>();
		string passwordHash = hasher.HashPassword(user, signupRequest.Password);

		_adminDbCtx.InsertAdminUser(signupRequest.Username, passwordHash, newDbConnStr);

		var newAccount = _adminDbCtx.GetAccount(newDbName);

		return Result<CelOrdAccount>.Success(newAccount);
	}

	public Company GetCompany(string subdomain)
	{
		var company = _adminDbCtx.Companies.FirstOrDefault(
			subDom => subDom.Subdomain == subdomain || subDom.CompanyName == subdomain);

		return company ?? new Company();
	}

	public User GetUser(Company company, string username, string password)
	{
		string query = "SELECT TOP 1 Id, FullName, Username, Password, UserType, CreatedAt " +
						"FROM Users " +
						"WHERE Username=@username";

		string? clientDbConnStr = _adminDbCtx.Database.GetConnectionString()?.Replace("coadmin", company.DbName);
		User user = new();

		using (var conn = new SqlConnection(clientDbConnStr))
		using (var cmd = new SqlCommand(query, conn))
		{
			conn.Open();

			cmd.Parameters.AddWithValue("username", username);
			var reader = cmd.ExecuteReader();

			if (!reader.Read())
			{
				return new User();
			}

			user.Id = reader.GetInt32(0);
			user.FullName = reader["FullName"].ToString() ?? "";
			user.Username = reader["Username"].ToString() ?? "";
			user.Password = reader["Password"].ToString() ?? "";
			user.UserType = (UserType)reader.GetByte(4);
			user.CreatedAt = reader.GetDateTime(5);
		}

		var hasher = new PasswordHasher<User>();
		var result = hasher.VerifyHashedPassword(user, user.Password, password);

		if (result == PasswordVerificationResult.Success)
		{
			return user;
		}

		return new User();
	}
}
