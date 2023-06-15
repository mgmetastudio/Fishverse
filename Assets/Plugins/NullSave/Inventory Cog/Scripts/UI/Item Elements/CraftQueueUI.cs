using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NullSave.TOCK.Inventory
{
    public class CraftQueueUI : MonoBehaviour
    {

        #region Variables

        public RecipeUI recipeUI;
        public Slider progress;
        public TextMeshProUGUI startedTime, endTime, timeRemaining, count;

        // https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-timespan-format-strings
        [Tooltip("Uses standard c# time formatting")] public string startEndFormat = "hh\\:mm\\:ss";

        // https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-timespan-format-strings
        [Tooltip("Uses standard c# time formatting")] public string timeFormat = "m\\:ss";

        public CraftingQueueItem item;

        #endregion

        #region Unity Methods

        private void Update()
        {
            UpdateUI();
        }

        #endregion

        #region Public Methods

        public void LoadQueue(CraftingQueueItem queueItem)
        {
            item = queueItem;

            if(recipeUI != null)
            {
                recipeUI.Inventory = queueItem.recipe.InventoryCog;
                recipeUI.LoadRecipe(queueItem.recipe, recipeUI.Inventory.GetRecipeCraftable(queueItem.recipe));
            }

            UpdateUI();
        }

        #endregion

        #region Private Methods

        private void UpdateUI()
        {
            if(progress != null)
            {
                progress.minValue = 0;
                progress.maxValue = 1;
                progress.value = item.recipe.InventoryCog.GetQueuedProgress(item);
            }

            if (count != null)
            {
                count.text = item.count.ToString();
            }

            if(startedTime != null)
            {
                startedTime.text = item.timeStarted.ToString(startEndFormat);
            }

            if (endTime != null)
            {
                startedTime.text = item.realWorldEnd.ToString(startEndFormat);
            }

            if (timeRemaining != null)
            {
                System.TimeSpan ts = System.TimeSpan.FromSeconds(item.secondsRemaining);
                timeRemaining.text = ts.ToString(timeFormat);
            }
        }

        #endregion

    }
}