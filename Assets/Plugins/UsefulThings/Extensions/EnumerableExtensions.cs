using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PM.UsefulThings.Extensions
{
	public static class CollectionExtensions
	{
		public static bool IsEmpty<T>(this T collection) where T : ICollection
		{
			return (collection == null) || (collection.Count == 0);
		}

		public static bool Find<T, V>(this Dictionary<T, V> collection, Predicate<V> predicate, out KeyValuePair<T, V> result) where V : class
		{
			if (collection.IsEmpty())
			{
				result = default(KeyValuePair<T, V>);
				return false;
			}

			foreach (var pair in collection)
			{
				if (predicate(pair.Value))
				{
					result = pair;
					return true;
				}
			}

			result = default(KeyValuePair<T, V>);
			return false;
		}

		public static bool Any<T, V>(this Dictionary<T, V> collection, Predicate<V> predicate)
		{
			if (collection.IsEmpty())
				return false;

			foreach (var pair in collection)
			{
				if (predicate(pair.Value))
				{
					return true;
				}
			}

			return false;
		}
		
		private static void Swap<T>(this IList<T> collection, int a, int b)
		{
			var tmp = collection[a];
			collection[a] = collection[b];
			collection[b] = tmp;
		}

		public static void Sort<T>(this IList<T> collection, Func<T, int> compareFunc, int firstIndex = 0, int lastIndex = -1)
		{
			if (lastIndex < 0)
			{
				lastIndex = collection.Count - 1;
			}
			if (firstIndex >= lastIndex)
			{
				return;
			}

			int middleIndex = (lastIndex - firstIndex) / 2 + firstIndex, currentIndex = firstIndex;

			collection.Swap(firstIndex, middleIndex);

			for (int i = firstIndex + 1; i <= lastIndex; ++i)
			{
				if (compareFunc((T)collection[i]) <= compareFunc((T)collection[firstIndex]))
				{
					collection.Swap(++currentIndex, i);
				}
			}

			collection.Swap(firstIndex, currentIndex);

			Sort(collection, compareFunc, firstIndex, currentIndex - 1);
			Sort(collection, compareFunc, currentIndex + 1, lastIndex);
		}

		public static void SortByDescending<T>(this IList<T> collection, Func<T, int> compareFunc, int firstIndex = 0, int lastIndex = -1)
		{
			if (lastIndex < 0)
			{
				lastIndex = collection.Count - 1;
			}
			if (firstIndex >= lastIndex)
			{
				return;
			}

			int middleIndex = (lastIndex - firstIndex) / 2 + firstIndex, currentIndex = firstIndex;

			collection.Swap(firstIndex, middleIndex);

			for (int i = firstIndex + 1; i <= lastIndex; ++i)
			{
				if (compareFunc((T)collection[i]) >= compareFunc((T)collection[firstIndex]))
				{
					collection.Swap(++currentIndex, i);
				}
			}

			collection.Swap(firstIndex, currentIndex);

			SortByDescending(collection, compareFunc, firstIndex, currentIndex - 1);
			SortByDescending(collection, compareFunc, currentIndex + 1, lastIndex);
		}
	}
}