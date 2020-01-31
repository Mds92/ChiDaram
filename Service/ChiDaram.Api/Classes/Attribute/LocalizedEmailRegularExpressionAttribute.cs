using System.ComponentModel.DataAnnotations;
using ChiDaram.Common.Helper;
using ChiDaram.Common.Properties;

namespace ChiDaram.Api.Classes.Attribute
{
	internal class LocalizedEmailRegularExpressionAttribute : RegularExpressionAttribute
    {
	    public LocalizedEmailRegularExpressionAttribute(string resourceId = "")
	        : base(@"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*")
	    {
	        if (string.IsNullOrEmpty(resourceId))
	            ErrorMessage = string.Format(Resources.ErrorMessageRequiredCorrectValueStringFormat, Resources.Email);
	        else
	            ErrorMessage = ResourcesHelper.GetMessageFromResource(resourceId);
	    }
    }
}