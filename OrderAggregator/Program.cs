
using Microsoft.EntityFrameworkCore;
using OrderAggregator.DB;
using OrderAggregator.Job;
using OrderAggregator.Logic;
using OrderAggregator.Logic.Interfaces;
using Quartz;

namespace OrderAggregator
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddMemoryCache();
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                var xmlFile = $"doc/documentation.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            builder.Services.AddTransient<IOrderLogic, OrderLogic>();
            builder.Services.AddTransient<IProductLogic, ProductLogic>();
            builder.Services.AddSingleton<IOrderAggregatorForJob, OrderAggregatorForJob>();
            builder.Services.AddSingleton<OrderAggregateJob, OrderAggregateJob>();

            // DB context
            builder.Services.AddDbContext<OrdersContext>(options =>
            {
                options.UseInMemoryDatabase("OrdersDB");
            });

            var jobCronSchedule = builder.Configuration["Quartz:AggregateOrdersCronSchedule"];
            if (string.IsNullOrEmpty(jobCronSchedule))
            {
                Console.WriteLine("Missing Quartz:AggregateOrdersCronSchedule configuraiton");
                return;
            }

            builder.Services.AddQuartz(q =>
            {
                var jobKey = new JobKey("SendOrdersJob");
                q.AddJob<OrderAggregateJob>(opts => opts.WithIdentity(jobKey));

                q.AddTrigger(opts => opts
                    .ForJob(jobKey)
                    .WithCronSchedule(jobCronSchedule)
                );

                q.UseJobFactory<JobFactory>();
            });

            builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

            builder.Logging.AddConsole();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            // Aplikace by mìla mít nìjákou autentizaci a autorizaci.
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
