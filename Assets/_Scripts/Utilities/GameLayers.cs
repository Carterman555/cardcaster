using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLayers {

    public static int GroundLayer => 3;
    public static int PlayerLayer => 6;
    public static int EnemyLayer => 7;
    public static int WallLayer => 8;
    public static int DoorBlockerLayer => 9;
    public static int RoomObjectLayer => 10;
    public static int MapLayer => 13;

    public static LayerMask EnemyLayerMask => 1 << EnemyLayer;
    public static LayerMask PlayerLayerMask => 1 << PlayerLayer;
    public static LayerMask RoomObjectLayerMask => 1 << RoomObjectLayer;
    public static LayerMask ObstacleLayerMask => 1 << WallLayer | 1 << DoorBlockerLayer | 1 << RoomObjectLayer;
}
