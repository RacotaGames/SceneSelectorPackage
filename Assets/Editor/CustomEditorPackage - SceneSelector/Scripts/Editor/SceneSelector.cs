using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AzSimX.Editor
{
    [InitializeOnLoad]
    public static class SceneSelector
    {
        static string selectedScenePath;
        static List<string> scenePaths = new();
        static List<string> sceneNames = new();
        static int selectedIndex;

        const string PrefKey = "AzSimX.SceneSelector.SceneFolders"; // stored in EditorPrefs as JSON

        static SceneSelector()
        {
            EditorApplication.update += OnEditorUpdate;

            EditorSceneManager.sceneOpened += OnSceneOpened;
            EditorSceneManager.activeSceneChangedInEditMode += OnActiveSceneChanged;
        }

        static void OnEditorUpdate()
        {
            EditorApplication.update -= OnEditorUpdate;
            LoadScenes();
            ToolbarExtender.RightToolbarGUI.Add(DrawDropdown);

            UpdateDropdownToCurrentScene();
        }

        // ---- MENU ITEMS ----
        [MenuItem("Tools/Scene Selector/Add Scenes Folder...")]
        static void AddScenesFolder()
        {
            string path = EditorUtility.OpenFolderPanel("Select Scenes Folder", "Assets", "");
            if (!string.IsNullOrEmpty(path))
            {
                if (path.StartsWith(Application.dataPath))
                {
                    path = "Assets" + path.Substring(Application.dataPath.Length);
                }

                var folders = GetSceneFolders();
                if (!folders.Contains(path))
                {
                    folders.Add(path);
                    SaveSceneFolders(folders);
                    LoadScenes();
                }
            }
        }

        [MenuItem("Tools/Scene Selector/Clear Scene Folders")]
        static void ClearSceneFolders()
        {
            EditorPrefs.DeleteKey(PrefKey);
            LoadScenes();
        }

        // ---- PERSISTENCE ----
        static List<string> GetSceneFolders()
        {
            if (EditorPrefs.HasKey(PrefKey))
            {
                string json = EditorPrefs.GetString(PrefKey);
                return JsonUtility.FromJson<FolderList>(json)?.folders ?? new List<string>();
            }
            return new List<string>();
        }

        static void SaveSceneFolders(List<string> folders)
        {
            var wrapper = new FolderList { folders = folders };
            string json = JsonUtility.ToJson(wrapper);
            EditorPrefs.SetString(PrefKey, json);
        }

        [System.Serializable]
        class FolderList { public List<string> folders; }

        // ---- SCENE HANDLING ----
        static void OnSceneOpened(Scene scene, OpenSceneMode mode) => UpdateDropdownToCurrentScene();
        static void OnActiveSceneChanged(Scene prev, Scene next) => UpdateDropdownToCurrentScene();

        static void LoadScenes()
        {
            scenePaths.Clear();
            sceneNames.Clear();

            foreach (var folder in GetSceneFolders())
            {
                if (Directory.Exists(folder))
                {
                    var paths = Directory.GetFiles(folder, "*.unity", SearchOption.AllDirectories)
                        .Select(p => p.Replace('\\', '/'));
                    scenePaths.AddRange(paths);
                }
            }

            sceneNames = scenePaths.Select(Path.GetFileNameWithoutExtension).ToList();
            selectedScenePath = scenePaths.FirstOrDefault();
        }

        static void UpdateDropdownToCurrentScene()
        {
            var currentScene = SceneManager.GetActiveScene();
            if (!string.IsNullOrEmpty(currentScene.path))
            {
                int sceneIndex = scenePaths.FindIndex(path => path == currentScene.path);
                if (sceneIndex >= 0)
                {
                    selectedIndex = sceneIndex;
                }
            }
        }

        // ---- DRAW DROPDOWN + REFRESH ----
        static void DrawDropdown()
        {
            if (sceneNames.Count == 0)
                return;

            GUILayout.BeginHorizontal();

            int newIndex = EditorGUILayout.Popup(selectedIndex, sceneNames.ToArray(), GUILayout.Width(180));
            if (newIndex != selectedIndex)
            {
                selectedIndex = newIndex;
                if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    EditorSceneManager.OpenScene(scenePaths[selectedIndex]);
                }
            }

            if (GUILayout.Button("↻", GUILayout.Width(25))) // Refresh button
            {
                LoadScenes();
                UpdateDropdownToCurrentScene();
            }

            GUILayout.EndHorizontal();
        }
    }
}
