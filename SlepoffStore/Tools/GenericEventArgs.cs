using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlepoffStore.Tools
{
    public sealed class GenericEventArgs<T> : EventArgs
    {
        public T Data { get; set; }

        public GenericEventArgs(T data)
        {
            Data = data;
        }
    }
}
