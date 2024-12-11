using UnityEngine;

public class DevToolsPanel : MonoBehaviour
{
    CanvasGroup _canvasGroup;

    private void Awake()
    {

#if !DEV_MODE
        Destroy(transform.parent.gameObject);
        return;
#endif
        _canvasGroup = GetComponent<CanvasGroup>();

    }

    public void OpenPanel()
    {
        LeanTween.alphaCanvas(_canvasGroup, 1, 0.3f).setOnComplete(() =>
        {
            _canvasGroup.blocksRaycasts = true;
        });
    }

    public void ClosePanel()
    {
        _canvasGroup.blocksRaycasts = false;
        LeanTween.alphaCanvas(_canvasGroup, 0, 0.3f);
    }
}
