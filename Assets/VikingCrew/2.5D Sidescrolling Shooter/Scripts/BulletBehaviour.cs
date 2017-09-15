using UnityEngine;
using System.Collections;

namespace VikingCrewTools.Sidescroller {
    public class BulletBehaviour : MonoBehaviour {
        public float ricochetChance = 0.1f;
        [Tooltip("If speed is below this then the bullet should not cause damage nor ricochet when colliding but will instead simply be removed.")]
        public float minSpeedToCount = 5f;
        public FirearmBehaviour gun;

        private Rigidbody rb3D;
        private Rigidbody2D rb2D;
        public Vector2 velocity {
            get { if (rb2D != null) return rb2D.velocity; else return rb3D.velocity; }
            set { if (rb2D != null) rb2D.velocity = value; else rb3D.velocity = value; }
        }
        
        public void Setup() {

            rb2D = GetComponent<Rigidbody2D>();
            rb3D = GetComponent<Rigidbody>();
        }

        void OnCollisionEnter(Collision coll) {
            Collide(coll.gameObject, coll.contacts[0].point, coll.contacts[0].normal);
           
        }
        

        void OnCollisionEnter2D(Collision2D coll) {
            
            Collide(coll.gameObject, coll.contacts[0].point, coll.contacts[0].normal);
        }

        void Collide(GameObject collidingObject, Vector2 position, Vector2 normal) {
            if(velocity.magnitude < minSpeedToCount)
                BulletManager.instance.bulletPool.PoolObject(gameObject);

            if (Random.Range(0f, 1f) < ricochetChance) {
                //Bounce
                
            } else {
                BulletManager.instance.bulletPool.PoolObject(gameObject);
            }

            IDamageable damageable = collidingObject.GetComponent<IDamageable>();
            HandleDamage(damageable, position, normal);
        }

        void HandleDamage(IDamageable damageable, Vector3 position, Vector3 normal) {
            //If we hits something that can take damage then it's up to that object to show effects like particles etc
            if (damageable != null) {
                Vector3 direction = normal;
                direction.Normalize();
                if (direction.magnitude != 1)
                    direction = Random.insideUnitSphere.normalized;
                damageable.ApplyDamage(gun.data.damage, transform.position, direction, gun.owner);

            } else {//If we hit something that can't be damaged then we need to show proper effects
                if (gun != null) {
                    gun.bulletHitSparks.transform.position = position;
                    gun.bulletHitSparks.transform.forward = normal;

                    gun.bulletHitSparks.Emit(1);

                }
            }
        }
    }
}