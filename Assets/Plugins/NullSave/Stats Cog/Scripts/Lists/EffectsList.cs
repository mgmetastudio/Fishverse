using System.Collections.Generic;
using UnityEngine;

namespace NullSave.TOCK.Stats
{
    public class EffectsList : MonoBehaviour
    {

        #region Variables

        [Tooltip("Stats Cog providing list")] public StatsCog statsCog;
        [Tooltip("Prefab used to display items")] public StatEffectUI listItem;
        [Tooltip("Container for prefab items")] public GameObject target;
        [Tooltip("Categories to display (if none provided all are displayed)")] public List<string> filterCats;

        private List<StatEffectUI> tracked;

        #endregion

        #region Unity Methods

        public void Start()
        {
            tracked = new List<StatEffectUI>();
        }

        public void Update()
        {
            // Remnove extra items
            if (tracked.Count > statsCog.Effects.Count)
            {
                int diff = tracked.Count - statsCog.Effects.Count;
                for (int i = 0; i < diff; i++)
                {
                    if (tracked[0] != null) Destroy(tracked[0].gameObject);
                    tracked.RemoveAt(0);
                }
            }

            // Add new items
            while (tracked.Count < statsCog.Effects.Count)
            {
                GameObject newItem = Instantiate(listItem.gameObject);
                newItem.transform.SetParent(target.transform, false);
                tracked.Add(newItem.GetComponent<StatEffectUI>());
            }

            // Update items
            for (int i = 0; i < tracked.Count; i++)
            {
                if (filterCats.Count == 0 || filterCats.Contains(statsCog.Effects[i].category))
                {
                    tracked[i].SetEffect(statsCog.Effects[i]);
                }
                else
                {
                    tracked[i].gameObject.SetActive(false);
                }
            }
        }

        #endregion

    }
}