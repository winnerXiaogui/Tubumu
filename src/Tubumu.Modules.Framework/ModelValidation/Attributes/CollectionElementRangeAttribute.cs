using System;
using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace Tubumu.Modules.Framework.ModelValidation.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class CollectionElementRangeAttribute : ValidationAttribute
    {
        public int Minimum { get; }
        public int Maximum { get; }

        public CollectionElementRangeAttribute(int minimum, int maximum)
        {
            Minimum = minimum;
            Maximum = maximum;
        }

        public override bool IsValid(object value)
        {
            if (value is ICollection list)
            {
                return list.Count >= Minimum && list.Count <= Maximum;
            }
            return false;
        }
    }
}
