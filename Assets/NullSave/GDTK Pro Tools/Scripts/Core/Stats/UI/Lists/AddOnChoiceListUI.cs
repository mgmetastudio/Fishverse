#if GDTK
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace NullSave.GDTK.Stats
{
    [AutoDoc("This UI component provides a list of Add-Ons the user may choose from. The number of allowed for selection is controlled by the Plug-in providing the list.")]
    public class AddOnChoiceListUI : statsInfoUI
    {

        #region Fields

        [Tooltip("Change timescale on enable, restores to previous timescale on disable")] public bool setTimeScale;
        [Tooltip("Timescale to set")] public float timeScale;
        [Tooltip("Format to apply to option text")] public string optionFormat;

        [Tooltip("Prefab to create for displaying choice")] public AddOnChoiceUI uiPrefab;
        [Tooltip("Transform to place prefabs inside")] public Transform content;
        [Tooltip("Automatically close after required selection(s) made")] public bool autoClose;

        [Tooltip("Event fired whenever selection is valid for submit")] public UnityEvent onCanSubmit;
        [Tooltip("Event fired whenever selection is not valid for submit")] public UnityEvent onCannotSubmit;

        private float restoreScale;

        private AddOnPlugin choicePlugin;

        private PlayerCharacterStats requester;

        private Action<List<ISelectableOption>> callback;

        private List<AddOnPluginChoice> pluginChoiceList;
        private AddOnPluginChoice selectChoice;
        private Action<AddOnPluginChoice> pluginChoiceCallback;

        private List<AddOnChoiceUI> loaded;

        #endregion

        #region Unity Methods


        private void Awake()
        {
            if (uiPrefab != null && uiPrefab.gameObject.scene.buildIndex != -1)
            {
                uiPrefab.gameObject.SetActive(false);
            }
        }

        private void OnDisable()
        {
            if (setTimeScale)
            {
                Time.timeScale = restoreScale;
            }
        }

        private void OnEnable()
        {
            if (setTimeScale)
            {
                restoreScale = Time.timeScale;
                Time.timeScale = timeScale;
            }
        }

        private void Reset()
        {
            setTimeScale = true;
            restoreScale = 1;
            optionFormat = "Select {0} to Continue";
            autoClose = true;
            content = transform;
        }

        #endregion

        #region Public Methods

        [AutoDoc("Cancel selection.", "using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    public AddOnChoiceListUI source;<br/><br/>    public void ExampleMethod()<br/>    {<br/>        source.Cancel();<br/>    }<br/><br/>}")]
        public void Cancel()
        {
            Destroy(gameObject);
        }

        [AutoDoc("Clears the currently loaded list of options.", "using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    public AddOnChoiceListUI source;<br/><br/>    public void ExampleMethod()<br/>    {<br/>        source.Clear();<br/>    }<br/><br/>}")]
        public void Clear()
        {
            if (loaded == null)
            {
                loaded = new List<AddOnChoiceUI>();
                return;
            }

            foreach (AddOnChoiceUI choice in loaded)
            {
                Destroy(choice.gameObject);
            }

            loaded.Clear();
        }

        [AutoDoc("Load a list of choices", "using NullSave.GDTK;<br/>using NullSave.GDTK.Stats;<br/>using System.Collections.Generic;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    public PlayerCharacterStats statSource;<br/>    public AddOnChoiceListUI source;<br/><br/>    public void ExampleMethod(AddOnPlugin addOn)<br/>    {<br/>        addOn.Initialize(statSource);<br/>        source.LoadChoices(addOn, statSource, SelectionComplete);<br/>    }<br/><br/>    private void SelectionComplete(List<ISelectableOption> result)<br/>    {<br/>        Debug.Log(\"Options Chosen: \" + result.Count);<br/>    }<br/><br/>}")]
        [AutoDocParameter("Plug-in providing list")]
        [AutoDocParameter("Object providing stats")]
        [AutoDocParameter("Callback to invoke when choice is made")]
        public void LoadChoices(AddOnPlugin plugin, BasicInfo info, PlayerCharacterStats source, Action<List<ISelectableOption>> resultsCallback = null)
        {
            Clear();

            choicePlugin = plugin;
            requester = source;
            callback = resultsCallback;

            this.info = info;

            ApplyImage();

            foreach (TemplatedLabel label in labels)
            {
                label.target.text = FormatInfo(label.format);
            }

            foreach (ISelectableOption option in choicePlugin.availableOptions)
            {
                AddOnChoiceUI choice = Instantiate(uiPrefab, content);
                choice.gameObject.SetActive(true);
                choice.LoadChoice(option, this);
                loaded.Add(choice);
            }
        }

        [AutoDoc("Load a list of choices", "using NullSave.GDTK;<br/>using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    public PlayerCharacterStats statSource;<br/>    public AddOnChoiceListUI source;<br/><br/>    public void ExampleMethod(AddOnPlugin addOn)<br/>    {<br/>        StatsDatabase statsDatabase = ToolRegistry.GetComponent<StatsDatabase>();<br/><br/>        // Clone entry to prevent choices applying to database<br/>        GDTKLevelReward rewardClone = statsDatabase.levelRewards[0].Clone();<br/>        source.LoadChoices(rewardClone.pluginChoices, statSource, SelectionComplete);<br/>    }<br/><br/>    private void SelectionComplete(AddOnPluginChoice result)<br/>    {<br/>        Debug.Log(\"Option Chosen: \" + result.info.id);<br/>    }<br/><br/>}")]
        [AutoDocParameter("List of Plug-ins to choose from")]
        [AutoDocParameter("Object providing stats")]
        [AutoDocParameter("Callback to invoke when choice is made")]
        public void LoadChoices(List<AddOnPluginChoice> choices, BasicInfo info, PlayerCharacterStats source, Action<AddOnPluginChoice> callback)
        {
            Clear();

            pluginChoiceList = choices;
            requester = source;
            pluginChoiceCallback = callback;

            this.info = info;

            ApplyImage();

            foreach (TemplatedLabel label in labels)
            {
                label.target.text = FormatInfo(label.format);
            }

            foreach (AddOnPluginChoice option in choices)
            {
                AddOnChoiceUI choice = Instantiate(uiPrefab, content);
                choice.gameObject.SetActive(true);
                choice.LoadChoice(option, this);
                loaded.Add(choice);
            }
        }

        [AutoDoc("Toggles whether or not an object has been selected", "using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    public AddOnChoiceListUI source;<br/><br/>    public void ExampleMethod(AddOnChoiceUI addOn)<br/>    {<br/>        source.ToggleChildSelect(addOn);<br/>    }<br/><br/><br/>}")]
        [AutoDocParameter("Child to target when toggling selected state")]
        public void ToggleChildSelect(AddOnChoiceUI child)
        {
            if (choicePlugin != null)
            {
                if (choicePlugin.OptionSelected(child.option))
                {
                    choicePlugin.RemoveSelection(child.option);
                    SetSelection(child.option, false);
                    onCannotSubmit?.Invoke();
                    return;
                }

                // Will we be over the limit if we add a choice?
                if(!choicePlugin.hasUnused)
                {
                    RemoveSelectedById(choicePlugin.selectedIds[0]);
                }

                // Add new option
                choicePlugin.AddSelection(child.option);
                SetSelection(child.option, true);

                if (!choicePlugin.hasUnused )
                {
                    onCanSubmit?.Invoke();
                    if (autoClose)
                    {
                        Submit();
                    }
                }
                else
                {
                    onCannotSubmit?.Invoke();
                }

                return;
            }
            else
            {
                if (autoClose)
                {
                    selectChoice = child.pluginOptionList;
                    Submit();
                }
                else
                {
                    if(selectChoice == child.pluginOptionList)
                    {
                        selectChoice = null;
                        child.selected = false;
                        onCannotSubmit?.Invoke();
                    }
                    else
                    {
                        if(selectChoice != null)
                        {
                            foreach(var option in loaded)
                            {
                                if(option.pluginOptionList == selectChoice)
                                {
                                    option.selected = false;
                                }
                            }
                        }

                        selectChoice = child.pluginOptionList;
                        child.selected = true;
                        onCanSubmit?.Invoke();
                    }
                }
            }
        }

        [AutoDoc("Finalize choices, invoke callback, and destroy object.", "using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    public AddOnChoiceListUI source;<br/><br/>    public void ExampleMethod()<br/>    {<br/>        if (!source.Submit())<br/>        {<br/>            Debug.Log(\"Invalid submit state\");<br/>        }<br/>    }<br/><br/><br/>}")]
        public void Submit()
        {
            if(pluginChoiceCallback != null)
            {
                if (selectChoice == null) return;
                Destroy(gameObject);
                pluginChoiceCallback?.Invoke(selectChoice);
                return;
            }

            if (choicePlugin.hasUnused)
            {
                return;
            }
            choicePlugin.Apply(requester, requester.globalStats);
            Destroy(gameObject);
            callback?.Invoke(choicePlugin.selectedOptions.ToList());
        }

        #endregion

        #region Private Methods

        private void RemoveSelectedById(string id)
        {
            foreach (AddOnChoiceUI ui in loaded)
            {
                if (ui.option.optionInfo.id == id)
                {
                    choicePlugin.RemoveSelection(ui.option);
                    ui.selected = false;
                    return;
                }
            }
        }

        private void SetSelection(ISelectableOption option, bool state)
        {
            foreach(AddOnChoiceUI ui in loaded)
            {
                if(ui.option == option)
                {
                    ui.selected = state;
                    return;
                }
            }
        }

        #endregion

    }
}
#endif