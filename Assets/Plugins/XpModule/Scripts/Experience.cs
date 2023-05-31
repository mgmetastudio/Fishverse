using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XpModule
{
  [Serializable]
  public class Experience
  {
    public delegate void IntValueUpdated (Experience experience, int value);
    public delegate void FloatValueUpdated (Experience experience, float value);
    public event IntValueUpdated EventLevelUpdated;
    public event FloatValueUpdated EventExperienceUpdated;

    public string id = "Main";

    [SerializeField]
    float points = 0;

    public List<float> Floors = new List<float>() { 50 };
    public bool Capped = false;

    public int Level {
      get {
        int level = 0;

        // We compare current experience to floors in order to retrieve current level
        // This way level adapt to floors modifications
        for(int i = 0; i < Floors.Count; i++)
        {
          if(Points >= Floors[i])
            level = i + 1;
        }

        return level;
      }
    }

    public float Points
    {
      get { return points; }
      set
      {
        // Return if value has not changed
        if (value == points)
          return;

        int oldLevel = Level;

        // Return if reached max level and experience is capped
        if(oldLevel >= Floors.Count && Capped)
          return;

        float oldPoints = points;
        points = value;
        int newLevel = Level;

        if(EventExperienceUpdated != null)
          EventExperienceUpdated(this, points);

        if(EventLevelUpdated != null && oldLevel != newLevel)
          EventLevelUpdated(this, newLevel);
      }
    }

    public float GetFloor(int level)
    {
      if (Floors == null || Floors.Count <= level || level < 0)
      {
        Debug.LogWarning("[PROGRESSION] Variable 'Floors' does not define value for level : " + level);
        return 0;
      }

      return Floors[level];
    }
  }
}
