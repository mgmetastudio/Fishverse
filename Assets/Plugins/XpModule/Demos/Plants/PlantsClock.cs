using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XpModule.Demos.Plants
{
  public class PlantsClock : MonoBehaviour
  {
    public PlantsClockNumber HourA;
    public PlantsClockNumber HourB;

    public PlantsClockNumber MinA;
    public PlantsClockNumber MinB;

    public PlantsClockNumber SecA;
    public PlantsClockNumber SecB;

    void Start()
    {
      Refresh(0);
    }

    // Update is called once per frame
    public void Refresh(long time)
    {
      if(time <= 0)
      {
        HourA.Refresh(0);
        HourB.Refresh(0);
        MinA.Refresh(0);
        MinB.Refresh(0);
        SecA.Refresh(0);
        SecB.Refresh(0);
        return;
      }


      long leftover = time;
      int hours = Mathf.FloorToInt(leftover / 60 / 60) | 0;
      leftover -= hours * 60 * 60;
      int mins = Mathf.FloorToInt(leftover / 60) | 0;
      leftover -= mins * 60;
      int seconds = (int) leftover | 0;

      HourA.Refresh(Mathf.FloorToInt(hours / 10));
      HourB.Refresh(hours % 10);
      MinA.Refresh(Mathf.FloorToInt(mins / 10));
      MinB.Refresh(mins % 10);
      SecA.Refresh(Mathf.FloorToInt(seconds / 10));
      SecB.Refresh(seconds % 10);
    }
  }
}
