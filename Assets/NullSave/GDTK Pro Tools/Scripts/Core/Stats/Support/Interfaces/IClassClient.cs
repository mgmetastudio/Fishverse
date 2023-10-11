#if GDTK
namespace NullSave.GDTK.Stats
{
    public interface IClassClient
    {

        #region Properties

        PlayerCharacterStats statSource { get; set; }

        string classId { get; set; }

        #endregion

        #region Public Methods

        void SetClassIdAndSource(PlayerCharacterStats source, string id);

        #endregion

    }
}
#endif