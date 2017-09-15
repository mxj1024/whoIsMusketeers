using UnityEngine;
using System.Collections;
using UnityEngine.Events;

namespace VikingCrewTools.Sidescroller {
    /// <summary>
    /// This class handles objects that are placed in the world and can be picked up by characters when they touch them
    /// 
    /// This class collaborates with InventoryBehaviour to that effect.
    /// </summary>
    public class ItemPickupBehaviour : MonoBehaviour {
        public enum ItemType {
            GUN,
            MEDKIT,
        }

        public ItemType itemType;
        [Header("If you like you could add events you want to happen here. Sounds could play or particle effects could emit")]
        public UnityEvent OnPickup;
        [Header("This script will instantiate the correct prefab as long as you assing this.")]
        public PickupData pickupData;
        public Transform rotatingChild;

        void Start() {
            Setup();
        }

        /// <summary>
        /// Spawns a prefab to show this pickup in the world
        /// </summary>
        void Setup() {

            if (rotatingChild == null) {
                GameObject drop = GameObject.Instantiate<GameObject>(pickupData.prefab);
                rotatingChild = drop.transform;
                switch (itemType) {
                    case ItemType.GUN:
                        drop.GetComponent<FirearmBehaviour>().data = (FirearmData)pickupData;
                        break;
                    case ItemType.MEDKIT:

                        break;
                    default:
                        break;
                }

                rotatingChild.SetParent(transform);
                rotatingChild.position = transform.position;

            }
        }

        // Update is called once per frame
        void Update() {
            if (rotatingChild != null) {
                rotatingChild.RotateAround(transform.position, Vector3.up, Time.deltaTime * 180);
            }
        }

        public void OnPickupCallback() {
            //Maybe play some sound or something?
            OnPickup.Invoke();
        }
    }
}