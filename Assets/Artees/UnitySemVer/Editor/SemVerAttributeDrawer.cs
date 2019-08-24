using UnityEditor;
using UnityEngine;

namespace Artees.UnitySemVer.Editor
{
    [CustomPropertyDrawer(typeof(SemVerAttribute))]
    internal class SemVerAttributeDrawer : SemVerDrawer
    {
        public SemVerAttributeDrawer()
        {
            DrawAutoBuildPopup = false;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.type == "string")
            {
                Target = SemVer.Parse(property.stringValue);
                var corrected = DrawSemVer(position, property, label);
                property.stringValue = corrected.ToString();
                return;
            }

            Debug.LogWarning($"{property.type} is not supported by {this}");
            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.PropertyField(position, property);
            EditorGUI.EndProperty();
        }
    }
}