using Microsoft.EntityFrameworkCore;
using Tests_and_Interviews_API.Data;
using Tests_and_Interviews_API.Repositories;
using Tests_and_Interviews_API.Repositories.Interfaces;
using Tests_and_Interviews_API.Services;
using Tests_and_Interviews_API.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<AppDbContext>();

builder.Services.AddScoped<ITestRepository, TestRepository>();
builder.Services.AddScoped<IInterviewSessionRepository, InterviewSessionRepository>();
builder.Services.AddScoped<IQuestionRepository, QuestionRepository>();
builder.Services.AddScoped<ISlotRepository, SlotRepository>();
builder.Services.AddScoped<IAnswerRepository, AnswerRepository>();
builder.Services.AddScoped<IApplicantRepository, ApplicantRepository>();
builder.Services.AddScoped<ICollaboratorsRepo, CollaboratorsRepo>();
builder.Services.AddScoped<ICompanyRepo, CompanyRepo>();
builder.Services.AddScoped<IEventsRepo, EventsRepo>();
builder.Services.AddScoped<IJobsRepository, JobsRepository>();
builder.Services.AddScoped<ILeaderboardRepository, LeaderboardRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<ITestAttemptRepository, TestAttemptRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddScoped<ITestService, TestService>();
builder.Services.AddScoped<IInterviewSessionService, InterviewSessionService>();
builder.Services.AddScoped<IQuestionService, QuestionService>();
builder.Services.AddScoped<ISlotService, SlotService>();
builder.Services.AddScoped<IAnswerService, AnswerService>();
builder.Services.AddScoped<IApplicantService, ApplicantService>();
builder.Services.AddScoped<ICollaboratorsService, CollaboratorsService>();
builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<IEventsService, EventsService>();
builder.Services.AddScoped<IJobsService, JobsService>();
builder.Services.AddScoped<ILeaderboardService, LeaderboardService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<ITestAttemptService, TestAttemptService>();
builder.Services.AddScoped<IUserService, UserService>();

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
