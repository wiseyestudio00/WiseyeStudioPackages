using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;

namespace WiseyeStudio.Singleton
{
    public class DontDestroyOnLoadSingleton<T> : MonoBehaviour, IDontDestroyOnLoadSingleton
        where T : MonoBehaviour
    {
        [Title("Singleton Setting")]
        [SerializeField] public bool _DestroyWhenRestart = true;
        
        public bool DestroyWhenRestart => _DestroyWhenRestart;

        public static T Instance;
        public void TryCreateInstance()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this as T;
            DontDestroyOnLoad(gameObject);
        }

        public GameObject GetInstanceGameObject()
        {
            return Instance.gameObject;
        }

        void Awake()
        {
            TryCreateInstance();
        }
    }
}
