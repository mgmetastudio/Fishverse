using NullSave.TOCK.Inventory;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ThemeSelection : MonoBehaviour
{

    public InventoryCog inventoryCog;

    private  TMP_Dropdown dropDown;

    // Start is called before the first frame update
    void Start()
    {
        dropDown = GetComponent<TMP_Dropdown>();
        dropDown.options.Clear();
        foreach (InventoryTheme theme in InventoryDB.Themes)
        {
            dropDown.options.Add(new TMP_Dropdown.OptionData(theme.displayName));
        }
        dropDown.captionText.text = InventoryDB.Themes[0].displayName;
        dropDown.onValueChanged.AddListener(ChangeTheme);
    }

    void ChangeTheme(int index)
    {
        inventoryCog.ActiveTheme = InventoryDB.Themes[index];
    }
}
