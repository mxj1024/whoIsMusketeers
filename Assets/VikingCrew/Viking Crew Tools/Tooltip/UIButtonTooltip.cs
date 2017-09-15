using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace VikingCrewTools {
	public class UIButtonTooltip : MonoBehaviour {
        [Header("If left empty will use text")]
		public UnityEngine.Object tooltipObject;
        [Header("Only used if object left empty")]
        public string text = "";
        [Header("The text component that is a child of Tooltip Canvas")]
		public Text txtTooltip;

		// Use this for initialization
		void Start () {
            if (txtTooltip == null)
                txtTooltip = GameObject.Find("Tooltip Canvas").transform.GetChild(0).GetComponent<Text>();

        }
		
		// Update is called once per frame
		void Update () {
		
		}

		public void OnMouseEnter(){
            if (!enabled) return;
            if (txtTooltip == null)
                return;
            string tooltipString;
            if (tooltipObject != null)
                tooltipString = tooltipObject.ToString();
            else
                tooltipString = text;

			txtTooltip.gameObject.SetActive(true);
			txtTooltip.text = tooltipString;
			txtTooltip.transform.GetChild(0).GetComponent<Text>().text = tooltipString;
            //txtTooltip.GetComponentInChildren<Text>().text = tooltipObject.tooltipString;

        }

        public void OnMouseExit(){
            if (txtTooltip == null)
                return;
            txtTooltip.gameObject.SetActive(false);
		}
	}
}