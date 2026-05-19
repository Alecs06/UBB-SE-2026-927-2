using Tests_and_Interviews.Web.Clients;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure HttpClient for future API calls (not used yet with mock data)
builder.Services.AddHttpClient("BackendAPI", client =>
{
    // TODO: Update this URL when backend is deployed - maybe an env file or smth
    client.BaseAddress = new Uri("https://localhost:7000");
});

builder.Services.AddHttpClient<TestsApiClient>(client =>
{
    client.BaseAddress = new Uri("https://localhost:5179");
});

//builder.Services.AddScoped<Interface, Service>

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();