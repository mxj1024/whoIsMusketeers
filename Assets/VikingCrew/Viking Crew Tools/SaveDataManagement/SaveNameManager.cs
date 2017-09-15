using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
namespace VikingCrewTools{
	public class SaveNameManager : MonoBehaviour {
        [System.Serializable]
        public class SaveEvent : UnityEvent<string> { }

        public InputField txtSaveName;
        public Dropdown ddoSaveNames;
        public List<string> saveNames;
        public string defaultSaveName = "Save 1";
        public string savesKey = "lkansdoi239071234ad";
        public string currentSavesKey = "asdpjmvdöoihaelkk9842";
        public bool doLoadCurrentSaveOnStart = false;
        public SaveEvent OnSave;
        public SaveEvent OnLoad;
        public SaveEvent OnSaveDoesNotExist;
        
        // Use this for initialization
        void Start () {
            string currentSave = defaultSaveName;
           
            //Set to current save if there is one
            if (SaveDataManagement.HasSavedData(currentSavesKey)) 
                currentSave = SaveDataManagement.LoadData<string>(currentSavesKey);

            //Handle list of all saves
            if (SaveDataManagement.HasSavedData(savesKey))
                saveNames = SaveDataManagement.LoadData<List<string>>(savesKey);
            else
                saveNames.Add(currentSave);

            if (txtSaveName != null)
                txtSaveName.text = currentSave;

            if (ddoSaveNames != null) {
                ddoSaveNames.ClearOptions();
                ddoSaveNames.AddOptions(saveNames);
                ddoSaveNames.onValueChanged.AddListener(HandleChooseSave);
            }

            if (doLoadCurrentSaveOnStart && SaveDataManagement.HasSavedData(currentSave))
                HandleLoad(currentSave);

        }
		
        public void HandleSave() {
            HandleSave(txtSaveName.text);
        }

        public void HandleSave(string currentSave) {
            if (!saveNames.Contains(currentSave)) {
                saveNames.Add(currentSave);
                ddoSaveNames.ClearOptions();
                ddoSaveNames.AddOptions(saveNames);
            }
            OnSave.Invoke(currentSave);
            SaveDataManagement.SaveData<List<string>>(saveNames, savesKey);
            SaveDataManagement.SaveData<string>(currentSave, currentSavesKey);
            if(ddoSaveNames != null)
                ddoSaveNames.value = saveNames.IndexOf(currentSave);
        }

        public void HandleLoad() {
            HandleLoad( txtSaveName.text);
        }

        public void HandleLoad(string currentSave) {
            if (!SaveDataManagement.HasSavedData(currentSave)) {
                OnSaveDoesNotExist.Invoke(currentSave);
                Debug.LogWarning("save: " + currentSave + "does not exist");
            } else {
                OnLoad.Invoke(currentSave);
                SaveDataManagement.SaveData<string>(currentSave, currentSavesKey);
                if(ddoSaveNames != null)
                    ddoSaveNames.value = saveNames.IndexOf(currentSave);
            }
        }

        void HandleChooseSave(int index) {
            txtSaveName.text = saveNames[index];
        }

        [ExposeMethodInEditor]
        public void DeleteAllSaves() {
            SaveDataManagement.DeleteAll();
        }

    }
}