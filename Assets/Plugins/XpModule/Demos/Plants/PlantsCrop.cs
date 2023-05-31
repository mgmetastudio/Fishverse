using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XpTracker = XpModule.XpTracker;
using XpDistributor_TimeBased = XpModule.XpDistributor_TimeBased;

namespace XpModule.Demos.Plants
{
  [RequireComponent(typeof(XpTracker))]
  public class PlantsCrop : MonoBehaviour
  {
    public PlantsClock Clock;
    public GameObject HarvestText;
    public List<GameObject> Skins = new List<GameObject>();

    XpTracker xpTracker;
    XpDistributor_TimeBased xpDistributor;

    void Start()
    {
      HarvestText.SetActive(false);
      Clock.gameObject.SetActive(true);

      // Get components and keep reference to it
      xpTracker = this.GetComponent<XpTracker>();
      xpDistributor = this.GetComponent<XpDistributor_TimeBased>();

      // Setup time tracker
      xpDistributor.Reset((long) Time.time);

      // Setup events
      xpTracker.Get("Main").EventLevelUpdated += OnLevelUpdated;

      foreach(GameObject skin in Skins)
      {
        skin.SetActive(false);
      }

      Skins[0].SetActive(true);
    }

    // This is triggered when XP Component cast an EventLevelUpdated event
    void OnLevelUpdated(Experience experience, int value)
    {
      if(value == 3)
        Clock.gameObject.SetActive(false);

      PlantsEvolution animation = this.gameObject.AddComponent<PlantsEvolution>();
      animation.skinA = Skins[value - 1];
      animation.skinB = Skins[value];
      animation.Init();

      animation.AnimationCompleted += () => {
        if(value == 3)
          HarvestText.SetActive(true);
      };
    }

    void Update()
    {
      Clock.Refresh((long) Time.time);
      xpDistributor.Refresh((long) Time.time);
    }
  }
}
