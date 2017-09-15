using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace VikingCrewTools {
	public class StripComponentsScript : MonoBehaviour {
        public List<Component> componentsToStrip;
		// Use this for initialization
		void Start () {
		
		}
		
		// Update is called once per frame
		void Update () {
		
		}

        public void Strip(GameObject obj, bool includeChildren) {
            foreach (var item in componentsToStrip) {
                Type T = item.GetType();
                if (includeChildren) {
                    foreach (var itemToStrip in obj.GetComponentsInChildren(T)) {
                        Destroy(itemToStrip);
                    }
                } else {
                    foreach (var itemToStrip in obj.GetComponents(T)) {
                        Destroy(itemToStrip);
                    }
                }
            }
        }

        public void StripIncludingChildren(GameObject obj) {
            Strip(obj, true);
        }

	}
}