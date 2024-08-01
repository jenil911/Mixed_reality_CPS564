using System;
using System.IO;
using System.Linq;
using Unity.Services.Tooling.Editor.AccessControl.Authoring.Model;
using UnityEditor;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace Unity.Services.Tooling.Editor.AccessControl.Authoring.UI
{
    [CustomEditor(typeof(AccessControlAsset))]
    class AccessControlResourceInspector : UnityEditor.Editor
    {
        const int k_MaxLines = 75;
        const string k_Template = "Packages/com.unity.services.tooling/Editor/AccessControl/Authoring/UI/Assets/AccessControlResourceInspector.uxml";

        public override VisualElement CreateInspectorGUI()
        {
            var myInspector = new VisualElement();
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(k_Template);
            visualTree.CloneTree(myInspector);

            ShowResourceBody(myInspector);

            return myInspector;
        }

        void ShowResourceBody(VisualElement myInspector)
        {
            var body = myInspector.Q<TextField>();
            if (targets.Length == 1)
            {
                body.visible = true;
                body.value = ReadResourceBody(targets[0]);
            }
            else
            {
                body.visible = false;
            }
        }

        static string ReadResourceBody(Object resource)
        {
            var path = AssetDatabase.GetAssetPath(resource);
            var lines = File.ReadLines(path).Take(k_MaxLines).ToList();
            if (lines.Count == k_MaxLines)
            {
                lines.Add("...");
            }
            return string.Join(Environment.NewLine, lines);
        }
    }
}
