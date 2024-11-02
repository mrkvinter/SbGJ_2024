using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RG.ContentSystem.Core;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RG.ContentSystem.UnityAdapter
{
    [CreateAssetMenu(fileName = "ContentDatabase", menuName = "Content/ContentDatabase")]
    public sealed class ContentDatabase : ScriptableObject
    {
        [field: SerializeReference, ReadOnly] public BaseScriptableSourceContentObject[] Contents { get; private set; }

#if UNITY_EDITOR
        [Button(ButtonSizes.Medium)]
        public void Populate()
        {
            var directory = new FileInfo(UnityEditor.AssetDatabase.GetAssetPath(this)).Directory;
            var contentPaths = directory!.GetDirectories().SelectMany(GetAllFiles)
                .Select(e =>
                    $"Assets/{e.FullName.Replace('\\', '/').Replace(Application.dataPath, string.Empty)}")
                .ToHashSet();

            var usedId = new HashSet<Key>();
            var contents = new List<BaseScriptableSourceContentObject>();
            foreach (var contentPath in contentPaths)
            {
                var content = UnityEditor.AssetDatabase.LoadAssetAtPath<BaseScriptableSourceContentObject>(contentPath);
                if (content == null)
                    continue;

                if (content is IScriptableContentObject<ContentEntry> scriptableContentObject)
                {
                    if (string.IsNullOrEmpty(scriptableContentObject.ContentObject.Id))
                    {
                        Debug.LogError($"Content object {contentPath} has no id", content);
                        return;
                    }

                    var key = new Key(scriptableContentObject.ContentObject.Id, scriptableContentObject.ContentType);
                    if (usedId.Contains(key))
                    {
                        Debug.LogError(
                            $"Content object {contentPath} has duplicate id {scriptableContentObject.ContentObject.Id}", content);
                        return;
                    }

                    usedId.Add(key);
                    contents.Add(content);
                    continue;
                }

                if (content is IScriptableSourceContentObjects<ContentEntry> scriptableSourceContentObjects)
                {
                    foreach (var contentObject in scriptableSourceContentObjects.ContentObjects)
                    {
                        if (string.IsNullOrEmpty(contentObject.Id))
                        {
                            Debug.LogError($"Content object {contentPath} has no id", content);
                            return;
                        }

                        var key = new Key(contentObject.Id, scriptableSourceContentObjects.ContentType);
                        if (usedId.Contains(key))
                        {
                            Debug.LogError(
                                $"Content object {contentPath} has duplicate id {contentObject.Id}", content);
                            return;
                        }

                        usedId.Add(key);
                    }
                    
                    contents.Add(content);
                    continue;
                }
            }

            Contents = contents.ToArray();
        }

        private IEnumerable<FileInfo> GetAllFiles(DirectoryInfo directory)
        {
            foreach (var file in directory.GetFiles())
            {
                yield return file;
            }

            foreach (var subDirectory in directory.GetDirectories())
            {
                foreach (var file in GetAllFiles(subDirectory))
                {
                    yield return file;
                }
            }
        }
        
        private struct Key : IEquatable<Key>
        {
            public string Id;
            public Type ContentType;

            public Key(string id, Type contentType)
            {
                Id = id;
                ContentType = contentType;
            }

            public bool Equals(Key other)
            {
                return Id == other.Id && Equals(ContentType, other.ContentType);
            }

            public override bool Equals(object obj)
            {
                return obj is Key other && Equals(other);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(Id, ContentType);
            }
        }
#endif
    }
}