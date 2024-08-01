using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;

namespace Unity.Services.Deployment.Editor.Interface.UI.Components
{
    class Collapsible : VisualElement
    {
        const string k_StylePath = "Packages/com.unity.services.deployment/Editor/Interface/UI/Assets/Collapsible.uss";
        const string k_CollapsedClass = "collapsed";

        Toggle m_CollapseToggle;
        VisualElement m_Container;

        string ContainerClass { get; set; }
        public bool IsContentVisible => m_Container?.visible ?? false;

        public Collapsible()
        {
            styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>(k_StylePath));
            RegisterCallback<GeometryChangedEvent>(SetupElement);
        }

        void SetupElement(GeometryChangedEvent e)
        {
            m_Container = this.Q(className: ContainerClass);
            m_CollapseToggle = this.Q<CollapseToggle>();
            m_CollapseToggle.RegisterCallback<ChangeEvent<bool>>(e => OnToggleValueChanged(e.newValue));
            OnToggleValueChanged(m_CollapseToggle.value);

            UnregisterCallback<GeometryChangedEvent>(SetupElement);
        }

        void OnToggleValueChanged(bool newValue)
        {
            m_Container.visible = !newValue;

            if (!newValue)
            {
                m_Container.RemoveFromClassList(k_CollapsedClass);
                return;
            }

            m_Container.AddToClassList(k_CollapsedClass);
        }

        public void SetVisibility(bool toggle)
        {
            m_CollapseToggle.value = !toggle;
        }

        public class CollapsibleUxmlTraits : VisualElement.UxmlTraits
        {
            UxmlStringAttributeDescription m_ContainerName =
                new UxmlStringAttributeDescription { name = "containerClass", defaultValue = null };

            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }

            public override void Init(VisualElement element, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(element, bag, cc);
                var collapsible = element as Collapsible;
                collapsible.ContainerClass = m_ContainerName.GetValueFromBag(bag, cc);
            }
        }

        new class UxmlFactory : UxmlFactory<Collapsible, CollapsibleUxmlTraits> {}
    }
}
