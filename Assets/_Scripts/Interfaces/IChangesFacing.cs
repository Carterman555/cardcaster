using System;

public interface IChangesFacing {
    public event Action<bool> OnChangedFacing; // bool: facing right
}
