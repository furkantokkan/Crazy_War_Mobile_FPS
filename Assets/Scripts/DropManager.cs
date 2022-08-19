using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropManager : MonoBehaviour
{
    public Transform[] spawnPoints;
    [SerializeField] private float dropTime = 15;

    private IEnumerator Start()
    {
        while (true)
        {
            SpawnDrop(GetRandomDrop());
            yield return new WaitForSeconds(dropTime);
        }
    }
    Transform GetRandomDrop()
    {
        List<GameObject> allDrops = new List<GameObject>();
        allDrops.AddRange(GameManager.instance.allDrop);
        return allDrops[Random.Range(0, allDrops.Count)].transform;
    }
    void SpawnDrop(Transform Drop)
    {
        Transform sp = spawnPoints[Random.Range(0, spawnPoints.Length)];

        if (sp.childCount > 0)
        {
            Destroy(sp.GetComponentInChildren<GameObject>());
        }

        Instantiate(Drop, sp.position, sp.rotation, sp);
    }
}
