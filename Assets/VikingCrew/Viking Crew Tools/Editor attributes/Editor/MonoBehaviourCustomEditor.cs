// The MIT License (MIT) - https://gist.github.com/bcatcho/1926794b7fd6491159e7
// Copyright (c) 2015 Brandon Catcho
using UnityEngine;
using UnityEditor;
using System.Reflection;

// Place this file in any folder that is or is a descendant of a folder named "Editor"
namespace VikingCrewTools {
    [CanEditMultipleObjects] // Don't ruin everyone's day
    [CustomEditor(typeof(UnityEngine.Object), true)] // Target all unity engine objects and descendants
    public class MonoBehaviourCustomEditor : UnityEditor.Editor {
        public override void OnInspectorGUI() {
            DrawDefaultInspector(); // Draw the normal inspector

            // Original code edited to work in edit mode as well as play mode

            // Get the type descriptor for the MonoBehaviour we are drawing
            var type = target.GetType();

            // Iterate over each private or public instance method (no static methods atm)
            foreach (var method in type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)) {
                // make sure it is decorated by our custom attribute
                var attributes = method.GetCustomAttributes(typeof(ExposeMethodInEditorAttribute), true);
                if (attributes.Length > 0) {

                    if (GUILayout.Button("Run: " + method.Name)) {
                        // If the user clicks the button, invoke the method immediately.

                        //((MonoBehaviour)target).Invoke(method.Name, 0f);
                        // --- Edited to make it possible to invoke in both play and edit mode ---
                        //Invoke the method
                        // (null- no parameter for the method call
                        // or you can pass the array of parameters...)
                        //mi.Invoke(target, null);
                        method.Invoke(target, null);
                    }
                }
            }
        }
    }
}