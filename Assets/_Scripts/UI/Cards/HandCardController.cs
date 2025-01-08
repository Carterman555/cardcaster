using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Windows;

public class HandCardController : HandCard {


    private void Update() {
        HandleInput();
    }

    private void HandleInput() {
        bool playInputPressed = GetPlayInput().WasReleasedThisFrame();

        if (!CanAffordToPlay) {
            if (playInputPressed) {
                CantPlayShake();
            }
            return;
        }


        //if press this cards play input
        //    if not showing or moving this card 

        //        if positional card
        //            movingCard = true

        //        if not positional card
        //            show card

        //    if showing or moving this card
        //        play card


        if (playInputPressed) {
            
        }

        //if press cancel button
        //    cancel card

        //if movingCard
        //    transform.position += moveInput * Time.deltatime
    }
}
