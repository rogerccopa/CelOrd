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
			return Result<CelOrdAccount>.Failure($"{signupRequest.CompanyName}  ya existe.");
		}

		if (_adminDbCtx.Companies.Any(c => c.Email == signupRequest.Username))
		{
			return Result<CelOrdAccount>.Failure($"{signupRequest.Username}  ya existe.");
		}

		var newCompany = new Company()
		{
			CompanyName = signupRequest.CompanyName,
			Subdomain = signupRequest.Subdomain,
			DbName = "",
			Email = signupRequest.Username,
		};

		_adminDbCtx.Add(newCompany);
		_adminDbCtx.SaveChanges();

		string newDbName = $"co{newCompany.Id}";

		newCompany.DbName = newDbName;
		_adminDbCtx.Update(newCompany);
		_adminDbCtx.SaveChanges();
		
		string sqlFilePath = $"{Environment.CurrentDirectory}/Files/ClientDbBase.sql";
		//string newAcctSqlScript = File.ReadAllText(sqlFilePath).Replace("[co_base]", $"[{newDbName}]");
		string newAcctSqlScript = File.ReadAllText(sqlFilePath);

		string newDbConnStr = _adminDbCtx.CreateDatabase(newDbName, newAcctSqlScript);

		User user = new() { Username = signupRequest.Username, Password = signupRequest.Password};
		var hasher = new PasswordHasher<User>();
		string passwordHash = hasher.HashPassword(user, signupRequest.Password);

		_adminDbCtx.InsertAdminUser(signupRequest.Username, passwordHash, newDbConnStr);

		var newAccount = _adminDbCtx.GetAccount(newDbName);

		return Result<CelOrdAccount>.Success(newAccount);
	}

	public Company? GetCompany(string subdomain)
	{
		var company = _adminDbCtx.Companies.FirstOrDefault(
			subDom => subDom.Subdomain == subdomain || subDom.CompanyName == subdomain);

		return company;
	}

	public User GetUser(Company company, string username, string password)
	{
		string query = "SELECT TOP 1 Id, FullName, Username, Password, Areas, CreatedAt " +
						"FROM Users " +
						"WHERE Username=@username";

		string? clientDbConnStr = _adminDbCtx.Database.GetConnectionString()?.Replace("co_admin", company.DbName);
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
			user.Areas = (reader["Areas"].ToString() ?? "").Split(',')
				.Select(a => Enum.Parse(typeof(Area), a))
				.Cast<Area>()
				.ToList() ?? new List<Area>();
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
