#if GDTK
namespace NullSave.GDTK.Stats
{
    public class TraitUI : statsInfoUI
    {

        #region Properties

        public GDTKTrait trait { get; private set; }

        #endregion

        #region Public Methods

        public void Load(GDTKTrait trait)
        {
            this.trait = trait;
            info = trait.info;

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