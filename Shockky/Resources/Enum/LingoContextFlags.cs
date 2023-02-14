namespace Shockky.Resources;

[Flags]
public enum LingoContextFlags : short
{
    None = 0,
    NotEmptyMaybe = 1 << 0,
    RemoveFromEnvironment = 1 << 1,
    RIFF_CHUNK_INDEXES_CHANGED_MAYBE = 1 << 2,
    EVENT_HANDLERS_CHANGED_MAYBE = 1 << 3,
    AllowOutdatedLingo = 1 << 5,
    UpdateMovieEnabled = 1 << 6
}
