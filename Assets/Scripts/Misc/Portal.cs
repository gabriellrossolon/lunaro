using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Portal : MonoBehaviour
{
    private GameObject transition;
    public enum PortalType
    {
        None,
        HUB,
        DarkChambers,
        FrozenRuins,
        HollowbarkDephts,
        ForsakenHamlet,
        ForgottenDungeons,
        TestScene
    }

    public PortalType portalType;

    private void Awake()
    {
        transition = GameObject.FindWithTag("Transition");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if(portalType == PortalType.None)
            {
                HudManager.Instance.OpenTeleportPage();
            }
            else
            {
                if(transition != null) { transition.GetComponent<Animator>().SetTrigger("start"); }
                StartCoroutine(LoadSceneWithDelay(other));
            }

        }
    }

    private IEnumerator LoadSceneWithDelay(Collider other)
    {
        yield return new WaitForSeconds(0.5f);
        other.GetComponent<CharacterController>().enabled = false;
        LoadSceneBasedOnPortalType();
    }

    public void LoadSceneBasedOnPortalType()
    {
        string sceneName = "";

        switch (portalType)
        {
            case PortalType.HUB:
                sceneName = "HUB";
                break;  
            case PortalType.DarkChambers:
                sceneName = "DarkChambers";
                break;
            case PortalType.FrozenRuins:
                sceneName = "FrozenRuins";
                break;
            case PortalType.HollowbarkDephts:
                sceneName = "HollowbarkDephts";
                break;
            case PortalType.ForsakenHamlet:
                sceneName = "ForsakenHamlet";
                break;
            case PortalType.ForgottenDungeons:
                sceneName = "ForgottenDungeons";
                break;
            case PortalType.TestScene:
                sceneName = "TestScene";
                break;
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene(sceneName);

        SFXManager.Instance.miscSoundSource.PlayOneShot(SFXManager.Instance.teleportSound);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameObject player = GameObject.FindWithTag("Player");
        GameObject spawnPoint = GameObject.FindWithTag("SpawnPoint");
        if (transition != null) { transition.GetComponent<Animator>().SetTrigger("end"); }
        player.transform.position = spawnPoint.transform.position;
        player.GetComponent<CharacterController>().enabled = true;
        InventoryManager.Instance.potionsCount = InventoryManager.Instance.maxPotionsCount;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
