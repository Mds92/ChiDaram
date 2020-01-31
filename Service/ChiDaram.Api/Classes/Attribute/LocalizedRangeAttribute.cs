using System.ComponentModel.DataAnnotations;
using ChiDaram.Common.Helper;
using ChiDaram.Common.Properties;

namespace ChiDaram.Api.Classes.Attribute
{
    internal class LocalizedRangeAttribute : RangeAttribute
    {
        public string ErrorMessageResourceId { get; set; }

        public LocalizedRangeAttribute(double minimum, double maximum) : base(minimum, maximum)
        {
            if (string.IsNullOrEmpty(ErrorMessageResourceId))
                ErrorMessage = Resources.ErrorMessageRangeValidationStringFormat;
            else
                ErrorMessage = ResourcesHelper.GetMessageFromResource(ErrorMessageResourceId);
        }
    }
}