using UnityEngine;

public class LevelBehavior : MonoBehaviour
{
    public int level = 0;
    public float experience = 0;
    public float experienceToUp = 20f;

    public void AddExperience(float value)
    {
        experience += value;
        CheckLevelUp();
        HudManager.Instance.UpdateHUDLevel();
    }

    private void CheckLevelUp()
    {
        while (experience >= experienceToUp)
        {
            experience -= experienceToUp;
            level++;
            experienceToUp = CalculateExperienceToNextLevel();
            SFXManager.Instance.miscSoundSource.PlayOneShot(SFXManager.Instance.levelUpSound);
        }
    }

    private float CalculateExperienceToNextLevel()
    {
        return experienceToUp * Mathf.Pow(1.1f, level - 1);
    }
}
