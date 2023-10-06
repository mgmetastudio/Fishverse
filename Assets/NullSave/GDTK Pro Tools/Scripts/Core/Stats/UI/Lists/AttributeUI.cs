#if GDTK
namespace NullSave.GDTK.Stats
{
    public class AttributeUI : statsInfoUI
    {

        #region Properties

        public GDTKAttribute attribute { get; private set; }

        #endregion

        #region Public Methods

        public void Load(GDTKAttribute attribute)
        {
            this.attribute = attribute;
            info = attribute.info;

            ApplyImage();

            foreach (TemplatedLabel label in labels)
            {
                label.target.text = FormatInfo(label.format);
            }
        }

        #endregion

    }
}
#endif