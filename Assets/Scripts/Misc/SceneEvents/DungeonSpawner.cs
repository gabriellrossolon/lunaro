using System.Collections.Generic;
using UnityEngine;

public class DungeonSpawner : MonoBehaviour
{
    [Header("Configurações de Spawn")]
    public List<GameObject> highChanceEnemies;
    public List<GameObject> mediumChanceEnemies;
    public List<GameObject> lowChanceEnemies;

    private List<Transform> spawnPoints = new List<Transform>();

    private void Start()
    {
        FindSpawnPoints();
        SpawnEnemies();
    }

    private void FindSpawnPoints()
    {
        spawnPoints.Clear();
        GameObject[] spawnPointObjects = GameObject.FindGameObjectsWithTag("EnemySpawnPoint");

        foreach (GameObject obj in spawnPointObjects)
            spawnPoints.Add(obj.transform);
    }

    public void SpawnEnemies()
    {
        if ((highChanceEnemies.Count == 0 && mediumChanceEnemies.Count == 0 && lowChanceEnemies.Count == 0) || spawnPoints.Count == 0)
            return;

        foreach (var spawnPoint in spawnPoints)
        {
            GameObject enemyToSpawn = ChooseEnemyByChance();
            Instantiate(enemyToSpawn, spawnPoint.position, Quaternion.identity);
        }
    }

    private GameObject ChooseEnemyByChance()
    {
        List<List<GameObject>> validLists = new List<List<GameObject>>();

        if (highChanceEnemies.Count > 0)
            validLists.Add(highChanceEnemies);
        if (mediumChanceEnemies.Count > 0)
            validLists.Add(mediumChanceEnemies);
        if (lowChanceEnemies.Count > 0)
            validLists.Add(lowChanceEnemies);

        if (validLists.Count == 0)
            return null;

        float randomChance = Random.Range(0f, 1f);
        List<GameObject> selectedList;

        if (randomChance < 0.50f && validLists.Contains(highChanceEnemies))
        {
            selectedList = highChanceEnemies;
        }
        else if (randomChance < 0.85f && validLists.Contains(mediumChanceEnemies))
        {
            selectedList = mediumChanceEnemies;
        }
        else
        {
            selectedList = lowChanceEnemies;
        }

        return selectedList[Random.Range(0, selectedList.Count)];
    }
}
