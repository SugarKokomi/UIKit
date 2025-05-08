using System;
using System.Collections;
using UI;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class PanelTestData : PanelData
{
    [LabelText("延迟出关闭按钮")]
    public float delayShowCloseButton = 2f;
    [Multiline(4)]
    public string text;
}
public class PanelTest1 : PanelBase<PanelTestData>
{
    [SerializeField]
    protected Button closeBtn;
    [SerializeField]
    protected Text mainContentText;
    protected override void OnInit()
    {
        base.OnInit();
        closeBtn.gameObject.SetActive(false);
        mainContentText.text = data.text;
    }
    protected override void OnOpen()
    {
        base.OnOpen();
        StartCoroutine(DelayShow());
        closeBtn.onClick.AddListener(CloseSelf);
    }
    IEnumerator DelayShow()
    {
        yield return new WaitForSeconds(data.delayShowCloseButton);
        closeBtn.gameObject.SetActive(true);
    }
}
