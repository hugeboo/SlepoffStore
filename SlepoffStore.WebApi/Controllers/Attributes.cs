using Microsoft.AspNetCore.Mvc;

namespace SlepoffStore.WebApi.Controllers
{
    public sealed class UserFromHeaderAttribute : FromHeaderAttribute
    {
        public UserFromHeaderAttribute()
        {
            Name = "SS-UserName";
        }
    }

    public sealed class DeviceFromHeaderAttribute : FromHeaderAttribute
    {
        public DeviceFromHeaderAttribute()
        {
            Name = "SS-DeviceName";
        }
    }
}
