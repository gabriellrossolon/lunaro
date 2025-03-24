using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LooteableBehavior : MonoBehaviour, IInteractable
{
    [Header("References")]
    private GameObject player;
    private PlayerStatus playerStatus;

    [Header("Configs")]
    public float maxDistance = 2f;

    [Header("Looting")]
    [SerializeField] private Vector2 goldRange;
    public float gold;

    private void Start()
    {
        player = PlayerManager.Instance.gameObject;
        playerStatus = player.GetComponent<PlayerStatus>();

        if (GetComponentInParent<EnemyStats>() is EnemyStats stats) //Caso o looteable em questão seja um Enemy
            gold = stats.goldCarryed;
        else
            gold = Random.Range((int)goldRange.x, (int)goldRange.y);
    }

    public void Interact()
    {
        playerStatus.ControlGold(gold);
        SFXManager.Instance.miscSoundSource.PlayOneShot(SFXManager.Instance.interactSound);
        DisableBehavior();
    }

    private void DisableBehavior()
    {
        gameObject.layer = LayerMask.NameToLayer("Default");

        if (GetComponent<Outline>() is Outline outline)
            outline.enabled = false;

        if (GetComponentInParent<EnemyEffects>() is EnemyEffects enemyEffects)
            RemoveEnemyMaterialGlow(enemyEffects);
        else
            RemoveObjectMaterialGlow();


        enabled = false;
    }

    public void RemoveEnemyMaterialGlow(EnemyEffects enemEffects)
    {
        SkinnedMeshRenderer[] skinnedMeshes = enemEffects.GetComponentsInChildren<SkinnedMeshRenderer>();

        foreach (var smr in skinnedMeshes)
        {
            Material[] materials = smr.materials;

            Material[] newMaterials = new Material[materials.Length - 1];
            for (int i = 0, j = 0; i < materials.Length; i++)
            {
                if (i != 1)
                {
                    newMaterials[j++] = materials[i];
                }
            }

            smr.materials = newMaterials;
        }
    }

    public void RemoveObjectMaterialGlow()
    {
        MeshRenderer[] skinnedMeshes = GetComponents<MeshRenderer>();

        foreach (var mr in skinnedMeshes)
        {
            Material[] materials = mr.materials;

            Material[] newMaterials = new Material[materials.Length - 1];
            for (int i = 0, j = 0; i < materials.Length; i++)
            {
                if (i != 1)
                {
                    newMaterials[j++] = materials[i];
                }
            }

            mr.materials = newMaterials;
        }
    }
}
