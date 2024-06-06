using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.DTOs;
using Talabat.APIs.Errors;
using Talabat.Core.Entities;
using Talabat.Core.Repositories.Interfaces;

namespace Talabat.APIs.Controllers
{
	
	public class BasketController : BaseApiController
	{
		private readonly IBasketRepository _basketRepository;
		private readonly IMapper _mapper;

		public BasketController(IBasketRepository basketRepository,IMapper mapper)
        {
			_basketRepository = basketRepository;
			_mapper = mapper;
		}

		[HttpGet] // Get : api/basket?id=11
		public async Task<ActionResult<CustomerBasket>> GetBasket(string id)
		{
			var basket = await _basketRepository.GetBasketAsync(id);

			return Ok(basket ?? new CustomerBasket(id));
		}

		[HttpPost] // Post : /api/basket
		public async Task<ActionResult<CustomerBasket>> UpdateBasket(CustomerBasketDto basket)
		{
			var mappedBasket = _mapper.Map <CustomerBasket> (basket);

			var CreatedOrUpdatedBasket = await _basketRepository.UpdateBasketAsync(mappedBasket);
			if (CreatedOrUpdatedBasket == null) return BadRequest(new ApiResponse(400));
			return Ok(CreatedOrUpdatedBasket);
		}

		[HttpDelete] //
		public async Task DeleteBasket(string id)
		{
			await _basketRepository.DeleteBasketAsync(id);
		}
    }
}
