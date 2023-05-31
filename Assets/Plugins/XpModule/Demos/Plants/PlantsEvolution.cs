using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XpModule.Demos.Plants
{
  public class PlantsEvolution : MonoBehaviour
  {
    public delegate void AnimationEvent ();
    public event AnimationEvent AnimationCompleted;

    int animationPhase = 0;

    bool switcher = false;

    float lerpCounter = 0f;

    float blinkCounter = 0;
    int blinkChangeCounter = 0;

    Vector3 localScaleRef = Vector3.zero;

    public GameObject skinA;
    public GameObject skinB;

    public void Init()
    {
      skinA.GetComponent<SpriteRenderer>().color = Color.white;
      skinB.GetComponent<SpriteRenderer>().color = Color.black;

      skinA.SetActive(true);
      skinB.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
      if(animationPhase == 0)
      {
        lerpCounter += Time.deltaTime;
        Color color = Color.Lerp(Color.white, Color.black, lerpCounter);

        skinA.GetComponent<SpriteRenderer>().color = color;

        if(lerpCounter >= 1)
        {
          lerpCounter = 0;
          skinA.SetActive(false);
          skinB.SetActive(true);
          animationPhase = 1;
        }
      }

      if(animationPhase == 1)
      {
        skinA.SetActive(switcher);
        skinB.SetActive(!switcher);

        blinkCounter += Time.deltaTime * 5;

        if(blinkCounter >= 1)
        {
          blinkCounter = 0;
          switcher = !switcher;
          blinkChangeCounter++;
        }

        if(blinkChangeCounter >= 2)
        {
          skinA.SetActive(false);
          skinB.SetActive(true);

          animationPhase = 2;
        }
      }

      if(animationPhase == 2)
      {
        lerpCounter += Time.deltaTime;
        Color color = Color.Lerp(Color.black, Color.white, lerpCounter);

        skinB.GetComponent<SpriteRenderer>().color = color;

        if(lerpCounter >= 1)
        {
          if(AnimationCompleted != null)
            AnimationCompleted();

          this.enabled = false;
          Destroy(this);
        }
      }
    }
  }
}
