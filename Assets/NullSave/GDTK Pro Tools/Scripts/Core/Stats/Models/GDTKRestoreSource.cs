#if GDTK
namespace NullSave.GDTK.Stats
{
    public class GDTKRestoreSource : IUniquelyIdentifiable
    {

        #region Properties

        public string instanceId { get; set; }

        #endregion

        #region Constructor

        public GDTKRestoreSource(string id)
        {
            instanceId = id;
        }

        #endregion

    }
}
#endif