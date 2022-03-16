using System;
using System.Collections.Generic;
using System.Text;

namespace SlepoffStore.Core
{
    public class ApiResult
    {
        public ApiResultStatus Status { get; set; } = ApiResultStatus.OK;
        public string Text { get; set; }
    }

    public enum ApiResultStatus
    {
        OK,
        Error
    }

    public sealed class IdApiResult : ApiResult
    {
        public long Id { get; set; }
    }

    public sealed class SectionsApiResult : ApiResult
    {
        public Section[] Sections { get; set; }
    }
}
