using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System;

namespace VikingCrewTools.Sidescroller {
    [RequireComponent(typeof(CharacterMovementController2D))]
    public class PlayerMovementControls : MonoBehaviour, ICamFollowTarget {
        
        public enum ControllerType {
            MOUSE_AND_KEYBOARD,
            XBOX_360_CONTROLLER,

        }

        [Header("Set you preferred controls here")]
        public KeyCode jumpKey = KeyCode.W;
        public KeyCode runLeftKey = KeyCode.A;
        public KeyCode runRightKey = KeyCode.D;
        public KeyCode crouchKey = KeyCode.S;
        [Header("Only mouse and keyboardcontrols are implemented at this point")]
        public ControllerType controller;
        [Header("Max amount camera will move along aim vector")]
        public float maxAimAheadDistance = 7f;
        [Header("Min amount of aim to start moving camera along aim vector")]
        public float startAimAheadDistance = 2f;

        CharacterMovementController2D character;

        #region Control values
        // Preferably set these in Update() as Unity will handle events like KeyDown and such in Update
        // Apply them to control of character in FixedUpdate as that is when physics is updated
        
        float run;
        bool doJump;
        bool doCrouch;
        Vector2 aimPosition;
        float zoomAmount;

        #endregion

        void Awake() {
            character = GetComponent<CharacterMovementController2D>();
        }

        void Update() {
            switch (controller) {
                case ControllerType.MOUSE_AND_KEYBOARD:
                    HandleKeyboardAndMouseControls();
                    break;
                case ControllerType.XBOX_360_CONTROLLER:
                    break;
                default:
                    break;
            }
            
        }

        void LateUpdate() {
            character.AimAtPosition(aimPosition);
        }

        void FixedUpdate() {
            
            if (doJump) {
                character.Jump();
                doJump = false;
            }

            character.Run(run);
            
            character.Crouch(doCrouch);
            
        }

        void HandleKeyboardAndMouseControls() {
            aimPosition = MouseAim();

            if (Input.GetKeyDown(jumpKey))
                doJump = true;
            if (Input.GetKey(runRightKey))
                run = 1;
            else if (Input.GetKey(runLeftKey))
                run = -1;
            else
                run = 0;
            
           
            if(Input.mouseScrollDelta.y != 0) {
                zoomAmount = -(Input.mouseScrollDelta.y);
            }

            doCrouch =(Input.GetKey(crouchKey));
            
        }

        Vector2 MouseAim() {
            float z = 0;
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = z - Camera.main.transform.position.z;
            Vector3 pos = Camera.main.ScreenToWorldPoint(mousePos);
            pos.z = z;
            return pos;
        }

        public Vector3 GetVelocity() {
            if (character)
                return character.velocity;
            else
                return Vector3.zero;
        }

        public Vector3 GetCameraAimPosition() {
            Vector3 dist = (Vector3)aimPosition - transform.position;
            dist.z = 0;
            float length = dist.magnitude;
            dist.Normalize();
            if (length < startAimAheadDistance)
                return transform.position;
            else
                length -= startAimAheadDistance;

            if (length > maxAimAheadDistance)
                length = maxAimAheadDistance;

            dist = length * dist;
            return transform.position + dist;
        }

        public float GetZoom() {
            float zoom = zoomAmount;
            zoomAmount = 0;
            return zoom;
        }
    }
}
