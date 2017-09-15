using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Events;

namespace VikingCrewTools.Sidescroller {
    public class InventoryBehaviour : MonoBehaviour {
        [System.Serializable]
        public class RatioUpdatedEvent : UnityEvent<float> { }
        [Tooltip("The script that will make the hands go to the right places on the weapon")]
        public IKControl ik;
        [Tooltip("The bone in the object hierarchy you want the weapon to follow. We use the spine as parent and then us IK to make the hands grab it.")]
        public Transform gunParentBone;
        public GameObject itemDropPrefab;

        public List<FirearmBehaviour> carriedGuns;
        public FirearmBehaviour currentGun;
        public System.Action<FirearmBehaviour> OnSwitchedWeapon;
        [Header("Connect this to a meter to display progress")]
        public RatioUpdatedEvent OnWeaponReloadUpdate;
        [Header("Connect this to health behaviour to add health on pickup")]
        public RatioUpdatedEvent OnMedkitPickup;
        [Tooltip("Is set to true then only stuff that is not already in inventory will be picked up when touching it")]
        public bool doOnlyPickupNewTypes = true;
        public UnityEvent OnReloadStart;
        public UnityEvent OnReloadFinished;
        public UnityEvent OnReloadAborted;
        public FirearmData weaponToStartWith = null;
        private int currentWeaponIndex = 0;
        // Use this for initialization
        void Start() {
            carriedGuns.Add(null);
            if (weaponToStartWith != null)
                PickupWeapon(weaponToStartWith);
        }

        // Update is called once per frame
        void Update() {

        }

        /// <summary>
        /// We use this to detect when the character touches an object that can be picked up
        /// </summary>
        /// <param name="coll"></param>
        void OnTriggerEnter2D(Collider2D coll) {
            ItemPickupBehaviour pickup = coll.gameObject.GetComponent<ItemPickupBehaviour>();

            if (pickup != null) {
                HandlePickup(pickup);
            }
        }

        void OnTriggerEnter(Collider coll) {
            ItemPickupBehaviour pickup = coll.gameObject.GetComponent<ItemPickupBehaviour>();

            if (pickup != null) {
                if (doOnlyPickupNewTypes && DoInventoryContain(pickup)) {
                    //We already have one type of that item so do not pick up another one
                    
                    return;

                }

                HandlePickup(pickup);
            }
        }

        void HandlePickup(ItemPickupBehaviour pickup) {
            pickup.OnPickupCallback();

            switch (pickup.itemType) {
                case ItemPickupBehaviour.ItemType.GUN:
                    PickupWeapon(pickup.pickupData as FirearmData);
                    break;
                case ItemPickupBehaviour.ItemType.MEDKIT:
                    PickupMedkit(pickup.pickupData as MedKitData);
                    break;
                default:
                    break;
            }

            Destroy(pickup.gameObject);
        }

        public bool DoInventoryContain(ItemPickupBehaviour pickup) {
            switch (pickup.itemType) {
                case ItemPickupBehaviour.ItemType.GUN:
                    var firearmData = pickup.pickupData as FirearmData;
                    foreach (var item in carriedGuns) {
                        if(item != null && item.data == firearmData) {
                            return true;
                        }
                    }
                    return false;
                case ItemPickupBehaviour.ItemType.MEDKIT:
                    //We don't have any medkits that can be carried yet
                    return false;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Cycles the list of guns one step.
        /// </summary>
        /// <param name="isRight"></param>
        public void CycleGuns(bool isRight) {
            currentWeaponIndex = Mathf.Clamp(currentWeaponIndex + (isRight ? 1 : -1), 0, carriedGuns.Count - 1);

            Equip(carriedGuns[currentWeaponIndex]);
        }

        /// <summary>
        /// Give an item to the character
        /// </summary>
        /// <param name="gun"></param>
        public void GiveItem(FirearmData gun) {
            PickupWeapon(gun);
        }

        void PickupMedkit(MedKitData medkitData) {
            OnMedkitPickup.Invoke(medkitData.healAmount);
        }

        /// <summary>
        /// This will instantiate AND equip the weapon
        /// </summary>
        /// <param name="gunData"></param>
        void PickupWeapon(FirearmData gunData) {

            GameObject gunObj = GameObject.Instantiate<GameObject>(gunData.prefab);
            gunObj.transform.SetParent(gunParentBone);

            FirearmBehaviour gunScript = gunObj.GetComponent<FirearmBehaviour>();
            gunObj.transform.localRotation = Quaternion.Euler(gunScript.parentBoneAngleOffset);
            gunObj.transform.localPosition = gunScript.parentBonePositionOffset;
            gunScript.data = gunData;
            gunScript.owner = gameObject;
            carriedGuns.Add(gunScript);

            gunScript.ownerBodyparts = GetComponent<CharacterController2D>().bodyParts;

            Equip(gunScript);
        }

        /// <summary>
        /// Sets a gun to be used as current weapon
        /// We are assuming that this gun is already included in "carried guns"
        /// </summary>
        /// <param name="gun"></param>
        private void Equip(FirearmBehaviour gun) {
            if (currentGun != null)
                currentGun.gameObject.SetActive(false);
            currentGun = gun;
            currentWeaponIndex = carriedGuns.IndexOf(gun);
            if (gun == null) {
                ik.LeftHandObj = null;
                ik.RightHandObj = null;
                ik.LookObj = null;
                ik.IkActive = false;
                if (OnSwitchedWeapon != null) {
                    OnSwitchedWeapon(null);
                }
                return;
            }

            ik.LeftHandObj = gun.leftHandle;
            ik.RightHandObj = gun.rightHandle;
            ik.LookObj = gun.muzzleOffset;
            ik.IkActive = true;
            gun.gameObject.SetActive(true);
            gun.PlayPickupSound();
            if (OnSwitchedWeapon != null) {
                OnSwitchedWeapon(gun);
            }
        }

        /// <summary>
        /// Handles the character death with regards to equipment by
        /// creating dropped items from everything in the inventory
        /// </summary>
        public void HandleDeath() {
            Equip(null);
            foreach (var item in carriedGuns) {
                if (item == null) continue;
                DropItem(item);
            }
        }

        /// <summary>
        /// Drops an item and makes it an item that can be picked up in the game world
        /// </summary>
        /// <param name="itemData"></param>
        void DropItem(FirearmBehaviour gunScript) {
            GameObject item = GameObject.Instantiate<GameObject>(itemDropPrefab);
            item.transform.position = transform.position + Vector3.up;
            item.transform.localScale = 2 * item.transform.localScale;
            ItemPickupBehaviour itemScript = item.GetComponent<ItemPickupBehaviour>();
            itemScript.pickupData = gunScript.data;
            itemScript.itemType = ItemPickupBehaviour.ItemType.GUN;
        }

        /// <summary>
        /// Tries to reload the currently equipped weapon
        /// </summary>
        public void ReloadCurrentWeapon() {
            if (!currentGun) return;
            if (currentGun.isReloading) return;
            if (currentGun.currentAmmo == currentGun.data.maxAmmo) return;
            currentGun.isReloading = true;
            StartCoroutine(Reload(currentGun.data.reloadTime));
        }

        /// <summary>
        /// This coroutine handles the reloading of a weapon.
        /// It will be aborted if a different weapon is equipped
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        private IEnumerator Reload(float time) {
            //TODO: in the future add different types of reloads, like one bullet at a time in a revolver or shotgun
            FirearmBehaviour gun = currentGun;
            float interval = time / 10f;
            WaitForSeconds wait = new WaitForSeconds(interval);
            float reloadLeft = time;
            OnReloadStart.Invoke();
            gun.PlayReloadSound();
            while (reloadLeft > 0) {
                yield return wait;
                //If somehow the current weapon changed then we stop this reload
                if (currentGun != gun) {
                    gun.isReloading = false;
                    OnReloadAborted.Invoke();
                    yield break;
                }
                reloadLeft -= interval;
                OnWeaponReloadUpdate.Invoke(reloadLeft / time);
            }
            gun.currentAmmo = gun.data.maxAmmo;
            gun.isReloading = false;
            if (gun.OnWeaponAmmoChange != null)
                gun.OnWeaponAmmoChange(gun);
            OnReloadFinished.Invoke();
        }
    }
}