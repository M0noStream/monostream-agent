using MonoStreamAgent.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoStreamAgent.Readers
{
    public interface IDataReader
    {
        public MonoDTO Read();
    }
}
