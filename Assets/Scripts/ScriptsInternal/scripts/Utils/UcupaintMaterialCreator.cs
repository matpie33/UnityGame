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

        string directory =
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
                $"Found material in file: {materialName}"
            );

            string texturePath =
                FindTexture(
                    directory,
                    materialName
                );

            if (texturePath == null)
            {
                Debug.LogWarning(
                    $"Texture not found for '{materialName}'.\n" +
                    $"Expected:\n" +
                    $"{directory}/{materialName}.png"
                );

                skipped++;
                continue;
            }

            Texture2D texture =
                AssetDatabase.LoadAssetAtPath<Texture2D>(
                    texturePath
                );

            if (texture == null)
            {
                Debug.LogWarning(
                    $"Could not load texture:\n{texturePath}"
                );

                skipped++;
                continue;
            }

            string materialPath =
                $"{directory}/Materials/{materialName}.mat";

            Material material =
                AssetDatabase.LoadAssetAtPath<Material>(
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

                    return;
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

            if (material.HasProperty("_BaseColorMap"))
            {
                material.SetTexture(
                    "_BaseColorMap",
                    texture
                );

                material.SetColor(
                    "_BaseColor",
                    Color.white
                );
            }

            EditorUtility.SetDirty(material);

            Debug.Log(
                $"Assigned Base Color:\n" +
                $"{materialName} → {texturePath}"
            );


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

    private static string FindTexture(
     string directory,
     string materialName)
    {
        string fileName = materialName + ".png";

        string path = $"{directory}/{fileName}";

        if (File.Exists(path))
        {
            Debug.Log(
                $"Found texture next to FBX: {path}"
            );

            return path;
        }

        string texturesDirectory =
            $"{directory}/Textures";

        path =
            $"{texturesDirectory}/{fileName}";

        if (File.Exists(path))
        {
            Debug.Log(
                $"Found texture in Textures folder: {path}"
            );

            return path;
        }

        string[] extensions =
        {
        ".jpg",
        ".jpeg",
        ".tga",
        ".tif",
        ".tiff",
        ".exr"
    };

        foreach (string extension in extensions)
        {
            // Same directory
            path =
                $"{directory}/{materialName}{extension}";

            if (File.Exists(path))
                return path;

            // Textures directory
            path =
                $"{texturesDirectory}/{materialName}{extension}";

            if (File.Exists(path))
                return path;
        }

        return null;
    }

}