using UnityEngine;
using System.Collections;
using UnityEngine.Events;
namespace VikingCrewTools.Sidescroller {
    public class FirearmBehaviour : MonoBehaviour {

        [Header("We use this particle system in case a bullet hits the environment and not a target")]
        public GameObject sparksParticleSystemPrefab;
        public Light muzzleFlashLight;
        public GameObject muzzleFlashParticleSystemPrefab;
        private ParticleSystem muzzleFlashParticles;
        [HideInInspector]
        public ParticleSystem bulletHitSparks;

        [Tooltip("The muzzle is the position where bullets are spawned")]
        public Transform muzzle;
        [Header("The following transforms are used for inverse kinematics"), Tooltip("The muzzle offset is the position where the character will point its head at")]
        public Transform muzzleOffset;
        [Tooltip("The leftHandle is the position where the left hand is positioned")]
        public Transform leftHandle;
        [Tooltip("The rightHandle is the position where the right hand is positioned")]
        public Transform rightHandle;

        [Header("Offset of weapon from the parent object (spine or hip)")]
        public Vector3 parentBonePositionOffset;
        public Vector3 parentBoneAngleOffset = new Vector3(90, 270, 0);
        [HideInInspector]
        public BodyPartCollider[] ownerBodyparts;

        #region Audio
        AudioSource _audio;
        new AudioSource audio {
            get {
                if (!_audio)
                    _audio = GetComponent<AudioSource>();
                return _audio;
            }
        }
        public AudioClip fireSound;
        public AudioClip reloadSound;
        public AudioClip pickupSound;
        public AudioClip clickSound;
        #endregion
        [HideInInspector]
        public FirearmData data;
        public System.Action<FirearmBehaviour> OnWeaponAmmoChange;
        public int currentAmmo;
        public float timeLeftOnReload;
        [HideInInspector]
        public bool isDrop = false;
        private int currentBurst = 0;
        private float currentFireCooldown = 0;
        public bool isReloading = false;
        /// <summary>
        /// The character or system carrying this gun. This can be null.
        /// This character will get credit for e.g. gun kills.
        /// </summary>
        public GameObject owner = null;

        // Use this for initialization
        void Start() {
            
            muzzleFlashParticles = GameObject.Instantiate<GameObject>(muzzleFlashParticleSystemPrefab).GetComponent<ParticleSystem>();
            muzzleFlashParticles.transform.SetParent(muzzle);
            muzzleFlashParticles.transform.localPosition = Vector3.zero;
            muzzleFlashParticles.transform.localRotation = Quaternion.identity;
            //Sets up particle system
            GameObject sparks = GameObject.Instantiate<GameObject>(sparksParticleSystemPrefab);
            sparks.transform.SetParent(transform);
            bulletHitSparks = sparks.GetComponent<ParticleSystem>();

            currentAmmo = data.maxAmmo;
            if (OnWeaponAmmoChange != null)
                OnWeaponAmmoChange(this);
        }

        // Update is called once per frame
        void Update() {
            currentFireCooldown -= Time.deltaTime;
            
        }

        public void ReleaseAutoFire() {
            currentBurst = 0;
        }

        public void PlayReloadSound() {
            audio.PlayOneShot(reloadSound);
        }

        public void PlayPickupSound() {
            audio.PlayOneShot(reloadSound);
        }

        public void AutoFire() {
            switch (data.fireMode) {
                case FirearmData.FireModeType.SINGLE_SHOT:
                    return;
                case FirearmData.FireModeType.BURST:
                    if (currentBurst > 3)
                        return;
                    Fire();

                    break;
                case FirearmData.FireModeType.FULL_AUTO:
                    Fire();
                    break;
                default:
                    break;
            }
        }

        public void Fire() {
            if (currentFireCooldown > 0) return;
            if (currentAmmo <= 0) {
                currentFireCooldown = data.fireCooldown;
                audio.PlayOneShot(clickSound);
                return;
            }
            if (isReloading) return;
            for (int i = 0; i < data.bulletsFiredAtATime; i++) {

                //Calculate a spread for the bullet
                float spreadAngle = Random.Range(-data.spread, data.spread);
                Quaternion spread = Quaternion.Euler(0, 0, spreadAngle);
                Vector3 direction = spread * muzzle.forward;
                direction.z = 0;
                float velocity = data.muzzleVelocity + Random.Range(-data.muzzleVelocityVariation, data.muzzleVelocityVariation);
                MuzzleFlash();

                //In order to make sure the bullet is instantiated inside the character collider we do this little trick. 
                //We do this to make sure that we will ALWAYS hit a target that may be in contact with us already.
                Vector3 bulletPosition = muzzle.localPosition;
                bulletPosition.z = 0;
                bulletPosition = transform.TransformPoint(bulletPosition);
                BulletBehaviour bullet = BulletManager.instance.Fire(bulletPosition, velocity * direction);
                bullet.GetComponent<BulletBehaviour>().gun = this;
                foreach (var bodyPart in ownerBodyparts) {
                    bodyPart.SetToIgnoreCollision(bullet.gameObject);
                }
            }

            currentFireCooldown = data.fireCooldown;
            audio.PlayOneShot(fireSound);
            currentBurst++;
            currentAmmo--;

            if (OnWeaponAmmoChange != null)
                OnWeaponAmmoChange(this);
        }

        void MuzzleFlash() {
            if (muzzleFlashLight) {
                muzzleFlashLight.intensity = 3;
                muzzleFlashParticles.Emit(1);
            }
        }

        void OnDisable() {
            muzzleFlashLight.intensity = 0;
        }

        void OnEnable() {
            StartCoroutine(MuzzleFlashUpdate());
        }

        IEnumerator MuzzleFlashUpdate() {
            while (true) {
                muzzleFlashLight.intensity -= 10 * Time.deltaTime;
                if (muzzleFlashLight.gameObject.activeSelf) {
                    if (muzzleFlashLight.intensity <= 0)
                        muzzleFlashLight.gameObject.SetActive(false);
                } else {
                    if (muzzleFlashLight.intensity > 0)
                        muzzleFlashLight.gameObject.SetActive(true);
                }
                yield return null;
            }
        }
    }
}