using UnityEngine;
using System.Collections;

namespace VikingCrewTools.MeterDemoScene {
    public class MeterTestScript : MonoBehaviour {
        public UIMeterBehaviour[] meters;
        public float value = 5;
        public float maxValue = 10f;
        private float _value = 0;
        // Use this for initialization
        void Start() {
        }

        // Update is called once per frame
        void Update() {
            if (_value != value) {
                _value = value;
                foreach (var meter in meters) {
                    meter.SetFillRatio(value / maxValue);
                }

            }
        }
    }
}
