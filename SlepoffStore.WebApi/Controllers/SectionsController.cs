using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SlepoffStore.Core;
using System.Net;

namespace SlepoffStore.WebApi.Controllers
{
    [ApiController]
    [Route("api/sections")]
    public class SectionsController : Controller
    {
        private readonly IRepository _repository;

        public SectionsController(IRepository repository)
        {
            _repository = repository;
        }

        // GET: api/sections
        [HttpGet]
        public async Task<ApiResult<Section[]>> Get([UserFromHeader] string userName)
        {
            return new ApiResult<Section[]> { Data = await _repository.GetSections(userName) };
        }

        // GET: api/sections/{id}
        [HttpGet]
        [Route("{id}")]
        public async Task<ApiResult<Section>> GetById(long id, [UserFromHeader] string userName)
        {
            return new ApiResult<Section> { Data = await _repository.GetSection(id, userName) };
        }

        // GET: api/sections/extended
        [HttpGet]
        [Route("extended")]
        public async Task<ApiResult<SectionEx[]>> GetEx([UserFromHeader] string userName)
        {
            return new ApiResult<SectionEx[]> { Data = await _repository.GetSectionsEx(userName) };
        }

        // GET: api/sections/{sectionId}/entries
        [HttpGet]
        [Route("{sectionId}/entries")]
        public async Task<ApiResult<Entry[]>> GetEntries(long sectionId, [UserFromHeader] string userName)
        {
            return new ApiResult<Entry[]> { Data = await _repository.GetEntriesBySectionId(sectionId, userName) };
        }

        // POST: api/sections
        [HttpPost]
        public async Task<ApiResult<long>> Insert([FromBody] Section section, [UserFromHeader] string userName)
        {
            return new ApiResult<long>
            {
                Status = ApiResultStatus.OK,
                Data = await _repository.InsertSection(section, userName)
            };
        }
    }
}
