using RG.ContentSystem.Core;
using RG.ContentSystem.UnityAdapter;
using Sirenix.OdinInspector.Editor;
using UnityEditor;

namespace RG.ContentSystem.Editor
{
    public sealed class ContentSelector<TContent> : OdinSelector<ContentRef<TContent>>
        where TContent : ContentEntry
    {
        protected override void BuildSelectionTree(OdinMenuTree tree)
        {
            var contentObjects = ContentDatabaseUtility.GetSourceContentObjects<TContent>();
            foreach (var contentObject in contentObjects)
            {
                if (contentObject is ScriptableContentObject<TContent> scriptableContentObject)
                {
                    var path = AssetDatabase.GetAssetPath(scriptableContentObject);
                    tree.Add(path, new ContentRef<TContent>(scriptableContentObject.ContentId));
                    continue;
                }

                if (contentObject is BaseScriptableTableContentObject<TContent> tableContentObject)
                {
                    const string prefixToRemove = "Assets/_GAME/";
                    var rootPath = AssetDatabase.GetAssetPath(tableContentObject);
                    if (rootPath.StartsWith(prefixToRemove))
                        rootPath = rootPath[prefixToRemove.Length..];
                    rootPath = rootPath.Replace(".asset", string.Empty);

                    for (var i = 0; i < tableContentObject.ContentObjects.Length; i++)
                    {
                        var content = tableContentObject.ContentObjects[i];
                        var path = $"{rootPath}/{content.Id}";
                        tree.Add(path, new ContentRef<TContent>(content.Id));
                    }
                    continue;
                }
            }
        }
    }
}