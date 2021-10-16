using Shockky.Resources.Cast;

namespace Shockky.Resources
{
    public interface ICastMemberMediaChunk<TProperties>
        where TProperties : IMemberProperties
    {
        void PopulateMedia(TProperties castProperties);
    }
}
