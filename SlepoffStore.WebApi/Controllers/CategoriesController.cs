using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SlepoffStore.Core;
using System.Net;

namespace SlepoffStore.WebApi.Controllers
{
    [ApiController]
    [Route("api/categories")]
    public class CategoriesController : Controller
    {
        private readonly IRepository _repository;

        public CategoriesController(IRepository repository)
        {
            _repository = repository;
        }

        // GET: api/categories
        [HttpGet]
        public ApiResult<Category[]> Get()
        {
            return new ApiResult<Category[]> { Data = _repository.GetCategories() };
        }

        // GET: api/categories/{categoryId}/entries
        [HttpGet]
        [Route("{categoryId}/entries")]
        public ApiResult<Entry[]> GetEntries(long categoryId)
        {
            return new ApiResult<Entry[]> { Data = _repository.GetEntriesByCategoryId(categoryId) };
        }

        // POST: api/categories
        [HttpPost]
        public ApiResult<long> Insert([FromBody] Category category)
        {
            return new ApiResult<long>
            {
                Status = ApiResultStatus.OK,
                Data = _repository.InsertCategory(category)
            };
        }
    }
}
