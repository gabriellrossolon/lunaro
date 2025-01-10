using UnityEngine;
using static Weapon;

public class SecondaryBehavior : MonoBehaviour
{
    public Secondary.SecondaryType secondaryType;
    private Animator secondaryAnim;
    public Animator armBonesAnim;

    [HideInInspector] public bool block = false;

    private void Start()
    {
        secondaryAnim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (secondaryType == Secondary.SecondaryType.Shield)
            secondaryAnim.SetBool("isBlocking", block);

        if (secondaryType == Secondary.SecondaryType.Orb)
        {
            armBonesAnim.SetBool("isWithOrb", true);
            armBonesAnim.SetBool("isWithShield", false);
            armBonesAnim.SetBool("isWithBook", false);
        }
        else if (secondaryType == Secondary.SecondaryType.Shield)
        {
            armBonesAnim.SetBool("isWithShield", true);
            armBonesAnim.SetBool("isWithOrb", false);
            armBonesAnim.SetBool("isWithBook", false);
        }
        else if (secondaryType == Secondary.SecondaryType.Book)
        {
            armBonesAnim.SetBool("isWithShield", false);
            armBonesAnim.SetBool("isWithOrb", false);
            armBonesAnim.SetBool("isWithBook", true);
        }
        else
        {
            armBonesAnim.SetBool("isWithOrb", false);
            armBonesAnim.SetBool("isWithShield", false);
            armBonesAnim.SetBool("isWithBook", false);
        }
    }
}
