using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Portal : MonoBehaviour
{
    public GameObject transition;
    public enum PortalType
    {
        HUB,
        DungeonA,
        Spawn
    }

    public PortalType portalType;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            transition.GetComponent<Animator>().SetTrigger("start");
            StartCoroutine(LoadSceneWithDelay(other));
        }
    }

    private IEnumerator LoadSceneWithDelay(Collider other)
    {
        // Aguardar 1 segundo
        yield return new WaitForSeconds(0.5f);
        other.GetComponent<CharacterController>().enabled = false;
        LoadSceneBasedOnPortalType();
    }

    private void LoadSceneBasedOnPortalType()
    {
        string sceneName = "";

        switch (portalType)
        {
            case PortalType.HUB:
                sceneName = "HUB";
                break;

            case PortalType.DungeonA:
                sceneName = "DungeonA";
                break;

            case PortalType.Spawn:
                sceneName = "SpawnScene";
                break;
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene(sceneName);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameObject player = GameObject.FindWithTag("Player");
        GameObject spawnPoint = GameObject.FindWithTag("SpawnPoint");
        transition.GetComponent<Animator>().SetTrigger("end");
        player.transform.position = spawnPoint.transform.position;
        player.GetComponent<CharacterController>().enabled = true;

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
