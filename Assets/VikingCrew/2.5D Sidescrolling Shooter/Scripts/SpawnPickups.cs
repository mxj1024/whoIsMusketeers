using UnityEngine;
using System.Collections;

namespace VikingCrewTools.Sidescroller {
    public class SpawnPickups : MonoBehaviour {

        public float chancePerSecondToSpawn;
        public PickupData[] itemsToSpawn;
        public Transform lastSpawn;
        public GameObject spawnPrefab;
        // Use this for initialization
        void Start() {
            StartCoroutine(SpawnItems());
        }

        // Update is called once per frame
        void Update() {

        }

        IEnumerator SpawnItems() {
            WaitForSeconds wait = new WaitForSeconds(1f);
            while (true) {
                if (Random.Range(0f, 1f) < chancePerSecondToSpawn && lastSpawn == null)
                    SpawnRandom();
                yield return wait;
            }
        }

        void SpawnRandom() {
            lastSpawn = GameObject.Instantiate<GameObject>(spawnPrefab).transform;
            lastSpawn.SetParent(transform);
            lastSpawn.transform.position = transform.position;

            ItemPickupBehaviour item = lastSpawn.GetComponent<ItemPickupBehaviour>();
            item.pickupData = itemsToSpawn[Random.Range(0, itemsToSpawn.Length)];
            if (item.pickupData is FirearmData)
                item.itemType = ItemPickupBehaviour.ItemType.GUN;
            if (item.pickupData is MedKitData)
                item.itemType = ItemPickupBehaviour.ItemType.MEDKIT;
        }
    }
}