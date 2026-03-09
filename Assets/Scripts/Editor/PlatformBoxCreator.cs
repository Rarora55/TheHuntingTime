using UnityEditor;
using UnityEngine;

/// <summary>
/// Editor window to quickly create platform boxes pre-configured with
/// BoxCollider2D, SpriteRenderer and LedgeMarker (with two corner children).
/// Open via Tools > Level Design > Platform Box Creator.
/// </summary>
public class PlatformBoxCreator : EditorWindow
{
    // ── Layout settings ───────────────────────────────────────────────────────
    private float boxWidth  = 3f;
    private float boxHeight = 1f;
    private string boxName  = "Platform";

    // ── Placement settings ────────────────────────────────────────────────────
    private Vector3 spawnPosition = Vector3.zero;
    private bool placeAtSceneViewCenter = true;

    // ── Visual settings ───────────────────────────────────────────────────────
    private Color boxColor = new Color(0.537f, 0.537f, 0.537f, 1f);

    // ── LedgeMarker settings ──────────────────────────────────────────────────
    private bool addLedgeMarker = true;
    private bool showLedgeDebug = true;

    // ── Parent settings ───────────────────────────────────────────────────────
    private GameObject parentObject;

    // ── Sprite used by existing boxes ─────────────────────────────────────────
    private const string DEFAULT_SPRITE_PATH =
        "Packages/com.unity.2d.sprite/Editor/ObjectMenuCreation/DefaultAssets/Textures/v2/Square.png";
    private const string DEFAULT_MATERIAL_PATH =
        "Packages/com.unity.render-pipelines.universal/Runtime/Materials/Sprite-Lit-Default.mat";

    [MenuItem("Tools/Level Design/Platform Box Creator")]
    public static void OpenWindow()
    {
        PlatformBoxCreator window = GetWindow<PlatformBoxCreator>("Platform Box Creator");
        window.minSize = new Vector2(320f, 400f);
        window.Show();
    }

    private void OnGUI()
    {
        EditorGUILayout.Space(8f);
        GUIStyle headerStyle = new GUIStyle(EditorStyles.boldLabel) { fontSize = 13 };
        EditorGUILayout.LabelField("Platform Box Creator", headerStyle);
        EditorGUILayout.Space(4f);
        DrawSeparator();

        // ── Dimensions ───────────────────────────────────────────────────────
        EditorGUILayout.LabelField("Dimensions (World Units)", EditorStyles.boldLabel);
        boxWidth  = Mathf.Max(0.1f, EditorGUILayout.FloatField("Width",  boxWidth));
        boxHeight = Mathf.Max(0.1f, EditorGUILayout.FloatField("Height", boxHeight));

        EditorGUILayout.Space(6f);
        DrawSeparator();

        // ── Identity ─────────────────────────────────────────────────────────
        EditorGUILayout.LabelField("Identity", EditorStyles.boldLabel);
        boxName      = EditorGUILayout.TextField("Name", boxName);
        parentObject = (GameObject)EditorGUILayout.ObjectField(
            "Parent", parentObject, typeof(GameObject), true);

        EditorGUILayout.Space(6f);
        DrawSeparator();

        // ── Placement ────────────────────────────────────────────────────────
        EditorGUILayout.LabelField("Placement", EditorStyles.boldLabel);
        placeAtSceneViewCenter = EditorGUILayout.Toggle("Scene View Center", placeAtSceneViewCenter);

        using (new EditorGUI.DisabledGroupScope(placeAtSceneViewCenter))
        {
            spawnPosition = EditorGUILayout.Vector3Field("Position", spawnPosition);
        }

        EditorGUILayout.Space(6f);
        DrawSeparator();

        // ── Visuals ───────────────────────────────────────────────────────────
        EditorGUILayout.LabelField("Visuals", EditorStyles.boldLabel);
        boxColor = EditorGUILayout.ColorField("Color", boxColor);

        EditorGUILayout.Space(6f);
        DrawSeparator();

        // ── LedgeMarker ───────────────────────────────────────────────────────
        EditorGUILayout.LabelField("Ledge Marker", EditorStyles.boldLabel);
        addLedgeMarker = EditorGUILayout.Toggle("Add Ledge Marker", addLedgeMarker);
        using (new EditorGUI.DisabledGroupScope(!addLedgeMarker))
        {
            showLedgeDebug = EditorGUILayout.Toggle("Show Debug Gizmos", showLedgeDebug);
        }

        EditorGUILayout.Space(12f);

        // ── Preview ───────────────────────────────────────────────────────────
        EditorGUILayout.HelpBox(
            $"Box: {boxWidth:F2} × {boxHeight:F2} units\n" +
            $"Name: \"{boxName}\"\n" +
            $"LedgeMarker: {(addLedgeMarker ? "Yes" : "No")}",
            MessageType.None);

        EditorGUILayout.Space(6f);

        using (new EditorGUI.DisabledGroupScope(!IsSceneLoaded()))
        {
            if (GUILayout.Button("Create Platform Box", GUILayout.Height(36f)))
            {
                CreatePlatformBox();
            }
        }

        if (!IsSceneLoaded())
        {
            EditorGUILayout.HelpBox("No scene loaded.", MessageType.Warning);
        }
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Creation
    // ─────────────────────────────────────────────────────────────────────────

    private void CreatePlatformBox()
    {
        Undo.SetCurrentGroupName("Create Platform Box");
        int undoGroup = Undo.GetCurrentGroup();

        // ── Resolve spawn position ────────────────────────────────────────────
        Vector3 position = placeAtSceneViewCenter ? GetSceneViewCenter() : spawnPosition;

        // ── Root GameObject ───────────────────────────────────────────────────
        GameObject root = new GameObject(boxName);
        Undo.RegisterCreatedObjectUndo(root, "Create Platform Box");

        // Layer: Ground (index 6 in this project)
        int groundLayer = LayerMask.NameToLayer("Ground");
        if (groundLayer >= 0) root.layer = groundLayer;

        root.transform.position  = position;
        root.transform.localScale = new Vector3(boxWidth, boxHeight, 1f);

        // ── SpriteRenderer ────────────────────────────────────────────────────
        SpriteRenderer spriteRenderer = root.AddComponent<SpriteRenderer>();
        Sprite defaultSprite = AssetDatabase.LoadAssetAtPath<Sprite>(DEFAULT_SPRITE_PATH);
        Material defaultMaterial = AssetDatabase.LoadAssetAtPath<Material>(DEFAULT_MATERIAL_PATH);

        if (defaultSprite != null)   spriteRenderer.sprite           = defaultSprite;
        if (defaultMaterial != null) spriteRenderer.sharedMaterial   = defaultMaterial;
        spriteRenderer.color = boxColor;
        spriteRenderer.sortingLayerName = "Default";

        // ── BoxCollider2D (main) ──────────────────────────────────────────────
        root.AddComponent<BoxCollider2D>();

        // ── LedgeMarker + corner children ────────────────────────────────────
        if (addLedgeMarker)
        {
            AddLedgeMarker(root);
        }

        // ── Parent ────────────────────────────────────────────────────────────
        if (parentObject != null)
        {
            root.transform.SetParent(parentObject.transform, true);
        }

        // ── Select and focus ──────────────────────────────────────────────────
        Selection.activeGameObject = root;
        EditorGUIUtility.PingObject(root);
        Undo.CollapseUndoOperations(undoGroup);

        Debug.Log($"<color=cyan>[PlatformBoxCreator] Created \"{boxName}\" " +
                  $"({boxWidth:F2} × {boxHeight:F2} units) at {position}</color>");
    }

    /// <summary>Adds LedgeMarker component and two corner child GameObjects.</summary>
    private void AddLedgeMarker(GameObject root)
    {
        // The sprite is 1×1 and scale represents world units, so local corners
        // sit at ±0.5 in X and +0.5 in Y (top-left / top-right of the box).
        // We compensate scale on the children so their world size stays small.
        float invX = 1f / root.transform.localScale.x;
        float invY = 1f / root.transform.localScale.y;

        // Right corner (positive X → "rightCorner" in LedgeMarker)
        GameObject rightGO = new GameObject("LedgeMarker");
        Undo.RegisterCreatedObjectUndo(rightGO, "Create Right LedgeMarker");
        rightGO.transform.SetParent(root.transform, false);
        rightGO.transform.localPosition = new Vector3( 0.487f, 0.494f, 0f);
        rightGO.transform.localScale    = new Vector3(invX * 0.35f, invY * 0.25f, 1f);

        // Left corner (negative X → "leftCorner" in LedgeMarker)
        GameObject leftGO = new GameObject("LedgeMarker (1)");
        Undo.RegisterCreatedObjectUndo(leftGO, "Create Left LedgeMarker");
        leftGO.transform.SetParent(root.transform, false);
        leftGO.transform.localPosition = new Vector3(-0.487f, 0.504f, 0f);
        leftGO.transform.localScale    = new Vector3(invX * 0.35f, invY * 0.25f, 1f);

        // LedgeMarker component
        LedgeMarker marker = root.AddComponent<LedgeMarker>();
        marker.leftCorner   = rightGO.transform;
        marker.rightCorner  = leftGO.transform;
        marker.ledgeLayer   = LayerMask.GetMask("Ground");
        marker.showDebug    = showLedgeDebug;
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Helpers
    // ─────────────────────────────────────────────────────────────────────────

    private static Vector3 GetSceneViewCenter()
    {
        if (SceneView.lastActiveSceneView != null)
        {
            return SceneView.lastActiveSceneView.pivot;
        }
        return Vector3.zero;
    }

    private static bool IsSceneLoaded()
    {
        return UnityEngine.SceneManagement.SceneManager.GetActiveScene().isLoaded;
    }

    private static void DrawSeparator()
    {
        Rect rect = EditorGUILayout.GetControlRect(false, 1f);
        EditorGUI.DrawRect(rect, new Color(0.35f, 0.35f, 0.35f, 1f));
        EditorGUILayout.Space(4f);
    }
}
