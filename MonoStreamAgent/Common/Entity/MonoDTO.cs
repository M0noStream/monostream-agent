﻿namespace MonoStreamAgent.Common
{
    /// <summary>
    /// This is the in between object which each message from any source will be casted into before being delivered to it's destination.
    /// </summary>
    public class MonoDTO
    {
        public DataPlatformEnum SourceType { get; set; }
        public string Data { get; set; }

        public override string ToString()
        {
            return $"SourceType: {SourceType}\nData: {Data}";
        }
    }
}
