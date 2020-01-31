using System.ComponentModel;
using ChiDaram.Common.Helper;

namespace ChiDaram.Api.Classes.Attribute
{
	internal class LocalizedDisplayNameAttribute : DisplayNameAttribute
	{
		public LocalizedDisplayNameAttribute(string resourceId) : base(ResourcesHelper.GetMessageFromResource(resourceId)) { }
	}
}