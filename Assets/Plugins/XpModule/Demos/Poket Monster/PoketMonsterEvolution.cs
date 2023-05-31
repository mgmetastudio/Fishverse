using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XpModule.Demos.PoketMonster
{
  public class PoketMonsterEvolution : MonoBehaviour
  {
    public delegate void AnimationEvent ();
    public event AnimationEvent AnimationCompleted;

    int animationPhase = 0;

    bool switcher = false;

    float positionCounter = 0;
    int positionChangeCounter = 0;

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
        skinA.GetComponent<Transform>().localPosition += switcher ? new Vector3(0, -0.75f * Time.deltaTime, 0) : new Vector3(0, 1.25f * Time.deltaTime, 0);
        positionCounter += switcher ? -0.75f * Time.deltaTime : 1.25f * Time.deltaTime;

        if(positionCounter >= 0.15f)
        {
          positionCounter = 0.15f;
          skinA.GetComponent<Transform>().localPosition = new Vector3(0, 0.15f, 0);

          switcher = !switcher;
          positionChangeCounter++;
        }

        if(positionCounter <= 0f)
        {
          positionCounter = 0f;
          skinA.GetComponent<Transform>().localPosition = new Vector3(0, 0f, 0);

          switcher = !switcher;
          positionChangeCounter++;
        }

        if(positionChangeCounter >= 4)
        {
          skinA.GetComponent<Transform>().localPosition = new Vector3(0, 0, 0);
          animationPhase = 1;
        }
      }

      if(animationPhase == 1)
      {
        lerpCounter += Time.deltaTime;
        Color color = Color.Lerp(Color.white, Color.black, lerpCounter);

        skinA.GetComponent<SpriteRenderer>().color = color;

        if(lerpCounter >= 1)
        {
          lerpCounter = 0;
          skinA.SetActive(false);
          skinB.SetActive(true);
          animationPhase = 2;
        }
      }

      if(animationPhase == 2)
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

        if(blinkChangeCounter >= 8)
        {
          skinA.SetActive(false);
          skinB.SetActive(true);

          animationPhase = 3;
        }
      }

      if(animationPhase == 3)
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
