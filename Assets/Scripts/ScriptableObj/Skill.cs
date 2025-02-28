using UnityEngine;

[CreateAssetMenu(fileName = "NewSkill", menuName = "Powers/Skills")]
public class Skill: ScriptableObject
{
    public string skillName;
    public Sprite skillIcon;
    public GameObject skillPrefab;

    public SkillType skillType;

    [Header ("Below 1f the Skill deal Less Damage")]
    public float damageMultiplier = 1f;
    public enum SkillType
    {
        None,
        Earthquake,
        MagicBeam,
        CircularCut,
        Fissure
    }
}
