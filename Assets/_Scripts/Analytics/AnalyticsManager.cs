using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Analytics;
using System;
using Unity.Services.Core;

public class AnalyticsManager : MonoBehaviour {

    async void Awake() {
        try {
            await UnityServices.InitializeAsync();
        }
        catch (Exception e) {
            Debug.LogException(e);
        }

        AnalyticsService.Instance.StartDataCollection();

        //CompleteGameEvent completeGameEvent = new() {
        //    TimeToComplete = Time.time
        //};

        //AnalyticsService.Instance.RecordEvent(completeGameEvent);
    }
}
