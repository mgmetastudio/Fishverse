#if GDTK
namespace NullSave.GDTK.Stats
{
    public class PerkUI : statsInfoUI
    {

        #region Properties

        public GDTKPerk perk { get; private set; }

        #endregion

        #region Public Methods

        public void Load(GDTKPerk perk)
        {
            this.perk = perk;
            info = perk.info;

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