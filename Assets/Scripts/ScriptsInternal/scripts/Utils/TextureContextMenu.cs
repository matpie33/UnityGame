using UnityEditor;
using UnityEngine;

public static class TextureContextMenu
{
    [MenuItem("Assets/Set Texture Settings")]
    private static void ApplyTextureSettings()
    {
        foreach (Object obj in Selection.objects)
        {
            string path = AssetDatabase.GetAssetPath(obj);
            if (string.IsNullOrEmpty(path))
                continue;

            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer == null)
                continue;

            importer.filterMode = FilterMode.Trilinear;
            importer.textureCompression = TextureImporterCompression.CompressedHQ;
            importer.compressionQuality = 100;

            string lowerName = obj.name.ToLower();
            if (lowerName.Contains("normal"))
            {
                importer.textureType = TextureImporterType.NormalMap;
            }

            EditorUtility.SetDirty(importer);
            importer.SaveAndReimport();

            Debug.Log($"Updated: {obj.name}  |  Normal map: {lowerName.Contains("normal")}");
        }
    }
}