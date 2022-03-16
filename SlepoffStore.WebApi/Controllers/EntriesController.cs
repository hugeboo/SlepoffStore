using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SlepoffStore.Core;
using System.Net;

namespace SlepoffStore.WebApi.Controllers
{
    [ApiController]
    [Route("api/entries")]
    public class EntriesController : Controller
    {
        private readonly IRepository _repository;

        public EntriesController(IRepository repository)
        {
            _repository = repository;
        }

        // GET: api/entries/{id}
        [HttpGet]
        [Route("{id}")]
        public ApiResult<Entry> Get(long id)
        {
            return new ApiResult<Entry> { Data = _repository.GetEntry(id) };
        }

        // POST: api/entries
        [HttpPost]
        public ApiResult<long> Insert([FromBody] Entry entry)
        {
            return new ApiResult<long>
            {
                Status = ApiResultStatus.OK,
                Data = _repository.InsertEntry(entry)
            };
        }

        // POST: api/entries/update
        [HttpPost]
        [Route("update")]
        public ApiResult Update([FromBody] Entry entry)
        {
            _repository.UpdateEntry(entry);
            return new ApiResult
            {
                Status = ApiResultStatus.OK
            };
        }
    }
}
