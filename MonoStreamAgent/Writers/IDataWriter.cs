using MonoStreamAgent.Common;

namespace MonoStreamAgent.Writers
{
    public interface IDataWriter
    {
        public void Write(MonoDTO data);
    }
}
