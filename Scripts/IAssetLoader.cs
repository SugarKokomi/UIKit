using UnityEngine;

#if ODIN_INSPECTOR
using System.Collections.Generic;
#endif

namespace UI
{
    public interface IAssetLoader
    {
        GameObject Load(string prefabName);
        void Unload(GameObject prefab);

#if ODIN_INSPECTOR
        IEnumerable<string> GetAllUIPrefabName();//用于给Odin在GUI提供不卸载资源的白名单列表
#endif

    }
}