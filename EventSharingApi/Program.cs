using EventSharingApi.Context;
using EventSharingApi.Repository;
using EventSharingApi.Scheduler;
using Microsoft.EntityFrameworkCore;
using Quartz;
using Quartz.Impl;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer")));

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddTransient<IEventRepository, EventRepository>();
builder.Services.AddTransient<IEventAttendeeRepository, EventAttendeeRepository>();

// Configure Quartz.NET scheduler
var schedulerFactory = new StdSchedulerFactory();
var scheduler = schedulerFactory.GetScheduler().Result;
builder.Services.AddSingleton(scheduler);

// Configure RabbitMQ connection factory
var host = builder.Configuration.GetConnectionString("RabbitMQHost");
var port = int.Parse(builder.Configuration.GetConnectionString("RabbitMQPort")!);
var username = builder.Configuration.GetConnectionString("RabbitMQUsername");
var password = builder.Configuration.GetConnectionString("RabbitMQPassword");
var factory = new ConnectionFactory()
{
    HostName = host,
    Port = port,
    UserName = username,
    Password = password
};
builder.Services.AddSingleton(factory);

// Configure the scheduled task
var queueName = "task_queue";
var job = JobBuilder.Create<ScheduledTask>()
                    .WithIdentity("task", "group")
                    .UsingJobData("queueName", queueName)
                    .Build();

var trigger = TriggerBuilder.Create()
                            .WithIdentity("trigger", "group")
                            .StartNow()
                            .WithSimpleSchedule(x => x
                                .WithInterval(TimeSpan.FromMinutes(1))
                                .RepeatForever())
                            .Build();

scheduler.ScheduleJob(job, trigger);
scheduler.Start().Wait();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<AppDbContext>();
    if (context.Database.GetPendingMigrations().Any())
    {
        context.Database.Migrate();
    }

    var consumer = new TaskConsumer(
        services.GetRequiredService<ConnectionFactory>(),
        queueName,
        services.GetRequiredService<IEventRepository>(),
        services.GetRequiredService<IEventAttendeeRepository>());
    consumer.Consume();
}

app.Run();
