using System;
using System.Collections.Generic;
using System.Text;

namespace SlepoffStore.Core
{
    public interface IWinFormsRepository : IRepository
    {
        long InsertUISheet(UISheet sheet);
        UISheet[] GetUISheets();
        void UpdateUISheet(UISheet sheet);
        void DeleteUISheet(UISheet sheet);
    }
}
