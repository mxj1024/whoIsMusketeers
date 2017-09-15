using UnityEngine;
using System.Collections;

namespace VikingCrewTools {
    public interface IDamageable {
        void ApplyDamage(float damage, Vector3 position, Vector3 incomingDirection, GameObject assailant);
    }

    public interface IKiller {
        void HandleVictimDeath(GameObject victim);
    }
}