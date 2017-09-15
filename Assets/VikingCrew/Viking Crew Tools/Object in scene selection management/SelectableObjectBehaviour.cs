using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VikingCrewTools.ObjectSelection {
	public class SelectableObjectBehaviour : MonoBehaviour {
        public Projector projector;

        private bool isSelected = false;

        public bool IsSelected
        {
            get
            {
                return isSelected;
            }
        }

        public System.Action<GameObject> OnSelected;
        public System.Action<GameObject> OnDeselected;

        // Use this for initialization
        void Start () {
            

		}

        // Update is called once per frame
        void Update () {
            if (!UnityImprovements.IsPointerOverUi() && Input.GetMouseButtonDown(0)) {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if(!IsSelected && Physics.Raycast(ray, out hit) && hit.collider.gameObject == gameObject) {
                    Select();
                }else if(IsSelected && !Input.GetKey(KeyCode.LeftControl) && !Input.GetKey(KeyCode.RightControl)) {
                    Deselect();
                }
            }
		}

        public void Select() {
            isSelected = true;
            projector.enabled = true;

            if (OnSelected != null)
                OnSelected(gameObject);
        }

        public void Deselect() {
            isSelected = false;
            projector.enabled = false;
            if (OnDeselected != null)
                OnDeselected(gameObject);
        }

        
	}
}