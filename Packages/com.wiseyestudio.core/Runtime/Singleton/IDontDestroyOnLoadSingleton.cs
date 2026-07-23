using UnityEngine;

namespace WiseyeStudio.Singleton
{
    public interface IDontDestroyOnLoadSingleton
    {
        bool DestroyWhenRestart { get; }
        public void TryCreateInstance();
        GameObject GetInstanceGameObject();
    }
}
