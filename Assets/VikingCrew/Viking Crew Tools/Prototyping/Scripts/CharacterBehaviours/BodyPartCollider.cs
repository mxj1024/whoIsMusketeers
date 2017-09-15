using UnityEngine;
using System.Collections;

namespace VikingCrewTools.Sidescroller {
    /// <summary>
    /// This class sort of encapsulates the collision functionality for a bodypart.
    /// It will be able to set the bodypart's size when doing some operations like jumpin and crouching
    /// It also tries to move any 2D/3D logic away from the CharacterController2D-class as we want that class to be
    /// agnostic regarding whether we use the 2D or 3D physics engine
    /// </summary>
    public class BodyPartCollider : MonoBehaviour {
        
        private Collider collider3D;
        private new Collider2D collider2D;
        [Tooltip("This determines how much smaller the hitbox of the character ill be when crouching")]
        public float crouchSizeRatio = 0.5f;
        [Tooltip("This determines how much the hitbox of the character should move when crouching")]
        public float crouchColliderOffset = -0.375f;
        [Tooltip("This determines how much smaller the hitbox of the character ill be when jumping")]
        public float jumpSizeRatio = 0.5f;
        [Tooltip("This determines how much the hitbox of the character should move when jumping")]
        public float jumpColliderOffset = 0.375f;
        protected bool is2D { get { return collider2D != null; } }

        private Vector3 originalLocalPos;
        private Vector3 originalLocalScale;

        public void Awake() {
            collider2D = GetComponent<Collider2D>();
            collider3D = GetComponent<Collider>();
            originalLocalPos = transform.localPosition;
            originalLocalScale = transform.localScale;
        }

        public void SetCrouchSizeAndPos() {
            SetCollider(crouchSizeRatio, crouchColliderOffset);
        }

        public void SetJumpSizeAndPos() {
            SetCollider(jumpSizeRatio, jumpColliderOffset);
        }

        public void SetOriginalSizeAndPos() {
            transform.localScale = originalLocalScale;
            transform.localPosition = originalLocalPos;
        }

        /// <summary>
        /// Set this bodypart to ignore collisions with another bodypart. 
        /// This is useful if you want players on the same team to not collide with each other despite 
        /// being in the same layer
        /// </summary>
        /// <param name="bodypart"></param>
        public void SetToIgnoreCollision(BodyPartCollider bodypart) {
            if (is2D) {
                Physics2D.IgnoreCollision(collider2D, bodypart.collider2D);
            } else {
                Physics.IgnoreCollision(collider3D, bodypart.collider3D);
            }
        }

        public void SetToIgnoreCollision(GameObject other) {
            if (is2D) {
                Physics2D.IgnoreCollision(collider2D, other.GetComponent<Collider2D>());
            } else {
                Physics.IgnoreCollision(collider3D, other.GetComponent<Collider>());
            }
        }

        /// <summary>
        /// Feels straight down from the front of the collider if there is contact with the ground.
        /// If there is then the body is likely on an upwards incline
        /// </summary>
        /// <param name="rightSide"></param>
        /// <param name="groundableMaterials"></param>
        /// <returns></returns>
        public bool FeelInFront(bool rightSide, LayerMask groundableMaterials) {
            Vector2 tmp = Vector2.zero;
            return FeelInFront(rightSide, groundableMaterials, out tmp);
        }

        /// <summary>
        /// Feels straight down from the front of the collider if there is contact with the ground.
        /// If there is then the body is likely on an upwards incline
        /// </summary>
        /// <param name="rightSide"></param>
        /// <param name="groundableMaterials"></param>
        /// <param name="normal">normal of hit position</param>
        /// <returns></returns>
        public bool FeelInFront(bool rightSide, LayerMask groundableMaterials, out Vector2 normal) {
            float distance = 0.45f * GetHeight();
            normal = Vector2.zero;
            Vector2 samplePos = (Vector2)transform.position + new Vector2((rightSide ? 0.6f : -0.6f) * GetWidth(), 0);
            bool isHit = false;
            if (is2D) {
                RaycastHit2D hit = Physics2D.Raycast(samplePos, Vector2.down, distance, groundableMaterials);
                isHit = hit.collider != null;
                if (isHit) {
                    Debug.DrawLine(samplePos, samplePos + distance * Vector2.down, Color.red);
                    normal = hit.normal;
                } else
                    Debug.DrawLine(samplePos, samplePos + distance * Vector2.down, Color.green);

            } else {
                RaycastHit hit;
                isHit = Physics.Raycast(samplePos, Vector2.down, out hit, distance, groundableMaterials);
                if (isHit) {
                    normal = hit.normal;
                }

            }
            return isHit;
        }

        protected void SetCollider(float newScale, float offset = 0) {
            Vector3 localScale = transform.localScale;
            Vector3 localPosition = transform.localPosition;

            localScale.y = newScale;
            localPosition.y = originalLocalPos.y + offset;

            transform.localScale = localScale;
            transform.localPosition = localPosition;
            
        }
        
        public float GetWidth() {
            if (is2D) {
                if (collider2D is CircleCollider2D)
                    return ((CircleCollider2D)collider2D).radius * 2;
                else if (collider2D is BoxCollider2D)
                    return ((BoxCollider2D)collider2D).size.x;
                else throw new System.NotImplementedException();
            } else {
                if (collider3D is SphereCollider)
                    return ((SphereCollider)collider3D).radius * 2;
                else if (collider3D is BoxCollider)
                    return ((BoxCollider)collider3D).size.x;
                else throw new System.NotImplementedException();
            }
        }

        public float GetHeight() {
            if (is2D) {
                if (collider2D is CircleCollider2D)
                    return ((CircleCollider2D)collider2D).radius * 2;
                else if (collider2D is BoxCollider2D)
                    return ((BoxCollider2D)collider2D).size.y;
                else throw new System.NotImplementedException();
            } else {
                if (collider3D is SphereCollider)
                    return ((SphereCollider)collider3D).radius * 2;
                else if (collider3D is BoxCollider)
                    return ((BoxCollider)collider3D).size.y;
                else throw new System.NotImplementedException();
            }
        }

        public void DisableCollider() {
            if (is2D) {
                collider2D.enabled = false;
            } else {
                collider3D.enabled = false;
            }
        }

        public void EnableCollider() {
            if (is2D) {
                collider2D.enabled = true;
            } else {
                collider3D.enabled = true;
            }
        }
    }
}