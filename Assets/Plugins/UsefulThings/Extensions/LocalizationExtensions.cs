using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PM.UsefulThings.Extensions
{
	public static class LocalizationExtensions
	{
		public static string Localized(this string key, bool disableWarning = false)
		{
			if (key == null)
			{
				return string.Empty;
			}

			key = key.Replace(" ", "_").ToLower();

			string result = TranslateManager(key);
			if (result != null)
			{
				return result;
			}

			return disableWarning ? key : "[!!!]" + key;
		}

		// todo find a real localization manager
		private static string TranslateManager(string key)
		{
			return key;
		}

		public static event System.Action OnLocaleChanged;
	}
}