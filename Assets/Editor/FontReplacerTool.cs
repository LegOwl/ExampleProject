using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Tools
{
    public class FontReplacerTool : EditorWindow
    {
        private Font _uiFont;
        private TMP_FontAsset _tmpFont;

        private bool _processScenes = true;
        private bool _processPrefabs = true;

        [MenuItem("Tools/Font Replacer")]
        public static void ShowWindow()
        {
            GetWindow<FontReplacerTool>("Font Replacer");
        }

        private void OnGUI()
        {
            GUILayout.Label("Font Replacement Tool", EditorStyles.boldLabel);

            _uiFont = (Font)EditorGUILayout.ObjectField("UI Font (Legacy)", _uiFont, typeof(Font), false);
            _tmpFont = (TMP_FontAsset)EditorGUILayout.ObjectField("TMP Font", _tmpFont, typeof(TMP_FontAsset), false);

            GUILayout.Space(10);

            _processScenes = EditorGUILayout.Toggle("Process Open Scene", _processScenes);
            _processPrefabs = EditorGUILayout.Toggle("Process Prefabs (Project)", _processPrefabs);

            GUILayout.Space(10);

            if (GUILayout.Button("Replace Fonts"))
            {
                ReplaceFonts();
            }
        }

        private void ReplaceFonts()
        {
            if (_processScenes)
            {
                ReplaceInScene();
            }

            if (_processPrefabs)
            {
                ReplaceInPrefabs();
            }

            Debug.Log("Font replacement complete.");
        }

        private void ReplaceInScene()
        {
            var allTexts = GameObject.FindObjectsOfType<Text>(true);
            foreach (var text in allTexts)
            {
                if (_uiFont != null)
                {
                    Undo.RecordObject(text, "Replace UI Font");
                    text.font = _uiFont;
                    EditorUtility.SetDirty(text);
                }
            }

            var allTMP = GameObject.FindObjectsOfType<TMP_Text>(true);
            foreach (var tmp in allTMP)
            {
                if (_tmpFont != null)
                {
                    Undo.RecordObject(tmp, "Replace TMP Font");
                    tmp.font = _tmpFont;
                    EditorUtility.SetDirty(tmp);
                }
            }
        }

        private void ReplaceInPrefabs()
        {
            var guids = AssetDatabase.FindAssets("t:Prefab");

            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

                if (prefab == null) continue;

                var instance = PrefabUtility.LoadPrefabContents(path);

                bool changed = false;

                var texts = instance.GetComponentsInChildren<Text>(true);
                foreach (var text in texts)
                {
                    if (_uiFont != null && text.font != _uiFont)
                    {
                        text.font = _uiFont;
                        changed = true;
                    }
                }

                var tmps = instance.GetComponentsInChildren<TMP_Text>(true);
                foreach (var tmp in tmps)
                {
                    if (_tmpFont != null && tmp.font != _tmpFont)
                    {
                        tmp.font = _tmpFont;
                        changed = true;
                    }
                }

                if (changed)
                {
                    PrefabUtility.SaveAsPrefabAsset(instance, path);
                }

                PrefabUtility.UnloadPrefabContents(instance);
            }
        }
    }
}