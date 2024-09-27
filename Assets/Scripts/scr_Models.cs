using System;
using System.Collections.Generic;
using UnityEngine;

public static class scr_Models {

    #region Player

    public enum PlayerStance {
        Stand,
        Crouch,
        Prone
    }

    [Serializable]
    public class PlayerSettingsModel {

        [Header("View Settings")]
        public float ViewXsensitivity;
        public float ViewYsensitivity;

        public float AimingSensitivityEffector;

        public bool ViewXInverted;
        public bool ViewYInverted;

        [Header("Movement Settings")]
        public bool SprintingHold;
        public float MovementSmoothing;

        [Header("Movement - Walking")]
        public float WalkingForwardSpeed;
        public float WalkingBackwardSpeed;
        public float WalkingStrafeSpeed;

        [Header("Movement - Sprinting")]
        public float RunningForwardSpeed;
        public float RunningStrafeSpeed;

        [Header("Jumping")]
        public float JumpingHeight;
        public float JumpingFalloff;
        public float FallingSmoothing;

        [Header("Speed Effectors")]
        public float SpeedEffector = 1;
        public float CrouchSpeedEffector;
        public float ProneSpeedEffector;
        public float FallingSpeedEffector;
        public float AimingSpeedEffector;

        [Header("Is Grounded / Falling")]
        public float isGroundedRadius;
        public float isFallingSpeed;
    }

    [Serializable]
    public class CharacterStance {
        public float CameraHeight;
        public CapsuleCollider StanceCollider;
    }

    #endregion

    #region Weapons

    public enum WeaponFireType {
        SemiAuto,
        Single,
        FullyAuto
    }

    [Serializable]
    public class WeaponSettingsModel {
        [Header("Weapon Sway")]
        public float SwayAmount;
        public bool SwayYInverted;
        public bool SwayXInverted;
        public float SwaySmoothing;
        public float SwayResetSmoothing;
        public float SwayClampX;
        public float SwayClampY;

        [Header("Weapon Movement Sway")]
        public float MovementSwayX;
        public float MovementSwayY;
        public bool MovementSwayYInverted;
        public bool MovementSwayXInverted;
        public float MovementSwaySmoothing;

        [Header("Enemy")]
        public float Damage = 10f;
        public float Range = 100f;

    }

    #endregion

}