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
        public ApiResult<Section[]> Get([UserFromHeader] string userName)
        {
            return new ApiResult<Section[]> { Data = _repository.GetSections(userName) };
        }

        // GET: api/sections/{id}
        [HttpGet]
        [Route("{id}")]
        public ApiResult<Section> GetById(long id, [UserFromHeader] string userName)
        {
            return new ApiResult<Section> { Data = _repository.GetSection(id, userName) };
        }

        // GET: api/sections/extended
        [HttpGet]
        [Route("extended")]
        public ApiResult<SectionEx[]> GetEx([UserFromHeader] string userName)
        {
            return new ApiResult<SectionEx[]> { Data = _repository.GetSectionsEx(userName).ToArray() };
        }

        // GET: api/sections/{sectionId}/entries
        [HttpGet]
        [Route("{sectionId}/entries")]
        public ApiResult<Entry[]> GetEntries(long sectionId, [UserFromHeader] string userName)
        {
            return new ApiResult<Entry[]> { Data = _repository.GetEntriesBySectionId(sectionId, userName) };
        }

        // POST: api/sections
        [HttpPost]
        public ApiResult<long> Insert([FromBody] Section section, [UserFromHeader] string userName)
        {
            return new ApiResult<long>
            {
                Status = ApiResultStatus.OK,
                Data = _repository.InsertSection(section, userName)
            };
        }
    }
}
