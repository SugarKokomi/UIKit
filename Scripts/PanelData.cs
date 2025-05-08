#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace UI
{
    public class PanelData { }
    
    [LabelText("UI层级")]
    public enum UILayer { Background = 0, Low = 1, Normal = 2, High = 3, Foreground = 4 }
}