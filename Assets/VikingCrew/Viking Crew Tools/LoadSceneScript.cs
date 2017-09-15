using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
namespace VikingCrewTools {
    public class LoadSceneScript : MonoBehaviour {
        public Scene scene;
        // Use this for initialization
        void Start() {

        }

        // Update is called once per frame
        void Update() {

        }

        public void LoadScene(string sceneName) {
            SceneManager.LoadScene(sceneName);
        }
    }
}