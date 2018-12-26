using System;

namespace Tubumu.Modules.Framework.Extensions
{
    public static class GuidExtensions
    {
        public static bool IsNullOrEmpty(this Guid? source)
        {
            return source == null || source.Value == Guid.Empty;
        }
    }
}
