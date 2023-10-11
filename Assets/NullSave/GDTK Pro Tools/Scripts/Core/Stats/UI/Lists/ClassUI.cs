#if GDTK
namespace NullSave.GDTK.Stats
{
    [AutoDoc("Automatically updates all Class Labels in children to use the Class associated with this object.")]
    public class ClassUI : statsInfoUI
    {

        #region Fields

        private GDTKClass playerClass;

        #endregion

        #region Properties

        [AutoDoc("Returns the Id of the current Class.", "using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    ClassUI source;<br/><br/>    public void ExampleMethod()<br/>    {<br/>        string id = source.id;<br/>    }<br/><br/>}")]
        public string id { get; private set; }

        #endregion

        #region Public Methods

        [AutoDoc("Load a Class and update all children.", "using NullSave.GDTK.Stats;<br/>using UnityEngine;<br/><br/>public class Example : MonoBehaviour<br/>{<br/><br/>    ClassUI source;<br/>    PlayerCharacterStats statSource;<br/><br/>    public void ExampleMethod()<br/>    {<br/>        source.Load(statSource, \"exampleId\");<br/>    }<br/><br/>}")]
        [AutoDocParameter("Source providing the Class info")]
        [AutoDocParameter("Id of the Class to load")]
        public void Load(PlayerCharacterStats source, string classId)
        {
            if(playerClass != null)
            {
                playerClass.onLevelChanged -= UpdateUI;
            }


            id = classId;
            if(source != null)
            {
                playerClass = source.GetClass(classId);
            }
            else
            {
                playerClass = ToolRegistry.GetComponent<StatsDatabase>().GetClass(classId);
            }

            info = playerClass?.info;
            if(playerClass != null) playerClass.onLevelChanged += UpdateUI;

            UpdateUI();
        }

        #endregion

        #region Private Methods

        private void UpdateUI()
        {
            ApplyImage();

            foreach (TemplatedLabel label in labels)
            {
                label.target.text = FormatInfo(label.format)
                    .Replace("{level}", playerClass?.level.ToString())
                    ;
            }
        }

        #endregion

    }
}
#endif