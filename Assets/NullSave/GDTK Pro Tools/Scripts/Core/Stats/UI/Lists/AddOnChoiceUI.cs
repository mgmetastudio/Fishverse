#if GDTK
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace NullSave.GDTK.Stats
{
    [AutoDoc("This UI component displays a single Add-On Choice for the user and can be use for either a ISelectableOption *or* a AddOnPluginChoice.")]
    public class AddOnChoiceUI : statsInfoUI
    {

        #region Fields

        [Tooltip("Allow item to be selected via pointer click")] public bool clickToSelect;

        [Tooltip("Event fired when item is selected")] public UnityEvent onSelected;
        [Tooltip("Event fired when item is loses selection")] public UnityEvent onDeselected;

        private AddOnChoiceListUI owner;
        private bool m_selected;

        #endregion

        #region Properties

        [AutoDoc("Returns the Selectable Option if one is set", "using NullSave.GDTK;<br/>using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    AddOnChoiceUI source;<br/><br/>    public void ExampleMethod()<br/>    {<br/>        ISelectableOption option = source.option;<br/>    }<br/><br/>}")]
        public ISelectableOption option { get; private set; }

        [AutoDoc("Returns the active Add-On Plugin Choice if one is set", "using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    AddOnChoiceUI source;<br/><br/>    public void ExampleMethod()<br/>    {<br/>        AddOnPluginChoice optionList = source.pluginOptionList;<br/>    }<br/><br/>}")]
        public AddOnPluginChoice pluginOptionList { get; private set; }

        public bool selected
        {
            get { return m_selected; }
            set
            {
                if (m_selected == value) return;
                m_selected = value;
                if (m_selected)
                {
                    onSelected?.Invoke();
                }
                else
                {
                    onDeselected?.Invoke();
                }
            }
        }

        #endregion

        #region Unity Methods

        [AutoDocSuppress]
        public override void OnPointerClick(PointerEventData eventData)
        {
            RequestSelection();
            base.OnPointerClick(eventData);
        }

        [AutoDocSuppress]
        public void Reset()
        {
            clickToSelect = true;
        }

        #endregion

        #region Public Methods

        [AutoDoc("Load data for an ISelectableOption", "using NullSave.GDTK;<br/>using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    public AddOnChoiceListUI source;<br/>    public AddOnChoiceUI uiPrefab;<br/><br/>    public void ExampleMethod(ISelectableOption option)<br/>    {<br/>        AddOnChoiceUI choice = Instantiate(uiPrefab, source.content);<br/>        choice.gameObject.SetActive(true);<br/>        choice.LoadChoice(option, source);<br/>    }<br/><br/>}")]
        [AutoDocParameter("Option to load")]
        [AutoDocParameter("UI List displaying this option")]
        public void LoadChoice(ISelectableOption option, AddOnChoiceListUI owner)
        {
            this.owner = owner;
            this.option = option;
            info = option.optionInfo;

            ApplyImage();

            foreach (TemplatedLabel label in labels)
            {
                label.target.text = FormatInfo(label.format);
            }
        }

        [AutoDoc("Load data for an AddOnPluginChoice", "using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    public AddOnChoiceListUI source;<br/>    public AddOnChoiceUI uiPrefab;<br/><br/>    public void ExampleMethod(AddOnPluginChoice option)<br/>    {<br/>        AddOnChoiceUI choice = Instantiate(uiPrefab, source.content);<br/>        choice.gameObject.SetActive(true);<br/>        choice.LoadChoice(option, source);<br/>    }<br/><br/>}")]
        [AutoDocParameter("Option to load")]
        [AutoDocParameter("UI List displaying this option")]
        public void LoadChoice(AddOnPluginChoice option, AddOnChoiceListUI owner)
        {
            this.owner = owner;
            pluginOptionList = option;
            info = option.info;

            ApplyImage();

            foreach (TemplatedLabel label in labels)
            {
                label.target.text = FormatInfo(label.format);
            }
        }

        [AutoDoc("Ask owner to toggle selection for this object.", "using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    public void ExampleMethod(AddOnChoiceUI option)<br/>    {<br/>        option.RequestSelection();<br/>    }<br/><br/>}")]
        public void RequestSelection()
        {
            if (owner == null) return;
            owner.ToggleChildSelect(this);
        }

        #endregion

    }
}
#endif