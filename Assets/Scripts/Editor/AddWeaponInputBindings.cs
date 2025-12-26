using UnityEngine;
using UnityEditor;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class AddWeaponInputBindings : EditorWindow
{
    private InputActionAsset inputActions;

    [MenuItem("Tools/Add Weapon Input Bindings")]
    public static void ShowWindow()
    {
        GetWindow<AddWeaponInputBindings>("Add Weapon Bindings");
    }

    private void OnGUI()
    {
        GUILayout.Label("Add Weapon Input Bindings", EditorStyles.boldLabel);
        GUILayout.Space(10);

        inputActions = (InputActionAsset)EditorGUILayout.ObjectField(
            "Input Actions Asset",
            inputActions,
            typeof(InputActionAsset),
            false
        );

        GUILayout.Space(10);

        if (GUILayout.Button("Add Fire, Reload, WeaponSwap Bindings", GUILayout.Height(40)))
        {
            AddBindings();
        }

        GUILayout.Space(10);
        EditorGUILayout.HelpBox(
            "Este script añadirá automáticamente los bindings para:\n\n" +
            "• Fire: Mouse Left + Gamepad RT\n" +
            "• Reload: R + Gamepad West (X)\n" +
            "• WeaponSwap: Q + Gamepad North (Y)",
            MessageType.Info
        );
    }

    private void AddBindings()
    {
        if (inputActions == null)
        {
            EditorUtility.DisplayDialog("Error", "Por favor asigna el Input Actions Asset primero", "OK");
            return;
        }

        var gameplayMap = inputActions.FindActionMap("GamePlay");
        if (gameplayMap == null)
        {
            EditorUtility.DisplayDialog("Error", "No se encontró el Action Map 'GamePlay'", "OK");
            return;
        }

        bool madeChanges = false;

        madeChanges |= AddFireBindings(gameplayMap);
        madeChanges |= AddReloadBindings(gameplayMap);
        madeChanges |= AddWeaponSwapBindings(gameplayMap);

        if (madeChanges)
        {
            EditorUtility.SetDirty(inputActions);
            AssetDatabase.SaveAssets();
            EditorUtility.DisplayDialog("Éxito", "Bindings añadidos correctamente. Regenera la clase C# si es necesario.", "OK");
            Debug.Log("<color=green>✅ Weapon input bindings añadidos correctamente</color>");
        }
        else
        {
            EditorUtility.DisplayDialog("Info", "No se hicieron cambios. Los bindings ya existen o las acciones no se encontraron.", "OK");
        }
    }

    private bool AddFireBindings(InputActionMap map)
    {
        var fireAction = map.FindAction("Fire");
        if (fireAction == null)
        {
            Debug.LogWarning("Acción 'Fire' no encontrada. Créala primero en el Input Actions Editor.");
            return false;
        }

        bool added = false;

        if (!HasBinding(fireAction, "<Mouse>/leftButton"))
        {
            fireAction.AddBinding("<Mouse>/leftButton").WithGroups("Keyboard");
            Debug.Log("✅ Binding añadido: Fire → <Mouse>/leftButton");
            added = true;
        }

        if (!HasBinding(fireAction, "<Gamepad>/rightTrigger"))
        {
            fireAction.AddBinding("<Gamepad>/rightTrigger").WithGroups("Gamepad");
            Debug.Log("✅ Binding añadido: Fire → <Gamepad>/rightTrigger");
            added = true;
        }

        return added;
    }

    private bool AddReloadBindings(InputActionMap map)
    {
        var reloadAction = map.FindAction("Reload");
        if (reloadAction == null)
        {
            Debug.LogWarning("Acción 'Reload' no encontrada. Créala primero en el Input Actions Editor.");
            return false;
        }

        bool added = false;

        if (!HasBinding(reloadAction, "<Keyboard>/r"))
        {
            reloadAction.AddBinding("<Keyboard>/r").WithGroups("Keyboard");
            Debug.Log("✅ Binding añadido: Reload → <Keyboard>/r");
            added = true;
        }

        if (!HasBinding(reloadAction, "<Gamepad>/buttonWest"))
        {
            reloadAction.AddBinding("<Gamepad>/buttonWest").WithGroups("Gamepad");
            Debug.Log("✅ Binding añadido: Reload → <Gamepad>/buttonWest");
            added = true;
        }

        return added;
    }

    private bool AddWeaponSwapBindings(InputActionMap map)
    {
        var swapAction = map.FindAction("WeaponSwap");
        if (swapAction == null)
        {
            Debug.LogWarning("Acción 'WeaponSwap' no encontrada. Créala primero en el Input Actions Editor.");
            return false;
        }

        bool added = false;

        if (!HasBinding(swapAction, "<Keyboard>/q"))
        {
            swapAction.AddBinding("<Keyboard>/q").WithGroups("Keyboard");
            Debug.Log("✅ Binding añadido: WeaponSwap → <Keyboard>/q");
            added = true;
        }

        if (!HasBinding(swapAction, "<Gamepad>/buttonNorth"))
        {
            swapAction.AddBinding("<Gamepad>/buttonNorth").WithGroups("Gamepad");
            Debug.Log("✅ Binding añadido: WeaponSwap → <Gamepad>/buttonNorth");
            added = true;
        }

        return added;
    }

    private bool HasBinding(InputAction action, string path)
    {
        foreach (var binding in action.bindings)
        {
            if (binding.effectivePath == path)
                return true;
        }
        return false;
    }
}
