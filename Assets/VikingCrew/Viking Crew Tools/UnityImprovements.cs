using UnityEngine;
using System.Collections;

namespace VikingCrewTools {
    /// <summary>
    /// This class contains some functionality that should already be included in the unity engine but, for some reason, is not.
    /// </summary>
    public static class UnityImprovements {
        /// <summary>
        /// Destroys all children of a transform. Note that the gameobjects may not actually be immediately destroyed but hte engine handles it
        /// when it feels like. This method will, however, detach the children from the parent so that we don't have to bother about them
        /// while they are dying
        /// </summary>
        /// <param name="transform"></param>
        public static void ClearChildren(this Transform transform) {
            for (int i = transform.childCount-1; i >= 0 ; i--) {
                GameObject.Destroy(transform.GetChild(i).gameObject);
            }
            //We detach the children while waiting for them to actually be destroyed so that they don't get in the way of game logic
            transform.DetachChildren();
        }
        public static bool IsPointerOverUi() {
            return UnityEngine.EventSystems.EventSystem.current != null && UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();
        }
        public static Bounds GetBounds(this Vector2[] points) {
            Bounds bounds = new Bounds();
            foreach (var point in points) {
                bounds.Encapsulate(point);
            }
            
            return bounds;
        }

        public static Vector2 GetRandomPointInside(this Rect rect) {
            return new Vector2(Random.Range(rect.xMin, rect.xMax), Random.Range(rect.yMin, rect.yMax));
        }

        public static Vector2 GetRandomPointOnEdge(this Rect rect) {
            Vector2 pos = new Vector2(Random.Range(rect.xMin, rect.xMax), Random.Range(rect.yMin, rect.yMax));
            if(Random.value < 0.5f) {
                if (Random.value < 0.5f) {
                    pos.x = rect.xMin;
                } else {
                    pos.x = rect.xMax;
                }
            } else {
                if (Random.value < 0.5f) {
                    pos.y = rect.yMin;
                } else {
                    pos.y = rect.yMax;
                }
            }
            return pos;
        }

        public static Bounds OrthographicBounds(this Camera camera) {
            float screenAspect = (float)Screen.width / (float)Screen.height;
            float cameraHeight = camera.orthographicSize * 2;
            Bounds bounds = new Bounds(
                camera.transform.position,
                new Vector3(cameraHeight * screenAspect, cameraHeight, 0));
            return bounds;
        }
    }
}