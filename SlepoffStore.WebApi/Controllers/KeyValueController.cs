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

        // GET: api/keyvalues
        [HttpGet]
        public ApiResult<string> Get([FromQuery] string key)
        {
            return new ApiResult<string> { Data = _repository[key] };
        }

        // POST: api/keyvalues
        [HttpPost]
        public ApiResult Insert([FromBody] KeyValue kv)
        {
            _repository[kv.Key] = kv.Value;
            return new ApiResult<long>
            {
                Status = ApiResultStatus.OK
            };
        }
    }
}
