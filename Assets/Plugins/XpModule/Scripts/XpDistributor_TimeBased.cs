using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using XpTracker = XpModule.XpTracker;

namespace XpModule
{
  [Serializable]
  [RequireComponent(typeof(XpTracker))]
  public class XpDistributor_TimeBased : MonoBehaviour
  {
    public string Id = "Main";
    public float PointsPerSecond = 1;

    [SerializeField]
    long lastUpdate = 0; 

    XpTracker xpTracker;

    void Start()
    {
      xpTracker = this.GetComponent<XpTracker>();

      if(xpTracker == null)
      {
        Debug.LogError("[XpDistributor_RealTime] Required component 'Xp' does not exist", this);
        this.enabled = false;
        return;
      }
    }

    public void Reset(long seconds)
    {
      lastUpdate = seconds;
    }

    public void Refresh(long seconds)
    {
      long elapsed = seconds - lastUpdate;
      lastUpdate = seconds;

      if(xpTracker != null)
      {
        xpTracker.Grant(Id, elapsed * PointsPerSecond);
      }
    }
  }
}