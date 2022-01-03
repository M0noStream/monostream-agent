using MonoStreamAgent.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoStreamAgent.Writers
{
    public interface IDataWriter
    {
        public void write(MonoDTO data);
    }
}
