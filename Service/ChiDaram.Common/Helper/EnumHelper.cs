using System;
using System.Collections.Generic;
using System.Reflection;
using ChiDaram.Common.Classes;

namespace ChiDaram.Common.Helper
{
	public static class EnumHelper
	{
		private static readonly Type ResourceAttributeType = typeof(ResourceAttribute);
		public static List<KeyValuePair<int, string>> GetEnumItemsValues(Type enumType)
		{
			var keyValuePairs = new List<KeyValuePair<int, string>>();
			foreach (var item in Enum.GetValues(enumType))
			{
				var resourceId = item.ToString();
				var intValue = Convert.ToInt32(item);
				var fieldInfo = enumType.GetField(item.ToString());
			    if (fieldInfo.GetCustomAttribute(ResourceAttributeType, false) is ResourceAttribute resourceAttribute)
					resourceId = resourceAttribute.ResourceId;
				keyValuePairs.Add(new KeyValuePair<int, string>(intValue, resourceId));
			}
			return keyValuePairs;
		}
	}
}