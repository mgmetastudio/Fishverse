#if GDTK
namespace NullSave.GDTK.Stats
{
    public interface IStatClient
    {

        #region Properties

        BasicStats statSource { get; set; }

        string statId { get; set; }

        #endregion

        #region Public Methods

        void SetStatIdAndSource(BasicStats source, string id);

        #endregion

    }
}
#endif