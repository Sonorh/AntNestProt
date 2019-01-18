using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace PM.UsefulThings
{
	public class Funcs
	{
		public static List<Type> GetListOfRealizations<T>() where T : class
		{
			var collection = new List<Type>();
			foreach (Type type in Assembly.GetAssembly(typeof(T)).GetTypes().Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(T))))
			{
				collection.Add(type);
			}
			return collection;
		}

		public static T CreateInstance<T>(params object[] constructorArgs) where T : class
		{
			return (T)Activator.CreateInstance(typeof(T), constructorArgs);
		}

		public static T CreateMonoInstance<T>() where T : MonoBehaviour
		{
			var go = new GameObject
			{
				name = "Instance of " + typeof(T).ToString()
			};
			return go.AddComponent(typeof(T)) as T;
		}

		public static T AddMonoInstance<T>(GameObject go) where T : MonoBehaviour
		{
			return go.AddComponent(typeof(T)) as T;
		}
	}
}