using System;

public interface ICloseable
{

    #region Properties

    Action onCloseCalled { get; set; }

    #endregion

}
