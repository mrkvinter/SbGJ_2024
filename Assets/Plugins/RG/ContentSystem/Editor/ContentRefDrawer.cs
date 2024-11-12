using System;
using System.Linq;
using RG.ContentSystem.Core;
using RG.ContentSystem.Editor.Drawers;
using RG.ContentSystem.UnityAdapter;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace RG.ContentSystem.Editor
{
    public sealed class ContentRefDrawer<TContent> : OdinValueDrawer<ContentRef<TContent>>
        where TContent : ContentEntry
    {
        private BaseScriptableContentObject selectedContentObject;
        private Type scriptableObjectType;
        private UnityEditor.Editor contentEditor;
        private bool showContentEditor;

        private ContentSelector<TContent> selector;
        private ContentAssetType? assetType;

        protected override void DrawPropertyLayout(GUIContent label)
        {
            assetType ??= ContentDatabaseUtility.GetAssetType<TContent>();

            switch (assetType)
            {
                case ContentAssetType.None:
                    SirenixEditorGUI.ErrorMessageBox("No content object found for type " + typeof(TContent).Name);
                    return;
                case ContentAssetType.TableContentObject:
                    DrawTableContentObject(label);
                    return;
                case ContentAssetType.ScriptableContentObject:
                    DrawScriptableContentObject(label);
                    return;
            }
        }

        private void DrawTableContentObject(GUIContent label)
        {
            if (selector == null)
            {
                selector = new ContentSelector<TContent>();
                selector.EnableSingleClickToSelect();
                selector.SetSelection(ValueEntry.SmartValue.Id);
            }

            var result = selector.GetCurrentSelection().FirstOrDefault();
            if (result.Id != ValueEntry.SmartValue.Id)
            {
                selector.SetSelection(ValueEntry.SmartValue.Id);
                result = ValueEntry.SmartValue;
            }

            var controlRect = EditorGUILayout.GetControlRect();

            GUILayout.BeginHorizontal();

            var labelWidth = label != GUIContent.none ? GUIHelper.BetterLabelWidth : 15;
            var rect = new Rect(controlRect.x + labelWidth, controlRect.y, controlRect.width - labelWidth,
                controlRect.height);

            label ??= GUIContent.none;
            var value = result;
            var text = string.IsNullOrEmpty(value.Id) ? "None" : value.Id;
            if (EditorGUI.DropdownButton(rect, new GUIContent(text), FocusType.Keyboard))
            {
                selector.SelectionConfirmed += selection =>
                {
                    var content = selection.FirstOrDefault();
                    ValueEntry.SmartValue = content;
                    selectedContentObject = GetContentObject(content.Id);
                };
                selector.OnInspectorGUI();
                selector.ShowInPopup();
            }

            EditorGUI.LabelField(controlRect, label);
            GUILayout.EndHorizontal();
        }

        private void DrawScriptableContentObject(GUIContent label)
        {
            var value = ValueEntry.SmartValue;

            var controlRect = EditorGUILayout.GetControlRect();

            label ??= GUIContent.none;

            if (scriptableObjectType == null)
            {
                scriptableObjectType = GetScriptableObjectType();
            }

            if (selectedContentObject == null && !string.IsNullOrEmpty(value.Id) || 
                selectedContentObject != null && selectedContentObject.ContentId != value.Id)
            {
                selectedContentObject = GetContentObject(value.Id);
            }

            EditorGUI.BeginChangeCheck();
            var labelWidth = label != GUIContent.none ? GUIHelper.BetterLabelWidth : 15;
            var rect = new Rect(controlRect.x + labelWidth, controlRect.y, controlRect.width - labelWidth,
                controlRect.height);

            var selected = EditorGUI
                    .ObjectField(rect, GUIContent.none, selectedContentObject, scriptableObjectType, false) as
                BaseScriptableContentObject;

            if (selected != selectedContentObject)
            {
                selectedContentObject = selected;
                var contentRef = new ContentRef<TContent>(selectedContentObject != null
                    ? selectedContentObject.ContentId
                    : string.Empty);
                ValueEntry.SmartValue = contentRef;

                UnityEngine.Object.DestroyImmediate(contentEditor);
                contentEditor = null;
            }

            UpdateContentEditor();
            showContentEditor = SirenixEditorGUI.Foldout(controlRect, showContentEditor && contentEditor != null,
                label);
            if (SirenixEditorGUI.BeginFadeGroup(this, showContentEditor) && contentEditor != null)
            {
                GUILayout.BeginVertical(EditorStyles.helpBox);
                contentEditor.OnInspectorGUI();
                GUILayout.EndVertical();
            }

            SirenixEditorGUI.EndFadeGroup();
            EditorGUI.EndChangeCheck();
        }

        private void UpdateContentEditor()
        {
            if (selectedContentObject == null && contentEditor != null)
            {
                UnityEngine.Object.DestroyImmediate(contentEditor);
                contentEditor = null;
            }
            else if (selectedContentObject != null && contentEditor == null)
            {
                contentEditor = UnityEditor.Editor.CreateEditor(selectedContentObject);
                if (contentEditor is ScriptableContentObjectDrawer scriptableContentObjectDrawer)
                {
                    scriptableContentObjectDrawer.SimpleMode = true;
                }
            }
        }

        private BaseScriptableContentObject GetContentObject(string id)
        {
            var guids = AssetDatabase.FindAssets($"t:{nameof(BaseScriptableContentObject)}");
            var assets = guids.Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<BaseScriptableContentObject>).ToList();
            var filteredAssets = assets
                .Where(e => e != null && e.ContentId == id && e.ContentType == typeof(TContent)).ToList();

            if (filteredAssets.Count > 1)
            {
                Debug.LogError($"Multiple content objects with id {id} found. [ {string.Join(", ", filteredAssets.Select(e => e.name))} ]");
                return null;
            }

            return filteredAssets.FirstOrDefault();
        }

        private Type GetScriptableObjectType()
        {
            var objectType = typeof(ScriptableContentObject<>).MakeGenericType(typeof(TContent));
            var type = AppDomain.CurrentDomain.GetAssemblies().SelectMany(e => e.GetTypes())
                .FirstOrDefault(e => e.IsSubclassOf(objectType));

            return type;
        }
        
        // private Type GetScriptableObjectType()
        // {
        //     var objectType = typeof(BaseScriptableContentObject); //.MakeGenericType(typeof(TContent));
        //     var type = AppDomain.CurrentDomain.GetAssemblies().SelectMany(e => e.GetTypes())
        //         .Where(t =>
        //         {
        //             if (! (t.IsClass && !t.IsAbstract) || !objectType.IsAssignableFrom(typeof(BaseScriptableContentObject)))
        //                 return false;
        //           
        //             var dummy = (BaseScriptableContentObject)Activator.CreateInstance(t);
        //             return dummy.ContentType == typeof(TContent);
        //             // .FirstOrDefault(e => e.IsSubclassOf(objectType));
        //         };
        //
        //     return type;
        // }
    }
}