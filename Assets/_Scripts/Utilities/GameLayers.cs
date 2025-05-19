using UnityEngine;

public class GameLayers {

    public static int DefaultLayer => 0;
    public static int GroundLayer => 3;
    public static int PlayerLayer => 6;
    public static int EnemyLayer => 7;
    public static int WallLayer => 8;
    public static int DoorBlockerLayer => 9;
    public static int RoomObjectLayer => 10;
    public static int ProjectileLayer => 11;
    public static int MapLayer => 13;
    public static int InvinciblePlayerLayer => 14;
    public static int BluePlantLayer => 15;

    public static LayerMask EnemyLayerMask => 1 << EnemyLayer;
    public static LayerMask PlayerLayerMask => 1 << PlayerLayer;
    public static LayerMask AllPlayerLayerMask => 1 << PlayerLayer | 1 << InvinciblePlayerLayer;
    public static LayerMask RoomObjectLayerMask => 1 << RoomObjectLayer;
    public static LayerMask AllRoomObjectLayerMask => 1 << RoomObjectLayer | 1 << BluePlantLayer;
    public static LayerMask WallLayerMask => 1 << WallLayer;
    public static LayerMask ObstacleLayerMask => 1 << WallLayer | 1 << DoorBlockerLayer | 1 << RoomObjectLayer;
    public static LayerMask PlayerTargetLayerMask = 1 << EnemyLayer | 1 << RoomObjectLayer | 1 << ProjectileLayer | 1 << BluePlantLayer;
}
