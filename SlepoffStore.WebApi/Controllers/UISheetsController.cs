using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SlepoffStore.Core;
using SlepoffStore.Repository;
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
        public async Task<ApiResult<long>> Insert([FromBody] UISheet sheet, 
            [UserFromHeader] string userName, [DeviceFromHeader] string deviceName)
        {
            return new ApiResult<long>
            {
                Status = ApiResultStatus.OK,
                Data = await _repository.CreateUISheet(sheet, userName, deviceName)
            };
        }

        // GET: api/uisheets
        [HttpGet]
        public async Task<ApiResult<UISheet[]>> Get([UserFromHeader] string userName, [DeviceFromHeader] string deviceName)
        {
            return new ApiResult<UISheet[]> { Data = await _repository.ReadUISheets(userName, deviceName) };
        }

        // POST: api/uisheets/update
        [HttpPost]
        [Route("update")]
        public async Task<ApiResult> Update([FromBody] UISheet sheet, [UserFromHeader] string userName)
        {
            await _repository.UpdateUISheet(sheet, userName);
            return new ApiResult
            {
                Status = ApiResultStatus.OK
            };
        }

        // DELETE: api/uisheets
        [HttpDelete]
        public async Task<ApiResult> Delete([FromBody] UISheet sheet, [UserFromHeader] string userName)
        {
            await _repository.DeleteUISheet(sheet, userName);
            return new ApiResult
            {
                Status = ApiResultStatus.OK
            };
        }
    }
}
