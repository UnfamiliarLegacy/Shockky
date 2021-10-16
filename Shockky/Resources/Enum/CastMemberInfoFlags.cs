namespace Shockky.Resources
{
    [Flags]
    public enum CastMemberInfoFlags : int
    {
        None,
        LinkedFile   = 1 << 0,
        AutoHilite   = 1 << 1,
        PurgeNever   = 1 << 2, 
        PurgeLast    = 1 << 3,
        PurgeNext    = PurgeNever | PurgeLast,
        SoundOn      = 1 << 4,

        Property42   = 1 << 5,
        Property40   = 1 << 6,
    }
}
