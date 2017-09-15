using UnityEngine;
using System.Collections;

public class ScrollZoom : MonoBehaviour {
    public float maxSize = 20;
    public float minSize = 5;
    public float scollSpeed = 1;
    Camera cam;
	// Use this for initialization
	void Start () {
        cam = GetComponent<Camera>();
	} 
	
	// Update is called once per frame
	void Update () {
	    float zoomAmount = scollSpeed * Input.GetAxis("Mouse ScrollWheel");

        if (cam.orthographic) {
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize - zoomAmount, minSize, maxSize);
            
        } else {
            Debug.Log("Nope, not implemented yet for perspective view, do it yourself lazybeans!");
        }
    }
}
