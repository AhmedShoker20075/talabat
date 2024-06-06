using Talabat.Core.Entities;

namespace Talabat.APIs.DTOs
{
	public class CustomerBasketDto
	{
		public CustomerBasketDto(string id)
		{
			Id = id;
			Items = new List<BasketItem>();
		}

		public string Id { get; set; }
		public List<BasketItem> Items { get; set; }
		public int? DeliveryMethodId { get; set; }
		public string? PaymentIntentId { get; set; }
		public string? ClientSecret { get; set; }
	}
}
