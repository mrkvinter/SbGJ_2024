using System.Linq;
using RG.ContentSystem.Core.Constants;
using RG.ContentSystem.UnityAdapter;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace RG.ContentSystem.Editor.Drawers
{
    public abstract class BaseScriptableContentObjectDrawer : OdinEditor
    {
        public bool SimpleMode { get; set; }

        public override void OnInspectorGUI()
        {
            var scriptableContentObject = (BaseScriptableSourceContentObject)serializedObject.targetObject;

            if (!SimpleMode)
            {
                GUI.enabled = false;
                SirenixEditorFields.UnityObjectField(GUIContent.none, scriptableContentObject,
                    typeof(BaseScriptableSourceContentObject), false);
                GUI.enabled = true;
            }

            GUILayout.Space(10);

            base.OnInspectorGUI();

            GUILayout.Space(10);

            if (!SimpleMode && scriptableContentObject.ContentType.GetCustomAttributes(true)
                    .Any(e => e is WithConstantsAttribute) &&
                GUILayout.Button("Generate types"))
            {
                GenerateConstants(scriptableContentObject);
            }
        }

        protected void GenerateConstants(BaseScriptableSourceContentObject scriptableContentObject)
        {
            var constsGenerator = new EntryConstantsClassGenerator(scriptableContentObject);
            constsGenerator.Generate();
        }
    }
}