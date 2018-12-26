using System;

namespace Tubumu.Modules.Framework.ActionResults
{
    public class DependencyJsonConverterGuid : DependencyJsonConverter<Guid>
    {
        public DependencyJsonConverterGuid(string propertyName, string equaValue) : base(propertyName, new Guid(equaValue))
        {
        }
    }
}
