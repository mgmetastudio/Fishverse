using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RessourceType {
  WOOD,
  STONE
}

// This is only for demo display
public class ProfessionsRessource : MonoBehaviour
{
  public GameObject Stone;
  public GameObject Wood;

  public RessourceType Type;

  bool isCollected = false;
  Vector3 origin;
  Vector3 target;
  float time;

  public void Refresh(RessourceType type)
  {
    Type = type;

    Stone.SetActive(type == RessourceType.STONE);
    Wood.SetActive(type == RessourceType.WOOD);
  }

  public void Collect (Vector3 target)
  {
    origin = this.GetComponent<Transform>().position;
    isCollected = true;
    this.target = target;
    time = 0;
  }

  void Update()
  {
    if(!isCollected)
      return;

    time += Time.deltaTime * 2;
    Vector3 position = Vector3.Lerp(origin, target, time);
    this.GetComponent<Transform>().position = position;

    if(time >= 1)
    {
      Destroy(this.gameObject);
    }
  }
}
