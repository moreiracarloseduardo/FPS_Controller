using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_ : MonoBehaviour {
    [SerializeField] float mouseSensivity = 3f;
    [SerializeField] float walkingSpeed = 2f;
    [SerializeField] float flyingSpeed = 10f;
    [SerializeField] float climbSpeed = 2f;
    [SerializeField] float acceleration = 20f;
    [SerializeField] float mass = 1f;
    public Transform cameraTransform;

    public bool isGrounded => controller.isGrounded;
    public float Height {
        get => controller.height;
        set => controller.height = value;
    }
    public event Action OnBeforeMove;
    public event Action<bool> OnGroundStateChange;
    internal float movementSpeedMultiplier;
    State _state;
    public State currentState {
        get => _state;
        set {
            _state = value;
            velocity = Vector3.zero;
        }
    }
    public enum State { Walking, Flying, Climbing }
    public enum SurfaceType { Wood, Concrete, Grass, Metal, Glass, WaterPuddle }

    CharacterController controller;
    internal Vector3 velocity;
    Vector2 look;
    bool wasGrounded;

    PlayerInput playerInput;
    InputAction moveAction;
    InputAction lookAction;
    InputAction sprintAction;
    InputAction flyUpDownAction;


    void Awake() {
        controller = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["move"];
        lookAction = playerInput.actions["look"];
        sprintAction = playerInput.actions["sprint"];
        flyUpDownAction = playerInput.actions["flyUpDown"];

    }

    private void Start() {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update() {
        movementSpeedMultiplier = 1f;
        switch (currentState) {
            case State.Walking:
                UpdateGround();
                UpdateGravity();
                UpdateMovement();
                UpdateLook();
                break;
            case State.Flying:
                UpdateMovementFlying();
                UpdateLook();
                break;
            case State.Climbing:
                UpdateMovementClimbing();
                UpdateLook();
                break;
        }

    }

    void UpdateSlopSliding() {
        if (isGrounded) {
            var sphereCastVerticalOffset = controller.height / 2 - controller.radius;
            var castOrigin = transform.position - new Vector3(0, sphereCastVerticalOffset, 0);

            if (Physics.SphereCast(castOrigin, controller.radius - .01f, Vector3.down, out var hit, .05f, ~LayerMask.GetMask("Player"), QueryTriggerInteraction.Ignore)) {
                var collider = hit.collider;
                var angle = Vector3.Angle(Vector3.up, hit.normal);

                if (angle > controller.slopeLimit) {
                    var normal = hit.normal;
                    var yInverse = 1f - normal.y;
                    velocity.x += yInverse * normal.x;
                    velocity.z += yInverse * normal.z;
                }
            }
        }

    }


    void UpdateGround() {
        UpdateSlopSliding();
        if (wasGrounded != isGrounded) {
            OnGroundStateChange?.Invoke(isGrounded);
            wasGrounded = isGrounded;
        }
    }

    void UpdateGravity() {
        var gravity = Physics.gravity * mass * Time.deltaTime;
        velocity.y = isGrounded ? -1f : velocity.y + gravity.y;
    }
    public SurfaceType GetSurfaceType() {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 1.5f)) {
            switch (hit.collider.gameObject.layer) {
                case 7:
                    return SurfaceType.Wood;
                case 8:
                    return SurfaceType.Concrete;
                case 9:
                    return SurfaceType.Grass;
                case 10:
                    return SurfaceType.Metal;
                case 11:
                    return SurfaceType.Glass;
                case 12:
                    return SurfaceType.WaterPuddle;
                default:
                    return SurfaceType.Concrete;
            }
        } else {
            return SurfaceType.Concrete;
        }
    }

    Vector3 GetMovementInput(float speed, bool horizontal = true) {
        var moveInput = moveAction.ReadValue<Vector2>();
        var flyUpDownInput = flyUpDownAction.ReadValue<float>();

        var input = new Vector3();
        var referenceTransform = horizontal ? transform : cameraTransform;
        input += referenceTransform.forward * moveInput.y;
        input += referenceTransform.right * moveInput.x;
        if (!horizontal) { input += transform.up * flyUpDownInput; }
        input = Vector3.ClampMagnitude(input, 1f);
        input *= speed * movementSpeedMultiplier;
        return input;
    }

    void UpdateMovement() {
        OnBeforeMove?.Invoke();


        var input = GetMovementInput(walkingSpeed);

        var factor = acceleration * Time.deltaTime;
        velocity.x = Mathf.Lerp(velocity.x, input.x, factor);
        velocity.z = Mathf.Lerp(velocity.z, input.z, factor);

        controller.Move(velocity * Time.deltaTime);

    }

    void UpdateMovementFlying() {
        var input = GetMovementInput(flyingSpeed, false);

        var factor = acceleration * Time.deltaTime;
        velocity = Vector3.Lerp(velocity, input, factor);

        controller.Move(velocity * Time.deltaTime);
    }
    void UpdateMovementClimbing() {
        var input = GetMovementInput(climbSpeed, false);
        var forwardInputFactor = Vector3.Dot(transform.forward, input.normalized);

        if (forwardInputFactor > 0) {

            input.x = input.x * .5f;
            input.z = input.z * .5f;

            if (Mathf.Abs(input.y) > .2f) {
                input.y = Mathf.Sign(input.y) * climbSpeed;
            }
        } else {
            input.y = 0;
            input.x = input.x * 3f;
            input.z = input.z * 3f;
        }


        var factor = acceleration * Time.deltaTime;
        velocity = Vector3.Lerp(velocity, input, factor);

        controller.Move(velocity * Time.deltaTime);
    }
    void UpdateLook() {
        var lookInput = lookAction.ReadValue<Vector2>();
        look.x += lookInput.x * mouseSensivity;
        look.y += lookInput.y * mouseSensivity;

        look.y = Mathf.Clamp(look.y, -89f, 89f);

        cameraTransform.localRotation = Quaternion.Euler(-look.y, 0, 0);
        transform.localRotation = Quaternion.Euler(0, look.x, 0);
    }

    void OnToggleFlying() {
        currentState = currentState == State.Flying ? State.Walking : State.Flying;
    }

}
