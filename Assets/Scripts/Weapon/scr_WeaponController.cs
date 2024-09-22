using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using static scr_Models;
using System.Linq;
using System.Collections;
using TMPro;

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
    private bool isFiring;
    private float nextFireTime;
    private int burstCount;
    public int bulletsPerBurst = 3;
    public float burstDelay = 0.1f;
    public float recoilAmount = 0.1f;

    public int totalAmmo = 120;
    public int magazineSize = 30;
    private int currentAmmoInMagazine;
    public float reloadTime = 2.0f;
    private bool isReloading = false;

    public TMP_Text ammoText;
    public TMP_Text reloadText;

    private scr_WeaponRecoil recoilScript;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip fireSound;
    [Range(0f, 1f)] public float fireSoundVolume = 1.0f;

    #region Start / Update

    private void Start() {
        newWeaponRotation = transform.localRotation.eulerAngles;

        recoilScript = transform.GetComponentInParent<scr_WeaponRecoil>();

        currentAmmoInMagazine = magazineSize;
        UpdateAmmoUI();

        currentFireType = allowedFireType.First();

        if (!audioSource) {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void Update() {
        if (!isInitialised) {
            return;
        }

        if (isReloading) return;

        if (isFiring && Time.time >= nextFireTime && currentAmmoInMagazine > 0) {
            if (burstCount < bulletsPerBurst) {
                Shoot();
                ApplyRecoil();
                burstCount++;
                nextFireTime = Time.time + burstDelay;
                UpdateAmmoUI();
            }
        }

        if (Input.GetKeyDown(KeyCode.R)) {
            StartCoroutine(Reload());
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
        if (isShooting && currentFireRate <= 0f) {
            Shoot();
            currentFireRate = 1f / rateOfFire;
        }

        if (currentFireRate > 0f) {
            currentFireRate -= Time.deltaTime;
        }
    }

    private void Shoot() {
        if (currentAmmoInMagazine <= 0) {
            return;
        }

        RaycastHit hit;
        Vector3 shootDirection = characterController.camera.transform.forward;

        PlayFiringSound();
        recoilScript.RecoilFire();

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

        currentAmmoInMagazine--;
        UpdateAmmoUI();
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

    private void ApplyRecoil() {
        float recoilX = Random.Range(-recoilAmount, recoilAmount);
        float recoilY = Random.Range(recoilAmount / 2, recoilAmount);
        characterController.leanPivot.transform.Rotate(-recoilY, recoilX, 0);
    }

    public void ShootingPressed() {
        if (!isReloading && currentAmmoInMagazine > 0) {
            isFiring = true;
            burstCount = 0;
            nextFireTime = Time.time;
        }
    }

    public void ShootingReleased() {
        isFiring = false;
        burstCount = bulletsPerBurst;
    }

    private IEnumerator Reload() {
        if (currentAmmoInMagazine == magazineSize || totalAmmo <= 0) {
            yield break;
        }

        isReloading = true;
        weaponAnimator.SetTrigger("Reload");
        reloadText.gameObject.SetActive(true);
        reloadText.text = "Reloading...";

        yield return new WaitForSeconds(reloadTime);

        int ammoToReload = Mathf.Min(magazineSize - currentAmmoInMagazine, totalAmmo);
        currentAmmoInMagazine += ammoToReload;
        totalAmmo -= ammoToReload;

        isReloading = false;
        reloadText.gameObject.SetActive(false);
        UpdateAmmoUI();
    }

    private void UpdateAmmoUI() {
        ammoText.text = currentAmmoInMagazine + " / " + totalAmmo;
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