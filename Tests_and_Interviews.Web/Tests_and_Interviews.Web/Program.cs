using Microsoft.AspNetCore.Authentication.Cookies;
using Tests_and_Interviews.Web.Services;

using Tests_and_Interviews.Web.Clients;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(new Microsoft.AspNetCore.Mvc.Authorization.AuthorizeFilter());
});

// Configure HttpClient for future API calls (not used yet with mock data)
builder.Services.AddHttpClient("BackendAPI", client =>
{
    // TODO: Update this URL when backend is deployed - maybe an env file or smth
    client.BaseAddress = new Uri("https://localhost:7000");
});

//builder.Services.AddScoped<Interface, Service>

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/Login";
    });

builder.Services.AddHttpClient<IAuthService, AuthService>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5179/");
});

builder.Services.AddHttpClient<TestsApiClient>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5179");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();