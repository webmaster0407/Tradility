using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tradility.Common.Utils
{
    public class EventArgs<T>
    {
        public T Value { get; set; }

        public EventArgs(T value)
        {
            Value = value;
        }
    }
}
