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
        public async Task<ApiResult<Entry>> Get(long id, [UserFromHeader] string userName)
        {
            return new ApiResult<Entry> { Data = await _repository.GetEntry(id, userName) };
        }

        // POST: api/entries
        [HttpPost]
        public async Task<ApiResult<long>> Insert([FromBody] Entry entry, [UserFromHeader] string userName)
        {
            return new ApiResult<long>
            {
                Status = ApiResultStatus.OK,
                Data = await _repository.InsertEntry(entry, userName)
            };
        }

        // POST: api/entries/update
        [HttpPost]
        [Route("update")]
        public async Task<ApiResult> Update([FromBody] Entry entry, [UserFromHeader] string userName)
        {
            await _repository.UpdateEntry(entry, userName);
            return new ApiResult
            {
                Status = ApiResultStatus.OK
            };
        }
    }
}
