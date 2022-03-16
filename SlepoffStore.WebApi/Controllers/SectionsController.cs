using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SlepoffStore.Core;
using System.Net;

namespace SlepoffStore.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SectionsController : Controller
    {
        private readonly IRepository _repository;

        public SectionsController(IRepository repository)
        {
            _repository = repository;
        }

        // GET: api/sections
        [HttpGet]
        public SectionsApiResult Get()
        {
            try
            {
                return new SectionsApiResult { Sections = _repository.GetSections() };
            }
            catch (Exception ex)
            {
                return new SectionsApiResult { Status = ApiResultStatus.Error, Text = ex.Message };
            }
        }

        // POST: api/sections
        [HttpPost]
        public IdApiResult Insert([FromBody] Section section)
        {
            try
            {
                return new IdApiResult
                {
                    Status = ApiResultStatus.OK,
                    Id = _repository.InsertSection(section)
                };
            }
            catch (Exception ex)
            {
                return new IdApiResult { Status = ApiResultStatus.Error, Text = ex.Message };
            }
        }
    }
}
