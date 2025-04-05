public class LevelCompleteEvent : Unity.Services.Analytics.Event {

    public LevelCompleteEvent() : base("onLevelComplete") {
    }

    public float TimeInLevel { set { SetParameter("amountOfTime", value); } } // time spent in level
    public int Level { set { SetParameter("level", value); } } // time spent in level
}
