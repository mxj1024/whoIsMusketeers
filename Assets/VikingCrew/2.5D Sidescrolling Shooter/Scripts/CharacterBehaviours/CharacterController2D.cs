using UnityEngine;
using System.Collections;


namespace VikingCrewTools.Sidescroller {
    /// <summary>
    /// This class controls the body of a character and handles all connections to things like inventory etc.
    /// </summary>
    [RequireComponent(typeof(InventoryBehaviour))]
    public class CharacterController2D : CharacterMovementController2D {
        
        public FirearmBehaviour gun {
            get {
                if (inventory == null) return null;
                return inventory.currentGun;
            }
        }
       
        [HideInInspector]
        public InventoryBehaviour inventory;
        
        // Use this for initialization
        void Start() {
            inventory = GetComponent<InventoryBehaviour>();
            if (!hasBeenSetup)
                Setup();
        }
        
        // Update is called once per frame
        void Update() {

            UpdateAnimationController();
        }

        void FixedUpdate() {
            UpdateGrounded();
        }
        
        public void Fire() {
            if (gun != null) {
                if (gun.currentAmmo == 0)
                    inventory.ReloadCurrentWeapon();

                gun.Fire();
            }
        }

        public void Reload() {
            inventory.ReloadCurrentWeapon();
        }

        public void AutoFire() {
            if (gun != null)
                gun.AutoFire();
        }

        public void ReleaseAutoFire() {
            if (gun != null)
                gun.ReleaseAutoFire();
        }

        /// <summary>
        /// Makes the weapon follow a world space position
        /// </summary>
        /// <param name="position"></param>
        public override void AimAtPosition(Vector3 position) {
            base.AimAtPosition(position);

            if (gun != null) {
                position.z = gun.transform.position.z;
                gun.transform.LookAt(position, Vector3.up);
            }
        }
        
        public void AimAtDirection(Vector3 direction) {
            
            Vector3 rot = bodyAnimator.transform.rotation.eulerAngles;
            isFacingRight = direction.x >= 0;
            if (isFacingRight) {
                rot.y = 90;
            } else {
                rot.y = 270;
            }
            //Turns the animated body to point the right direction
            bodyAnimator.transform.rotation = Quaternion.Euler(rot);
            if (gun != null) {
                if (direction == Vector3.zero)
                    direction = new Vector3(isFacingRight ? 1 : -1, 0, 0);
                gun.transform.forward = direction.normalized;

            }
        }

        public void UpdateAim(Transform target) {
            AimAtPosition(target.position);
        }

        void UpdateGrounded() {
            
            if (CheckForGround()) {
                if (!isGrounded) {
                    //Just landed so need to set collider sizes
                    foreach (var bodypart in bodyParts) {
                        bodypart.SetOriginalSizeAndPos();
                    }
                    audio.PlayOneShot(landSound);
                }
                isGrounded = true;
                currentDoubleJumps = 0;
            } else {
                
                if (isGrounded) {//Just took off so need to set collider sizes
                    foreach (var bodypart in bodyParts) {
                        bodypart.SetJumpSizeAndPos();
                    }
                }
                isGrounded = false;
            }
        }
    }
}