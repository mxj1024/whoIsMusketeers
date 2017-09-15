using UnityEngine;
using System.Collections;
namespace VikingCrewTools {
    public class DrawCameraBounds : MonoBehaviour {
        public Transform rectangle;
        Camera cam;
        public float z = 0.25f;
        // Use this for initialization
        void Start() {
            cam = GetComponent<Camera>();
        }

        // Update is called once per frame
        void Update() {
            Bounds camBounds = cam.OrthographicBounds();
            Vector3 pos = camBounds.center;
            pos.z = z;
            rectangle.position = pos;
            rectangle.localScale = new Vector3(camBounds.size.x, camBounds.size.y, 1);
        }
    }
}