// #define QFRAMEWORK
using System.Collections.Generic;
using UnityEngine;

#if QFRAMEWORK
using QFramework;
#endif

#if ODIN_INSPECTOR
using System.IO;
using Sirenix.OdinInspector;
#endif

namespace UI
{
    public class UIRoot : MonoSingleton<UIRoot>
    {
        private Dictionary<UILayer, Transform> m_layerCache;
        private Canvas m_canvas;
        public Canvas Canvas { get { if (!m_canvas) m_canvas = GetComponent<Canvas>(); return m_canvas; } }
        public Camera uiCamera => Canvas.worldCamera ? Canvas.worldCamera : Camera.main;
        public Transform GetLayer(UILayer layer)
        {
            if (!m_layerCache.TryGetValue(layer, out var ret) || !ret)
            {
                ret = this.transform.Find(layer.ToString());
                m_layerCache.Add(layer, ret);
            }
            return ret;
        }
        public override void OnSingletonInit()
        {
            base.OnSingletonInit();
            m_layerCache = new Dictionary<UILayer, Transform>();

            DontDestroyOnLoad(gameObject);
            DontDestroyOnLoad(uiCamera.gameObject);
        }
#if ODIN_INSPECTOR
        [ValueDropdown("GetUIPrefabNames")]
#endif
        [LabelText("预制体缓存白名单")]
        [Tooltip("UI预制体缓存白名单。自行时机执行会尝试清理释放一次资源。添加进去后不会清除对应的预制体缓存")]
        public List<string> whiteList;
        protected override void OnDestroy()
        {
            base.OnDestroy();
            m_layerCache.Clear();
            // UIKit.ClearUIPrefabInCache();
        }
#if UNITY_EDITOR && ODIN_INSPECTOR
        IEnumerable<string> GetUIPrefabNames() => UIManager.Instance.AssetLoader.GetAllUIPrefabName();
#endif
    }
}
