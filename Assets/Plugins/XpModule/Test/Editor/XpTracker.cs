using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using NUnit.Framework;
using XpTracker = XpModule.XpTracker;

public class Xp {

  // AddExperience
  //
  [Test]
  [Description("Should succeed : add experience when no floor is defined")]
  public void Xp_AddExperience_PositiveValue_NoFloor()
  {
    var gameObject = new GameObject();
    XpTracker xp = gameObject.AddComponent<XpTracker> ();
    xp.Get("Main").Floors = new List<float>() { 100 };
    xp.Get("Main").Points += 50;

    var experience = xp.Get("Main").Points;
    var level = xp.Get("Main").Level;

    Assert.AreEqual (50, experience);
    Assert.AreEqual (0, level);
  }

  [Test]
  [Description("Should succeed : remove experience when no floor is defined")]
  public void Xp_AddExperience_NegativeValue_NoFloor()
  {
    var gameObject = new GameObject();
    XpTracker xp = gameObject.AddComponent<XpTracker> ();
    xp.Get("Main").Floors = new List<float>() { 100 };
    xp.Get("Main").Points -= 50;

    var experience = xp.Get("Main").Points;
    var level = xp.Get("Main").Level;

    Assert.AreEqual (-50, experience);
    Assert.AreEqual (0, level);
  }

  [Test]
  [Description("Should succeed : add experience, floor defined but not reached")]
  public void Xp_AddExperience_PositiveValue_FloorNotReached()
  {
    var gameObject = new GameObject();
    XpTracker xp = gameObject.AddComponent<XpTracker> ();
    xp.Get("Main").Floors = new List<float>() { 100 };
    xp.Get("Main").Points += 50;

    var experience = xp.Get("Main").Points;
    var level = xp.Get("Main").Level;

    Assert.AreEqual (50, experience);
    Assert.AreEqual (0, level);
  }

  [Test]
  [Description("Should succeed : add experience, floor defined and exactly reached")]
  public void Xp_AddExperience_PositiveValue_FloorExactlyReached()
  {
    var gameObject = new GameObject();
    XpTracker xp = gameObject.AddComponent<XpTracker> ();
    xp.Get("Main").Floors = new List<float>() { 100 };
    xp.Get("Main").Points += 100;

    var experience = xp.Get("Main").Points;
    var level = xp.Get("Main").Level;

    Assert.AreEqual (100, experience);
    Assert.AreEqual (1, level);
  }

  [Test]
  [Description("Should succeed : add experience, floor defined and over reached")]
  public void Xp_AddExperience_PositiveValue_FloorOverReached()
  {
    var gameObject = new GameObject();
    XpTracker xp = gameObject.AddComponent<XpTracker> ();
    xp.Get("Main").Floors = new List<float>() { 100 };
    xp.Get("Main").Points += 120;

    var experience = xp.Get("Main").Points;
    var level = xp.Get("Main").Level;

    Assert.AreEqual (120, experience);
    Assert.AreEqual (1, level);
  }

    [Test]
  [Description("Should succeed : add experience, floor defined and reached multiple times at once")]
  public void Xp_AddExperience_PositiveValue_FloorReachedMultiplesTimes()
  {
    var gameObject = new GameObject();
    XpTracker xp = gameObject.AddComponent<XpTracker> ();
    xp.Get("Main").Floors = new List<float>() { 100, 200, 300 };
    xp.Get("Main").Points += 500;

    var experience = xp.Get("Main").Points;
    var level = xp.Get("Main").Level;

    Assert.AreEqual (500, experience);
    Assert.AreEqual (3, level);
  }

  // GetFloor
  [Test]
  [Description("Should succeed : get floor when not defined")]
  public void GetFloor_NullDefined()
  {
    var gameObject = new GameObject();
    var xp = gameObject.AddComponent<XpTracker> ();
    xp.Get("Main").Floors = null;

    var floor = xp.Get("Main").GetFloor(5);

    Assert.AreEqual(0, floor);
  }

  [Test]
  [Description("Should succeed : get floor when level is out of range")]
  public void GetFloor_OutOfRange()
  {
    var gameObject = new GameObject();
    var xp = gameObject.AddComponent<XpTracker> ();
    xp.Get("Main").Floors = new List<float>() { 100, 200, 300 };

    var floor = xp.Get("Main").GetFloor(5);

    Assert.AreEqual(0, floor);
  }

  [Test]
  [Description("Should succeed : get floor when level is defined")]
  public void GetFloor_InRange()
  {
    var gameObject = new GameObject();
    var xp = gameObject.AddComponent<XpTracker> ();
    xp.Get("Main").Floors = new List<float>() { 100, 200, 300 };

    var floor = xp.Get("Main").GetFloor(2);

    Assert.AreEqual(300, floor);
  }
}
