using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace XpModule.Demos.Professions
{
  // This is only for demo display
  public class ProfessionsXpBar : MonoBehaviour
  {
    public ProfessionsNumber Level;
    public GameObject EmptyBar;
    public GameObject ExperienceBar;
    public Slider slider;

    float floor = 0f;
    float experience = 0f;

    void Start()
    {
      ExperienceBar.GetComponent<Transform>().localScale = new Vector3(0, 1, 1);
    }

    public void RefreshExperience(float currentExperience, float currentFloor, float previousFloor)
    {
      floor = currentFloor;
      experience = currentExperience;

      float scale = (experience - previousFloor) / (floor - previousFloor);
      // ExperienceBar.GetComponent<Transform>().localScale = new Vector3(scale * 6, 1, 1);
      slider.value = scale;
      print("SCALE: " + scale);
    }

    public void RefreshLevel(int value)
    {
      Level.Refresh(value);
    }
  }
}
