using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.Errors;
using Talabat.Core.Repositories.Interfaces;
using Talabat.Repositry.Data;

namespace Talabat.APIs.Controllers
{
	
	public class BuggyController : BaseApiController
	{
		private readonly StoreDbContext _context;

		public BuggyController(StoreDbContext context)
        {
			_context = context;
		}

		[HttpGet("notfound")] //Get : api/Buggy/notfound
		public ActionResult GetNotFoundRequest()
		{
			var Product = _context.Products.Find(1000);
			if(Product is null)
				return NotFound(new ApiResponse(404));
			return Ok(Product);
		}

		[HttpGet("servererror")] //Get : api/Buggy/servererror
		public ActionResult GetNotServerRequest()
		{
			var Product = _context.Products.Find(1000);
			var Result = Product.ToString(); //will Throw Exception [NullReferenceException]
			return Ok(Result);
		}

		[HttpGet("badrequest")] //Get : api/Buggy/badrequest
		public ActionResult GetNotBadRequest()
		{
			return BadRequest(new ApiResponse(400));
		}

		[HttpGet("badrequest/{id}")] //Get : api/Buggy/badrequest/5
		public ActionResult GetNotBadRequest(int? id)  //Validation Error
		{
			return Ok();
		}

		[HttpGet("unauthorized")] //Get : api/Buggy/unauthorized
		public ActionResult GetNotUnauthorizedRequest()  //Validation Error
		{
			return Unauthorized(new ApiResponse(401));
		}

	}
}
