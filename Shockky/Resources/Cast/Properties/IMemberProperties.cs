using Shockky.IO;

namespace Shockky.Resources.Cast
{
    public interface IMemberProperties
    {
        int GetBodySize();
        void WriteTo(ShockwaveWriter output);
    }
}
