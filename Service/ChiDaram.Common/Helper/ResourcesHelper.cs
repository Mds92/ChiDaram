using System;
using System.Globalization;
using System.Resources;
using ChiDaram.Common.Properties;

namespace ChiDaram.Common.Helper
{
	public static class ResourcesHelper
	{
	    public static string CultureName { get; set; }

	    private static CultureInfo _currentCultureInfo;
	    public static CultureInfo CurrentCultureInfo
	    {
	        get
	        {
	            if (_currentCultureInfo == null) return _currentCultureInfo;
	            _currentCultureInfo= new CultureInfo(CultureName);
	            return _currentCultureInfo;
	        }
	    }

	    private static Type _resourceType;
	    private static Type ResourceType
		{
			get
			{
				if (_resourceType != null) return _resourceType;
				_resourceType = typeof (Resources);
				return _resourceType;
			}
		}

	    private static ResourceManager _resourceManager;
	    private static ResourceManager ResourceManager
		{
			get
			{
				if (_resourceManager != null) return _resourceManager;
				_resourceManager = new ResourceManager(ResourceType);
				return _resourceManager;
			}
		}

		public static string GetMessageFromResource(string resourceId)
		{
			var value = ResourceManager.GetString(resourceId, CurrentCultureInfo);
			if (string.IsNullOrEmpty(value)) return resourceId;
			return value;
		}
	}
}