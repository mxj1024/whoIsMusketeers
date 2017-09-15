using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VikingCrewTools.ObjectSelection{
	public class SelectionManager : MonoBehaviour {

		// Use this for initialization
		void Start () {
		
		}
		
		// Update is called once per frame
		void Update () {
		
		}

        public UnityEventGameObject OnCharacterWasSelected;
        public UnityEventGameObject OnCharacterWasDeselected;

        public void RegisterInstantiatedObject(GameObject newObj) {
            var selectable = newObj.GetComponent<SelectableObjectBehaviour>();
            if (selectable == null)
                return;
            selectable.OnSelected += HandleSelected;
            selectable.OnDeselected += HandleDeselected;
        }

        public void HandleSelected(GameObject selected) {
            StartCoroutine(DelayHandleSelected(selected));
        }

        /// <summary>
        /// Bit of a hack here, by waiting until end of frame we let any deselection logic process before mocing on.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private IEnumerator DelayHandleSelected(GameObject obj) {
            yield return new WaitForEndOfFrame();
            OnCharacterWasSelected.Invoke(obj);
        }

        public void HandleDeselected(GameObject selected) {
            OnCharacterWasDeselected.Invoke(selected);
        }
        
    }
}