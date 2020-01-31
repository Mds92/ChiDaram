using System.ComponentModel.DataAnnotations;
using ChiDaram.Common.Helper;

namespace ChiDaram.Api.Classes.Attribute
{
    internal class LocalizedCompareAttribute : CompareAttribute
    {
        public LocalizedCompareAttribute(string otherPropertyName, string errorMessageResourceId) : base(otherPropertyName)
        {
            if (!string.IsNullOrEmpty(errorMessageResourceId))
                ErrorMessage = ResourcesHelper.GetMessageFromResource(errorMessageResourceId);
        }
    }
}
