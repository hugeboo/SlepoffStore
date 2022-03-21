using System;
using System.Collections.Generic;
using System.Text;

namespace SlepoffStore.Core
{
    public class ApiResult
    {
        public ApiResultStatus Status { get; set; } = ApiResultStatus.OK;
        public string Text { get; set; }

        public override string ToString()
        {
            return $"Status:{Status} Text:{Text}";
        }
    }

    public sealed class ApiResult<T> : ApiResult
    {
        public T Data { get; set; }
    }

    public enum ApiResultStatus
    {
        OK,
        Error
    }
}
