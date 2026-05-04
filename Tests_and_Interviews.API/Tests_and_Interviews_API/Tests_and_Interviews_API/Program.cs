using Microsoft.EntityFrameworkCore;
using Tests_and_Interviews_API.Data;
using Tests_and_Interviews_API.Repositories;
using Tests_and_Interviews_API.Repositories.Interfaces;
using Tests_and_Interviews_API.Services;
using Tests_and_Interviews_API.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


//builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddScoped<ITestRepository, TestRepository>();

builder.Services.AddScoped<ITestService, TestService>();


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseSwagger();
app.UseSwaggerUI();

app.Run();
