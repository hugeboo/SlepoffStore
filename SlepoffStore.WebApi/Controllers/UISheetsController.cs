using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SlepoffStore.Core;
using System.Net;

namespace SlepoffStore.WebApi.Controllers
{
    [ApiController]
    [Route("api/uisheets")]
    public class UISheetsController : Controller
    {
        private readonly IRepository _repository;

        public UISheetsController(IRepository repository)
        {
            _repository = repository;
        }

        // POST: api/uisheets
        [HttpPost]
        public ApiResult<long> Insert([FromBody] UISheet sheet, 
            [UserFromHeader] string userName, [DeviceFromHeader] string deviceName)
        {
            return new ApiResult<long>
            {
                Status = ApiResultStatus.OK,
                Data = _repository.InsertUISheet(sheet, userName, deviceName)
            };
        }

        // GET: api/uisheets
        [HttpGet]
        public ApiResult<UISheet[]> Get([UserFromHeader] string userName, [DeviceFromHeader] string deviceName)
        {
            return new ApiResult<UISheet[]> { Data = _repository.GetUISheets(userName, deviceName) };
        }

        // POST: api/uisheets/update
        [HttpPost]
        [Route("update")]
        public ApiResult Update([FromBody] UISheet sheet, [UserFromHeader] string userName)
        {
            _repository.UpdateUISheet(sheet, userName);
            return new ApiResult
            {
                Status = ApiResultStatus.OK
            };
        }

        // DELETE: api/uisheets
        [HttpDelete]
        public ApiResult Delete([FromBody] UISheet sheet, [UserFromHeader] string userName)
        {
            _repository.DeleteUISheet(sheet, userName);
            return new ApiResult
            {
                Status = ApiResultStatus.OK
            };
        }
    }
}
