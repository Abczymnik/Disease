using UnityEngine;

public class SpawnPoints : MonoBehaviour
{
    public void Awake()
    {
        Transform zombies = GameObject.Find("/Zombie").transform;
        int zombieCount = zombies.childCount;

        //Set spawnpoint gameobject for every zombie at scene
        for(int i = 1; i<zombieCount+1; i++)
        {
            GameObject spawnPoint = new GameObject("SpawnPoint " + i.ToString());
            spawnPoint.transform.position = zombies.GetChild(i-1).position;
            spawnPoint.tag = "Spawn point";
            spawnPoint.transform.SetParent(transform);
        }
    }
}
