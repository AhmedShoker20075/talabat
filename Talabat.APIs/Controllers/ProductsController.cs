using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.DTOs;
using Talabat.APIs.Errors;
using Talabat.APIs.Helpers;
using Talabat.Core.Entities;
using Talabat.Core.Repositories.Interfaces;
using Talabat.Core.Specifications;
using Talabat.Core.Specifications.ProductSpecs;

namespace Talabat.APIs.Controllers
{
	
	public class ProductsController : BaseApiController
	{
		private readonly IGenericRepository<Product> _ProductRepo;
		private readonly IMapper _mapper;
		private readonly IGenericRepository<ProductBrand> _BrandRepo;
		private readonly IGenericRepository<ProductType> _CaregoriesRepo;

		//GetAll
		//GetById
		public ProductsController(
			IGenericRepository<Product> ProductRepo,
			IMapper mapper, 
			IGenericRepository<ProductBrand> BrandRepo,
			IGenericRepository<ProductType> CaregoriesRepo)
			
        {
			_ProductRepo = ProductRepo;
			_mapper = mapper;
			_BrandRepo = BrandRepo;
			_CaregoriesRepo = CaregoriesRepo;
		}
		// /api/Products
	
		[HttpGet]
        public async Task<ActionResult<IReadOnlyList<ProductToReturnDto>>> GetProducts( [FromQuery] ProductSpecParams productSpec)
		{
			//var Products = await _ProductRepo.GetAllAsync();

			//var spec = new BaseSpecifications<Product>();
			var spec = new ProductWithBrandAndCategorySpecification(productSpec);
			var products = await _ProductRepo.GetAllWithSpecAsync(spec);

			//JsonResult result = new JsonResult(Products);
			//OkResult result = new OkResult();
			//OkObjectResult result = new OkObjectResult(Products);
			//result.ContentTypes = new Microsoft.AspNetCore.Mvc.Formatters.MediaTypeCollection();

			var data = _mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductToReturnDto>>(products);

			var countSpec = new ProductWithFilterationForCountSpecifications(productSpec);

			int count = await _ProductRepo.GetCountAsync(countSpec);

			return Ok(new Pagination<ProductToReturnDto>(productSpec.PageSize, productSpec.PageIndex, count, data));
		}

		[ProducesResponseType(typeof(ProductToReturnDto), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
		[HttpGet("{id}")]
		public async Task<ActionResult<ProductToReturnDto>> GetProductById(int id)
		{
			var specs = new ProductWithBrandAndCategorySpecification(id);
			var Product = await _ProductRepo.GetWithSpecAsync(specs);
			if (Product == null) 
				return NotFound(new ApiResponse(404));

			var Result = _mapper.Map<Product, ProductToReturnDto>(Product);


			return Ok(Result);
		}


		[HttpGet("brands")] // Get : /api/Products/brands
		public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetBrands()
		{
			var brands = await _BrandRepo.GetAllAsync();

			return Ok(brands);
		}

		[HttpGet("types")] // Get : /api/Products/categories
		public async Task<ActionResult<IReadOnlyList<ProductType>>> GetCaregories()
		{
			var Caregories = await _CaregoriesRepo.GetAllAsync();

			return Ok(Caregories);
		}
	}
}
