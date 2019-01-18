using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PM.UsefulThings
{
    public class PoolForGO
    {
        public bool IsReady = true;
        public Transform Parent { get; set; }
        public int Reserve
        {
            get
            {
                return Stack.Count;
            }
        }

        protected Stack<GameObject> Stack = new Stack<GameObject>();
        protected GameObject Sample { get; set; }

        public PoolForGO()
        {
            if (Sample == null)
                Sample = new GameObject();
            if (Parent == null)
                Parent = (new GameObject()).transform;

            Parent.name = "Pool of game objects";
        }

        public GameObject Pull()
        {
            GameObject result;
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

        public void Push(GameObject item)
        {
            item.gameObject.SetActive(false);
            item.transform.parent = Parent;
            Stack.Push(item);
        }

        public void Clear()
        {
            foreach (GameObject item in Stack)
            {
                GameObject.Destroy(item.gameObject);
            }
            Stack = new Stack<GameObject>();
        }

        public void PrepareReserve(int quantity)
        {
            if (!IsReady)
                return;

            IsReady = false;
            CoroutineManagerUT.Instance.StartCoroutine(SpawnReserve(quantity));
        }

        private IEnumerator SpawnReserve(int quantity)
        {
            GameObject item;
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