using UnityEngine;
using System.Collections;
using UnityEngine.Events;

namespace VikingCrewTools.Sidescroller {
    public class TakeDamageBehaviour : MonoBehaviour, IDamageable {
        [System.Serializable]
        public class CharacterDeathEvent : UnityEvent<TakeDamageBehaviour> { }
        [System.Serializable]
        public class CharacterHealthChangedEvent : UnityEvent<float> { }
        public ParticleSystem tissueDamagePrefab;
        public ParticleSystem armorDamage;

        public float maxHealth = 5;
        public float currentHealth = 5;
        public float hitpointsRegeneratedPerSecond = 1;
        public bool isAlive = true;
        public int maxEmissionParticles = 10;
        public float particlesPerPointOfDamage = 1;
        public Transform ragdollParent;
        public float timeFromDeathToDelete = 5f;
        public CharacterDeathEvent OnDeath;
        [Header("Fires when the character gets hurt or regenerates. Sends the amount of life left as a ratio 0-1")]
        public CharacterHealthChangedEvent OnHealthChange;
        // Use this for initialization
        void Start() {
            if (hitpointsRegeneratedPerSecond != 0)
                StartCoroutine(Regenerate(hitpointsRegeneratedPerSecond));
            if (tissueDamagePrefab != null) {
                tissueDamagePrefab = Instantiate(tissueDamagePrefab);
                tissueDamagePrefab.transform.SetParent(transform);
            }
        }

        // Update is called once per frame
        void Update() {

        }

        IEnumerator Regenerate(float hitpointsPerSecond) {
            float timeBetweenUpdates = 0.25f;
            WaitForSeconds wait = new WaitForSeconds(timeBetweenUpdates);
            while (isAlive) {
                ChangeHealth(hitpointsPerSecond * timeBetweenUpdates);

                yield return wait;
            }

        }

        /// <summary>
        /// Changes health without returning anything. We need this method in order to call from unity events
        /// e.g. when picking up medkits
        ///
        /// </summary>
        /// <param name="amount"></param>
        public void AddHealth(float amount) {
            ChangeHealth(amount);
        }

        /// <summary>
        /// Changes health. Returns true if death occcured (will also raise event for death)
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public bool ChangeHealth(float amount) {
            currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
            OnHealthChange.Invoke(currentHealth / maxHealth);
            if (currentHealth <= 0 && isAlive) {
                isAlive = false;
                HandleDeath();
                return true;//Died
            }

            return false;//Did not die
        }
        [ExposeMethodInEditor]
        public void EmitParticles() {
            if (tissueDamagePrefab != null) {
                tissueDamagePrefab.transform.position = transform.position;
                tissueDamagePrefab.transform.forward = Vector3.right;
                tissueDamagePrefab.Play(true);
                //tissueDamage.Emit(10);
            }
        }

        public void ApplyDamage(float damage, Vector3 position, Vector3 incomingDirection, GameObject assailant) {
            if (tissueDamagePrefab != null) {
                tissueDamagePrefab.transform.position = position;
                tissueDamagePrefab.transform.forward = -incomingDirection;

                tissueDamagePrefab.Emit(Mathf.Min((int)(particlesPerPointOfDamage * damage), maxEmissionParticles));
            }

            bool didDie = ChangeHealth(-damage);

            if (didDie && assailant != null) {
                IKiller killer = assailant.GetComponent<IKiller>();
                if (killer != null)
                    killer.HandleVictimDeath(gameObject);
            }

            OnHealthChange.Invoke(currentHealth / maxHealth);
        }

        void HandleDeath() {
            Rigidbody[] rbs3D = ragdollParent.GetComponentsInChildren<Rigidbody>();
            Collider[] colls3D = ragdollParent.GetComponentsInChildren<Collider>();
            foreach (var rb3D in rbs3D) {
                rb3D.isKinematic = false;
                rb3D.useGravity = true;
            }
            foreach (var col3D in colls3D) {
                col3D.enabled = true;
            }
            foreach (var item in GetComponents<Collider2D>()) {
                item.enabled = false;
            }


            CharacterController2D controller = GetComponent<CharacterController2D>();
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if(rb!= null) {
                rb.isKinematic = true;
                rb.gravityScale = 0;
                foreach (var item in controller.bodyParts) {
                    item.GetComponent<Collider2D>().enabled = false;
                }
            }
            Rigidbody rb3d = GetComponent<Rigidbody>();
            if (rb3d != null) {
                rb3d.isKinematic = true;
                rb3d.useGravity = false;
                

                foreach (var item in controller.bodyParts) {
                    item.GetComponent<Collider>().enabled = false;
                }
            }
            

            GetComponentInChildren<Animator>().enabled = false;

            if (tissueDamagePrefab != null) {
                tissueDamagePrefab.loop = true;
                tissueDamagePrefab.Play();
                tissueDamagePrefab.transform.SetParent(ragdollParent.GetChild(0));
            }
            OnDeath.Invoke(this);
            Destroy(gameObject, timeFromDeathToDelete);
        }

        
    }
}