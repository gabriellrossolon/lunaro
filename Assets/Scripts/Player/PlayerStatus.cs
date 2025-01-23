using UnityEngine;
using System.Collections;

public class PlayerStatus : MonoBehaviour
{
    [Header("Movement Stats")]
    public float playerGravity;
    public float walkSpeed;
    public float runSpeed;
    public float actualSpeed;
    public float jumpHeight;

    [Header("Combat Stats")]
    public float damage;

    public float maxHp;
    public float currentHp;

    public float maxStamina;
    public float currentStamina;

    public float maxMana;
    public float currentMana;

    [Header("Regeneration Stats")]
    public float hpRegenRate = 1f;
    public bool canRegenHp = true;
    public float staminaRegenRate = 10f;
    public bool canRegenStamina = true;
    public float manaRegenRate = 10f;
    public bool canRegenMana = true;

    [Header("Misc Stats")]
    public float playerGold;

    private void Start()
    {
        currentHp = maxHp;
        currentStamina = maxStamina;
        currentMana = maxMana;
    }

    private void Update()
    {
        HpRegen();
        StaminaRegen();
        ManaRegen();
    }

    //Funções que controlam a adição ou subtração de valores aos status do jogador:

    public void ControlBaseDamage(float value)
    {
        damage += value;
        if (damage < 0)
            damage = 0;
    }
    public void ControlBaseHealth(float value)
    {
        maxHp += value;
    }
    public void ControlBaseStamina(float value)
    {
        maxStamina += value;
    }
    public void ControlBaseMana(float value)
    {
        maxMana += value;
    }

    public void ControlBaseHealthRegeneration(float value)
    {
        hpRegenRate += value;
    }
    public void ControlBaseStaminaRegeneration(float value)
    {
        staminaRegenRate += value;
    }
    public void ControlBaseManaRegeneration(float value)
    {
        manaRegenRate += value;
    }

    public void ControlDamagePercent(float value)
    {

    }
    public void ControlHealthPercent(float value)
    {

    }
    public void ControlStaminaPercent(float value)
    {

    }
    public void ControlManaPercent(float value)
    {

    }

    //Funções que controlam as regenerações dos Status do jogador:

    private void HpRegen()
    {
        if (canRegenHp && currentHp < maxHp)
            currentHp += hpRegenRate * Time.deltaTime;

        if (currentHp > maxHp)
            currentHp = maxHp;
    }

    private void StaminaRegen()
    {
        if (canRegenStamina && currentStamina < maxStamina)
            currentStamina += staminaRegenRate * Time.deltaTime;

        if (currentStamina > maxStamina)
            currentStamina = maxStamina;
    }

    private void ManaRegen()
    {
        if (canRegenMana && currentMana < maxMana)
            currentMana += manaRegenRate * Time.deltaTime;

        if (currentMana > maxMana)
            currentMana = maxMana;
    }

    // Funções que subtraem status do jogador, como por exemplo habilidades que exigem uso de Mana ou Stamina:
    public void SubtractStamina(float value)
    {
        if (currentStamina >= value)
        {
            currentStamina -= value;
            canRegenStamina = true;
        }
    }
    public void SubtractMana(float value)
    {
        if (currentMana >= value)
        {
            currentMana -= value;
        }
    }
   
    //Controle de Gold
    public void ControlGold(float value)
    {
        playerGold += value;
        HudManager.Instance.shortcutsAnim.SetTrigger("enterGold");
    }
}
