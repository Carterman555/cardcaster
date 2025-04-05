public class RoomEndEvent : Unity.Services.Analytics.Event {

    public RoomEndEvent() : base("onRoomEnd") {
    }

    public string Room { set { SetParameter("room", value); } } // time until clear or die
    public float TimeToEnd { set { SetParameter("amountOfTime", value); } } // time until clear or die
    public float HealthLost { set { SetParameter("healthLost", value); } }
    public bool Survived { set { SetParameter("survived", value); } }
}
