using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FootStepsGroup : MonoBehaviour
{
    public AudioGroup[] Groups;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    public AudioGroup GetGroupFor(string tag)
    {
        for (int i = 0; i < Groups.Length; i++)
        {
            if (Groups[i].Tag.Equals(tag))
            {
                return Groups[i];
            }
        }
        return Groups[0];
    }

    [Serializable]
    public class AudioGroup
    {
        public string Tag;
        public AudioClip[] StepSounds;

        public AudioClip GetRandomClip()
        {
            return StepSounds[UnityEngine.Random.Range(0, StepSounds.Length)];
        }
    }
}
