using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MenuButtonCustom : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private Button btn;
    private Toggle toggle;
    private Animator anim;

    private void Awake()
    {
        btn = GetComponent<Button>();
        toggle = GetComponent<Toggle>();
        anim = GetComponent<Animator>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (MenuManager.Instance != null)
            MenuManager.Instance.SetCursor(MenuManager.Instance.cursorClicker);

        MenuManager.Instance.menuSoundSource.PlayOneShot(MenuManager.Instance.btnHoverSound);

        if (anim != null)
            anim.SetBool("Selected", true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (MenuManager.Instance != null)
            MenuManager.Instance.SetCursor(MenuManager.Instance.cursorPointer);

        if (anim != null)
            anim.SetBool("Selected", false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        MenuManager.Instance.menuSoundSource.PlayOneShot(MenuManager.Instance.btnClickSound);

        if (anim != null)
            anim.SetBool("Selected", true);
    }  
}
