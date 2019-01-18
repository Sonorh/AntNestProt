using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PM.UsefulThings
{
    /// <summary>
    /// it doesn't create instance if there isn't any.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SoftMonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;

        private static object _lock = new object();

		public static bool HasInstance
		{
			get
			{
				return _instance != null;
			}
		}

        public static T Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        //trying to find created instance
                        _instance = (T)FindObjectOfType(typeof(T));

                        if (FindObjectsOfType(typeof(T)).Length > 1)
                        {
                            Debug.LogError("[Singleton] Something went really wrong " +
                                " - there should never be more than 1 singleton!");
                            return _instance;
                        }
                    }

                    return _instance;
                }
            }
        }

        protected virtual void OnDestroy()
        {
            isInstance = false;
            _instance = null;
        }

        protected bool isInstance { get; set; }
        /// <summary>
        /// If we will need to override Awake we must be sure it won't do anything
        /// if it isn't an instance. 
        /// </summary>
        virtual protected void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;
                isInstance = true;
                this.name = "(softsingleton) " + typeof(T).ToString();
            }
            else if (_instance != this)
            {
                isInstance = false;
                Destroy(this.gameObject);
            }
        }
    }
}