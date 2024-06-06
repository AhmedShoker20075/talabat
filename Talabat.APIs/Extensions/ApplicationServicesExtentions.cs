using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.Errors;
using Talabat.APIs.Helpers;
using Talabat.Core.Repositories.Interfaces;
using Talabat.Core.Services.Interfaces;
using Talabat.Repositry;
using Talabat.Repositry.Repositories;
using Talabat.Service;

namespace Talabat.APIs.Extensions
{
	public static class ApplicationServicesExtentions
	{
		public static IServiceCollection AddApplicationServices(this IServiceCollection services)
		{
			//builder.Services.AddScoped<IGenericRepository<Product>, GenericRepository<Product>>();
			//builder.Services.AddScoped<IGenericRepository<ProductCaregory>, GenericRepository<ProductCaregory>>();
			//builder.Services.AddScoped<IGenericRepository<ProductBrand>, GenericRepository<ProductBrand>>();

			
			services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
			services.AddScoped<IPaymentService, PaymentService>();
			services.AddScoped<IUnitOfWork, UnitOfWork>();
			services.AddScoped<IOrderService, OrderService>();
			//builder.Services.AddAutoMapper(M => M.AddProfile(new MappingProfile()));
			services.AddAutoMapper(typeof(MappingProfile));

			//Edit Configrations Of ModelState To Handle Validation Error
			services.Configure<ApiBehaviorOptions>(options =>
			{
				options.InvalidModelStateResponseFactory = (actionContext) =>
				{
					var errors = actionContext.ModelState.Where(P => P.Value.Errors.Count > 0)
											.SelectMany(P => P.Value.Errors)
											.Select(E => E.ErrorMessage).ToArray();

					var validationErrorResponse = new ApiValidationErrorResponse()
					{
						Errors = errors
					};
					return new BadRequestObjectResult(validationErrorResponse);
				};
			});

			return services;
		}
	}
}
