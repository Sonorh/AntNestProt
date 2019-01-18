using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PM.UsefulThings
{
    public class CoroutineManagerUT : MonoSingleton<CoroutineManagerUT>
    {
        int counter = 0;
        private Dictionary<int, Coroutine> Coroutines = new Dictionary<int, Coroutine>();

        public int CoroutineRunningCount
        {
            get
            {
                return Coroutines.Count;
            }
        }

        public new int StartCoroutine(IEnumerator coroutine)
        {
            counter++;
            Coroutines.Add(counter, base.StartCoroutine(CoroutineWrapper(coroutine)));
            return counter;
        }

        private IEnumerator CoroutineWrapper(IEnumerator coroutine)
        {
            bool isActive = true;
            yield return coroutine;
            if (isActive)
            {
                isActive = false;
                Coroutines.Remove(counter);
            }
        }

        public void StopCoroutine(int id)
        {
            if (Coroutines.ContainsKey(id))
            {
                if (this != null)
                    StopCoroutine(Coroutines[id]);
                Coroutines.Remove(id);
            }
        }

        public new void StopAllCoroutines()
        {
            base.StopAllCoroutines();
            Coroutines.Clear();
        }
    }
}