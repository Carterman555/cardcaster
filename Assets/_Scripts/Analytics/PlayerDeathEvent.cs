using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Analytics;

public class PlayerDeathEvent : Unity.Services.Analytics.Event {

    public PlayerDeathEvent() : base("onDeath") {
    }

    public float RunTime { set { SetParameter("amountOfTime", value); } } // amount of time on current run
    public string Room { set { SetParameter("room", value); } }
    public int Level { set { SetParameter("level", value); } }
}
