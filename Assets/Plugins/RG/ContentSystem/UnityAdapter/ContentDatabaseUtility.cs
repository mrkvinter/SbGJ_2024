#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using RG.ContentSystem.Core;
using UnityEngine;

namespace RG.ContentSystem.UnityAdapter
{
    public static class ContentDatabaseUtility
    {
        private static bool initialized;
        private static ContentDatabase contentDatabase;

        private static void Initialize()
        {
            if (initialized)
                return;

            initialized = true;

            contentDatabase = AssetDatabaseUtility.GetAllAssetsOfType<ContentDatabase>().FirstOrDefault();

            if (contentDatabase == null)
            {
                Debug.LogError("No content database found");
            }
        }

        public static List<BaseScriptableSourceContentObject> GetSourceContentObjects<T>() where T : ContentEntry
        {
            Initialize();

            var contentObjects = new List<BaseScriptableSourceContentObject>();
            foreach (var contentObject in contentDatabase.Contents)
            {
                if (contentObject.ContentType == typeof(T))
                    contentObjects.Add(contentObject);
            }

            return contentObjects;
        }
        
        public static List<T> GetContentObjects<T>() where T : ContentEntry
        {
            Initialize();

            var contentObjects = new List<T>();
            foreach (var contentObject in contentDatabase.Contents)
            {
                if (contentObject.ContentType == typeof(T))
                {
                    if (contentObject is ScriptableContentObject<T> scriptableContentObject)
                    {
                        contentObjects.Add(scriptableContentObject.ContentObject);
                        continue;
                    }
                    
                    if (contentObject is BaseScriptableTableContentObject<T> tableContentObject)
                    {
                        contentObjects.AddRange(tableContentObject.ContentObjects);
                        continue;
                    }
                }
            }

            return contentObjects;
        }

        public static List<ContentEntry> GetContentObjects(Type type)
        {
            Initialize();

            var contentObjects = new List<ContentEntry>();
            foreach (var contentObject in contentDatabase.Contents)
            {
                if (contentObject.ContentType == type)
                {
                    switch (contentObject)
                    {
                        case IScriptableContentObject<ContentEntry> scriptableContentObject:
                            contentObjects.Add(scriptableContentObject.ContentObject);
                            break;
                        case IScriptableSourceContentObjects<ContentEntry> tableContentObject:
                            contentObjects.AddRange(tableContentObject.ContentObjects);
                            break;
                    }
                }
            }

            return contentObjects;
        }

        public static T GetContentObject<T>(ContentRef<T> contentRef) where T : ContentEntry
        {
            Initialize();

            foreach (var contentObject in contentDatabase.Contents)
            {
                if (contentObject.ContentType == typeof(T))
                {
                    if (contentObject is ScriptableContentObject<T> scriptableContentObject 
                        && scriptableContentObject.ContentId == contentRef.Id)
                    {
                        return scriptableContentObject.ContentObject;
                    }
                    
                    if (contentObject is BaseScriptableTableContentObject<T> tableContentObject)
                    {
                        foreach (var contentEntry in tableContentObject.ContentObjects)
                        {
                            if (contentEntry.Id == contentRef.Id)
                                return contentEntry;
                        }
                    }
                }
            }

            return null;
        }

        public static ContentEntry GetContentObject(string id)
        {
            Initialize();

            foreach (var contentObject in contentDatabase.Contents)
            {
                if (contentObject is IScriptableContentObject<ContentEntry> scriptableContentObject 
                    && scriptableContentObject.ContentObject.Id == id)
                    return scriptableContentObject.ContentObject;
                
                if (contentObject is IScriptableSourceContentObjects<ContentEntry> tableContentObject)
                {
                    foreach (var contentEntry in tableContentObject.ContentObjects)
                    {
                        if (contentEntry.Id == id)
                            return contentEntry;
                    }
                }
            }

            return null;
        }

        public static ContentAssetType GetAssetType<T>() where T : ContentEntry
        {
            var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(e => e.GetTypes()).ToList();

            var objectType = typeof(ScriptableContentObject<>).MakeGenericType(typeof(T));
            if (types.Any(e => e.IsSubclassOf(objectType)))
                return ContentAssetType.ScriptableContentObject;

            objectType = typeof(BaseScriptableTableContentObject<>).MakeGenericType(typeof(T));
            if (types.Any(e => e.IsSubclassOf(objectType)))
                return ContentAssetType.TableContentObject;

            return ContentAssetType.None;
        }
    }

    public enum ContentAssetType
    {
        None,
        ScriptableContentObject,
        TableContentObject
    }
}
#endif