using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using System.Linq;
using RG.ContentSystem.Core.Constants;
using RG.ContentSystem.UnityAdapter;
using UnityEditor;
using UnityEngine;

namespace RG.ContentSystem.Editor
{
    public sealed class EntryConstantsClassGenerator
    {
        private readonly BaseScriptableSourceContentObject sourceContentObject;

        public EntryConstantsClassGenerator(BaseScriptableSourceContentObject sourceContentObject)
        {
            this.sourceContentObject = sourceContentObject;
        }

        public void Generate()
        {
            var contentType = sourceContentObject.ContentType;
            var script = AssetDatabase
                .FindAssets("t:script")
                .Select(AssetDatabase.GUIDToAssetPath)
                .FirstOrDefault(e => Path.GetFileNameWithoutExtension(e) == contentType.Name);

            if (script == null)
            {
                Debug.LogError(
                    $"Script file for {contentType.Name} not found. Check if it exists and has the same name as the content type.");
                return;
            }

            var className = EntryConstantsUtility.GetEntryName(sourceContentObject.ContentType);
            var path = $"{Path.GetDirectoryName(script)}\\{className}.cs";
            var provider = CodeDomProvider.CreateProvider("CSharp");

            using var file = File.CreateText(path);
            var codeUnit = GetClassConstants(sourceContentObject.ContentType, className);
            provider.GenerateCodeFromCompileUnit(codeUnit, file, new CodeGeneratorOptions());
            AssetDatabase.Refresh();
        }

        private CodeCompileUnit GetClassConstants(Type contentType, string className)
        {
            var compileUnit = new CodeCompileUnit();

            var codeNamespace = new CodeNamespace(contentType.Namespace);
            compileUnit.Namespaces.Add(codeNamespace);

            var entries = ContentDatabaseUtility.GetContentObjects(contentType);

            var constantsClass = new CodeTypeDeclaration(className);
            constantsClass.IsClass = true;
            constantsClass.Attributes = MemberAttributes.Public | MemberAttributes.Static | MemberAttributes.Final;
            constantsClass.TypeAttributes =
                System.Reflection.TypeAttributes.Public | System.Reflection.TypeAttributes.Sealed;
            codeNamespace.Types.Add(constantsClass);

            foreach (var entry in entries)
            {
                var field = new CodeMemberField(typeof(string), IdToName(entry.Id));
                field.Attributes = MemberAttributes.Public | MemberAttributes.Const;
                field.InitExpression = new CodePrimitiveExpression(entry.Id);
                constantsClass.Members.Add(field);
            }

            return compileUnit;

            string IdToName(string id)
            {
                return id.Trim()
                    .Replace(" ", "")
                    .Replace("-", "");
            }
        }
    }
}