using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    public enum PortalType
    {
        HUB,
        Sewers,
        DungeonA,
        Spawn
    }

    public PortalType portalType;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            LoadSceneBasedOnPortalType();
        }
    }

    private void LoadSceneBasedOnPortalType()
    {
        string sceneName = "";

        switch (portalType)
        {
            case PortalType.HUB:
                sceneName = "HUB";
                break;

            case PortalType.Sewers:
                sceneName = "SewerDungeon";
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
        if (player != null)
        {
            player.transform.position = Vector3.zero;
            player.transform.rotation = Quaternion.Euler(0, 90, 0);
        }

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}

