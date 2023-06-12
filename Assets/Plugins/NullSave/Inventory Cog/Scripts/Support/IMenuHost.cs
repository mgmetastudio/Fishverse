namespace NullSave.TOCK.Inventory
{
    public interface IMenuHost
    {

        #region Properties

        InventoryCog Inventory { get; set; }

        #endregion

        #region Methods

        void LoadComponents();

        void Load(string filename);

        void Load(System.IO.Stream stream);

        void Save(string filename);

        void Save(System.IO.Stream stream);

        #endregion

    }
}