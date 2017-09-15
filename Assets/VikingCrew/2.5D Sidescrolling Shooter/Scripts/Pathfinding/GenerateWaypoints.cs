using UnityEngine;
using System.Collections;

namespace VikingCrewTools.Sidescroller {
    public class GenerateWaypoints : MonoBehaviour {
        public GameObject waypointPrefab;
        public Transform startPoint;
        public Transform endPoint;
        public float interval = 5f;
        public float distanceToCheck = 0.3f;
        public float minDistanceToCollider = 0.1f;
        // Use this for initialization
        void Awake() {

            CheckDirection(Vector2.down, startPoint);
            CheckDirection(Vector2.right, startPoint);
            CheckDirection(Vector2.left, startPoint);
            CheckDirection(Vector2.down, endPoint);
            CheckDirection(Vector2.right, endPoint);
            CheckDirection(Vector2.left, endPoint);

            Vector3 distance = endPoint.position - startPoint.position;
            Vector3 dir = distance.normalized;
            GameObject waypoint;
            for (float f = 0; f < distance.magnitude; f += interval) {
                waypoint = GameObject.Instantiate<GameObject>(waypointPrefab);
                waypoint.transform.position = startPoint.position + f * dir;
            }
            waypoint = GameObject.Instantiate<GameObject>(waypointPrefab);
            waypoint.transform.position = endPoint.position;
        }

        /// <summary>
        /// We check each direction to make sure the waypoints have some distance to each collider,
        /// otherwise the pathfinding raycasting may not get through properly
        /// </summary>
        /// <param name="dir"></param>
        void CheckDirection(Vector2 dir, Transform trans) {
            RaycastHit2D hit;
            hit = Physics2D.Raycast(trans.position, dir, 5 * minDistanceToCollider);
            if (hit.collider != null) {
                trans.position = hit.point - dir * minDistanceToCollider;
            }
        }

        // Update is called once per frame
        void Update() {

        }
    }
}
