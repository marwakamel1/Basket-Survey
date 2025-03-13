using basketSurvey;
using FluentValidation;
using FluentValidation.AspNetCore;
using Hangfire;
using Serilog;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//builder.Services.AddIdentityApiEndpoints<ApplicationUser>()
//    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddDependencies(builder.Configuration);
builder.Host.UseSerilog((context,configuration) =>

    configuration.ReadFrom.Configuration(context.Configuration)
);
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    //app.MapOpenApi();
}

app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
app.UseHangfireDashboard("/jobs");

var scopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();
using var scope = scopeFactory.CreateScope();
var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

RecurringJob.AddOrUpdate("SendNewPollNotification",() => notificationService.NotifyNewPoll(null),Cron.Daily);

app.UseCors();
app.UseAuthorization();

//app.MapIdentityApi<ApplicationUser>();
app.MapControllers();

app.UseExceptionHandler();

app.Run();
