using System.Collections.Generic;
using UnityEngine;


public class SpawnAssginer : MonoBehaviour
{
    [SerializeField] private List<Transform> spawnPositionList;
 
    private void Shuffle()
    {
        for (int i = 0; i < spawnPositionList.Count; i++)
        {
            int ranNum = Random.Range(0, 233) % spawnPositionList.Count;
            Transform ori = spawnPositionList[i];
            spawnPositionList[i] = spawnPositionList[ranNum];
            spawnPositionList[ranNum] = ori;
        }
    }

    public List<Transform> GetSpawnPositionList()
    {
        Shuffle();

        return spawnPositionList;
    }
}

