#if !ODIN_INSPECTOR
using System;
using UnityEngine;

namespace UI
{
    /// <summary>
    /// 重定位Odin的LabelTextAttribute为Unity的Header
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public class LabelTextAttribute : HeaderAttribute
    {
        public LabelTextAttribute(string header) : base(header)
        { }
    }
}
#endif