using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NullSave.TOCK.Inventory
{
    [HierarchyIcon("category", false)]
    public class CraftingCategoryUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {

        #region Variables

        public TextMeshProUGUI categoryName;
        public Image categoryImage;
        public Color activeColor = Color.white;
        public Color inactiveColor = new Color(1, 1, 1, 0.4f);
        public Color activeTextColor = Color.white;
        public Color inactiveTextColor = new Color(1, 1, 1, 0.4f);

        public UnityEvent onClick, onPointerEnter, onPointerExit;

        #endregion

        #region Properties

        public CraftingCategory Category { get; set; }

        #endregion

        #region Unity Methods

        public void OnPointerClick(PointerEventData eventData)
        {
            onClick?.Invoke();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            onPointerEnter?.Invoke();
        }


        public void OnPointerExit(PointerEventData eventData)
        {
            onPointerExit?.Invoke();
        }

        #endregion

        #region Public Methods

        public void LoadCategory(CraftingCategory category)
        {
            Category = category;

            if (categoryName != null) categoryName.text = category.displayName;
            if (categoryImage != null) categoryImage.sprite = category.icon;
        }

        public void SetActive(bool active)
        {
            if (categoryImage != null)
            {
                categoryImage.color = active ? activeColor : inactiveColor;
            }

            if (categoryName != null)
            {
                categoryName.color = active ? activeTextColor : inactiveTextColor;
            }
        }

        #endregion

    }
}