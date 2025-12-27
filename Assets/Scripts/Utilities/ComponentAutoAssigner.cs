using UnityEngine;

namespace TheHunt.Utilities
{
    public class ComponentAutoAssigner
    {
        public T FindComponent<T>(string tag = "Player") where T : Component
        {
            GameObject taggedObject = GameObject.FindGameObjectWithTag(tag);
            if (taggedObject != null)
            {
                T component = taggedObject.GetComponent<T>();
                if (component != null)
                {
                    Debug.Log($"<color=green>[AUTO ASSIGNER] Found {typeof(T).Name} on GameObject with tag '{tag}'</color>");
                    return component;
                }
            }
            
            T sceneComponent = Object.FindFirstObjectByType<T>();
            if (sceneComponent != null)
            {
                Debug.Log($"<color=yellow>[AUTO ASSIGNER] Found {typeof(T).Name} in scene (no tag match)</color>");
                return sceneComponent;
            }
            
            Debug.LogWarning($"<color=red>[AUTO ASSIGNER] Could not find {typeof(T).Name} in scene</color>");
            return null;
        }

        public T GetOrFindComponent<T>(T existingComponent, string tag = "Player") where T : Component
        {
            if (existingComponent != null)
            {
                Debug.Log($"<color=cyan>[AUTO ASSIGNER] Using existing {typeof(T).Name} reference</color>");
                return existingComponent;
            }

            return FindComponent<T>(tag);
        }
    }
}
