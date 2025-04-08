using Backend.Data;
using Backend.Data.Repository;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
        });

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        //builder.Services.AddSwaggerGen();

        builder.Services.AddControllers();

        // read from appsettings.json
        var adminDbConnStr = builder.Configuration.GetConnectionString("AdminDbConnStr") ??
                             throw new Exception("AdminDbConnStr not found in appsettings.json");
        adminDbConnStr = adminDbConnStr
            .Replace("DbUsername", builder.Configuration["DbUsername"])     // read from system EnvVars
            .Replace("DbPassword", builder.Configuration["DbPassword"]);    // read from system EnvVars

        var clientDbConnStr = builder.Configuration.GetConnectionString("ClientDbConnStr") ??
                             throw new Exception("ClientDbConnStr not found in appsettings.json");
        clientDbConnStr = clientDbConnStr
            .Replace("DbUsername", builder.Configuration["DbUsername"])     // read from system EnvVars
            .Replace("DbPassword", builder.Configuration["DbPassword"]);    // read from system EnvVars

        builder.Services.AddDbContext<AdminDbContext>(Options => Options.UseSqlServer(adminDbConnStr));
        builder.Services.AddDbContext<ClientDbContext>(Options => Options.UseSqlServer(clientDbConnStr));
        
        builder.Services.AddScoped<IRepository, Repository>();
        builder.Services.AddAuthentication(options =>
		{
            options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
		})
            .AddCookie(options =>
		    {
			    options.SlidingExpiration = true;
                options.Cookie.Name = "CelOrden";
                options.Cookie.HttpOnly = true;
				options.LoginPath = "/api/auth/login";
			    options.LogoutPath = "/api/auth/logout";
		    });
        builder.Services.AddAuthorization();

		var app = builder.Build();

        app.UseCors();
        // app.UseHttpsRedirection();
        app.MapControllers();

        app.Run();
    }
}