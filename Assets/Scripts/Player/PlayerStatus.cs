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

    public void ControlDamage(float value)
    {
        damage += value;
        if (damage < 0)
            damage = 0;
    }

    public void ControlHp(float value)
    {
        maxHp += value;
    }

    private void HpRegen()
    {
        if (canRegenHp && currentHp < maxHp)
        {
            currentHp += hpRegenRate * Time.deltaTime;
        }
        if (currentHp > maxHp)
            currentHp = maxHp;
    }

    private void StaminaRegen()
    {
        if (canRegenStamina && currentStamina < maxStamina)
        {
            currentStamina += staminaRegenRate * Time.deltaTime;
        }
        if (currentStamina > maxStamina)
            currentStamina = maxStamina;
    }

    private void ManaRegen()
    {
        if (canRegenMana && currentMana < maxMana)
        {
            currentMana += manaRegenRate * Time.deltaTime;
        }
        if (currentMana > maxMana)
            currentMana = maxMana;
    }

    // Método para subtrair Stamina de forma suave
    public void SubtractStamina(float value)
    {
        if (currentStamina >= value)
        {
            canRegenStamina = false; // Desativa a regeneração enquanto subtrai
            StartCoroutine(SubtractStaminaSmoothly(value));
        }
    }

    // Método para subtrair Mana de forma suave
    public void SubtractMana(float value)
    {
        if (currentMana >= value)
        {
            canRegenMana = false; // Desativa a regeneração enquanto subtrai
            StartCoroutine(SubtractManaSmoothly(value));
        }
    }

    private IEnumerator SubtractStaminaSmoothly(float value)
    {
        float startStamina = currentStamina;
        float endStamina = currentStamina - value;
        float elapsedTime = 0f;
        float smoothTime = 0.5f; // Tempo para a subtração ser concluída (ajustável)

        while (elapsedTime < smoothTime)
        {
            currentStamina = Mathf.Lerp(startStamina, endStamina, (elapsedTime / smoothTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        currentStamina = endStamina;
        canRegenStamina = true; // Restaura a regeneração após subtrair
    }

    private IEnumerator SubtractManaSmoothly(float value)
    {
        float startMana = currentMana;
        float endMana = currentMana - value;
        float elapsedTime = 0f;
        float smoothTime = 0.5f; // Tempo para a subtração ser concluída (ajustável)

        while (elapsedTime < smoothTime)
        {
            currentMana = Mathf.Lerp(startMana, endMana, (elapsedTime / smoothTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        currentMana = endMana;
        canRegenMana = true; // Restaura a regeneração após subtrair
    }
}
