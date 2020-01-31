using System.ComponentModel.DataAnnotations;
using ChiDaram.Common.Helper;
using ChiDaram.Common.Properties;

namespace ChiDaram.Api.Classes.Attribute
{
    internal class LocalizedStringLengthAttribute : StringLengthAttribute
    {
        public string ErrorMessageResourceId { get; set; }

        public LocalizedStringLengthAttribute(int minimumLength, int maximumLength) : base(maximumLength)
        {
            MinimumLength = minimumLength;
            ErrorMessage = string.IsNullOrWhiteSpace(ErrorMessageResourceId) 
                ? Resources.ErrorMessageStringLengthValidationStringFormat 
                : ResourcesHelper.GetMessageFromResource(ErrorMessageResourceId);
        }
    }
}