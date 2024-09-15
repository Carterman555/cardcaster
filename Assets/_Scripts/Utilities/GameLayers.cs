using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLayers {

    public static int GroundLayer => 3;
    public static int PlayerLayer => 6;
    public static int EnemyLayer => 7;
    public static int WallLayer => 8;
    public static int RoomObjectLayer => 10;

    public static LayerMask EnemyLayerMask => 1 << EnemyLayer;

}
