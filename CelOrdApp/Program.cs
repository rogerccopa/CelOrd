using CelOrdApp.Data.Repository;
using CelOrdApp.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

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

// read from env vars and store in AppParams
var appParams = new AppParams
{
	SmtpAccount = builder.Configuration["CelOrden_SmtpAccount"] ?? string.Empty,
	SmtpPassword = builder.Configuration["CelOrden_SmtpPassword"] ?? string.Empty,
	ClientBaseDbConnStr = clientDbConnStr
};
builder.Services.AddSingleton(appParams);

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
	options.LoginPath = "/login";
	options.LogoutPath = "/logout";
});

builder.Services.AddAuthorization();

builder.Services.AddHttpContextAccessor(); // So later, we can access the HttpContext in the repository or services

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

/*
app.MapControllerRoute(
	name: "areas",
	pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
);
*/
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.Run();
