using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Talabat.APIs.Errors;
using Talabat.Core.Entities;
using Talabat.Core.Services.Interfaces;

namespace Talabat.APIs.Controllers
{
	[Authorize]
	public class PaymentsController : BaseApiController
	{
		private readonly IPaymentService _paymentService;
		const string endpointSecret = "whsec_8fdea329f81a5c6822db7f77b07f93d8074d819007ad24ff7a165940de29d524";


		public PaymentsController(IPaymentService paymentService )
        {
			_paymentService = paymentService;
		}

		[ProducesResponseType(typeof(CustomerBasket),StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(ApiResponse),StatusCodes.Status400BadRequest)]
        [HttpPost("{basketId}")] //POST : /api/payments/basketId
		public async Task<ActionResult<CustomerBasket>> CreateOrUpdatePaymentIntent(string basketId)
		{
			var basket = await _paymentService.CreateOrUpdatePaymentIntent(basketId);
			if (basket is null) return BadRequest(new ApiResponse(400, "There is a problem with your Basket!"));
			return Ok(basket);
		}

		[AllowAnonymous]
		[HttpPost("webhook")] //POST : https://localhost:7188/api/payments/webhook
		public async Task<IActionResult> StripeWebhook()
		{
			var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
			try
			{
				var stripeEvent = EventUtility.ConstructEvent(json,
					Request.Headers["Stripe-Signature"], endpointSecret);

				var paymentIntent = stripeEvent.Data.Object as PaymentIntent;

				// Handle the event
				if (stripeEvent.Type == Events.PaymentIntentPaymentFailed)
				{

					await _paymentService.UpdatePaymentIntentToSuccessOrFailed(paymentIntent.Id, false);
				}
				else if (stripeEvent.Type == Events.PaymentIntentSucceeded)
				{

					 await _paymentService.UpdatePaymentIntentToSuccessOrFailed(paymentIntent.Id, true);

				}
				// ... handle other event types
				else
				{
					Console.WriteLine("Unhandled event type: {0}", stripeEvent.Type);
				}

				return Ok();
			}
			catch (StripeException e)
			{
				return BadRequest();
			}
		}
	}
}
