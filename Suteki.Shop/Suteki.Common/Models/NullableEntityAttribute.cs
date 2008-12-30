using System;

namespace Suteki.Common.Models
{
    /// <summary>
    /// Add this attribute to any entity property that can be null
    /// This is used by the validating binder to decide whether to raise an exception
    /// when an attempt is made to bind a child entity with Id == 0
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class NullableEntityAttribute : Attribute
    {
    }
}