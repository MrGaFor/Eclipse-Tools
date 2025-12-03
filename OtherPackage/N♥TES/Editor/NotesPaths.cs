using UnityEditor;
using System.IO;

public static class NotesPaths
{
    public static string NoteCachePath
    {
        get
        {
            string[] guids = AssetDatabase.FindAssets("t:Folder N♥TES");
            if (guids.Length == 0)
            {
                // Если папки нет — создаем её в Assets
                AssetDatabase.CreateFolder("Assets", "N♥TES");
                guids = AssetDatabase.FindAssets("t:Folder N♥TES");
            }

            string folderPath = AssetDatabase.GUIDToAssetPath(guids[0]);
            string editorFolder = Path.Combine(folderPath, "Editor");

            if (!Directory.Exists(editorFolder))
                Directory.CreateDirectory(editorFolder);

            return Path.Combine(editorFolder, "NoteCache.json");
        }
    }
}
