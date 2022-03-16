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
        public ApiResult<Section[]> Get()
        {
            return new ApiResult<Section[]> { Data = _repository.GetSections() };
        }

        // GET: api/sections/extended
        [HttpGet]
        [Route("extended")]
        public ApiResult<SectionEx[]> GetEx()
        {
            return new ApiResult<SectionEx[]> { Data = _repository.GetSectionsEx().ToArray() };
        }

        // GET: api/sections/{sectionId}/entries
        [HttpGet]
        [Route("{sectionId}/entries")]
        public ApiResult<Entry[]> GetEntries(long sectionId)
        {
            return new ApiResult<Entry[]> { Data = _repository.GetEntriesBySectionId(sectionId) };
        }

        // POST: api/sections
        [HttpPost]
        public ApiResult<long> Insert([FromBody] Section section)
        {
            return new ApiResult<long>
            {
                Status = ApiResultStatus.OK,
                Data = _repository.InsertSection(section)
            };
        }
    }
}
