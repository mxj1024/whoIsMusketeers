using UnityEngine;
using System.Collections;

namespace VikingCrewTools {
    public interface ICamFollowTarget {
        float GetZoom();
        Vector3 GetVelocity();
        Vector3 GetCameraAimPosition();
    }

    public class SmoothFollow2D : MonoBehaviour {
        // The target we are following
        [SerializeField]
        public ICamFollowTarget target;
        // The distance in the x-z plane to the target
        //[SerializeField]
        private float distance = 10.0f;
        [SerializeField]
        private float maxDistance = 20.0f;
        [SerializeField]
        private float minDistance = 5f;
        [SerializeField]
        private float moveDamping = 1;

        public float lookAheadFactor = 1;
        public UnityEngine.Events.UnityEvent OnStart;
        void Awake() {
            OnStart.Invoke();
        }

        // Update is called once per frame
        void FixedUpdate() {
            // Early out if we don't have a target
            if ((MonoBehaviour)target == null)
                return;
            
            Vector3 velocity = target.GetVelocity();

            Zoom(target.GetZoom());

            var wantedPosition = target.GetCameraAimPosition() + Vector3.back * distance + lookAheadFactor * velocity;
            float x = Mathf.Lerp(transform.position.x, wantedPosition.x, moveDamping * Time.fixedDeltaTime);
            float y = Mathf.Lerp(transform.position.y, wantedPosition.y, moveDamping * Time.fixedDeltaTime);
            float z = Mathf.Lerp(transform.position.z, wantedPosition.z, moveDamping * Time.fixedDeltaTime);

            // Set the position of the camera on the x-y plane to:
            // distance meters behind the target
            transform.position = new Vector3(x, y, z);
            // Always look at the target
            //transform.LookAt(target);
        }

        public void Zoom(float value) {
            float newval = distance + value;
            distance = Mathf.Clamp(newval, minDistance, maxDistance);
            
        }
        
    }
}