using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PM.UsefulThings
{
	public class DictionaryPool<K, T> where T : MonoBehaviour
	{
		public bool IsReady = true;
		public Transform Parent { get; set; }
		public T Sample { get; set; }

		// todo everything
		//private Dictionary<K, T> dictionary = new Dictionary<K, T>();
		private Stack<T> stack = new Stack<T>();

		public DictionaryPool()
		{
			if (Parent == null)
				Parent = (new GameObject()).transform;

			Parent.name = "Pool of monobehaviours";
		}

		public DictionaryPool(T sample)
		{
			if (Sample == null)
				Sample = sample;
			if (Parent == null)
				Parent = (new GameObject()).transform;

			Parent.name = "Pool of monobehaviours";
		}

		public T Pull()
		{
			T result;
			if (stack.Count > 0)
			{
				result = stack.Pop();
			}
			else
			{
				if (Sample == null)
				{
					Debug.LogError("Wrong call of pull. Sample is null.");
					return null;
				}

				result = GameObject.Instantiate(Sample);
				result.transform.parent = Parent;
			}
			result.gameObject.SetActive(true);
			return result;
		}

		public void Push(T item)
		{
			item.gameObject.SetActive(false);
			item.transform.parent = Parent;
			stack.Push(item);
		}

		public void Clear()
		{
			foreach (T item in stack)
			{
				GameObject.Destroy(item.gameObject);
			}
			stack = new Stack<T>();
		}

		public int GetReserve()
		{
			return stack.Count;
		}

		public void PrepareReserve(int quantity)
		{
			if (!IsReady)
				return;

			IsReady = false;
			Sample.StartCoroutine(SpawnReserve(quantity));
		}

		private IEnumerator SpawnReserve(int quantity)
		{
			T item;
			while (stack.Count < quantity)
			{
				if (Sample == null)
				{
					Debug.LogError("Wrong call of pull. Sample is null.");
					break;
				}

				item = GameObject.Instantiate(Sample);
				item.transform.parent = Parent;
				item.gameObject.SetActive(false);
				stack.Push(item);
				yield return null;
			}
			IsReady = true;
		}
	}
}