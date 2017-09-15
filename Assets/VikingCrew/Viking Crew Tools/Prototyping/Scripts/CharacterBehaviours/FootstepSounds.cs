using UnityEngine;
using System.Collections;

namespace VikingCrewTools.Sidescroller {
    public class FootstepSounds : MonoBehaviour {
        public AudioClip[] stepSounds;
        public new AudioSource audio;

        private Rigidbody2D body2D; private Rigidbody body3D;
        private Vector2 velocity { get { return body2D != null ? body2D.velocity : (Vector2)body3D.velocity; } }
        public float maxVelocity = 3f;
        void Start() {
            body2D = GetComponentInParent<Rigidbody2D>();
            body3D = GetComponentInParent<Rigidbody>();
        }

        public void OnTakeStep() {
            int index = Random.Range(0, stepSounds.Length);

            float volume = Mathf.Clamp01(Mathf.Abs(velocity.x) / maxVelocity);
            audio.PlayOneShot(stepSounds[index], volume);
        }
    }
}