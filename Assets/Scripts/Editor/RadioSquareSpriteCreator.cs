using UnityEngine;
using UnityEditor;

public class RadioSquareSpriteCreator
{
    [MenuItem("Tools/Radio System/Create Square Sprite")]
    public static void CreateSquareSprite()
    {
        int size = 64;
        Texture2D texture = new Texture2D(size, size);
        
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                texture.SetPixel(x, y, Color.white);
            }
        }
        
        texture.Apply();
        
        string path = "Assets/Sprites/RadioSquare.png";
        System.IO.Directory.CreateDirectory("Assets/Sprites");
        byte[] bytes = texture.EncodeToPNG();
        System.IO.File.WriteAllBytes(path, bytes);
        
        AssetDatabase.Refresh();
        
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer != null)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.spritePixelsPerUnit = 100;
            importer.filterMode = FilterMode.Point;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        }
        
        Debug.Log($"<color=green>[RADIO] Square sprite created at: {path}</color>");
    }
}
