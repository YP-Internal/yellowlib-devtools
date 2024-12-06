using UnityEngine;

public class DevToolsPanel : MonoBehaviour
{
    CanvasGroup canvasGroup;

    private void Awake()
    {

#if !DEV_MODE
        Destroy(transform.parent.gameObject);
        return;
#endif
        canvasGroup = GetComponent<CanvasGroup>();

    }

    public void OpenPanel()
    {
        LeanTween.alphaCanvas(canvasGroup, 1, 0.3f).setOnComplete(() =>
        {
            canvasGroup.blocksRaycasts = true;
        });
    }

    public void ClosePanel()
    {
        canvasGroup.blocksRaycasts = false;
        LeanTween.alphaCanvas(canvasGroup, 0, 0.3f);
    }
}
