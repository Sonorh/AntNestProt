using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PM.UsefulThings
{
    public class SimplePool<T> where T : MonoBehaviour
    {
        public Transform Parent { get; set; }
        public bool IsReady = true;
        public int Reserve
        {
            get
            {
                return Stack.Count;
            }
        }

        private Stack<T> Stack = new Stack<T>();
        private T Sample { get; set; }

        private SimplePool()
        {
            //closed
        }

        public SimplePool(T _sample)
        {
            Sample = _sample;
            Parent = (new GameObject()).transform;
            Parent.name = "Pool of " + (typeof(T)).ToString();
        }


        public T Pull()
        {
            T result;
            if (Stack.Count > 0)
            {
                result = Stack.Pop();
            }
            else
            {
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
            Stack.Push(item);
        }

        public void Clear()
        {
            foreach (T item in Stack)
            {
                GameObject.Destroy(item.gameObject);
            }
            Stack = new Stack<T>();
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
            while (Stack.Count < quantity)
            {
                item = GameObject.Instantiate(Sample);
                item.transform.parent = Parent;
                item.gameObject.SetActive(false);
                Stack.Push(item);
                yield return null;
            }
            IsReady = true;
        }
    }
}