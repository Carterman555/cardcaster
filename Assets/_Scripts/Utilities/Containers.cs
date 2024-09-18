using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Containers : StaticInstance<Containers> {

    [field:SerializeField] public Transform Enemies { get; private set; }
    [field:SerializeField] public Transform Projectiles { get; private set; }
    [field:SerializeField] public Transform Items { get; private set; }
    [field:SerializeField] public Transform Rooms { get; private set; }
    [field:SerializeField] public Transform EnvironmentObjects { get; private set; }
    [field:SerializeField] public Transform Effects { get; private set; }

    [field:SerializeField] public Transform WorldUI { get; private set; }

}
