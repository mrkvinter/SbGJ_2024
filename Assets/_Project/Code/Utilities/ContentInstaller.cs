using System;
using System.Collections.Generic;
using System.Linq;
using RG.ContentSystem.Core;
using RG.ContentSystem.UnityAdapter;

namespace Code.Utilities
{
    public static class ContentInstaller
    {
        public static void Install(ContentDatabase contentDatabase)
        {
            var maps = new List<IContentMap>();
            var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(e => e.GetTypes()).ToList();
            var allContentTypes = types
                .Where(e => e.IsSubclassOf(typeof(ContentEntry)))
                .ToList();

            var contentTypes = allContentTypes.Where(e => !e.IsAbstract && !e.IsGenericType).ToList();
            var abstractContentTypes = allContentTypes.Where(e => e.IsAbstract && !e.IsGenericType).ToList();

            var configuratorType = typeof(ContentMapConfigurator<>);
            foreach (var contentType in contentTypes)
            {
                var configurator =
                    (IContentMapConfigurator)Activator.CreateInstance(configuratorType.MakeGenericType(contentType));
                var map = configurator.Configure(contentDatabase.Contents);
                maps.Add(map);
            }

            foreach (var contentType in abstractContentTypes)
            {
                var configurator =
                    (IContentMapConfigurator)Activator.CreateInstance(configuratorType.MakeGenericType(contentType));
                var map = configurator.Configure(maps);
                maps.Add(map);
            }

            var contentResolver = new ContentResolver(maps);
            ContentManager.Initialize(contentResolver);
        }
    }
}