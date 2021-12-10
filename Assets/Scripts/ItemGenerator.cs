using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

/*
 * The script to generate new items.
 * It contains two arrays: 1 for the items and 1 for the spawn points
 * Around every 7 seconds (depending on the current difficulty, see GameManager.cs), it generate a new item at a random spawn point.
 * I plan to make the object spawn farther and farther as the difficulty increases.
 * 
 * Originally, the rare items were less likely to spawn the more the difficulty, but for some reason that caused huuge memory overload (probably due to the fact that i create a long array containing GameObject)
 * so i turned it off for now.
 * 
 */

public class ItemGenerator : MonoBehaviour
{
    public Item[] items;
    public Transform[] itemSpawners;

    void Start() {
        Invoke("SpawnNewItem", UnityEngine.Random.Range(3, 6));
    }

    void SpawnNewItem() {
        int itemSpawnerIndex = UnityEngine.Random.Range(0, itemSpawners.Length);
        List<GameObject> itemsToSpawn = new List<GameObject>();
        /*
         * Each item contains a float called rarity. And for each .1 rarity, I add 1 more item to the array. Then I pick on randomly on this array and it will be the object to spawn.
         * The problem with this is that it contains a lot of object (and currently there is only 3 types of holdable.) so i'll probably change it in the next versions.
         */
        for (int i = 0; i < items.Length; i++) {
            for (int j = 0; j < items[i].rarity * 10; j++) { //  / (GameManager.Instance.difficulty / 2)
                itemsToSpawn.Add(items[i].prefab);
            }
        }

        Instantiate(itemsToSpawn[UnityEngine.Random.Range(0, itemsToSpawn.Count-1)], itemSpawners[itemSpawnerIndex].position, Quaternion.identity);
        Invoke("SpawnNewItem", UnityEngine.Random.Range(3, 5 + GameManager.Instance.difficulty / 2));
    }

    [Serializable]
    public struct Item {
        public GameObject prefab;
        public float rarity;
    }

}
