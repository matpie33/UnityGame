using UnityEngine;
using UnityEditor;
using System.IO;

public static class UcupaintMaterialCreator
{
    private const string MenuPath =
        "Assets/Recreate ucupaint materials";

    [MenuItem(MenuPath, true)]
    private static bool ValidateCreateMaterials()
    {
        return GetSelectedFBXPath() != null;
    }

    [MenuItem(MenuPath)]
    private static void CreateMaterials()
    {
        string fbxPath = GetSelectedFBXPath();

        if (string.IsNullOrEmpty(fbxPath))
        {
            Debug.LogError("Please select an FBX file.");
            return;
        }

        string modelsDirectory =
            Path.GetDirectoryName(fbxPath).Replace("\\", "/");

        Debug.Log($"Processing file: {fbxPath}");

        Object[] assets =
            AssetDatabase.LoadAllAssetsAtPath(fbxPath);

        int created = 0;
        int updated = 0;
        int skipped = 0;

        foreach (Object asset in assets)
        {
            if (!(asset is Material sourceMaterial))
                continue;

            string materialName = sourceMaterial.name;

            Debug.Log(
                $"Found material name: {materialName}"
            );

            string diffuseTexturePath =
                FindDiffuseTexture(
                    modelsDirectory,
                    materialName
                );

            string normalTexturePath =
                FindNormalTexture(
                    modelsDirectory,
                    materialName
                );

            if (diffuseTexturePath == null &&
                normalTexturePath == null)
            {
                Debug.LogWarning(
                    $"Textures not found for '{materialName}'.\n"
                );

                skipped++;
                continue;
            }

            Texture2D diffuseTexture = LoadDiffuseTexture(ref diffuseTexturePath);
            Texture2D normalTexture = LoadAndConfigureNormalTexture(ref normalTexturePath);

            if (diffuseTexture == null &&
                normalTexture == null)
            {
                skipped++;
                continue;
            }

            Material material;
            bool shaderLitFound = CreateMaterial(modelsDirectory, ref created, ref updated, materialName, out material);
            if (!shaderLitFound)
            {
                return;
            }

            AssignDiffuseTexture(materialName, diffuseTexturePath, diffuseTexture, material);
            AssignNormalTexture(materialName, normalTexturePath, normalTexture, material);

            EditorUtility.SetDirty(material);


            ModelImporter importer =
                AssetImporter.GetAtPath(fbxPath)
                as ModelImporter;

            if (importer != null)
            {
                importer.AddRemap(
                    new AssetImporter.SourceAssetIdentifier(
                        typeof(Material),
                        materialName
                    ),
                    material
                );

                EditorUtility.SetDirty(importer);
            }
        }

        AssetDatabase.SaveAssets();

        AssetDatabase.ImportAsset(
            fbxPath,
            ImportAssetOptions.ForceUpdate
        );

        Debug.Log(
            $"Finished.\n\n" +
            $"Created: {created}\n" +
            $"Updated: {updated}\n" +
            $"Skipped: {skipped}"
        );
    }

    private static Texture2D LoadAndConfigureNormalTexture(ref string normalTexturePath)
    {
        Texture2D normalTexture = null;

        if (normalTexturePath != null)
        {
            ConfigureNormalMapTexture(normalTexturePath);

            normalTexture =
                AssetDatabase.LoadAssetAtPath<Texture2D>(
                    normalTexturePath
                );

            if (normalTexture == null)
            {
                Debug.LogWarning(
                    $"Could not load normal texture:\n{normalTexturePath}"
                );

                normalTexturePath = null;
            }
        }

        return normalTexture;
    }

    private static Texture2D LoadDiffuseTexture(ref string diffuseTexturePath)
    {
        Texture2D diffuseTexture = null;

        if (diffuseTexturePath != null)
        {
            diffuseTexture =
                AssetDatabase.LoadAssetAtPath<Texture2D>(
                    diffuseTexturePath
                );

            if (diffuseTexture == null)
            {
                Debug.LogWarning(
                    $"Could not load texture:\n{diffuseTexturePath}"
                );

                diffuseTexturePath = null;
            }
        }

        return diffuseTexture;
    }

    private static void AssignNormalTexture(string materialName, string normalTexturePath, Texture2D normalTexture, Material material)
    {
        if (normalTexture != null &&
                        material.HasProperty("_NormalMap"))
        {
            material.SetTexture(
                "_NormalMap",
                normalTexture
            );

            if (material.HasProperty("_NormalScale"))
            {
                material.SetFloat(
                    "_NormalScale",
                    1f
                );
            }

            Debug.Log(
                $"Assigned Normal Map:\n" +
                $"{materialName} → {normalTexturePath}"
            );
        }
    }

    private static void AssignDiffuseTexture(string materialName, string diffuseTexturePath, Texture2D diffuseTexture, Material material)
    {
        if (diffuseTexture != null &&
                        material.HasProperty("_BaseColorMap"))
        {
            material.SetTexture(
                "_BaseColorMap",
                diffuseTexture
            );

            material.SetColor(
                "_BaseColor",
                Color.white
            );

            Debug.Log(
                $"Assigned Base Color:\n" +
                $"{materialName} → {diffuseTexturePath}"
            );
        }
    }

    private static bool CreateMaterial(string modelsDirectory, ref int created, ref int updated, string materialName, out Material material)
    {
        string materialPath =
                        $"{modelsDirectory}/Materials/{materialName}.mat";

        material = AssetDatabase.LoadAssetAtPath<Material>(
                materialPath
            );
        if (material == null)
        {
            Shader shader =
                Shader.Find("HDRP/Lit");

            if (shader == null)
            {
                Debug.LogError(
                    "HDRP/Lit shader not found."
                );

                return false;
            }

            material =
                new Material(shader);

            material.name =
                materialName;

            AssetDatabase.CreateAsset(
                material,
                materialPath
            );

            created++;

            Debug.Log(
                $"Created: {materialPath}"
            );
        }
        else
        {
            updated++;

            Debug.Log(
                $"Updating: {materialPath}"
            );
        }

        return true;
    }

    private static string GetSelectedFBXPath()
    {
        Object[] selectedObjects =
            Selection.objects;

        if (selectedObjects == null ||
            selectedObjects.Length != 1)
        {
            return null;
        }

        Object selected =
            selectedObjects[0];

        string path =
            AssetDatabase.GetAssetPath(selected);

        if (string.IsNullOrEmpty(path))
            return null;

        path =
            path.Replace("\\", "/");

        if (!path.EndsWith(
                ".fbx",
                System.StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        return path;
    }

    private static string FindDiffuseTexture(
     string modelsDirectory,
     string materialName)
    {

        string[] suffixes =
        {
            " Color", " Diffuse", ""
        };

        foreach (string suffix in suffixes)
        {
            string path = $"{modelsDirectory}/textures/{materialName}{suffix}.png";
            if (File.Exists(path))
            {
                Debug.Log(
                    $"Found diffuse texture in textures directory: {path}"
                );

                return path;
            }
        }

        Debug.Log($"Did not find diffuse texture in models/textures directory for {materialName}");
        return null;
    }

    private static string FindNormalTexture(
        string modelsDirectory,
        string materialName)
    {


            string[] files =
                Directory.GetFiles(
                    $"{modelsDirectory}/Textures"
                );

            foreach (string file in files)
            {
                string fileName =
                    Path.GetFileName(file);

                if (fileName.Equals($"{materialName} Normal.png"))
                {
                    Debug.Log(
                        $"Found normal texture: {file}"
                    );
                    return file.Replace("\\", "/");
                }

            }
        Debug.Log(
                $"Did not find normal texture for {materialName} in models/textures directory"
            );
        return null;
    }

    private static void ConfigureNormalMapTexture(
        string texturePath)
    {
        TextureImporter importer =
            AssetImporter.GetAtPath(texturePath)
            as TextureImporter;

        if (importer == null)
            return;

        importer.textureType =
            TextureImporterType.NormalMap;

        importer.sRGBTexture = false;

        EditorUtility.SetDirty(importer);
        importer.SaveAndReimport();

        Debug.Log(
            $"Configured normal map texture: {texturePath}"
        );
    }
}