using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XpModule.Demos.Professions
{
  // This is only for demo display
  public class ProfessionsNumber : MonoBehaviour
  {
    public List<GameObject> Numbers = new List<GameObject>();
    [SerializeField] TMPro.TMP_Text lvlNum;

    void Start()
    {
      Refresh(0);
    }

    public void Refresh(int number)
    {
      lvlNum.SetText(number);

      return;
      for(int i = 0; i < Numbers.Count; i++)
         Numbers[i].SetActive(false);

      Numbers[number].SetActive(true);
    }
  }
}
