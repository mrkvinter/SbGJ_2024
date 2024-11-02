using System.Collections.Generic;
using RG.ContentSystem.Core;

namespace RG.ContentSystem.UnityAdapter
{
    public interface IContentMapConfigurator
    {
        IContentMap Configure(IReadOnlyCollection<BaseScriptableSourceContentObject> contents);
        IContentMap Configure(IReadOnlyCollection<IContentMap> maps);
    }

    public class ContentMapConfigurator<T> : IContentMapConfigurator
        where T : ContentEntry
    {
        public IContentMap Configure(IReadOnlyCollection<BaseScriptableSourceContentObject> contents)
        {
            var result = new List<T>();
            foreach (var sourceContent in contents)
            {
                if (sourceContent == null)
                    throw new System.Exception("Source content is null");

                if (sourceContent is IScriptableContentObject<T> scriptableContentObject)
                    result.Add(scriptableContentObject.ContentObject);

                if (sourceContent is IScriptableSourceContentObjects<T> scriptableSourceContentObjects)
                    result.AddRange(scriptableSourceContentObjects.ContentObjects);
            }

            return new ContentMap<T>(result);
        }

        public IContentMap Configure(IReadOnlyCollection<IContentMap> maps)
        {
            var result = new List<T>();
            foreach (var map in maps)
            {
                if (map is IContentMap<T> casted)
                    result.AddRange(casted.ContentEntries);
            }

            return new ContentMap<T>(result);
        }
    }
}