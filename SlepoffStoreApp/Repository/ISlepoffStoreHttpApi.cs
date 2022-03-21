using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Refit;
using SlepoffStore.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlepoffStoreApp.Repository
{
    [Headers("Accept: application/json")]
    public interface ISlepoffStoreHttpApi
    {
        [Get("/api/sections")]
        Task<ApiResult<Section[]>> GetSections();
    }
}