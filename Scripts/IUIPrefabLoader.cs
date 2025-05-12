using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace UI
{
    public interface IUIPrefabLoader
    {
        GameObject Load(string prefabName);
        void Unload(GameObject prefab);

#if ODIN_INSPECTOR
        IEnumerable<string> GetAllUIPrefabName();//用于给Odin在GUI提供不卸载资源的白名单列表
#endif

    }
    public sealed class DefaultUIPrefabLoader : IUIPrefabLoader
    {
        public GameObject Load(string prefabName)
        {
            //姑且假设所有UI存放路径为：Resources/UI/PanelDialogue.asset
            return Resources.Load<GameObject>($"UI/{prefabName}");
        }
        public void Unload(GameObject prefab)
        {
            // Debug.Log("Resource 不可卸载单个GameObject资源。仅可通过AB包卸载单个。操作忽略。");
        }
        public IEnumerable<string> GetAllUIPrefabName()
        {
#if UNITY_EDITOR
            HashSet<string> strings = new HashSet<string>();
            string rootPath = Application.dataPath + "/Resources/UI";
            if (!Directory.Exists(rootPath))
            {
                Debug.LogWarning($"路径> {rootPath} <不存在！");
                return strings;
            }

            // 获取文件夹下的所有文件
            string[] filePaths = Directory.GetFiles(rootPath);

            // 遍历文件路径并输出文件名
            foreach (string filePath in filePaths)
            {
                // 过滤掉.meta 文件
                if (Path.GetExtension(filePath) != ".meta")
                {
                    // 获取文件名（不带路径）
                    string fileName = Path.GetFileNameWithoutExtension(filePath);
                    strings.Add(fileName);
                }
            }
            return strings;
#else
            return null;
#endif
        }
    }
}