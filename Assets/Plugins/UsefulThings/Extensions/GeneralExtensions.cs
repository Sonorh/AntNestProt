using UnityEngine;
using System;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Reflection;

namespace PM.UsefulThings.Extensions
{
	public static class GeneralExtensions
	{
		public static T TryParseEnum<T>(string fromValue, T defaultValue)
		{
			var type = typeof(T);
			if (!type.IsEnum)
			{
				Debug.LogError("Trying to parse enum to NOT a enum type " + type.Name);
				return defaultValue;
			}

			if (!Enum.IsDefined(type, fromValue))
				return defaultValue;

			return (T)Enum.Parse(type, fromValue);
		}

		public static DateTime MakeTime(int fromSecondsFromNow)
		{
			return DateTime.Now + new TimeSpan(0, 0, fromSecondsFromNow);
		}

		public static int GetMultiplayerDayNumber(this DateTime dateTime)
		{
			DateTime startingPoint = new DateTime(2015, 1, 1, 5, 0, 0);
			return (int)Math.Truncate((dateTime - startingPoint).TotalDays);
		}

		public static T1 With<T, T1>(this T callSite, Func<T, T1> selector) where T : class
		{
			if (callSite == null)
				return default(T1);

			return selector(callSite);
		}

		public static void Call(this Action action)
		{
			if (action != null)
			{
				action();
			}
		}

		public static void Call<T1, T2>(this Action<T1, T2> action, T1 param1, T2 param2)
		{
			if (action != null)
			{
				action(param1, param2);
			}
		}

		public static void Call<T1, T2, T3>(this Action<T1, T2, T3> action, T1 param1, T2 param2, T3 param3)
		{
			if (action != null)
			{
				action(param1, param2, param3);
			}
		}

		public static void SafeCall(this Action action)
		{
#if !UNITY_EDITOR && !DEV_BUILD
			try
			{
#endif
				if (action != null)
				{
					action();
				}
#if !UNITY_EDITOR && !DEV_BUILD
			}
			catch (Exception e)
			{
				Debug.LogError("Exception in callback: " + e);
			}
#endif
		}

		public static void Call<T>(this Action<T> action, T param)
		{
			if (action != null)
			{
				action(param);
			}
		}

		public static void SafeCall<T>(this Action<T> action, T param)
		{
#if !UNITY_EDITOR && !DEV_BUILD
			try
			{
#endif
				Call(action, param);
#if !UNITY_EDITOR && !DEV_BUILD
			}
			catch (Exception e)
			{
				Debug.LogError("Exception in callback: " + e);
			}
#endif
		}

		public static void SafeCall<T, R>(this Action<T, R> action, T param1, R param2)
		{
#if !UNITY_EDITOR && !DEV_BUILD
			try
			{
#endif
				if (action != null)
				{
					action(param1, param2);
				}
#if !UNITY_EDITOR && !DEV_BUILD
			}
			catch (Exception e)
			{
				Debug.LogError("Exception in callback: " + e.ToString());
			}
#endif
		}

		public static Type GetInitialType(this Type type)
		{
			var baseType = type;
			var objectType = typeof(object);
			if (baseType.BaseType != null)
			{
				while (baseType.BaseType != objectType)
				{
					baseType = baseType.BaseType;
				}
			}
			return baseType;
		}

		public static void UntilInitialType(this Type type, Action<Type> action)
		{
			var currentType = type;
			var objectType = typeof(object);
			if (currentType.BaseType != null)
			{
				while (currentType != objectType)
				{
					action(currentType);
					currentType = currentType.BaseType;
				}
			}
		}

		public static void UntilType<T>(this Type type, Action<Type> action)
		{
			var currentType = type;
			var targetType = typeof(T);
			var objectType = typeof(object);
			if (currentType.BaseType != null)
			{
				while (currentType != targetType && currentType != objectType)
				{
					action(currentType);
					currentType = currentType.BaseType;
				}
			}
		}

		/**
		 * Maps each element in collection of T to the value of R and returns collection of resulting values
		 */
		public static List<R> Map<T, R>(this ICollection<T> collection, Func<T, R> provider)
		{
			var result = new List<R>(collection.Count);
			if (provider == null)
				throw new ArgumentNullException("provider");

			foreach (T elem in collection)
			{
				result.Add(provider(elem));
			}

			return result;
		}

		public static R Reduce<T, R>(this ICollection<T> collection, R initialValue, Func<T, R, R> func)
		{
			if (func == null)
				throw new ArgumentNullException("func");

			R accum = initialValue;
			foreach (T elem in collection)
			{
				accum = func(elem, accum);
			}

			return accum;
		}

		public static bool HasAny<T>(this ICollection<T> collection, Func<T, bool> func)
		{
			if (func == null)
				throw new ArgumentNullException("func");

			foreach (T elem in collection)
			{
				if (func(elem))
					return true;
			}
			return false;
		}


		public static T AddAndReturn<T>(this ICollection<T> collection, T item)
		{
			collection.Add(item);
			return item;
		}

		public static T PopLast<T>(this IList<T> list)
		{
			var lastIndex = list.Count - 1;
			if (lastIndex < 0)
				return default(T);

			var popped = list[lastIndex];
			list.RemoveAt(lastIndex);
			return popped;
		}

		public static T Last<T>(this IList<T> list)
		{
			var lastIndex = list.Count - 1;
			if (lastIndex < 0)
				return default(T);

			return list[lastIndex];
		}

		public static List<T> GetRange<T>(this IList<T> list, int index, int count)
		{
			var maxCount = Math.Min(count, Math.Max(0, list.Count - index));
			var result = new List<T>(maxCount);
			for (int i = 0; i < maxCount; ++i)
			{
				result.Add(list[index + i]);
			}

			return result;
		}

		public static object MemberwiseClone(this object obj)
		{
			if (obj == null || obj.GetType().IsValueType)
				return obj;

			var type = obj.GetType();
			var result = Activator.CreateInstance(type);
			foreach (var field in type.GetFields(BindingFlags.Instance | BindingFlags.Public))
			{
				field.SetValue(result, field.GetValue(obj));
			}

			foreach (var property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
			{
				if (property.GetIndexParameters().Length > 0 || !property.CanRead || !property.CanWrite)
					continue;

				property.SetValue(result, property.GetValue(obj, null), null);
			}

			return result;
		}

		public static byte[] ReadFully(this Stream stream)
		{
			if (stream == null)
				return new byte[0];

			if (stream is MemoryStream)
			{
				var casted = (MemoryStream)stream;
				return casted.ToArray();
			}

			throw new Exception("You can remove this exception, but BEWARE: this code branch was not tested");
			/*
			using(MemoryStream ms = new MemoryStream((int)stream.Length))
			{
				const int bufferLength = 4 * 1024;
				byte[] buffer = new byte[bufferLength];

				int readCount;
				while((readCount = stream.Read(buffer, 0, bufferLength)) > 0)
				{
					ms.Write(buffer, 0, readCount);
				}

				return ms.ToArray();
			}
			*/
		}

		/// <summary>
		/// Reads next count bytes from stream. Throws in case of early end of stream.
		/// </summary>
		/// <returns>The read bytes.</returns>
		public static byte[] ForcedReadBytes(this Stream stream, int count)
		{
			if (count <= 0)
				return new byte[0];

			if (stream == null)
				throw new Exception("Null stream to read " + count + " bytes from");

			using (MemoryStream ms = new MemoryStream(count))
			{
				int bufferLength = 4 * 1024;
				if (count < bufferLength)
				{
					bufferLength = count;
				}

				byte[] buffer = new byte[bufferLength];

				int totalReadCount = 0;
				Func<int> getBytesCountToReadNext = () =>
				{
					var leftToRead = count - totalReadCount;
					return leftToRead < bufferLength ? leftToRead : bufferLength;
				};

				int readCount;
				while ((readCount = stream.Read(buffer, 0, getBytesCountToReadNext())) > 0)
				{
					ms.Write(buffer, 0, readCount);
					totalReadCount += readCount;
				}

				if (totalReadCount < count)
					throw new Exception("Not enough data in stream to read. Required " + count + ", available " + totalReadCount);

				return ms.ToArray();
			}
		}

		public static bool IncludesIndex<T>(this List<T> list, int index) where T : class
		{
			if (list.Count == 0)
			{
				return false;
			}

			if (index >= 0 && index < list.Count)
			{
				return true;
			}

			return false;
		}

		public static bool HasPrefix(this string s, string prefix)
		{
			if (s.Length < prefix.Length)
			{
				return false;
			}

			for (int i = 0; i < prefix.Length; i++)
			{
				if (s[i] != prefix[i])
				{
					return false;
				}
			}

			return true;
		}

		//public static string Zip(this string str)
		//{
		//	var bytes = Encoding.UTF8.GetBytes(str);
		//	var memoryStream = new MemoryStream();
		//	using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
		//	{
		//		gZipStream.Write(bytes, 0, bytes.Length);
		//	}
		//	memoryStream.Position = 0;

		//	var data = new byte[memoryStream.Length];
		//	memoryStream.Read(data, 0, data.Length);

		//	var buffer = new byte[data.Length + 4];
		//	Buffer.BlockCopy(data, 0, buffer, 4, data.Length);
		//	Buffer.BlockCopy(BitConverter.GetBytes(bytes.Length), 0, buffer, 0, 4);

		//	return Convert.ToBase64String(buffer);
		//}

		//public static string Unzip(this string str)
		//{
		//	var bytes = Convert.FromBase64String(str);
		//	using (var memoryStream = new MemoryStream())
		//	{
		//		var dataLength = BitConverter.ToInt32(bytes, 0);
		//		memoryStream.Write(bytes, 4, bytes.Length - 4);
		//		memoryStream.Position = 0;

		//		var buffer = new byte[dataLength];
		//		using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
		//		{
		//			gZipStream.Read(buffer, 0, buffer.Length);
		//		}

		//		return Encoding.UTF8.GetString(buffer);
		//	}
		//}

		public static bool FastSequenceEqual<T>(this IList<T> a, IList<T> b)
		{
			if ((a == null && b != null) || (a != null && b == null))
			{
				return false;
			}

			if (a == null && b == null)
			{
				return true;
			}

			if (a.Count != b.Count)
			{
				return false;
			}

			for (int i = 0; i < a.Count; i++)
			{
				if (!a[i].Equals(b[i]))
				{
					return false;
				}
			}

			return true;
		}

		public static bool HasParameter(this Animator animator, int nameHash)
		{
			for (int i = 0; i < animator.parameters.Length; i++)
			{
				if (animator.parameters[i].nameHash == nameHash)
				{
					return true;
				}
			}
			return false;
		}

		public static bool HasParameter(this Animator animator, string name)
		{
			for (int i = 0; i < animator.parameters.Length; i++)
			{
				if (animator.parameters[i].name == name)
				{
					return true;
				}
			}
			return false;
		}
	}
}