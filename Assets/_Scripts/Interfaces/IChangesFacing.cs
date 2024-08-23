using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IChangesFacing {
    public event Action<bool> OnChangedFacing; // bool: facing right
}
