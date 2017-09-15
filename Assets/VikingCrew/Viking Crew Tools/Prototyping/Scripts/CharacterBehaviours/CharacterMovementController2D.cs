using UnityEngine;
using System.Collections;


namespace VikingCrewTools.Sidescroller {
    /// <summary>
    /// This class controls the movement of a physics based character.
    /// It doesn't matter if Physics or Physics2D is useds
    /// </summary>
    public class CharacterMovementController2D : MonoBehaviour {
        [Header("Hover over variable names to see tooltips!"), Tooltip("Increase this to make the character faster and stronger")]
        public float moveForce;
        [Tooltip("Increase this to make the character jump higher")]
        public float jumpVelocity;
        [Tooltip("This force is added to the character body when walking up an incline. Otherwise it might be very hard to wal up a hill or stairs")]
        public float upforceAtIncline;
        [Tooltip("When the character is crouching she should move slower. Thus we reduce the force a bit by multiplying with this value."), Range(0f,1f)]
        public float crouchingMoveForceMultiplier = 0.5f;
        public float crouchingMaxSpeedMultiplier = 0.5f;
        #region Physics properties
        /* It has come to our attention that many who make 2D games actually for different reasons need to still use the 3D physics
        engine. One such reason is that procedural generation of 2D colliders is pretty slow while the mesh collider for 3D is very
        performant. We therefore will from now on include the possibility to also use 3D physics. Unfortunately this makes the code
        a bit more complex but we hope you will bear with us as this makes the system a lot more usable in future updates where we may
        include voxel world integrations for example.
        */
        private bool is2D { get { return rb2D != null; } }
        internal Vector2 velocity {
            get {
                if (is2D) return rb2D.velocity;
                else return rb3D.velocity;
            }
        }
        public float mass {
            get {
                if (is2D) return rb2D.mass;
                else return rb3D.mass;
            }
            set
            {
                if (is2D) rb2D.mass = value;
                else rb3D.mass = value;
            }
        }
        public float drag
        {
            get
            {
                if (is2D) return rb2D.drag;
                else return rb3D.drag;
            }
            set
            {
                if (is2D) rb2D.drag = value;
                else rb3D.drag = value;
            }
        }

        public bool HasBeenSetup
        {
            get
            {
                return hasBeenSetup;
            }
        }

        private Rigidbody2D rb2D;
        private Rigidbody rb3D;
        
        public BodyPartCollider mainBody;
        public BodyPartCollider feet;
        [HideInInspector]
        public BodyPartCollider[] bodyParts;
        #endregion Physics properties

        public bool isGrounded;
        public bool isFacingRight;
        public bool isCrouching;
        public LayerMask groundableMaterials;
        public Animator bodyAnimator;

        public new AudioSource audio;
        public AudioClip jumpSound;
        public AudioClip landSound;

        public int maxDoubleJumps = 1;
        protected int currentDoubleJumps = 0;

        public float maxSpeed = 5f;
        [Tooltip("Draws detectors for detecting ground beneath the feet")]
        public bool doDebugDrawDetectors = true;
        [Tooltip("Only applied when not running")]
        public float stoppingFriction = 0.5f;
        protected bool hasBeenSetup = false;

        protected float currentControlDirection = 0;

        // Use this for initialization
        void Start() {
            if (!hasBeenSetup)
                Setup();
        }

        public void Setup() {
            bodyParts = feet.transform.parent.GetComponentsInChildren<BodyPartCollider>();
            rb2D = GetComponent<Rigidbody2D>();
            rb3D = GetComponent<Rigidbody>();

            audio = GetComponent<AudioSource>();
            hasBeenSetup = true;
        }

        // Update is called once per frame
        void Update() {

            UpdateAnimationController();
        }

        void FixedUpdate() {
            UpdateGrounded();
            UpdateFriction();
        }

        public void AddForce(float horizontalForce, ForceMode2D mode = ForceMode2D.Force) {
            if (is2D)
                rb2D.AddForce(new Vector2(horizontalForce,0), mode);
            else
                rb3D.AddForce(new Vector2(horizontalForce, 0), (ForceMode)mode);
        }

        public void AddForce(Vector2 force, ForceMode2D mode = ForceMode2D.Force) {
            if (is2D)
                rb2D.AddForce(force, mode);
            else
                rb3D.AddForce(force, (ForceMode)mode);
        }

        private void UpdateFriction() {
            bool isRunningForward = velocity.x * currentControlDirection > 0;
            //Apply friction
            if (Mathf.Abs(velocity.x) > 0 && !isRunningForward && isGrounded) {
                //if velocity is very low then we might add so much friction that the body changes direction, like a little bounce. We don't want that.
                float maxForce = 0.5f * mass * velocity.x * velocity.x / Time.fixedDeltaTime;
                float frictionForce = mass * 9.82f * stoppingFriction;
                frictionForce = Mathf.Min(frictionForce, maxForce);

                if (velocity.x > 0) frictionForce *= -1;

                AddForce(frictionForce);
            }

            currentControlDirection = 0;
        }

        /// <summary>
        /// Applies movement physics. 
        /// </summary>
        /// <param name="direction"></param>
        public void Run(float direction) {
            currentControlDirection = direction;
            bool isRunningForward = velocity.x * direction > 0;
            float currentMaxSpeed = isCrouching ? crouchingMaxSpeedMultiplier * maxSpeed : maxSpeed;

            if (isRunningForward && // If velocity and direction point the same direction then we are trying to increase speed
               Mathf.Abs(velocity.x) > currentMaxSpeed) //If we are at max speed already don't increase speed any more
                direction = 0;
            
            if (direction == 0) return;

            //If we're on an incline add some force upwards
            if (IsIncline(direction > 0)) {
                Vector2 normal = GetInclineNormal(direction > 0);
                float angle = Vector2.Angle(Vector2.up, normal);
                if(angle > 10 && angle < 60) {
                    Vector2 force = Vector2.up * upforceAtIncline;
                    AddForce(force);
                }
            }
            
            if (Mathf.Abs(direction) > 1)
                direction /= Mathf.Abs(direction);
            if (isCrouching)
                AddForce(direction * moveForce * crouchingMoveForceMultiplier);
            else
                AddForce(direction * moveForce);
        }

        /// <summary>
        /// Tries to determine if this character is walking up a slope. If so then we add a force upwards to reduce friction
        /// Think of this like walking up stairs where you would push yourself upwards a bit
        /// </summary>
        /// <returns></returns>
        bool IsIncline(bool rightSide) {
            return feet.FeelInFront(rightSide, groundableMaterials);
        }

        Vector2 GetInclineNormal(bool rightSide) {
            Vector2 normal;
            feet.FeelInFront(rightSide, groundableMaterials, out normal);
            return normal;
        }

        public void Jump() {
            if (isGrounded || currentDoubleJumps < maxDoubleJumps) {
                //This represents a velocity change up to the velocity of jumpVelocity
                AddForce(Vector2.up * (jumpVelocity - velocity.y) * mass, ForceMode2D.Impulse);
                audio.PlayOneShot(jumpSound);

                currentDoubleJumps++;
            }
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
        /// <summary>
        /// Makes the weapon follow a world space position
        /// </summary>
        /// <param name="position"></param>
        public virtual void AimAtPosition(Vector3 position) {

            Vector3 rot = bodyAnimator.transform.rotation.eulerAngles;
            isFacingRight = position.x >= transform.position.x;
            if (isFacingRight) {
                rot.y = 90;
            } else {
                rot.y = 270;
            }
            bodyAnimator.transform.rotation = Quaternion.Euler(rot);
            
        }
        protected bool CheckForGround() {
            Vector2 circleCastStart = (Vector2)feet.transform.position + 0.5f * Vector2.up;
            Vector2 circleCastDirection = Vector2.down;
            float radius = feet.GetWidth() / 2;
            float circleCastDistance = 0.5f + (transform.position - feet.transform.position).magnitude + feet.GetWidth() / 2f;
            bool didHit = false;

            if (is2D) {
                RaycastHit2D hit = Physics2D.CircleCast(circleCastStart, radius, circleCastDirection, circleCastDistance, groundableMaterials);
                didHit = hit.collider != null;
            } else {
                RaycastHit hit;
                Physics.SphereCast(circleCastStart,  radius, circleCastDirection, out hit,  circleCastDistance, groundableMaterials);
                didHit = hit.collider != null;
            }

            if (doDebugDrawDetectors) {
                if (didHit)
                    DebugDrawPhysics.DebugDrawCircleCast(circleCastStart, radius, circleCastDirection, circleCastDistance, Color.red);
                else
                    DebugDrawPhysics.DebugDrawCircleCast(circleCastStart, radius, circleCastDirection, circleCastDistance, Color.green);
            }
            return didHit;
        }

        protected void UpdateAnimationController() {
            bodyAnimator.SetBool("OnGround", isGrounded);
            bodyAnimator.SetFloat("Forward", (isFacingRight ? velocity.x : -velocity.x));
        }

        public void Crouch(bool doCrouch) {
            doCrouch = doCrouch && isGrounded;

            //If we want to stand up then we need to check if there is room for it
            if (!doCrouch && isCrouching) {
                //TODO: maybe check for all bodyparts? Maybe even use a shape cast in potential position and size?
                float distance = mainBody.GetHeight() / mainBody.crouchSizeRatio / 2;
                Vector3 origin = mainBody.transform.position + 0.1f * Vector3.up;
                if (is2D && Physics2D.Raycast(origin, Vector2.up, distance, groundableMaterials))
                    return;
                if (!is2D && Physics.Raycast(origin, Vector3.up, distance, groundableMaterials))
                    return;
            }

            bodyAnimator.SetBool("Crouch", doCrouch);
            //Make body smaller when crouching
            if (doCrouch && !isCrouching) {
                foreach (var bodypart in bodyParts) {
                    bodypart.SetCrouchSizeAndPos();
                }
            }
            if (!doCrouch && isCrouching) {
                foreach (var bodypart in bodyParts) {
                    bodypart.SetOriginalSizeAndPos();
                }
            }

            isCrouching = doCrouch;
        }

        public void HandleDeathCallback() {
            foreach (var bodypart in bodyParts) {
                bodypart.DisableCollider();
            }
        }
    }
}