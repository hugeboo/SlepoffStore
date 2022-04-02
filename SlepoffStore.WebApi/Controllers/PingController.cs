using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SlepoffStore.Core;
using SlepoffStore.Repository;
using System.Net;

namespace SlepoffStore.WebApi.Controllers
{
    [ApiController]
    [Route("api/ping")]
    public class PingController : Controller
    {
        private readonly IRepository _repository;

        public PingController(IRepository repository)
        {
            _repository = repository;
        }

        // GET: api/ping
        [HttpGet]
        public async Task<ApiResult> Get()
        {
            return new ApiResult { Status = await _repository.Ping() ? ApiResultStatus.OK : ApiResultStatus.Error };
        }
    }
}
