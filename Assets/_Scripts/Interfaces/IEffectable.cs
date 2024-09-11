using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEffectable {
    public void AddEffect(UnitEffectBase effect, bool removeAfterDuration = false, float duration = 0);
    public void RemoveEffect(UnitEffectBase effect);

    public bool ContainsEffect(UnitEffectBase effect);
}
