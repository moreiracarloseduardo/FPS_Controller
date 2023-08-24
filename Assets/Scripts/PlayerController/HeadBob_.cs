using UnityEngine;

public class HeadBob_ : MonoBehaviour {
    [SerializeField] float stepSoundFrequency = 1f;
    float stepTimer = 0f;
    float previousYPosition;
    float bobbingThreshold = .02f;
    float initialYPosition;
    bool isPlayingFootstep = false;
    public float walkingBobbingSpeed = 14f;
    public float bobbingAmount = 0.05f;
    public Player_ player;
    public PlayerCrouching_ playerCrouching;
    public PlayerSprinting_ playerSprinting;
    public FootstepAudioManager_ footstepAudioManager;

    float defaultPosY = 0;
    float timer = 0;

    void Awake() {
        player = GetComponentInParent<Player_>();
        playerSprinting = GetComponentInParent<PlayerSprinting_>();
        playerCrouching = GetComponentInParent<PlayerCrouching_>();
        footstepAudioManager = GetComponentInParent<FootstepAudioManager_>();
        defaultPosY = transform.localPosition.y; 

        initialYPosition = transform.localPosition.y;
    }

    void Update() {
        if (playerCrouching.IsCrouching) return;

        float currentWalkingBobbingSpeed = playerSprinting.IsSprinting
            ? walkingBobbingSpeed * playerSprinting.SpeedMultiplier
            : walkingBobbingSpeed;

        if (Mathf.Abs(player.velocity.x) > 0.1f || Mathf.Abs(player.velocity.z) > 0.1f) {
            timer += Time.deltaTime * currentWalkingBobbingSpeed;
            transform.localPosition = new Vector3(transform.localPosition.x, defaultPosY + Mathf.Sin(timer) * bobbingAmount, transform.localPosition.z);

            // Se a posição atual de y é maior que a posição anterior de y e ainda não estamos tocando o som do passo, então estamos começando a subir de novo
            if (transform.localPosition.y > previousYPosition && !isPlayingFootstep && (previousYPosition - initialYPosition) < bobbingThreshold) {
                footstepAudioManager.PlayFootstepSound();
                isPlayingFootstep = true; 
            } else if (transform.localPosition.y < previousYPosition) {
                isPlayingFootstep = false; 
            }

            previousYPosition = transform.localPosition.y;
        } else {
            timer = 0;
            transform.localPosition = new Vector3(transform.localPosition.x, Mathf.Lerp(transform.localPosition.y, defaultPosY, Time.deltaTime * currentWalkingBobbingSpeed), transform.localPosition.z);
        }
    }

}
