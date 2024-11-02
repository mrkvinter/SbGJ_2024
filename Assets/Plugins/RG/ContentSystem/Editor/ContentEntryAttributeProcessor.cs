using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RG.ContentSystem.Core;
using RG.ContentSystem.UnityAdapter;
using Sirenix.OdinInspector.Editor;
using UnityEngine;

namespace RG.ContentSystem.Editor
{
    public sealed class ContentEntryAttributeProcessor : OdinAttributeProcessor<ContentEntry>
    {
        private BaseScriptableContentObject mock;

        public override void ProcessChildMemberAttributes(InspectorProperty property, MemberInfo memberInfo, List<Attribute> attributes)
        {
            base.ProcessChildMemberAttributes(property, memberInfo, attributes);

            var parentType = property.BaseValueEntry.ParentType;
            if (!typeof(BaseScriptableContentObject).IsAssignableFrom(parentType))
                return;
            
            if (mock == null || mock.GetType() != parentType)
                mock = (BaseScriptableContentObject) Activator.CreateInstance(parentType);

            if (!attributes.Any(e => e is SerializeField))
                return;

            var name = memberInfo.Name;
            if (name.EndsWith("k__BackingField"))
                name = name.Substring(1, name.Length - 17);

            if (mock is IProcessSelfAttributes processSelfAttributes)
                attributes.AddRange(processSelfAttributes.ProcessSelfAttributes(name));
        }
    }
}