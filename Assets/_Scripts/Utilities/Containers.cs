using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Containers : StaticInstance<Containers> {

    [field: SerializeField] public Transform Enemies { get; private set; }
    [field: SerializeField] public Transform Projectiles { get; private set; }
    [field: SerializeField] public Transform Drops { get; private set; }
    [field: SerializeField] public Transform Rooms { get; private set; }
    [field: SerializeField] public Transform MapIcons { get; private set; }
    [field: SerializeField] public Transform RoomMapIcons { get; private set; }
    [field: SerializeField] public Transform Hallways { get; private set; }
    [field: SerializeField] public Transform RoomOverlapCheckers { get; private set; }
    [field: SerializeField] public Transform EnvironmentObjects { get; private set; }
    [field: SerializeField] public Transform Effects { get; private set; }

    [field: SerializeField] public Transform HUD { get; private set; }
    [field: SerializeField] public Transform ActiveModifierImages { get; private set; }
    [field: SerializeField] public Transform WorldUI { get; private set; }

    public Transform GetContainer(Container container) {
        switch (container) {
            case Container.Enemies:
                return Enemies;
            case Container.Projectiles:
                return Projectiles;
            case Container.Drops:
                return Drops;
            case Container.Rooms:
                return Rooms;
            case Container.LevelMapIcons:
                return MapIcons;
            case Container.Hallways:
                return Hallways;
            case Container.RoomOverlapCheckers:
                return RoomOverlapCheckers;
            case Container.EnvironmentObjects:
                return EnvironmentObjects;
            case Container.Effects:
                return Effects;
            default:
                Debug.LogError($"Container type {container} not handled in GetContainer");
                return null;
        }
    }
}

[Serializable]
public enum Container {
    Enemies = 0,
    Projectiles = 1,
    Drops = 2,
    Rooms = 3,
    Hallways = 4,
    RoomOverlapCheckers = 5,
    EnvironmentObjects = 6,
    Effects = 7,
    LevelMapIcons = 8
}
