using EmployeePayroll.Data;
using EmployeePayroll.Services;
using Microsoft.EntityFrameworkCore;

namespace EmployeePayroll
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            const string DevUiCorsPolicy = "DevUiCorsPolicy";

            // Add services to the container.

            builder.Services.AddControllers();

            builder.Services.AddSwaggerGen();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy(DevUiCorsPolicy, policy =>
                {
                    policy
                        .SetIsOriginAllowed(origin =>
                        {
                            if (string.Equals(origin, "null", StringComparison.OrdinalIgnoreCase))
                            {
                                return true;
                            }

                            if (!Uri.TryCreate(origin, UriKind.Absolute, out var uri))
                            {
                                return false;
                            }

                            return uri.IsLoopback;
                        })
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });

            builder.Services.AddDbContext<AppDbContext>(options => 
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddScoped<IEmployeeService, EmployeeService>();


            var app = builder.Build();

            // Configure the HTTP request pipeline.

            if(app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                app.UseCors(DevUiCorsPolicy);
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
