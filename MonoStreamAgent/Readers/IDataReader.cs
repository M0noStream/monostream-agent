using MonoStreamAgent.Common;

namespace MonoStreamAgent.Readers
{
    public interface IDataReader
    {
        public MonoDTO Read();
    }
}
