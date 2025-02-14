using UnityEngine;

[CreateAssetMenu(fileName = "NewSkill", menuName = "Powers/Skills")]
public class Skill: ScriptableObject
{
    public string skillName;
    public Sprite skillIcon;
    public GameObject skillPrefab;
    public enum SkillType
    {
        None,
        Earthquake,
        MagicBeam,
        CircularCut
    }

    public SkillType skillType;
}
