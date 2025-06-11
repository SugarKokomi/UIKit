// #define QFRAMEWORK

using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

#if QFRAMEWORK
using QFramework;
#endif

namespace UI
{
    internal sealed class UIManager : Singleton<UIManager>
    {
        private IUIPrefabLoader assetLoader;
        public IUIPrefabLoader AssetLoader
        {
            get
            {
                assetLoader ??= new DefaultUIPrefabLoader();
                return assetLoader;
            }
            set
            {
                if (value != null)
                {
                    assetLoader = value;
                    Debug.Log("UI资源加载器设置成功！");
                }
                else Debug.LogWarning("不可设置空的资源加载器");
            }
        }
        public EasyEvent<PanelBase> onOpenPanel, onClosePanel;

        #region 普通数据和初始化函数
        /// <summary> 缓存UI预制体的容器 </summary>
        private Dictionary<string, GameObject> panelPrefabs;
        /// <summary> 根据类型分类的，开启的所有UI的容器 </summary>
        private Dictionary<string, List<PanelBase>> panels;
        /// <summary> 根据开启时间连接的，开启的所有UI的容器 </summary>
        private LinkedList<PanelBase> fakeOpenPanels;
        /// <summary> UI出入栈的容器(装暂时入栈隐藏的UI) </summary>
        private Stack<PanelBase> uiStack;
        public UIRoot UIRoot => UIRoot.Instance;
        public override void OnSingletonInit()
        {
            panelPrefabs = new Dictionary<string, GameObject>();
            panels = new Dictionary<string, List<PanelBase>>();
            fakeOpenPanels = new LinkedList<PanelBase>();
            uiStack = new Stack<PanelBase>();

            onOpenPanel = new EasyEvent<PanelBase>();
            onClosePanel = new EasyEvent<PanelBase>();
        }
        private GameObject GetUIPrefab(string panelName)
        {
            if (!panelPrefabs.TryGetValue(panelName, out var uiPanelGO) || !uiPanelGO)//如果没有缓存Prefab，或者存储的Prefab是null
            {
                uiPanelGO = AssetLoader.Load(panelName);
                AddOrSetValue(panelPrefabs, panelName, uiPanelGO);
            }
            if (!uiPanelGO)//如果还是没有加载到，再尝试加载一个隐藏在UIRoot之下的，未编入资源预制体的UI
            {
                foreach (UILayer layer in Enum.GetValues(typeof(UILayer)))
                {
                    var panel_ = UIRoot.GetLayer(layer).Find(panelName);
                    if (panel_)
                    {
                        uiPanelGO = panel_.gameObject;
                        AddOrSetValue(panelPrefabs, panelName, uiPanelGO);
                        break;
                    }
                }
            }
            if (uiPanelGO) return uiPanelGO;
            else throw new Exception($"未能加载到UI资源> {panelName} <");//如果还是没有加载到，报错。
        }
        /// <summary>
        /// 除了白名单之外，清除所有通过Resource或AB包加载的预制体
        /// </summary>
        /// <param name="whiteList"></param>
        internal void ClearUIPrefabInCache(IEnumerable<string> whiteList)
        {
            HashSet<string> blackList;
            //由于可能存在循环内增删迭代器元素，因而拆开循环，第一遍循环先筛选要移除的预制体名字
            if (whiteList != null)
            {
                blackList = new HashSet<string>();
                foreach (var kvp in panelPrefabs)
                {
                    if (whiteList.Contains(kvp.Key)) continue;
                    blackList.Add(kvp.Key);
                }
            }
            else
            {
                // blackList = panelPrefabs.Keys.ToHashSet();
                blackList = new HashSet<string>();
                foreach (var key in panelPrefabs.Keys)
                    blackList.Add(key);
            }
            foreach (var uiNameToMove in blackList)
            {
                if (panelPrefabs.TryGetValue(uiNameToMove, out var uiPanelGO))
                {
                    panelPrefabs[uiNameToMove] = null;
                    panelPrefabs.Remove(uiNameToMove);
                    assetLoader.Unload(uiPanelGO);
                }
            }
        }
        private void AddUIGameObjectInCache(PanelBase panelBase)
        {
            string panelName = panelBase.GetType().Name;
            if (!panels.TryGetValue(panelName, out var panelCacheList))
            {
                panelCacheList = new List<PanelBase>();
                panels.Add(panelName, panelCacheList);
            }
            panelCacheList.Add(panelBase);
            fakeOpenPanels.AddLast(panelBase);
        }
        private void RemoveUIGameObjectInCache(PanelBase panelBase)
        {
            string panelName = panelBase.GetType().Name;
            panels[panelName].Remove(panelBase);
            fakeOpenPanels.Remove(panelBase);
        }
        #endregion

        #region OpenPanel 打开UI
        /// <summary> 通过泛型开启UI面板 </summary>
        public T OpenPanel<T>(PanelData data = null, UILayer uiLayer = UILayer.Normal)
            where T : PanelBase
            => OpenPanel(typeof(T), data, uiLayer) as T;
        /// <summary> 通过类Type开启UI面板 </summary>
        public PanelBase OpenPanel(Type type, PanelData data = null, UILayer uiLayer = UILayer.Normal)
            => OpenPanel(type.Name, data, uiLayer);
        /// <summary> 通过UI名字开启UI面板 </summary>
        public PanelBase OpenPanel(string panelName, PanelData data = null, UILayer uiLayer = UILayer.Normal)
        {
            GameObject uiGO = GameObject.Instantiate(GetUIPrefab(panelName));
            uiGO.name = panelName;
            uiGO.SetActive(true);
            uiGO.transform.SetParent(UIRoot.GetLayer(uiLayer));
            ResetRectTransform(uiGO.transform as RectTransform);

            var uiPanel = uiGO.GetComponent<PanelBase>();

            //if (uiPanel is PanelBase<PanelData> panelWithData) panelWithData.data = data;//双重继承会直接变为和基类不一样的类型
            uiPanel.SetData(data);
            InitPanelBase(uiPanel);

            return uiPanel;
        }
        internal void InitPanelBase(PanelBase uiPanel)
        {
            AddUIGameObjectInCache(uiPanel);
            uiPanel.isInit = true;
            uiPanel.Init();
            uiPanel.Open();
            onOpenPanel.Trigger(uiPanel);
        }
        #endregion

        #region  ClosePanel 关闭UI
        public void ClosePanel<T>() where T : PanelBase => ClosePanel(typeof(T));
        public void ClosePanel(Type type) => ClosePanel(type.Name);
        public void ClosePanel(string panelName) => ClosePanel(GetPanel(panelName));
        public void ClosePanel(PanelBase uiPanel)
        {
            if (!uiPanel) return;
            ClearPanelBase(uiPanel);
            GameObject.Destroy(uiPanel.gameObject);
        }
        internal void ClearPanelBase(PanelBase uiPanel)
        {
            uiPanel.isInit = false;
            uiPanel.Close();
            RemoveUIGameObjectInCache(uiPanel);
            onClosePanel.Trigger(uiPanel);
        }
        #endregion

        #region  GetPanel/GetPanels/TryGetPanel （尝试）获取一个类型的UI、一个类型的所有UI
        /// <summary> 返回最近打开的UIPanel </summary>
        public T GetPanel<T>() where T : PanelBase => GetPanel(typeof(T)) as T;
        public PanelBase GetPanel(Type type) => GetPanel(type.Name);
        public PanelBase GetPanel(string panelName)
        {
            if (panels.TryGetValue(panelName, out var panelCacheList) && panelCacheList.Count > 0)
                return panelCacheList.LastOrDefault();
            else
            {
                //寻找UI多数时候为找刚打开的，因此根据时间倒序遍历
                var panelBase = fakeOpenPanels.Last;
                while (panelBase != null && panelBase.Value)
                {
                    if (panelBase.Value.gameObject.name == panelName)
                        return panelBase.Value;
                    else panelBase = panelBase.Previous;
                }
            }
            Debug.LogWarning($"未能查询到> {panelName} <。确保你想要查询的UI存在。");
            return null;

        }

        public IEnumerable<PanelBase> GetPanels<T>() where T : PanelBase => GetPanels(typeof(T));
        public IEnumerable<PanelBase> GetPanels(Type type) => GetPanels(type.Name);
        public IEnumerable<PanelBase> GetPanels(string panelName)
        {
            if (panels.TryGetValue(panelName, out var panelCacheList) && panelCacheList != null)
                return panelCacheList;
            else
            {
                Debug.LogWarning($"未能查询到> {panelName} <。确保你想要查询的UI存在。");
                return null;
            }
        }

        public bool TryGetPanel<T>(out T panel) where T : PanelBase
        {
            panel = GetPanel<T>();
            return panel;
        }
        public bool TryGetPanel(Type type, out PanelBase panel)
        {
            panel = GetPanel(type);
            return panel;
        }
        public bool TryGetPanel(string panelName, out PanelBase panel)
        {
            panel = GetPanel(panelName);
            return panel;
        }
        #endregion

        #region ShowPanel 显示UI
        public void ShowPanel<T>() where T : PanelBase => ShowPanel(typeof(T));
        public void ShowPanel(Type type) => ShowPanel(type.Name);
        private void ShowPanel(string panelName) => ShowPanel(GetPanel(panelName));
        public void ShowPanel(PanelBase uiPanel)
        {
            if (!uiPanel) return;
            if (!uiPanel.gameObject.activeSelf)
            {
                uiPanel.gameObject.SetActive(true);
                uiPanel.Show();
            }
        }
        #endregion

        #region HidePanel 隐藏UI
        public void HidePanel<T>() where T : PanelBase => HidePanel(typeof(T));
        public void HidePanel(Type type) => HidePanel(type.Name);
        private void HidePanel(string panelName) => HidePanel(GetPanel(panelName));
        public void HidePanel(PanelBase uiPanel)
        {
            if (!uiPanel) return;
            if (uiPanel.gameObject.activeSelf)
            {
                uiPanel.gameObject.SetActive(false);
                uiPanel.Hide();
            }
        }
        #endregion

        #region UI Stack UI面板出入栈
        public void Push<T>() where T : PanelBase => Push(typeof(T));
        public void Push(Type type) => Push(type.Name);
        private void Push(string panelName) => Push(GetPanel(panelName));
        /// <summary> UI面板入栈并隐藏 </summary>
        public void Push(PanelBase uiPanel)
        {
            if (!uiPanel) return;
            HidePanel(uiPanel);
            uiStack.Push(uiPanel);
        }
        /// <summary> (根据UI开启时间)反复入栈，直到达成某条件，或者已没有打开的UI。返回满足这个条件的UI，或者最后入栈的不为空的UI </summary>
        public PanelBase PushUntil(Predicate<PanelBase> predicate)
        {
            PanelBase lastPanel = null;
            var listNode = fakeOpenPanels.Last;
            while (true)
            {
                if (!listNode.Value) return lastPanel;
                Push(listNode.Value);
                if (predicate(listNode.Value)) return listNode.Value;
                lastPanel = listNode.Value;
                listNode = listNode.Previous;
            }
        }
        /// <summary> UI面板出栈并显示 </summary>
        public PanelBase Pop()
        {
            if (uiStack.Count >= 1)
            {
                var uiPanel = uiStack.Pop();
                ShowPanel(uiPanel);
                return uiPanel;
            }
            return null;
        }
        /// <summary> (根据已入栈UI)反复出栈，直到达成某条件，或者空栈。返回满足这个条件的UI，或者最后出栈的不为空的UI </summary>
        public PanelBase PopUntil(Predicate<PanelBase> predicate)
        {
            PanelBase lastPanel = null;
            while (true)
            {
                var panel = Pop();
                if (!panel) return lastPanel; // 栈为空，返回最后一个panel
                if (predicate(panel)) return panel; // 满足条件，返回当前panel
                lastPanel = panel; // 记录最后一个panel
            }
        }
        #endregion

        #region 其他
        static void ResetRectTransform(RectTransform rect)
        {
            rect.anchoredPosition3D = Vector3.zero;
            rect.localEulerAngles = Vector3.zero;
            rect.localScale = Vector3.one;
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.sizeDelta = Vector2.zero;
        }
        /// <summary>
        /// 给字典添加值或者替换值
        /// <para>低版本C#中，无法使用添加或替换字典值的语法糖</para>
        /// </summary>
        /// <returns> 返回添加(true)还是替换(false) </returns>
        static bool AddOrSetValue<K, V>(Dictionary<K, V> dictionary, K key, V value)
        {
            if (dictionary.ContainsKey(key))
            {
                dictionary[key] = value;
                return false;
            }
            else
            {
                dictionary.Add(key, value);
                return true;
            }
        }
        #endregion
    }
}