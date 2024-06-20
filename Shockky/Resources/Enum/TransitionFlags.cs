﻿namespace Shockky.Resources.Enum;

[Flags]
public enum TransitionFlags : byte
{
    None,
    EntireStage = 1 << 0,
    Standard = 1 << 1
}
