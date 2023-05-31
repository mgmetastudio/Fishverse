using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XpModule
{
  [Serializable]
  public class XpTracker : MonoBehaviour
  {
    public List<Experience> Experiences = new List<Experience>() { new Experience() };

    public Experience Get(string id)
    {
      Experience xp = Experiences.Find((Experience xp) => { return xp.id == id; } );

      if(xp == null)
        Debug.LogWarning("[XpTracker] No Experience registered for id : " + id);

      return xp;
    }

    public void Grant(string id, float value)
    {
      Experience xp = Get(id);

      if(xp == null)
        return;

      xp.Points += value;
    }
  }
}
