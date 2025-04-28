using Backend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using static Backend.Models.EntityTypes;

namespace Backend.Data.Repository;

public class Repository(AdminDbContext adminDbContext) : IRepository
{
	private readonly AdminDbContext _adminDbCtx = adminDbContext;

	public Result<CelOrdenAccount> SetupNewAccount(string companyName, string adminEmail, string password)
	{
		string newSubdomain = GenerateSlug(companyName);

		if (_adminDbCtx.Companies.Any(c => c.Subdomain == newSubdomain))
		{
			return Result<CelOrdenAccount>.Failure($"[{companyName}]  ya existe.");
		}

		if (_adminDbCtx.Companies.Any(c => c.Email == adminEmail))
		{
			return Result<CelOrdenAccount>.Failure($"[{adminEmail}] ya existe.");
		}

		var newCompany = new Company()
		{
			CompanyName = companyName,
			Subdomain = newSubdomain,
			DbName = "",
			Email = adminEmail
		};

		_adminDbCtx.Add(newCompany);
		_adminDbCtx.SaveChanges();

		string newDbName = $"co{newCompany.Id}";

		newCompany.DbName = newDbName;
		_adminDbCtx.Update(newCompany);
		_adminDbCtx.SaveChanges();

		string sqlFilePath = $"{Environment.CurrentDirectory}/Files/ClientDbBase.sql";
		string newAcctSqlScript = File.ReadAllText(sqlFilePath).Replace("[co100]", $"[{newDbName}]");

		string newDbConnStr = _adminDbCtx.CreateDatabase(newDbName, newAcctSqlScript);

		User user = new() { Username = adminEmail, Password = password };
		var hasher = new PasswordHasher<User>();
		string passwordHash = hasher.HashPassword(user, password);

		_adminDbCtx.InsertAdminUser(adminEmail, passwordHash, newDbConnStr);

		var newAccount = _adminDbCtx.GetAccount(newDbName);

		return Result<CelOrdenAccount>.Success(newAccount);
	}

	public Company? GetCompany(string subdomain)
	{
		var company = _adminDbCtx.Companies.FirstOrDefault(
				comp => comp.Subdomain == subdomain || comp.CompanyName == subdomain);

		return company;
	}

	public User GetUser(Company company, string username, string password)
	{
		string query = "SELECT TOP 1 Id, FullName, Username, Password, UserType, CreatedAt " +
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

	public string GenerateSlug(string phrase)
	{
		string str = RemoveDiacritics(phrase).ToLower();

		// invalid chars
		str = Regex.Replace(str, @"[^a-z0-9\s-]", "");

		// convert multiple spaces into one space
		str = Regex.Replace(str, @"\s+", " ").Trim();

		// cut and trim
		str = str.Substring(0, str.Length <= 45 ? str.Length : 45).Trim();
		str = Regex.Replace(str, @"\s", "-"); // hyphens

		return str;
	}

	private string RemoveDiacritics(string text)
	{
		var normalizedText = text.Normalize(System.Text.NormalizationForm.FormD);
		var sb = new StringBuilder(capacity: normalizedText.Length);

		for (int i = 0; i < normalizedText.Length; i++)
		{
			char c = normalizedText[i];
			var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);

			if (unicodeCategory != UnicodeCategory.NonSpacingMark)
			{
				sb.Append(c);
			}
		}

		return sb.ToString().Normalize(NormalizationForm.FormC);
	}

	public void LogError(string moduleName, string errorMessage)
	{
		using (var conn = new SqlConnection(_adminDbCtx.Database.GetConnectionString()!))
		using (var cmd = new SqlCommand("INSERT INTO AppErrors(Module, Error) VALUES(@module, @error)", conn))
		{
			conn.Open();
			cmd.Parameters.AddWithValue("@module", moduleName);
			cmd.Parameters.AddWithValue("@error", errorMessage.Substring(0, Math.Min(300, errorMessage.Length)));
			cmd.ExecuteNonQuery();
		}
	}
}
