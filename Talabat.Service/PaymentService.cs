using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Order;
using Talabat.Core.Repositories.Interfaces;
using Talabat.Core.Services.Interfaces;
using Talabat.Core.Specifications.OrderSpecs;
using Product = Talabat.Core.Entities.Product;

namespace Talabat.Service
{
	public class PaymentService : IPaymentService
	{
		private readonly IBasketRepository _basketRepository;
		private readonly IUnitOfWork _unitOfWork;
		private readonly IConfiguration _configuration;

		public PaymentService(IBasketRepository basketRepository , IUnitOfWork unitOfWork , IConfiguration configuration)
        {
			_basketRepository = basketRepository;
			_unitOfWork = unitOfWork;
			_configuration = configuration;
		}
        public async Task<CustomerBasket?> CreateOrUpdatePaymentIntent(string basketId)
		{
			// Get Basket

			var basket = await _basketRepository.GetBasketAsync(basketId);
			if (basket == null) return null;

			// Calculate Total Price

			if(basket.Items.Count > 0)
			{
                foreach (var item in basket.Items)
                {
                    var Product = await _unitOfWork.Repository<Product>().GetAsync(item.Id);
					if(item.Price != Product.Price)
					{
						item.Price = Product.Price;
					}
                }
            }
			//SubTotal
			var SubTotal = basket.Items.Sum(I => I.Price * I.Quantity);

			var ShippingPrice = 0m;
			if(basket.DeliveryMethodId.HasValue)
			{
				var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetAsync(basket.DeliveryMethodId.Value);
				ShippingPrice = deliveryMethod.Cost;
			}

			//Call Stripe 

			StripeConfiguration.ApiKey = _configuration["StripeKeys:Secretkey"];
			var Service = new PaymentIntentService();
			PaymentIntent PaymentIntent;

			if(string.IsNullOrEmpty(basket.PaymentIntentId))
			{
				//Create New PaymentIntentId
				var options = new PaymentIntentCreateOptions()
				{
					Amount =(long) (SubTotal*100 + ShippingPrice*100),
					Currency = "usd",
					PaymentMethodTypes = new List<string>() { "card"},
				};
				PaymentIntent = await Service.CreateAsync(options);
				basket.PaymentIntentId = PaymentIntent.Id;
				basket.ClientSecret = PaymentIntent.ClientSecret;
					
			}
			else
			{
				//Update New PaymentIntentId
				var Options = new PaymentIntentUpdateOptions()
				{
					Amount = (long)(SubTotal * 100 + ShippingPrice * 100),
				};
				PaymentIntent = await Service.UpdateAsync(basket.PaymentIntentId,Options);
				basket.PaymentIntentId = PaymentIntent.Id;
				basket.ClientSecret = PaymentIntent.ClientSecret;
			}

			// Return Basket Included PaymentIntentId And Client Secret

			await _basketRepository.UpdateBasketAsync(basket);
			return basket;

		}

		public async Task<Order> UpdatePaymentIntentToSuccessOrFailed(string paymentIntentId, bool flag)
		{
			var spec = new OrderWithPaymentIntentSpecification(paymentIntentId);
			var order = await _unitOfWork.Repository<Order>().GetWithSpecAsync(spec);

			if (flag)
			{
				order.Status = OrderStatus.PaymentRecieved;
			}
			else
			{
				order.Status = OrderStatus.PaymentFailed;

			}
			_unitOfWork.Repository<Order>().Update(order);
			await _unitOfWork.CompleteAsync();

			return order;
		}
	}
}
