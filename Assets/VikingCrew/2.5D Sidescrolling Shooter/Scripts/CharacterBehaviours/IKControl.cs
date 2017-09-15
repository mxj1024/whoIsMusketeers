using UnityEngine;
using System;
using System.Collections;

namespace VikingCrewTools {
    /// <summary>
    /// This script lets the character hands go to handles on equiment she is carrying and the head will point where she is aiming
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class IKControl : MonoBehaviour {
        public Vector3 lefthandRotationOffset;
        public Vector3 righthandRotationOffset;
        protected Animator animator;

        private bool ikActive = false;
        private Transform rightHandObj = null;
        private Transform leftHandObj = null;
        private Transform lookObj = null;
       
        public Transform RightHandObj
        {
            get
            {
                return rightHandObj;
            }

            set
            {
                rightHandObj = value;
            }
        }

        public Transform LeftHandObj
        {
            get
            {
                return leftHandObj;
            }

            set
            {
                leftHandObj = value;
            }
        }

        public Transform LookObj
        {
            get
            {
                return lookObj;
            }

            set
            {
                lookObj = value;
            }
        }

        public bool IkActive
        {
            get
            {
                return ikActive;
            }

            set
            {
                ikActive = value;
            }
        }

        void Start() {
            animator = GetComponent<Animator>();
        }

        //a callback for calculating IK
        void OnAnimatorIK() {
            if (animator) {

                //if the IK is active, set the position and rotation directly to the goal. 
                if (IkActive) {

                    // Set the look target position, if one has been assigned
                    if (lookObj != null) {
                        animator.SetLookAtWeight(1);
                        animator.SetLookAtPosition(lookObj.position);
                    }

                    // Set the right hand target position and rotation, if one has been assigned
                    if (rightHandObj != null) {
                        animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
                        animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
                        animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandObj.position);
                        animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandObj.rotation *Quaternion.Euler( righthandRotationOffset));
                    }
                    // Set the right hand target position and rotation, if one has been assigned
                    if (leftHandObj != null) {
                        animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
                        animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
                        animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandObj.position);
                        animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandObj.rotation * Quaternion.Euler(lefthandRotationOffset));
                    }
                }

                //if the IK is not active, set the position and rotation of the hand and head back to the original position
                else {
                    animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
                    animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
                    animator.SetLookAtWeight(0);
                }
            }
        }
    }
}