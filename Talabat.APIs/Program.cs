using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Talabat.APIs.Errors;
using Talabat.APIs.Helpers;
using Talabat.APIs.MiddleWares;
using Talabat.Core.Entities;
using Talabat.Core.Repositories.Interfaces;
using Talabat.Repositry.Data;
using Talabat.Repositry.Repositories;
using Talabat.APIs.Extensions;
using StackExchange.Redis;
using Talabat.Repositry.Identity;
using Microsoft.AspNetCore.Identity;
using Talabat.Core.Services.Interfaces;
using Talabat.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Talabat.Core.Entities.Identity;


namespace Talabat.APIs
{
    public class Program
	{
        //Entry Point
        public static async Task Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.

			#region ConfigureServices

			builder.Services.AddControllers();

			// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen();

			builder.Services.AddDbContext<StoreDbContext>(option =>
			{
				option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
			});

			builder.Services.AddDbContext<AppIdentityDbContext>(option =>
			{
				option.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection"));
			});

			builder.Services.AddSingleton<IConnectionMultiplexer>((seerviceProvider) =>
			{
				var connection = builder.Configuration.GetConnectionString("Redis");

				return ConnectionMultiplexer.Connect(connection);
			});

			//builder.Services.AddScoped<IBasketRepository, BasketRepository>();	
			builder.Services.AddScoped(typeof(IBasketRepository), typeof(BasketRepository));

			builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
			{
				//options.Password.RequireDigit = true;
				//options.Password.RequiredUniqueChars = 2;				
			}).AddEntityFrameworkStores<AppIdentityDbContext>();

			builder.Services.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			})
							.AddJwtBearer(options =>
							{
								options.TokenValidationParameters = new TokenValidationParameters()
								{
									ValidateIssuer = true,
									ValidIssuer = builder.Configuration["JWT:ValidIssure"],
									ValidateAudience = true,
									ValidAudience = builder.Configuration["JWT:ValidAudience"],
									ValidateLifetime = true,
									ValidateIssuerSigningKey = true,
									IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"])),
								};
							});

			builder.Services.AddScoped<ITokenService, TokenService>();

			builder.Services.AddCors(options =>
			{
				options.AddPolicy("MyPolicy", config =>
				{
					config.AllowAnyHeader();
					config.AllowAnyMethod();
					config.WithOrigins(builder.Configuration["FrontEndBaseUrl"]);
				});
			});

			builder.Services.AddApplicationServices();
			#endregion

			var app = builder.Build();

			using var scope = app.Services.CreateScope();

			var service = scope.ServiceProvider;

			//Ask CLR To Inject Object From StoreDbContext Explicitly
			var _context = service.GetRequiredService<StoreDbContext>();

			//Ask CLR To Inject Object From AppIdentityDbContext Explicitly
			var _IdentityContext = service.GetRequiredService<AppIdentityDbContext>();

			var loggerFactory = service.GetRequiredService<ILoggerFactory>();

			try
			{
				await _context.Database.MigrateAsync(); //Update Database (Businees)
				//Data Seeding
				await StoreDbContextSeed.SeedAsync(_context);

				await _IdentityContext.Database.MigrateAsync(); //Update Database (Identity)

				//Data Seeding
				var _userManager = service.GetRequiredService<UserManager<AppUser>>();

				await AppIdentityDbContextSeed.SeedUsersAsync(_userManager);
			}
			catch (Exception ex)
			{
				var logger = loggerFactory.CreateLogger<Program>();
				logger.LogError(ex,"An Error Has Been Occured During Appling Migrations");

				//Console.WriteLine(ex.Message);
			}

			// Configure the HTTP request pipeline.
			#region Configure

			app.UseMiddleware<ExceptionMiddleware>();

			if (app.Environment.IsDevelopment())
			{
				//app.UseDeveloperExceptionPage();
				app.UseSwagger();
				app.UseSwaggerUI();
			}

			if (app.Environment.IsProduction())
			{
				//app.UseDeveloperExceptionPage();
				app.UseSwagger();
				app.UseSwaggerUI();
			}

			app.UseStatusCodePagesWithReExecute("/errors/{0}");

			app.UseHttpsRedirection();

			app.UseStaticFiles();

			app.UseCors("MyPolicy");

			app.UseAuthentication();
			app.UseAuthorization();

			app.MapControllers();

			#endregion

			app.Run();
		}
	}
}
