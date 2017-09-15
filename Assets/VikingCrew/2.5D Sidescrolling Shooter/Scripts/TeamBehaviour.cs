using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace VikingCrewTools.Sidescroller {
    /// <summary>
    /// The responsibility of this class is to keep track of team data, not team AI.
    /// It will make sure to spawn new guys for each team.
    /// </summary>
    public class TeamBehaviour : MonoBehaviour {
        [System.Serializable]
        public class CharacterSelectedEvent : UnityEngine.Events.UnityEvent<GameObject> { }

        public string teamName = "Team";
        public List<Transform> spawnPoints;
        public bool doKeepSpawning = true;
        public int maxTotalSpawns = -1;
        [Header("If using AI you should set this to false otherwise they may get stuck against each other")]
        public bool doCollideWithTeammates = false;
        public int maxNoOfTeammates = 3;
        public float timeBetweenSpawns = 5;
        public GameObject characterPrefab;
        public Transform teammatesParent;
        public List<Transform> teammates;
        public TeamBehaviour enemyTeam;
        public Material teamColors;
        public int totalSpawns = 0;
        public FirearmData[] weaponsToChoseFrom;
        public Transform playerControlledCharacter;
        public AIControls.AIState nextSpawnAIState;
        public Color minimapColor = Color.white;
        public CharacterSelectedEvent OnPlayerCharacterChanged;
        public bool isPlayerControlled = false;
        // Use this for initialization
        void Start() {
            StartCoroutine(ContinousSpawn());
            if (playerControlledCharacter != null)
                SetPlayerControl(playerControlledCharacter.gameObject);
        }

        // Update is called once per frame
        void Update() {

        }

        IEnumerator ContinousSpawn() {
            while (doKeepSpawning) {
                
                if (teammates.Count < maxNoOfTeammates && spawnPoints.Count > 0 && (maxTotalSpawns == -1 || maxTotalSpawns < totalSpawns )) {
                    Spawn();
                    nextSpawnAIState = AIControls.GetRandomAIState();
                }
                yield return new WaitForSeconds(timeBetweenSpawns);
            }
        }

        private void Spawn() {
            totalSpawns++;
            GameObject teammate = (GameObject)GameObject.Instantiate(characterPrefab, GetRandomSpawnPoint().position, Quaternion.identity);
            teammate.name = teamName + " " + characterPrefab.name + " " + totalSpawns.ToString();
            teammate.transform.SetParent(teammatesParent);
            HandleTeammateGenerated(teammate);
        }

        Transform GetRandomSpawnPoint() {
            return spawnPoints[Random.Range(0, spawnPoints.Count)];
        }

        /// <summary>
        /// Adds callbacks and such. Note that this can be called from outside this class if you have players in the scene from start or
        /// want to generate players in some other way
        /// </summary>
        /// <param name="data">Data.</param>
        public void HandleTeammateGenerated(GameObject teammate) {
            CharacterController2D character = teammate.GetComponent<CharacterController2D>();
            character.Setup();

            TakeDamageBehaviour health = teammate.GetComponentInChildren<TakeDamageBehaviour>();
            health.OnDeath.AddListener(TeammateKilledCallback);
            teammates.Add(teammate.transform);

            AIControls ai = teammate.GetComponent<AIControls>();
            ai.enemies = enemyTeam.teammates;
            ai.allies = teammates;
            ai.EnterAIState(nextSpawnAIState);

            teammate.GetComponent<PlayerControls>().enabled = false;

            teammate.GetComponent<InventoryBehaviour>().GiveItem(weaponsToChoseFrom[Random.Range(0, weaponsToChoseFrom.Length)]);

            if (teamColors != null) {
                SkinnedMeshRenderer renderer = teammate.GetComponentInChildren<SkinnedMeshRenderer>();
                renderer.material = teamColors;
            }

            //Set teammates to not collide with each other. This is mainly due to AI being hard to program correctly otherwise
            //as they would then need local avoidance
            if (!doCollideWithTeammates) {
                SetTeammemberToIgnoreCollisionsWithTeammates(character);
            }

            //Setup minimap 
            SpriteRenderer sprite = teammate.GetComponentInChildren<SpriteRenderer>();
            sprite.color = minimapColor;

            if(isPlayerControlled && playerControlledCharacter == null) {
                SetPlayerControl(teammate);
            }

            Debug.Log(teammate + " was spawned");
        }

        /// <summary>
        /// Not having to reroute characters around each other in a team makes ai pathfinding sooo much easier
        /// </summary>
        /// <param name="character"></param>
        private void SetTeammemberToIgnoreCollisionsWithTeammates(CharacterController2D character) {
            foreach (var teammate in teammates) {
                if (teammate == null) continue;
                foreach (var teammateBodypart in teammate.GetComponent<CharacterController2D>().bodyParts) {
                    foreach (var bodypart in character.bodyParts) {
                        teammateBodypart.SetToIgnoreCollision(bodypart);
                    }
                }
            }
        }

        /// <summary>
        /// A callback called from a unity event. The unity event will be assigned at character creation
        /// </summary>
        /// <param name="teammateHealth"></param>
        public void TeammateKilledCallback(TakeDamageBehaviour teammateHealth) {
            Transform teammate = teammateHealth.transform;
            StartCoroutine(RemoveTeammateNextFrame(teammate));
            
            //Set the player to gain control of some other character if the killed one was controled by player
            if (playerControlledCharacter != null && teammate.transform == playerControlledCharacter)
                Invoke("SetPlayerControl", timeBetweenSpawns);
        }

        /// <summary>
        /// Waits until next frame to remove a teammate from the team. We use this to allow all other logic to finish
        /// processing on the death event before removing from the team.
        /// </summary>
        /// <param name="teammate"></param>
        /// <returns></returns>
        private IEnumerator RemoveTeammateNextFrame(Transform teammate) {
            yield return null;
            teammates.Remove(teammate);
        }

        /// <summary>
        /// Just gives control to an arbitrary character if there is one available
        /// </summary>
        public void SetPlayerControl() {
            if (teammates.Count > 0)
                SetPlayerControl(teammates[0].gameObject);
            else
                Invoke("SetPlayerControl", timeBetweenSpawns);
        }

        /// <summary>
        /// Sets the teammate to come under AI control
        /// </summary>
        /// <param name="teammate"></param>
        public void SetAIControl(GameObject teammate) {
            AIControls ai = teammate.GetComponent<AIControls>();
            if (ai != null)
                ai.isAiControlled = true;
            teammate.GetComponent<PlayerControls>().enabled = false;
        }

        /// <summary>
        /// Set the teammate to come under player control
        /// </summary>
        /// <param name="teammate"></param>
        public void SetPlayerControl(GameObject teammate) {
            playerControlledCharacter = teammate.transform;
            AIControls ai = teammate.GetComponent<AIControls>();
            if (ai != null)
                ai.isAiControlled = false;
            teammate.GetComponent<PlayerControls>().enabled = true;
            Camera.main.GetComponent<SmoothFollow2D>().target = teammate.GetComponent<PlayerControls>();
            Debug.Log("player controls new guy");
            OnPlayerCharacterChanged.Invoke(teammate);
        }
    }
}