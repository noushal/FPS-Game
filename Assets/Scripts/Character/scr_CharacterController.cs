using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using static scr_Models;

public class scr_CharacterController : MonoBehaviour {

    private CharacterController characterController;
    private DefaultInput defaultInput;
    [HideInInspector]
    public Vector2 input_Movement;
    [HideInInspector]
    public Vector2 input_View;

    private Vector3 newCameraRotation;
    private Vector3 newCharacterRotation;

    public LayerMask ground;

    [Header("References")]
    public Transform cameraHolder;
    public Camera camera;
    public Transform feetTransform;

    [Header("Settings")]
    public PlayerSettingsModel playerSettings;
    public float viewClampYMin = -70;
    public float viewClampYMax = 80;
    public LayerMask playerMask;
    public LayerMask groundMask;

    [Header("Gravity")]
    public float gravityAmount;
    public float gravityMin;
    private float playerGravity;

    public Vector3 jumpingForce;
    private Vector3 jumpingForceVelocity;

    [Header("Stance")]
    public PlayerStance playerStance;
    public float playerStanceSmoothing;
    public CharacterStance playerStandStance;
    public CharacterStance playerCrouchStance;
    public CharacterStance playerProneStance;
    public float stanceCheckErrorMargin = 0.05f;
    private float cameraHeight;
    private float cameraHeightVelocity;

    private Vector3 stanceCapsuleCenterVelocity;
    private float stanceCapsuleHeightVelocity;

    [HideInInspector]
    public bool isSprinting;

    private Vector3 newMovementSpeed;
    private Vector3 newMovementSpeedVelocity;

    [Header("Weapon")]
    public scr_WeaponController currentWeapon;
    public scr_ScopeScript scopeScript;
    public float weaponAnimationSpeed;
    [HideInInspector]
    public bool isGrounded;
    [HideInInspector]
    public bool isFalling;

    public List<scr_WeaponController> weapons;
    private int currentWeaponIndex = 0;

    [Header("Lean")]
    public Transform leanPivot;
    private float currentLean;
    private float targetLean;
    public float leanAngle;
    public float leanSmoothing;
    private float leanVelocity;

    private bool isLeaningLeft;
    private bool isLeaningRight;

    [Header("Aiming In")]
    public bool isAimingIn;


    #region Awake
    private void Awake() {

        defaultInput = new DefaultInput();

        defaultInput.Character.Movement.performed += e => input_Movement = e.ReadValue<Vector2>();
        defaultInput.Character.View.performed += e => input_View = e.ReadValue<Vector2>();
        defaultInput.Character.Jump.performed += e => Jump();
        defaultInput.Character.Crouch.performed += e => Crouch();
        defaultInput.Character.Prone.performed += e => Prone();
        defaultInput.Character.Sprint.performed += e => ToggleSprint();
        defaultInput.Character.SprintReleased.performed += e => StopSprint();

        defaultInput.Character.LeanLeftPressed.performed += e => isLeaningLeft = true;
        defaultInput.Character.LeanLeftReleased.performed += e => isLeaningLeft = false;

        defaultInput.Character.LeanRightPressed.performed += e => isLeaningRight = true;
        defaultInput.Character.LeanRightReleased.performed += e => isLeaningRight = false;

        defaultInput.Weapon.Fire1Pressed.performed += e => currentWeapon.ShootingPressed();
        defaultInput.Weapon.Fire1Released.performed += e => currentWeapon.ShootingReleased();

        defaultInput.Weapon.Weapon1.performed += e => SwitchWeapon(0);
        defaultInput.Weapon.Weapon2.performed += e => SwitchWeapon(1);
        defaultInput.Weapon.SwitchWeapons.performed += e => SwitchToNextWeapon();

        defaultInput.Enable();

        newCameraRotation = cameraHolder.localRotation.eulerAngles;
        newCharacterRotation = transform.localRotation.eulerAngles;

        characterController = GetComponent<CharacterController>();

        cameraHeight = cameraHolder.localPosition.y;

        if (weapons.Count > 0) {
            currentWeapon = weapons[currentWeaponIndex];
            currentWeapon.Initialise(this);
            currentWeapon.gameObject.SetActive(true);
        }
    }

    #endregion

    #region Update
    private void Update() {
        SetIsGrounded();
        SetIsFalling();
        CalculateView();
        CalculateMovement();
        CalculateJump();
        CalculateStance();
        CalculateLeaning();
        CalculateAimingIn();
    }

    #endregion

    #region IsGrounded / IsFalling

    private void SetIsGrounded() {
        isGrounded = Physics.CheckSphere(feetTransform.position, playerSettings.isGroundedRadius, groundMask);
    }

    private void SetIsFalling() {
        isFalling = (!isGrounded && characterController.velocity.magnitude >= playerSettings.isFallingSpeed);
    }

    #endregion

    #region View / Movement
    private void CalculateView() {

        newCharacterRotation.y += (isAimingIn ? playerSettings.ViewXsensitivity * playerSettings.AimingSensitivityEffector : playerSettings.ViewXsensitivity) * (playerSettings.ViewXInverted ? -input_View.x : input_View.x) * Time.deltaTime;
        transform.localRotation = Quaternion.Euler(newCharacterRotation);


        newCameraRotation.x += (isAimingIn ? playerSettings.ViewYsensitivity * playerSettings.AimingSensitivityEffector : playerSettings.ViewYsensitivity) * (playerSettings.ViewYInverted ? input_View.y : -input_View.y) * Time.deltaTime;
        newCameraRotation.x = Mathf.Clamp(newCameraRotation.x, viewClampYMin, viewClampYMax);

        cameraHolder.localRotation = Quaternion.Euler(newCameraRotation);
    }

    private void CalculateMovement() {

        if (input_Movement.y <= 0.2f) {
            isSprinting = false;
        }

        var verticalSpeed = playerSettings.WalkingForwardSpeed;
        var horizontalSpeed = playerSettings.WalkingStrafeSpeed;

        if (isSprinting) {
            verticalSpeed = playerSettings.RunningForwardSpeed;
            horizontalSpeed = playerSettings.RunningStrafeSpeed;
        }

        if (!isGrounded) {
            playerSettings.SpeedEffector = playerSettings.FallingSpeedEffector;
        } else if (playerStance == PlayerStance.Crouch) {
            playerSettings.SpeedEffector = playerSettings.CrouchSpeedEffector;
        } else if (playerStance == PlayerStance.Prone) {
            playerSettings.SpeedEffector = playerSettings.ProneSpeedEffector;
        } else if (isAimingIn) {
            playerSettings.SpeedEffector = playerSettings.AimingSpeedEffector;
        } else {
            playerSettings.SpeedEffector = 1;
        }

        weaponAnimationSpeed = characterController.velocity.magnitude / (playerSettings.WalkingForwardSpeed * playerSettings.SpeedEffector);

        if (weaponAnimationSpeed > 1) {
            weaponAnimationSpeed = 1;
        }

        verticalSpeed *= playerSettings.SpeedEffector;
        horizontalSpeed *= playerSettings.SpeedEffector;

        newMovementSpeed = Vector3.SmoothDamp(newMovementSpeed, new Vector3(horizontalSpeed * input_Movement.x * Time.deltaTime, 0, verticalSpeed * input_Movement.y * Time.deltaTime), ref newMovementSpeedVelocity, isGrounded ? playerSettings.MovementSmoothing : playerSettings.FallingSmoothing);
        var movementSpeed = transform.TransformDirection(newMovementSpeed);

        if (playerGravity > gravityMin) {
            playerGravity -= gravityAmount * Time.deltaTime;
        }
        if (playerGravity < -0.1f && isGrounded) {
            playerGravity = -0.1f;
        }

        movementSpeed.y += playerGravity;
        movementSpeed += jumpingForce * Time.deltaTime;
        characterController.Move(movementSpeed);
    }

    #endregion

    #region Lean

    private void CalculateLeaning() {

        if (isLeaningLeft) {
            targetLean = leanAngle;
        } else if (isLeaningRight) {
            targetLean = -leanAngle;
        } else {
            targetLean = 0;
        }

        currentLean = Mathf.SmoothDamp(currentLean, targetLean, ref leanVelocity, leanSmoothing);

        leanPivot.localRotation = Quaternion.Euler(new Vector3(0, 0, currentLean));
    }

    #endregion

    #region Jump

    private void CalculateJump() {

        if (isGrounded) {
            return;
        }
        jumpingForce = Vector3.SmoothDamp(jumpingForce, Vector3.zero, ref jumpingForceVelocity, playerSettings.JumpingFalloff);
    }

    private void Jump() {

        
        

        if (!characterController.isGrounded || playerStance == PlayerStance.Prone) {
            
            return;
        }

       if (playerStance == PlayerStance.Crouch) {

            if (StanceCheck(playerStandStance.StanceCollider.height)) {
                return;
            }

            playerStance = PlayerStance.Stand;
            return;
        }

        jumpingForce = Vector3.up * playerSettings.JumpingHeight;
        playerGravity = 0;
        currentWeapon.TriggerJump();
        isGrounded = true;
    }

    #endregion

    #region Stance

    private void CalculateStance() {

        var currentStance = playerStandStance;
        if(playerStance == PlayerStance.Crouch) {
            currentStance = playerCrouchStance;
        }else if(playerStance == PlayerStance.Prone) {
            currentStance = playerProneStance;
        }

        cameraHeight = Mathf.SmoothDamp(cameraHolder.localPosition.y, currentStance.CameraHeight, ref cameraHeightVelocity, playerStanceSmoothing);
        cameraHolder.localPosition = new Vector3(cameraHolder.localPosition.x, cameraHeight, cameraHolder.localPosition.z);

        characterController.height = Mathf.SmoothDamp(characterController.height, currentStance.StanceCollider.height, ref stanceCapsuleHeightVelocity, playerStanceSmoothing);
        characterController.center = Vector3.SmoothDamp(characterController.center, currentStance.StanceCollider.center, ref stanceCapsuleCenterVelocity, playerStanceSmoothing);
    }

    private void Crouch() {

        if(playerStance == PlayerStance.Crouch) {

            if(StanceCheck(playerStandStance.StanceCollider.height)) {
                return;
            }

            playerStance = PlayerStance.Stand;
            return;
        }

        if (StanceCheck(playerCrouchStance.StanceCollider.height)) {
            return;
        }

        playerStance = PlayerStance.Crouch;
    }

    private void Prone() {
        playerStance = PlayerStance.Prone;
    }

    private bool StanceCheck(float stanceCheckHeight) {

        var start = new Vector3(feetTransform.position.x, feetTransform.position.y + characterController.radius + stanceCheckErrorMargin, feetTransform.position.z);
        var end = new Vector3(feetTransform.position.x, feetTransform.position.y - characterController.radius - stanceCheckErrorMargin + stanceCheckHeight, feetTransform.position.z);

        return Physics.CheckCapsule(start, end, characterController.radius, playerMask);
    }

    #endregion

    #region Sprint

    private void ToggleSprint() {

        if(input_Movement.y <= 0.2f) {
            isSprinting = false;
            return;
        }

        isSprinting = !isSprinting;
    }

    private void StopSprint() {

        if (playerSettings.SprintingHold) {
            isSprinting = false;
        }
    }

    #endregion

    #region Gizmos

    private void OnDrawGizmos() {
        Gizmos.DrawWireSphere(feetTransform.position, playerSettings.isGroundedRadius);
    }

    #endregion

    #region Aim
    private void AimingInPressed() {
        isAimingIn = true;
    }

    private void AimingInReleased() {
        isAimingIn = false;
    }

    private void CalculateAimingIn() {
        if (!currentWeapon) {
            return;
        }

        currentWeapon.isAimingIn = isAimingIn;
    }

    #endregion

    #region Weapon Switch

    private void SwitchWeapon(int index) {
        if (index < 0 || index >= weapons.Count || index == currentWeaponIndex) return;

        weapons[currentWeaponIndex].gameObject.SetActive(false);

        currentWeaponIndex = index;

        weapons[currentWeaponIndex].gameObject.SetActive(true);

        currentWeapon = weapons[currentWeaponIndex];
        currentWeapon.Initialise(this);

        if (currentWeapon.name == "Operator") {
            scopeScript.isSniper = true;
        } else {
            scopeScript.isSniper = false;
        }
    }

    private void SwitchToNextWeapon() {
        int nextWeaponIndex = (currentWeaponIndex + 1) % weapons.Count;
        SwitchWeapon(nextWeaponIndex);
        currentWeapon.UpdateAmmoUI();
    }

    #endregion

}