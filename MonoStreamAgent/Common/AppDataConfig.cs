using MonoStreamAgent.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoStreamAgent
{
    public class AppData
    {
        public Source Source { get; set; }
        public Destination Destination { get; set; }
    }

    public class Source
    {
        public DataPlatformEnum? TypeName { get; set; }
        public string Cluster { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string SourceName { get; set; }
    }

    public class Destination
    {
        public DataPlatformEnum? TypeName { get; set; }
        public string Cluster { get; set; }
        public string Vhost { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Exchange { get; set; }
        public string SourceName { get; set; }

    }
}
