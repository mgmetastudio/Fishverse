using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XpModule.Demos.Plants
{
  public class PlantsClockNumber : MonoBehaviour
  {
    public List<GameObject> Numbers = new List<GameObject>();

    void Start()
    {
      Refresh(0);
    }

    public void Refresh(int number)
    {
      for(int i = 0; i < Numbers.Count; i++)
         Numbers[i].SetActive(false);

      Numbers[number].SetActive(true);
    }
  }
}
