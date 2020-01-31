using System;

namespace ChiDaram.Common.Classes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ResourceAttribute : Attribute
    {
        public string ResourceId { get; set; }
    }
}
