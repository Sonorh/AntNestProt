using System;
using UnityEngine;

namespace PM.UsefulThings
{
    public class Singleton<T> where T : new()
    {
        private static T _instance;

        private static object _lock = new object();

        public static T Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        //instance not found. create new instance
                        _instance = new T();

                        Debug.Log("[Singleton] An instance of " + typeof(T) +
                            " is needed, so '" + _instance + "' was created.");
                    }

                    return _instance;
                }
            }
        }
    }
}