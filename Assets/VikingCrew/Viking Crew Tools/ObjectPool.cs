using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace VikingCrewTools {
    public class ObjectPool : MonoBehaviour {

        /// <summary>
        /// The object prefabs which the pool can handle.
        /// </summary>
        public GameObject objectPrefab;

        /// <summary>
        /// The pooled objects currently available.
        /// </summary>
        public Stack<GameObject> pooledObjects;

        /// <summary>
        /// The amount of objects of each type to buffer.
        /// </summary>
        public int preheatAmountToBuffer;

        /// <summary>
        /// The container object that we will keep unused pooled objects so we dont clog up the editor with objects.
        /// </summary>
        protected Transform containerTransform;

        void Awake() {
        }

        // Use this for initialization
        void Start() {
            containerTransform = transform;

            //Loop through the object prefabs and make a new list for each one.
            //We do this because the pool can only support prefabs set to it in the editor,
            //so we can assume the lists of pooled objects are in the same order as object prefabs in the array
            pooledObjects = new Stack<GameObject>();

            for (int n = 0; n < preheatAmountToBuffer; n++) {
                GameObject newObj = Instantiate(objectPrefab) as GameObject;
                newObj.name = objectPrefab.name;
                PoolObject(newObj);
            }
        }

        /// <summary>
        /// Gets a new object for the name type provided.  If no object type exists or if onlypooled is true and there is no objects of that type in the pool
        /// then null will be returned.
        /// </summary>
        /// <returns>
        /// The object for type.
        /// </returns>
        /// <param name='objectType'>
        /// Object type.
        /// </param>
        /// <param name='onlyPooled'>
        /// If true, it will only return an object if there is one currently pooled.
        /// </param>
        public GameObject GetObject(bool onlyPooled) {
            if (pooledObjects.Count > 0) {
                GameObject pooledObject = pooledObjects.Pop();
                pooledObject.transform.parent = null;
                pooledObject.SetActive(true);

                return pooledObject;
            } else if (!onlyPooled) {
                return Instantiate(objectPrefab) as GameObject;
            }

            //If we have gotten here either there was no object of the specified type or non were left in the pool with onlyPooled set to true
            return null;
        }

        /// <summary>
        /// Pools the object specified.  Will not be pooled if there is no prefab of that type.
        /// </summary>
        /// <param name='obj'>
        /// Object to be pooled.
        /// </param>
        public void PoolObject(GameObject obj) {
            obj.SetActive(false);
            obj.transform.SetParent(containerTransform);
            pooledObjects.Push(obj);
        }

    }
}
