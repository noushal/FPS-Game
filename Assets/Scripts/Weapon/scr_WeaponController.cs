using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using static scr_Models;
using System.Linq;

public class scr_WeaponController : MonoBehaviour {

    private scr_CharacterController characterController;

    [Header("References")]
    public Animator weaponAnimator;
    public GameObject bulletPrefab;
    public Transform bulletSpawn;

    public GameObject bulletHolePrefab;
    public GameObject hitParticlePrefab;
    public LayerMask bulletHoleLayers;

    [Header("Settings")]
    public WeaponSettingsModel settings;

    bool isInitialised;

    Vector3 newWeaponRotation;
    Vector3 newWeaponRotationVelocity;

    Vector3 targetWeaponRotation;
    Vector3 targetWeaponRotationVelocity;

    Vector3 newWeaponMovementRotation;
    Vector3 newWeaponMovementRotationVelocity;

    Vector3 targetWeaponMovementRotation;
    Vector3 targetWeaponMovementRotationVelocity;

    private bool isGroundedTrigger;

    private float fallingDelay;

    [Header("Weapon Sway")]
    public Transform weaponSwayObject;

    public float swayAmountA = 1;
    public float swayAmountB = 2;
    public float swayScale = 600;
    public float swayLerpSpeed = 14;
    private float swayTime;
    private Vector3 swayPosition;

    [Header("Sights")]
    public Transform sightTarget;
    public float sightOffset;
    public float aimingInTime;
    private Vector3 weaponSwayPosition;
    private Vector3 weaponSwayPositionVelocity;
    [HideInInspector]
    public bool isAimingIn;

    [Header("Shooting")]
    public float rateOfFire;
    private float currentFireRate;
    public List<WeaponFireType> allowedFireType;
    public WeaponFireType currentFireType;
    [HideInInspector]
    public bool isShooting;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip fireSound;
    [Range(0f, 1f)] public float fireSoundVolume = 1.0f;

    #region Start / Update

    private void Start() {
        newWeaponRotation = transform.localRotation.eulerAngles;

        currentFireType = allowedFireType.First();

        if (!audioSource) {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void Update() {
        if (!isInitialised) {
            return;
        }
        CalculateWeaponRotation();
        SetWeaponAnimations();
        CalculateWeaponSway();
        CalculateAimingIn();
        CalculateShooting();

    }

    #endregion

    #region Shooting

    private void CalculateShooting() {
        if (isShooting) {
            Shoot();

            if (currentFireType == WeaponFireType.SemiAuto) {
                isShooting = false;
            }
        }
    }

    private void Shoot() {
        RaycastHit hit;
        Vector3 shootDirection = characterController.camera.transform.forward;
        PlayFiringSound();
        if (Physics.Raycast(characterController.camera.transform.position, shootDirection, out hit, settings.Range)) {
            scr_Enemy enemy = hit.collider.GetComponent<scr_Enemy>();
            if (enemy != null) {
                GameObject hitParticles = Instantiate(hitParticlePrefab, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(hitParticles, 0.1f);
                enemy.TakeDamage(settings.Damage);
            } else if (bulletHoleLayers == (bulletHoleLayers | (1 << hit.collider.gameObject.layer))) {
                SpawnBulletHole(hit);
            }
        }
    }
    private void PlayFiringSound() {
        if (fireSound && audioSource) {
            audioSource.PlayOneShot(fireSound, fireSoundVolume);
        }
    }

    private void SpawnBulletHole(RaycastHit hit) {
        GameObject bulletHole = Instantiate(bulletHolePrefab, hit.point, Quaternion.LookRotation(hit.normal));
        bulletHole.transform.SetParent(hit.transform);
        Destroy(bulletHole, 10f);
    }

    #endregion

    #region Initialise

    public void Initialise(scr_CharacterController CharacterController) {
        characterController = CharacterController;
        isInitialised = true;
    }

    #endregion

    #region Aiming In

    private void CalculateAimingIn() {
        var targetPosition = transform.position;

        if (isAimingIn) {
            targetPosition = characterController.camera.transform.position + (transform.position - sightTarget.transform.position) + (characterController.camera.transform.forward * sightOffset);
        }

        weaponSwayPosition = weaponSwayObject.transform.position;
        weaponSwayPosition = Vector3.SmoothDamp(weaponSwayPosition, targetPosition, ref weaponSwayPositionVelocity, aimingInTime);
        weaponSwayObject.transform.position = weaponSwayPosition + swayPosition;
    }

    #endregion

    #region Jumping

    public void TriggerJump() {
        isGroundedTrigger = false;
        weaponAnimator.SetTrigger("Jump");
    }

    #endregion

    #region Rotation

    private void CalculateWeaponRotation() {

        targetWeaponRotation.y += (isAimingIn ? settings.SwayAmount / 3 : settings.SwayAmount) * (settings.SwayXInverted ? -characterController.input_View.x : characterController.input_View.x) * Time.deltaTime;
        targetWeaponRotation.x += (isAimingIn ? settings.SwayAmount / 3 : settings.SwayAmount) * (settings.SwayYInverted ? characterController.input_View.y : -characterController.input_View.y) * Time.deltaTime;

        targetWeaponRotation.x = Mathf.Clamp(targetWeaponRotation.x, -settings.SwayClampX, settings.SwayClampX);
        targetWeaponRotation.y = Mathf.Clamp(targetWeaponRotation.y, -settings.SwayClampY, settings.SwayClampY);
        targetWeaponRotation.z = isAimingIn ? 0 : targetWeaponRotation.y;

        targetWeaponRotation = Vector3.SmoothDamp(targetWeaponRotation, Vector3.zero, ref targetWeaponRotationVelocity, settings.SwayResetSmoothing);
        newWeaponRotation = Vector3.SmoothDamp(newWeaponRotation, targetWeaponRotation, ref newWeaponRotationVelocity, settings.SwaySmoothing);

        targetWeaponMovementRotation.z = (isAimingIn ? settings.MovementSwayX / 3 : settings.MovementSwayX) * (settings.MovementSwayXInverted ? -characterController.input_Movement.x : characterController.input_Movement.x);
        targetWeaponMovementRotation.x = (isAimingIn ? settings.MovementSwayY / 3 : settings.MovementSwayY) * (settings.MovementSwayYInverted ? -characterController.input_Movement.y : characterController.input_Movement.y);

        targetWeaponMovementRotation = Vector3.SmoothDamp(targetWeaponMovementRotation, Vector3.zero, ref targetWeaponMovementRotationVelocity, settings.MovementSwaySmoothing);
        newWeaponMovementRotation = Vector3.SmoothDamp(newWeaponMovementRotation, targetWeaponMovementRotation, ref newWeaponMovementRotationVelocity, settings.MovementSwaySmoothing);

        transform.localRotation = Quaternion.Euler(newWeaponRotation + newWeaponMovementRotation);

    }

    #endregion

    #region Animations

    private void SetWeaponAnimations() {

        if (isGroundedTrigger) {
            fallingDelay = 0;
        } else {
            fallingDelay += Time.deltaTime;
        }

        if (characterController.isGrounded && !isGroundedTrigger && fallingDelay > 0.1f) {
            weaponAnimator.SetTrigger("Land");
            isGroundedTrigger = true;
        } else if (!characterController.isGrounded && isGroundedTrigger) {
            weaponAnimator.SetTrigger("Falling");
            isGroundedTrigger = false;
        }

        weaponAnimator.SetBool("IsSprinting", characterController.isSprinting);
        weaponAnimator.SetFloat("WeaponAnimationSpeed", characterController.weaponAnimationSpeed);
    }

    #endregion

    #region Sway

    private void CalculateWeaponSway() {
        var targetPosition = LissajousCurve(swayTime, swayAmountA, swayAmountB) / (isAimingIn ? swayScale * 3 : swayScale);

        swayPosition = Vector3.Lerp(swayPosition, targetPosition, Time.smoothDeltaTime * swayLerpSpeed);
        swayTime += Time.deltaTime;

        if (swayTime > 6.3f) {
            swayTime = 0;
        }
    }

    private Vector3 LissajousCurve(float Time, float A, float B) {
        return new Vector3(Mathf.Sin(Time), A * Mathf.Sin(B * Time + Mathf.PI));
    }

    #endregion

}