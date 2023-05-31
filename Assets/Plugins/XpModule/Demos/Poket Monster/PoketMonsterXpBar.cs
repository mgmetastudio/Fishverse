using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XpModule.Demos.PoketMonster
{
  public class PoketMonsterXpBar : MonoBehaviour
  {
    public GameObject EmptyBar;
    public GameObject ExperienceBar;

    float floor = 0f;
    float experience = 0f;

    void Start()
    {
      ExperienceBar.GetComponent<Transform>().localScale = new Vector3(0, 1, 1);
    }

    public void Refresh(float currentExperience, float currentFloor, float previousFloor)
    {
      floor = currentFloor;
      experience = currentExperience;

      float scale = (experience - previousFloor) / (floor - previousFloor);
      ExperienceBar.GetComponent<Transform>().localScale = new Vector3(scale, 1, 1);

    }
  }
}
