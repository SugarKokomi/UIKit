using System.Collections.Generic;
using System.IO;
using UI;
using UnityEngine;

public class InitUI : MonoBehaviour
{
    void Awake()
    {
        UIKit.SetAssetLoader(new UIAssetLoader());
    }
}
public class UIAssetLoader : UI.IAssetLoader
{
    public GameObject Load(string prefabName)
    {
        //姑且假设所有UI存放路径为这样：Resources/UIPrefab/PanelDialogue.asset
        return Resources.Load<GameObject>($"UIPrefab/{prefabName}");
    }

    public void Unload(GameObject prefab)
    {
        Debug.Log("Resource 不可卸载单个GameObject资源。仅可通过AB包卸载单个。操作忽略。");
    }
    public IEnumerable<string> GetAllUIPrefabName()
    {
        HashSet<string> strings = new HashSet<string>();
        string rootPath = Application.dataPath + "/Resources/UIPrefab";
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
    }
}