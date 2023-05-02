using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish_Spawner : MonoBehaviour
{
    [Header("Fish Spawner")]
    public GameObject[] All_Fishes;
    public int Max_Amount_Of_Fishes;
    [Header("Fish Prefabs")]
    public GameObject[] All_Fish_Prefabs;
    [Header("Spawn Position")]
    public Transform Fish_Spawn_Position;
    [Header("Behavior")]
    public bool DisableAwaySwimmingIfOnFishingRod = false;

    public void Start()
    {
        StartCoroutine("Check_Fishes");
    }

    IEnumerator Check_Fishes()
    {
        while (true)
        {
            //Clear current fish array and search all fishes and put them into the array
            All_Fishes = null;
            All_Fishes = GameObject.FindGameObjectsWithTag("Fish");

            //Checks if all fishes are there, if not spawn a new random fish.
            if (All_Fishes.Length < Max_Amount_Of_Fishes)
            {
                Spawn_Fish();
            }

            yield return new WaitForSeconds(30);
        }
    }

    public void Spawn_Fish()
    {
        Instantiate(All_Fish_Prefabs[Random.Range(0, All_Fish_Prefabs.Length)], Fish_Spawn_Position.position, Fish_Spawn_Position.rotation);

        if(DisableAwaySwimmingIfOnFishingRod == true)
        {
            StartCoroutine(Disable_Away_Swimming_Behavior());
        }
    }

    IEnumerator Disable_Away_Swimming_Behavior()
    {
        yield return new WaitForSeconds(1);

        //Clear current fish array and search all fishes and put them into the array
        All_Fishes = null;
        All_Fishes = GameObject.FindGameObjectsWithTag("Fish");

        //Disable swimming away behavior, from all fishes.
        foreach(GameObject Fishes in All_Fishes)
        {
            Fishes.GetComponent<AIFishControl>().isSwimmingAfterBite = false;
        }
    }
}
