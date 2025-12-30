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
                    return component;
                }
            }
            
            T sceneComponent = Object.FindFirstObjectByType<T>();
            if (sceneComponent != null)
            {
                return sceneComponent;
            }
            
            Debug.LogWarning($"<color=red>[AUTO ASSIGNER] Could not find {typeof(T).Name} in scene</color>");
            return null;
        }

        public T GetOrFindComponent<T>(T existingComponent, string tag = "Player") where T : Component
        {
            if (existingComponent != null)
            {
                return existingComponent;
            }

            return FindComponent<T>(tag);
        }
    }
}
