using UnityEngine;
using System.Collections;

public class UIFollowScreenSpace : MonoBehaviour {
    public Transform objectToFollow;

    public Vector2 offset;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        var pos = Camera.main.WorldToScreenPoint(objectToFollow.position);
        pos += (Vector3)offset;
        pos.z = 0;
        transform.position = pos;
	}
}
