using UnityEngine;
using UnityEditor;

namespace TheHunt.Player.Editor
{
    // DISABLED - Uncomment [CustomEditor] attribute to re-enable custom Player inspector organization
    // [CustomEditor(typeof(GameObject))]
    public class PlayerInspectorOrganizer : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            GameObject gameObject = (GameObject)target;
            
            if (gameObject.CompareTag("Player"))
            {
                DrawPlayerInspector(gameObject);
            }
            else
            {
                base.OnInspectorGUI();
            }
        }
        
        private void DrawPlayerInspector(GameObject player)
        {
            EditorGUILayout.Space(10);
            
            EditorGUILayout.LabelField("═══════ MOVEMENT ═══════", EditorStyles.boldLabel);
            DrawComponent<global::Player>(player);
            DrawComponent<Rigidbody2D>(player);
            DrawComponent<Animator>(player);
            
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("═══════ INVENTORY ═══════", EditorStyles.boldLabel);
            DrawComponent<TheHunt.Inventory.InventorySystem>(player);
            DrawComponent<TheHunt.Inventory.InventoryUIController>(player);
            DrawComponent<TheHunt.Inventory.WeaponInventoryManager>(player);
            DrawComponent<TheHunt.Inventory.AmmoInventoryManager>(player);
            
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("═══════ UI & INPUT ═══════", EditorStyles.boldLabel);
            DrawComponent<TheHunt.Input.InputContextManager>(player);
            DrawComponent<TheHunt.UI.DialogService>(player);
            
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("═══════ HEALTH ═══════", EditorStyles.boldLabel);
            DrawComponent<HealthController>(player);
        }
        
        private void DrawComponent<T>(GameObject gameObject) where T : Component
        {
            T component = gameObject.GetComponent<T>();
            
            if (component != null)
            {
                UnityEditor.Editor componentEditor = CreateEditor(component);
                
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField(typeof(T).Name, EditorStyles.miniBoldLabel);
                componentEditor.OnInspectorGUI();
                EditorGUILayout.EndVertical();
                
                EditorGUILayout.Space(5);
            }
        }
    }
}
