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
        public ApiResult<Category[]> Get([UserFromHeader] string userName)
        {
            return new ApiResult<Category[]> { Data = _repository.GetCategories(userName) };
        }

        // GET: api/categories/{categoryId}/entries
        [HttpGet]
        [Route("{categoryId}/entries")]
        public ApiResult<Entry[]> GetEntries(long categoryId, [UserFromHeader] string userName)
        {
            return new ApiResult<Entry[]> { Data = _repository.GetEntriesByCategoryId(categoryId, userName) };
        }

        // POST: api/categories
        [HttpPost]
        public ApiResult<long> Insert([FromBody] Category category, [UserFromHeader] string userName)
        {
            return new ApiResult<long>
            {
                Status = ApiResultStatus.OK,
                Data = _repository.InsertCategory(category, userName)
            };
        }
    }
}
