// #define QFRAMEWORK
using UnityEngine;

#if QFRAMEWORK
using QFramework;
#endif

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace UI
{
    public abstract class PanelBase<Data> : PanelBase where Data : PanelData, new()
    {
        [SerializeField, LabelText("预先打开的Data")]
        private Data m_data;
        public Data data { get { m_data ??= new Data(); return m_data; } }
        internal override void SetData(PanelData data)
        {
            if (data != null && data is Data data_)
                m_data = data_;
        }
    }
    [DisallowMultipleComponent]
    public abstract partial class PanelBase : MonoBehaviour
    {
        /// <summary> 用于标记PanelBase有没有被初始化。如果不是通过UI框架打开的(比如一开始就在场景里)，就使用MonoBehavior的生命周期进行初始化 </summary>
        protected internal bool isInit = false;
        protected virtual void Start()
        {
            if (!isInit)
                UIManager.Instance.InitPanelBase(this);
        }
        protected virtual void OnDestroy()
        {
            if (isInit)
                UIManager.Instance.ClearPanelBase(this);
        }
        /// <summary> 该函数在初始化调用，类似Awake()。用于复位UI视觉样式，通过UIManager进行管理。 </summary>
        protected virtual void OnInit() { }
        /// <summary> 该函数在UI打开时调用，类似Start()。用于注册监听委托事件，通过UIManager进行管理。 </summary>
        protected virtual void OnOpen() { }
        /// <summary> 该函数在显示UI时调用，类似OnEnable()。通过UIManager进行管理。 </summary>
        protected virtual void OnShow() { }
        /// <summary> 该函数在隐藏UI时调用，类似OnDisable()。通过UIManager进行管理。 </summary>
        protected virtual void OnHide() { }
        /// <summary> 该函数在UI关闭时调用，类似OnDestroy()。用于取消注册监听委托事件，通过UIManager进行管理。 </summary>
        protected virtual void OnClose() { }
        //以下事件都会比自己的生命周期函数先执行
        public EasyEvent onOpen = new EasyEvent(), onClose = new EasyEvent(), onShow = new EasyEvent(), onHide = new EasyEvent();

        internal virtual void SetData(PanelData data) { }
    }
    #region 程序集内部定义、外部拓展
    public abstract partial class PanelBase
    {
        protected void CloseSelf() => UIKit.ClosePanel(this);
        protected void ShowSelf() => UIKit.ShowPanel(this);
        protected void HideSelf() => UIKit.HidePanel(this);
        protected void PushSelf() => UIKit.Push(this);
        internal void Init() => OnInit();
        internal void Open() { OnOpen(); onOpen.Trigger(); }
        internal void Close() { OnClose(); onClose.Trigger(); }
        internal void Hide() { OnHide(); onHide.Trigger(); }
        internal void Show() { OnShow(); onShow.Trigger(); }
    }
    #endregion
}