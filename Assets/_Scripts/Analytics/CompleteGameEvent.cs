public class CompleteGameEvent : Unity.Services.Analytics.Event {

    public CompleteGameEvent() : base("onCompleteGame") {
    }

    public float TimeToComplete { set { SetParameter("amountOfTime", value); } } // amount of time on current run
}
