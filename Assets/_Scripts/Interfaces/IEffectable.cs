using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEffectable {
    public void AddEffect(UnitEffectBase effect);
    public void RemoveEffect(UnitEffectBase effect);
}
