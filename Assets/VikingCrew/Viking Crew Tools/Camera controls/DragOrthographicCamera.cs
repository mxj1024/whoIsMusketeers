using UnityEngine;
using System.Collections;

// Attach to orthographic camera with rotation (0,0,0)
public class DragOrthographicCamera : MonoBehaviour {

    private Vector3 startMousePos;
    private bool isDown = false;
    Camera cam;
    void Start() {
        cam = GetComponent<Camera>();
    }

    void Update() {
        if (Input.GetMouseButtonDown(0) && (UnityEngine.EventSystems.EventSystem.current == null || UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject == null)) {
            startMousePos = cam.ScreenToWorldPoint(Input.mousePosition);
            startMousePos.z = 0.0f;
            isDown = true;
        }

        if (Input.GetMouseButton(0) && isDown) {
            Vector3 nowMousePos = cam.ScreenToWorldPoint(Input.mousePosition);
            nowMousePos.z = 0.0f;
            transform.position += startMousePos - nowMousePos;
        } else {
            isDown = false;
        }
    }
}