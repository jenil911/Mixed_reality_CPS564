using Unity.Services.Deployment.Editor.Interface.UI.Events;
using UnityEditor;
using UnityEngine.UIElements;
using Toggle = UnityEngine.UIElements.Toggle;

namespace Unity.Services.Deployment.Editor.Interface.UI.Components
{
    class Selectable : Toggle
    {
        const string k_TemplatePath = "Packages/com.unity.services.deployment/Editor/Interface/UI/Assets/Selectable.uss";

        public Selectable()
        {
            styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>(k_TemplatePath));
            RegisterCallback<PointerDownEvent>(e =>
            {
                if (e.button == 1)
                {
                    e.StopPropagation();
                    RightClickedEvent.Send(this);
                }
            });
        }

        new class UxmlFactory : UxmlFactory<Selectable> {}
    }
}
