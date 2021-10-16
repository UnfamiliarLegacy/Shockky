using System.Data.Common;

namespace Shockky.Resources.Types
{
    public readonly record struct ResourceId(ResourceKind Kind, int Id);
}
