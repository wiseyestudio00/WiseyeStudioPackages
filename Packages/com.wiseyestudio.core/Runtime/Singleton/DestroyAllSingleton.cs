using System.Linq;
using UnityEngine;

namespace WiseyeStudio.Singleton
{
    public static class DestroyAllSingleton
    {
        public static void DestroyAll()
        {
            var singletons = Object.FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<IDontDestroyOnLoadSingleton>();
            foreach (var singleton in singletons)
            {
                if (singleton.GetInstanceGameObject() != null && singleton.DestroyWhenRestart)
                {
                    Debug.Log($"Destroy {singleton.GetInstanceGameObject().name}");
                    Object.Destroy(singleton.GetInstanceGameObject());
                }
            }
        }
    }
}