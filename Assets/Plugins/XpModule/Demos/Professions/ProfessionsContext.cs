using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using XpTracker = XpModule.XpTracker;

namespace XpModule.Demos.Professions
{
  public class ProfessionsContext : MonoBehaviour
  {
    public ProfessionsXpBar Fishing;

    public XpTracker Tracker;

    ProfessionsRessource ressource;
    int ressourceClicks = 0;
    bool ressourceDestroying = false;
    bool ressourceCreating = false;

    void Start()
    {
      Tracker.Get("Fishing").EventExperienceUpdated += (Experience experience, float value) =>
      {
        int level = experience.Level;
        Fishing.RefreshExperience(value, experience.GetFloor(level), experience.GetFloor(level - 1));
      };

      Tracker.Get("Fishing").EventLevelUpdated += (Experience experience, int value) =>
      {
        Fishing.RefreshLevel(value);
      };
    }
  }
}
