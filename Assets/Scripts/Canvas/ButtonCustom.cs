using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonCustom : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private Button btn;

    private void Awake()
    {
        btn = GetComponent<Button>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(HudManager.Instance != null)
            HudManager.Instance.SetCursor(HudManager.Instance.cursorClicker);

        SFXManager.Instance.UIMiscSource.PlayOneShot(SFXManager.Instance.btnHoverSound);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(HudManager.Instance != null)
            HudManager.Instance.SetCursor(HudManager.Instance.cursorPointer);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        SFXManager.Instance.UIMiscSource.PlayOneShot(SFXManager.Instance.btnClickSound);
    }
}
