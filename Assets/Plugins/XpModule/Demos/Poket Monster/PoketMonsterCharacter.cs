using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XpTracker = XpModule.XpTracker;

namespace XpModule.Demos.PoketMonster
{
  [RequireComponent(typeof(XpTracker))]
  public class PoketMonsterCharacter : MonoBehaviour
  {
    public PoketMonsterXpBar XpBar;
    public GameObject EvolveText;

    public List<GameObject> Skins = new List<GameObject>();

    XpTracker xpTracker;

    void Start()
    {
      EvolveText.SetActive(false);
      XpBar.gameObject.SetActive(true);

      // Get Xp Component and keep reference to it
      xpTracker = this.GetComponent<XpTracker>();

      // Setup events
      xpTracker.Get("Main").EventExperienceUpdated += OnExperienceUpdated;
      xpTracker.Get("Main").EventLevelUpdated += OnLevelUpdated;

      foreach(GameObject skin in Skins)
      {
        skin.SetActive(false);
      }

      Skins[0].SetActive(true);
    }

    // This is triggered when XP Component cast an EventExperienceUpdated event
    void OnExperienceUpdated(Experience experience, float value)
    {
      XpBar.Refresh(value, experience.GetFloor(experience.Level), experience.GetFloor(experience.Level - 1));
    }

    // This is triggered when XP Component cast an EventLevelUpdated event
    void OnLevelUpdated(Experience experience, int value)
    {
      EvolveText.SetActive(true);
      XpBar.gameObject.SetActive(false);

      PoketMonsterEvolution animation = this.gameObject.AddComponent<PoketMonsterEvolution>();
      animation.skinA = Skins[value - 1];
      animation.skinB = Skins[value];
      animation.Init();

      animation.AnimationCompleted += () => {
        EvolveText.SetActive(false);
        XpBar.gameObject.SetActive(true);
      };
    }

    void Update()
    {
      if(Input.anyKeyDown)
      {
        xpTracker.Grant("Main", 50);
      }
    }
  }
}
