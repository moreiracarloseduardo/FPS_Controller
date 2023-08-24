using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder_ : MonoBehaviour {
    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player") && other.TryGetComponent<Player_>(out var player)) {
            player.currentState = Player_.State.Climbing;
        }
    }
    void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player") && other.TryGetComponent<Player_>(out var player)) {
            player.currentState = Player_.State.Walking;
        }
    }
}
