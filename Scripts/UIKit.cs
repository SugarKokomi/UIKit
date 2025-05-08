// #define QFRAMEWORK

using System;
using System.Collections.Generic;

#if QFRAMEWORK
using QFramework;
#endif

namespace UI
{
    public static class UIKit
    {
        public static void SetAssetLoader(IAssetLoader assetLoader) => UIManager.Instance.AssetLoader = assetLoader;
        public static void ClearUIPrefabInCache() => UIManager.Instance.ClearUIPrefabInCache(UIRoot.Instance.whiteList);

        public static EasyEvent<PanelBase> onOpenPanel => UIManager.Instance.onOpenPanel;
        public static EasyEvent<PanelBase> onClosePanel => UIManager.Instance.onClosePanel;

        /// <summary> 通过PanelBase的泛型类型打开一个UI </summary>
        public static T OpenPanel<T>(PanelData data = null, UILayer uiLayer = UILayer.Normal)
            where T : PanelBase, new()
            => UIManager.Instance.OpenPanel<T>(data, uiLayer);
        /// <summary> 通过PanelBase的类型打开一个UI </summary>
        public static PanelBase OpenPanel(Type type, PanelData data = null, UILayer uiLayer = UILayer.Normal)
            => UIManager.Instance.OpenPanel(type, data, uiLayer);
        /// <summary> 通过PanelBase的名字打开一个UI </summary>
        public static PanelBase OpenPanel(string panelName, PanelData data = null, UILayer uiLayer = UILayer.Normal)
            => UIManager.Instance.OpenPanel(panelName, data, uiLayer);


        /// <summary> 通过PanelBase的泛型类型查找并关闭一个UI </summary>
        public static void ClosePanel<T>() where T : PanelBase => UIManager.Instance.ClosePanel<T>();
        /// <summary> 通过PanelBase的类型查找并关闭一个UI </summary>
        public static void ClosePanel(Type type) => UIManager.Instance.ClosePanel(type);
        /// <summary> 通过PanelBase的名称查找并关闭一个UI </summary>
        public static void ClosePanel(string panelName1) => UIManager.Instance.ClosePanel(panelName1);
        /// <summary> 通过PanelBase的直接实例关闭一个UI </summary>
        public static void ClosePanel(PanelBase uiPanel) => UIManager.Instance.ClosePanel(uiPanel);


        /// <summary> 通过PanelBase的泛型类型查找一个已开启的UI </summary>
        public static T GetPanel<T>() where T : PanelBase => UIManager.Instance.GetPanel<T>();
        /// <summary> 通过PanelBase的类型查找一个已开启的UI </summary>
        public static PanelBase GetPanel(Type type) => UIManager.Instance.GetPanel(type);
        /// <summary> 通过PanelBase的名字查找一个已开启的UI </summary>
        public static PanelBase GetPanel(string panelName) => UIManager.Instance.GetPanel(panelName);


        /// <summary> 通过PanelBase的泛型类型查找所有已开启的UI </summary>
        public static IEnumerable<PanelBase> GetPanels<T>() where T : PanelBase => UIManager.Instance.GetPanels<T>();
        /// <summary> 通过PanelBase的类型查找所有已开启的UI </summary>
        public static IEnumerable<PanelBase> GetPanels(Type type) => UIManager.Instance.GetPanels(type);
        /// <summary> 通过PanelBase的名字查找所有已开启的UI </summary>
        public static IEnumerable<PanelBase> GetPanels(string panelName) => UIManager.Instance.GetPanels(panelName);


        /// <summary> 通过PanelBase的泛型类型查找并显示一个UI </summary>
        public static void ShowPanel<T>() where T : PanelBase => UIManager.Instance.ShowPanel<T>();
        /// <summary> 通过PanelBase的类型查找并显示一个UI </summary>
        public static void ShowPanel(Type type) => UIManager.Instance.ShowPanel(type);
        /// <summary> 通过PanelBase的名字查找并显示一个UI </summary>
        public static void ShowPanel(PanelBase uiPanel) => UIManager.Instance.ShowPanel(uiPanel);


        /// <summary> 通过PanelBase的泛型类型查找并隐藏一个UI </summary>
        public static void HidePanel<T>() where T : PanelBase => UIManager.Instance.HidePanel<T>();
        /// <summary> 通过PanelBase的类型查找并隐藏一个UI </summary>
        public static void HidePanel(Type type) => UIManager.Instance.HidePanel(type);
        /// <summary> 通过PanelBase的名字查找并隐藏一个UI </summary>
        public static void HidePanel(PanelBase uiPanel) => UIManager.Instance.HidePanel(uiPanel);


        /// <summary> 通过PanelBase的泛型类型查找并隐藏一个UI，同时入栈加入缓存 </summary>
        public static void Push<T>() where T : PanelBase => UIManager.Instance.Push<T>();
        /// <summary> 通过PanelBase的类型查找并隐藏一个UI，同时入栈加入缓存 </summary>
        public static void Push(Type type) => UIManager.Instance.Push(type);
        /// <summary> 通过PanelBase的实例隐藏一个UI，同时入栈加入缓存 </summary>
        public static void Push(PanelBase uiPanel) => UIManager.Instance.Push(uiPanel);
        /// <summary> 将当前所有打开的UI，根据打开时间从新到旧，不断入栈并隐藏，直到满足某条件，或已无打开的UI </summary>
        public static PanelBase PushUntil(Predicate<PanelBase> predicate) => UIManager.Instance.PushUntil(predicate);


        /// <summary> 弹出并显示一个栈里隐藏的UI </summary>
        public static PanelBase Pop() => UIManager.Instance.Pop();
        /// <summary> 将当前所有隐藏的UI，根据入栈时间从新到旧，不断出栈并显示，直到满足某条件，或已无打开的UI </summary>
        public static PanelBase PopUntil(Predicate<PanelBase> predicate) => UIManager.Instance.PopUntil(predicate);

        
    }
}