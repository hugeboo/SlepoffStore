using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SlepoffStore.Core;
using System.Net;

namespace SlepoffStore.WebApi.Controllers
{
    [ApiController]
    [Route("api/keyvalues")]
    public class KeyValueController : Controller
    {
        private readonly IRepository _repository;

        public KeyValueController(IRepository repository)
        {
            _repository = repository;
        }

        // GET: api/keyvalues?key={key}
        [HttpGet]
        public async Task<ApiResult<string>> Get([FromQuery] string key, [UserFromHeader] string userName)
        {
            return new ApiResult<string> { Data = await _repository.GetValue(key, userName) };
        }

        // POST: api/keyvalues
        [HttpPost]
        public async Task<ApiResult> Insert([FromBody] KeyValue kv, [UserFromHeader] string userName)
        {
            await _repository.SetValue(kv.Key, kv.Value, userName);
            return new ApiResult
            {
                Status = ApiResultStatus.OK
            };
        }
    }
}
