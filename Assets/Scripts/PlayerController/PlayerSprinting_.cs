using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Player_))]
public class PlayerSprinting_ : MonoBehaviour {
    [SerializeField] float speedMultiplier = 2f;
    Player_ player;
    PlayerInput playerInput;
    InputAction sprintAction;
    public float SpeedMultiplier { get { return speedMultiplier; } }
    public bool IsSprinting { get; private set; }

    void Awake() {
        player = GetComponent<Player_>();
        playerInput = GetComponent<PlayerInput>();
        sprintAction = playerInput.actions["sprint"];
    }

    void OnEnable() => player.OnBeforeMove += OnBeforeMove;
    void OnDisable() => player.OnBeforeMove -= OnBeforeMove;

    void OnBeforeMove() {
        var sprintInput = sprintAction.ReadValue<float>();
        IsSprinting = sprintInput > 0;
        if (sprintInput == 0) return;
        var forwardMovementFactor = Mathf.Clamp01(Vector3.Dot(player.transform.forward, player.velocity.normalized));
        var multiplier = Mathf.Lerp(1f, speedMultiplier, forwardMovementFactor);
        player.movementSpeedMultiplier *= multiplier;
    }

}
