using UnityEngine;
using System.Collections;


using Pathfinding;
/// <summary>
/// We use this class to process a grid graph once it is constructed or changed
/// 
/// The point here is to set a higher cost to nodes that are higher up from the ground so we prefer running on the ground
/// 
/// </summary>
public class PostProcessGridGraph : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void PostProcessWholeGrid(GridGraph grid) {
        
    }
}
