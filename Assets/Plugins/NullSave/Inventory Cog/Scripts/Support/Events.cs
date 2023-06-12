﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace NullSave.TOCK.Inventory
{

    [Serializable]
    public class CategorySelected : UnityEvent<Category> { }

    [Serializable]
    public class ContainerDropped : UnityEvent<InventoryItem, int, List<ItemReference>> { }

    [Serializable]
    public class CraftingFailed : UnityEvent<CraftingRecipe, int> { }

    [Serializable]
    public class DisplayNameRequest : UnityEvent<PopupUI, object> { }

    [Serializable]
    public class DLCUpdate : UnityEvent<int, int, long, long> { }

    [Serializable]
    public class ItemChanged : UnityEvent<InventoryItem> { }

    [Serializable]
    public class ItemCountChanged : UnityEvent<InventoryItem, int> { }

    [Serializable]
    public class ItemUIClick : UnityEvent<ItemUI> { }

    [Serializable]
    public class AttachmentSlotUIClick : UnityEvent<AttachmentSlotUI> { }

    [Serializable]
    public class OptionUIClick : UnityEvent<ItemMenuOptionUI> { }

    [Serializable]
    public class ItemListSubmit : UnityEvent<InventoryItemList, ItemUI, List<Category>> { }

    [Serializable]
    public class RecipeListSubmit : UnityEvent<RecipeList, RecipeUI> { }

    [Serializable]
    public class PageChanged : UnityEvent<int> { }

    [Serializable]
    public class RecipeUIClick : UnityEvent<RecipeUI> { }

    [Serializable]
    public class SkillSlotChanged : UnityEvent<string, InventoryItem> { }

    [Serializable]
    public class ThemeWindowEvent : UnityEvent<GameObject> { }

}