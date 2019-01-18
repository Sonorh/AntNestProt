using UnityEngine;
using System;

namespace PM.UsefulThings.Extensions
{
	public static class Tools
	{
		public static bool[] PreCache = new bool[100];
		public static int[] PreCacheInt = new int[100];
	}

	public static class MutableExtensions
	{
		// Fisher Yates Shuffle
		public static void Shuffle<T>(this T[] array)
		{
			if (array.IsEmpty())
			{
				return;
			}

			for (var i = 0; i < array.Length; ++i)
			{
				var r = UnityEngine.Random.Range(i, array.Length);
				var tmp = array[i];
				array[i] = array[r];
				array[r] = tmp;
			}
		}

		public static void MapMut<T>(this T[] array, Func<T, T> mutator)
		{
			if (array.IsEmpty())
			{
				return;
			}

			for (int i = 0; i < array.Length; ++i)
			{
				array[i] = mutator(array[i]);
			}
		}

		public static bool ReplaceFirst<T>(this T[] array, T source, T dest)
		{
			if (array.IsEmpty())
			{
				return false;
			}

			var index = Array.IndexOf(array, source);
			if (index != -1)
			{
				array[index] = dest;
				return true;
			}

			return false;
		}

		public static bool ReplaceFirst<T>(this T[] array, T source, T dest, out int index)
		{
			index = -1;
			if (array.IsEmpty())
			{
				return false;
			}

			index = Array.IndexOf(array, source);
			if (index != -1)
			{
				array[index] = dest;
				return true;
			}

			return false;
		}

		public static void Swap<T>(this T[] array, int i, int j)
		{
			if (array.IsEmpty())
			{
				return;
			}

			T tmp = array[i];
			array[i] = array[j];
			array[j] = tmp;
		}
	}

	// "immutable" section can result one of it's arguments. So, at this point it have no pure immutable here. 
	// WARNING: Need to be rewritten if some multi thread planned. 
	public static class ImmutableExtensions
	{
		public static T[] Add<T>(this T[] lhs, T[] rhs)
		{
			if (rhs.IsEmpty())
			{
				return (lhs == null) ? new T[0] : lhs;
			}

			if (lhs.IsEmpty())
			{
				return (rhs == null) ? new T[0] : rhs;
			}

			lhs = Resize(lhs, lhs.Length + rhs.Length);
			rhs.CopyTo(lhs, lhs.Length - rhs.Length);
			return lhs;
		}

		public static T[] Add<T>(this T[] lhs, T rhs)
		{
			if (lhs.IsEmpty())
			{
				return new T[1] { rhs };
			}

			lhs = Resize(lhs, lhs.Length + 1);
			lhs[lhs.Length - 1] = rhs;
			return lhs;
		}

		public static int Count<T>(this T[] array, Predicate<T> predicate)
		{
			if (array.IsEmpty())
			{
				return 0;
			}

			var count = 0;
			for (int i = 0; i < array.Length; ++i)
			{
				if (predicate(array[i]))
				{
					++count;
				}
			}
			return count;
		}

		private static int CountInternal<T>(this T[] array, Predicate<T> predicate)
		{
			if (array.IsEmpty())
			{
				return 0;
			}

			if (Tools.PreCache.Length < array.Length)
			{
				Debug.LogError("ArrayExtension: PreCache depleted, re-allocate!");
				Tools.PreCache = Tools.PreCache.Resize(array.Length);
			}

			var count = 0;
			for (int i = 0; i < array.Length; ++i)
			{
				Tools.PreCache[i] = predicate(array[i]);
				if (Tools.PreCache[i])
				{
					++count;
				}
			}
			return count;
		}

		private static int CountInternal<T>(this T[] array, Func<T, int, bool> predicate)
		{
			if (array.IsEmpty())
			{
				return 0;
			}

			if (Tools.PreCache.Length < array.Length)
			{
				Debug.LogError("ArrayExtension: PreCache depleted, re-allocate!");
				Tools.PreCache = Tools.PreCache.Resize(array.Length);
			}

			var count = 0;
			for (int i = 0; i < array.Length; ++i)
			{
				Tools.PreCache[i] = predicate(array[i], i);
				if (Tools.PreCache[i])
				{
					++count;
				}
			}
			return count;
		}

		public static int CountNotNull<T>(this T[] array, Predicate<T> predicate)
		{
			if (array.IsEmpty())
			{
				return 0;
			}

			var count = 0;
			for (int i = 0; i < array.Length; ++i)
			{
				if ((array[i] != null) && predicate(array[i]))
				{
					++count;
				}
			}
			return count;
		}

		public static int Count<T>(this T[] array, Func<T, int, bool> predicate)
		{
			if (array.IsEmpty())
			{
				return 0;
			}

			var count = 0;
			for (int i = 0; i < array.Length; ++i)
			{
				if (predicate(array[i], i))
				{
					++count;
				}
			}
			return count;
		}

		public static bool Contains<T>(this T[] array, T value)
		{
			if (array.IsEmpty())
			{
				return false;
			}

			return (Array.IndexOf(array, value) != -1);
		}

		public static E[] Convert<T, E>(this T[] array, Converter<T, E> convertor)
		{
			if (array.IsEmpty())
			{
				return new E[0];
			}

			return Array.ConvertAll(array, convertor);
		}

		public static T[] Drop<T>(this T[] array, int count)
		{
			if (count <= 0)
			{
				return (array == null) ? new T[0] : array;
			}

			if ((array.IsEmpty()) || (count >= array.Length))
			{
				return new T[0];
			}

			var result = new T[array.Length - count];
			Array.Copy(array, count, result, 0, array.Length - count);
			return result;
		}

		public static T[] DropRight<T>(this T[] array, int count)
		{
			if (count <= 0)
			{
				return (array == null) ? new T[0] : array;
			}

			if ((array.IsEmpty()) || (count >= array.Length))
			{
				return new T[0];
			}

			var result = new T[array.Length - count];
			Array.Copy(array, 0, result, 0, array.Length - count);
			return result;
		}

		public static bool Exists<T>(this T[] array, Predicate<T> predicate)
		{
			if (array.IsEmpty())
			{
				return false;
			}

			return Array.Exists(array, predicate);
		}

		public static T[] Filter<T>(this T[] array, Predicate<T> predicate)
		{
			if (array.IsEmpty())
			{
				return new T[0];
			}

			return Array.FindAll(array, predicate);
		}

		public static T[] FilterNotNull<T>(this T[] array, Predicate<T> predicate)
		{
			if (array.IsEmpty())
			{
				return new T[0];
			}

			return Array.FindAll(array, el => (el != null) && predicate(el));
		}

		public static T[] Filter<T>(this T[] array, Func<T, int, bool> predicate)
		{
			if (array.IsEmpty())
			{
				return new T[0];
			}

			var count = array.CountInternal(predicate);
			var result = new T[count];
			var j = 0;
			if (count > 0)
			{
				for (int i = 0; i < array.Length; ++i)
				{
					if (Tools.PreCache[i])
					{
						result[j] = array[i];
						++j;
					}
				}
			}

			return result;
		}

		public static T First<T>(this T[] array)
		{
			if (array.IsEmpty())
			{
				return default(T);
			}

			return array[0];
		}

		public static T Find<T>(this T[] array, Predicate<T> predicate)
		{
			if (array.IsEmpty())
			{
				return default(T);
			}

			return Array.Find(array, predicate);
		}

		public static T[] FindAll<T>(this T[] array, Predicate<T> predicate)
		{
			if (array.IsEmpty())
			{
				return new T[0];
			}

			return Array.FindAll(array, predicate);
		}

		public static int IndexOf<T>(this T[] array, T value)
		{
			if (array.IsEmpty())
			{
				return -1;
			}

			return Array.IndexOf(array, value);
		}

		public static int IndexOf<T>(this T[] array, Predicate<T> predicate)
		{
			if (array.IsEmpty())
			{
				return -1;
			}

			for (int i = 0; i < array.Length; i++)
			{
				if (predicate(array[i]))
				{
					return i;
				}
			}
			return -1;
		}

		public static int FindLast<T>(this T[] array, T value)
		{
			if (array.IsEmpty())
			{
				return -1;
			}

			return Array.LastIndexOf(array, value);
		}

		public static bool ForAll<T>(this T[] array, Predicate<T> predicate)
		{
			if (array.IsEmpty())
			{
				return false;
			}

			return Array.TrueForAll(array, predicate);
		}

		public static void ForEach<T>(this T[] array, Action<T> action)
		{
			if (array.IsEmpty())
			{
				return;
			}

			for (int i = 0; i < array.Length; ++i)
			{
				action(array[i]);
			}
		}

		public static void ForEach<T>(this T[] array, Action<T, int> action)
		{
			if (array.IsEmpty())
			{
				return;
			}

			for (int i = 0; i < array.Length; ++i)
			{
				action(array[i], i);
			}
		}

		public static void ForEachNotNull<T>(this T[] array, Action<T> action)
		{
			if (array.IsEmpty())
			{
				return;
			}

			for (int i = 0; i < array.Length; ++i)
			{
				if (array[i] != null)
				{
					action(array[i]);
				}
			}
		}

		public static int GetLength<T>(this T[] array)
		{
			return (array == null) ? 0 : array.Length;
		}

		public static bool IsEmpty<T>(this T[] array)
		{
			return (array == null) || (array.Length == 0);
		}

		public static bool IsNonEmpty<T>(this T[] array)
		{
			return (array != null) && (array.Length != 0);
		}

		public static bool SequenceEqual<T>(this T[] array, T[] target)
		{
			if (array.IsEmpty() && target.IsEmpty())
			{
				return true;
			}

			if (array.Length != target.Length)
			{
				return false;
			}

			for (int i = 0; i < array.Length; i++)
			{
				if (!array[i].Equals(target[i]))
				{
					return false;
				}
			}
			return true;
		}

		public static T[] MapImut<T>(this T[] array, Converter<T, T> mutator)
		{
			if (array.IsEmpty())
			{
				return new T[0];
			}

			return Array.ConvertAll(array, mutator);
		}

		public static T[] RemoveAt<T>(this T[] array, int index)
		{
			if ((array.IsEmpty()) || (index < 0) || (index >= array.Length))
			{
				return (array == null) ? new T[0] : array;
			}

			var result = new T[array.Length - 1];

			if (index > 0)
			{
				Array.Copy(array, 0, result, 0, index);
			}

			if (index < array.Length - 1)
			{
				Array.Copy(array, index + 1, result, index, array.Length - index - 1);
			}

			return result;
		}

		public static T[] RemoveAll<T>(this T[] array, T value) where T : class
		{
			if (array.IsEmpty())
			{
				return (array == null) ? new T[0] : array;
			}

			return Array.FindAll(array, (x) => x != value);
		}

		public static T[] RemoveAllS<T>(this T[] array, T value) where T : struct
		{
			if (array.IsEmpty())
			{
				return (array == null) ? new T[0] : array;
			}

			return Array.FindAll(array, (x) => !x.Equals(value));
		}

		public static T[] Resize<T>(this T[] array, int newSize)
		{
			if ((!array.IsEmpty()) && (newSize == array.Length))
			{
				return array;
			}

			if (newSize <= 0)
			{
				return new T[0];
			}

			var result = new T[newSize];
			if (array.IsNonEmpty())
			{
				Array.Copy(array, 0, result, 0, Math.Min(newSize, array.Length));
			}
			return result;
		}

		public static T SelectRandom<T>(this T[] array, Predicate<T> predicate)
		{
			if (array.IsEmpty())
			{
				return default(T);
			}

			var count = array.CountInternal(predicate);
			if (count > 0)
			{
				if (Tools.PreCacheInt.Length < count)
				{
					Debug.LogError("ArrayExtension: PreCacheInt depleted, re-allocate!");
					Tools.PreCacheInt = Tools.PreCacheInt.Resize(array.Length);
				}

				for (int i = 0, j = 0; i < array.Length; ++i)
				{
					if (Tools.PreCache[i])
					{
						Tools.PreCacheInt[j] = i;
						++j;
					}
				}

				return array[Tools.PreCacheInt[UnityEngine.Random.Range(0, count)]];
			}

			return default(T);
		}

		public static int SelectRandomIndex<T>(this T[] array, Predicate<T> predicate)
		{
			if (array.IsEmpty())
			{
				return -1;
			}

			var count = array.CountInternal(predicate);
			if (count > 0)
			{
				if (Tools.PreCacheInt.Length < count)
				{
					Debug.LogError("ArrayExtension: PreCacheInt depleted, re-allocate!");
					Tools.PreCacheInt = Tools.PreCacheInt.Resize(array.Length);
				}

				for (int i = 0, j = 0; i < array.Length; ++i)
				{
					if (Tools.PreCache[i])
					{
						Tools.PreCacheInt[j] = i;
						++j;
					}
				}

				return Tools.PreCacheInt[UnityEngine.Random.Range(0, count)];
			}

			return -1;
		}

		public static T[][] Split<T>(this T[] array, Predicate<T> predicate)
		{
			if (array.IsEmpty())
			{
				return new T[2][] { new T[0], new T[0] };
			}

			var count = array.CountInternal(predicate);
			var result = new T[2][] { new T[count], new T[array.Length - count] };
			if (count > 0)
			{
				for (int i = 0, p = 0, n = 0; i < array.Length; ++i)
				{
					if (Tools.PreCache[i])
					{
						result[0][p] = array[i];
						++p;
					}
					else
					{
						result[1][n] = array[i];
						++n;
					}
				}
			}

			return result;
		}

		public static T[] SyncLength<T, E>(this T[] src, E[] dest)
		{
			var srcLength = src.GetLength();
			var dstLength = dest.GetLength();
			if (srcLength != dstLength)
			{
				if ((srcLength == 0) || (dstLength == 0))
				{
					src = new T[dstLength];
				}
				else
				{
					src = src.Resize(dstLength);
				}
			}
			return (src == null) ? new T[0] : src;
		}

		public static T[] Copy<T>(this T[] array)
		{
			if (array == null)
			{
				return null;
			}
			if (array.Length == 0)
			{
				return new T[0];
			}

			var copy = new T[array.Length];
			Array.Copy(array, copy, array.Length);
			return copy;
		}
	}
}