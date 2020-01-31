using System.ComponentModel.DataAnnotations;
using ChiDaram.Common.Helper;
using ChiDaram.Common.Properties;

namespace ChiDaram.Api.Classes.Attribute
{
    internal class LocalizedRequiredAttribute : RequiredAttribute
    {
        public LocalizedRequiredAttribute(string resourceId = "")
        {
            if (string.IsNullOrEmpty(resourceId))
                ErrorMessage = Resources.ErrorMessageRequiredValidationStringFormat;
            else
                ErrorMessage = ResourcesHelper.GetMessageFromResource(resourceId);
        }
    }
}
