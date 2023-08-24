using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class FootstepAudioManager_ : MonoBehaviour {
    [SerializeField] AudioClip[] woodSounds;
    [SerializeField] AudioClip[] concreteSounds;
    [SerializeField] AudioClip[] grassSounds;
    [SerializeField] AudioClip[] metalSounds;
    [SerializeField] float minPitch = 0.9f;
    [SerializeField] float maxPitch = 1.1f;

    AudioSource audioSource;
    Player_ player;
    PlayerSprinting_ playerSprinting;

    void Awake() {
        audioSource = GetComponent<AudioSource>();
        player = GetComponent<Player_>();
        playerSprinting = GetComponent<PlayerSprinting_>();
    }

    public void PlayFootstepSound() {
        // if (audioSource.isPlaying) return;
        audioSource.clip = GetAudioClip(player.GetSurfaceType());
        audioSource.volume = GetVolumeBasedOnPlayerSpeed();
        audioSource.pitch = Random.Range(minPitch, maxPitch);
        audioSource.Play();
    }

    AudioClip GetAudioClip(Player_.SurfaceType surfaceType) {
        switch (surfaceType) {
            case Player_.SurfaceType.Wood:
                return woodSounds[Random.Range(0, woodSounds.Length)];
            case Player_.SurfaceType.Concrete:
                return concreteSounds[Random.Range(0, concreteSounds.Length)];
            case Player_.SurfaceType.Grass:
                return grassSounds[Random.Range(0, grassSounds.Length)];
            case Player_.SurfaceType.Metal:
                return metalSounds[Random.Range(0, metalSounds.Length)];
            default:
                return null;
        }
    }

    float GetVolumeBasedOnPlayerSpeed() {
        // return Mathf.Clamp(playerSprinting.SpeedMultiplier / 10f, 0.1f, 1f);
        return 1f;

    }
}
