#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;

public static class CreateMaterialFromTextureHDRP
{
    private const string HDRP_SHADER = "HDRP/Lit";
    private const string TARGET_FOLDER = "Assets/Models/Materials";

    [MenuItem("Assets/Create Material From Texture (HDRP)", true)]
    private static bool ValidateCreateMaterial()
    {
        return Selection.activeObject is Texture2D;
    }

    [MenuItem("Assets/Create Material From Texture (HDRP)")]
    private static void CreateMaterial()
    {
        Texture2D texture = Selection.activeObject as Texture2D;
        if (texture == null)
        {
            Debug.LogError("Selected asset is not a Texture2D.");
            return;
        }

        if (!AssetDatabase.IsValidFolder(TARGET_FOLDER))
        {
            if (!AssetDatabase.IsValidFolder("Assets/Models"))
                AssetDatabase.CreateFolder("Assets", "Models");

            if (!AssetDatabase.IsValidFolder("Assets/Models/Materials"))
                AssetDatabase.CreateFolder("Assets/Models", "Materials");
        }

        string textureName = Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(texture));
        string materialPath = $"{TARGET_FOLDER}/{textureName}_Mat.mat";

        Shader shader = Shader.Find(HDRP_SHADER);
        if (shader == null)
        {
            Debug.LogError("HDRP/Lit shader not found. Make sure HDRP is installed.");
            return;
        }

        Material material = new Material(shader);
        material.name = textureName + "_Mat";

        if (material.HasProperty("_BaseColorMap"))
            material.SetTexture("_BaseColorMap", texture);
        else
            Debug.LogWarning("HDRP property _BaseColorMap not found.");

        AssetDatabase.CreateAsset(material, materialPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = material;

        Debug.Log("Created HDRP material at: " + materialPath);
    }
}
#endif