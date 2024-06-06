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

namespace Talabat.Service
{
	public class OrderService : IOrderService
	{
		private readonly IBasketRepository _basketRepository;
		private readonly IUnitOfWork _unitOfWork;
		private readonly IPaymentService _paymentService;

		public OrderService(IBasketRepository basketRepository,IUnitOfWork unitOfWork,IPaymentService paymentService)
        {
			_basketRepository = basketRepository;
			_unitOfWork = unitOfWork;
			_paymentService = paymentService;
		}
        public async Task<Order?> CreateOrderAsync(string BuyerEmail, string basketId, int DeliveryMethodId, Address ShippingAddress)
		{
			//1.Get Basket From Basket Repo
			var basket = await _basketRepository.GetBasketAsync(basketId);

			//2. Get Selected Items From Basket
			var OrderItem = new List<OrderItem>();
			if (basket?.Items.Count() > 0)
			{
                foreach (var item in basket.Items)
                {
					var Product = await _unitOfWork.Repository<Product>().GetAsync(item.Id);
					var ProductItemOrder = new ProductItemOrderd(Product.Id, Product.Name, Product.PictureUrl);
					var orderItem = new OrderItem(ProductItemOrder, item.Price, item.Quantity);
					
					OrderItem.Add(orderItem);
				}             
			}

			//3. Calculate SubTotal
			var SubTotal = OrderItem.Sum(OI => OI.Price * OI.Quantity);

			//4. Get DeliveryMethod From Database
			var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetAsync(DeliveryMethodId);

			//5. Create Order

			//Check if PaymentIntentId Exists for Another Order

			var spec = new OrderWithPaymentIntentSpecification(basket.PaymentIntentId);
			var ExOrder = await _unitOfWork.Repository<Order>().GetWithSpecAsync(spec);

			if(ExOrder != null)
			{
				_unitOfWork.Repository<Order>().Delete(ExOrder);
				//Update PaymentIntentId With Amount of Basket If Changed
				basket = await _paymentService.CreateOrUpdatePaymentIntent(basketId);
			}

			var order = new Order(BuyerEmail, ShippingAddress, deliveryMethod, OrderItem, SubTotal , basket.PaymentIntentId);

			//6. Add Order Locally
			await _unitOfWork.Repository<Order>().AddAsync(order);

			//7. Save Order To DataBase

			var result = await _unitOfWork.CompleteAsync();

			if(result <= 0) return null;
			return order;
		}

		public async Task<IReadOnlyList<Order?>> GetOrderForSpecificUserAsync(string BuyerEmail)
		{
			var spec = new OrderSpecifications(BuyerEmail);

			var orders = await _unitOfWork.Repository<Order>().GetAllWithSpecAsync(spec);

			return orders;
			
		}

		public async Task<Order?> GetOrderByIdForSpecificUserAsync(string BuyerEmail, int OrderId)
		{
			var spec = new OrderSpecifications(BuyerEmail, OrderId);

			var order = await _unitOfWork.Repository<Order>().GetWithSpecAsync(spec);

			if(order is null) return null;
			return order;
		}

		
	}
}
