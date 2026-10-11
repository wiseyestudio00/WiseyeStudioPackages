using UnityEngine;

namespace WiseyeStudio.Localization
{
    public interface ILocalizedValue<out T>
    {
        T Get();
        T Get(SystemLanguage language);
    }
}
