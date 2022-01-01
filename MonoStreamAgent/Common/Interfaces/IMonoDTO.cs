﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoStreamAgent.Common
{
    /// <summary>
    /// This is the in between object which each message from any source will be casted into before being delivered to it's destination.
    /// </summary>
    public interface IMonoDTO
    {
        public DataPlatformEnum SourceType { get; set; }
        public string data { get; set; }
    }
}