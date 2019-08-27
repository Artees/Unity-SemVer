using System;
using UnityEditor;
using UnityEngine;

namespace Artees.UnitySemVer.Editor
{
    [CustomPropertyDrawer(typeof(SemVer))]
    internal class SemVerDrawer : PropertyDrawer
    {
        private const float IncrementButtonWidth = 40f;

        private float _yMin;
        private Rect _position;

        protected SemVer Target { private get; set; }
        protected bool DrawAutoBuildPopup { private get; set; }

        public SemVerDrawer()
        {
            DrawAutoBuildPopup = true;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var targetObject = property.serializedObject.targetObject;
            var source = fieldInfo.GetValue(targetObject) as SemVer;
            if (source == null) return;
            Target = source.Clone();
            var corrected = DrawSemVer(position, property, label);
            if (corrected == source && corrected.autoBuild == source.autoBuild) return;
            EditorUtility.SetDirty(targetObject);
            fieldInfo.SetValue(targetObject, corrected);
        }

        protected SemVer DrawSemVer(Rect position, SerializedProperty property, GUIContent label)
        {
            InitPosition(position);
            label.text = $"{label.text} {Target}";
            property.isExpanded =
                EditorGUI.Foldout(GetNextPosition(), property.isExpanded, label.text, true);
            if (!property.isExpanded) return Target;
            EditorGUI.indentLevel++;
            CreateMajorField();
            CreateMinorField();
            CreatePatchField();
            CreatePreReleaseField();
            CreateBuildField();
            var corrected = Validate();
            EditorGUI.indentLevel--;
            return corrected;
        }

        private void InitPosition(Rect position)
        {
            _yMin = position.yMin;
            _position = position;
            _position.height = EditorGUIUtility.singleLineHeight;
            _position.y -= EditorGUIUtility.singleLineHeight;
        }

        private Rect GetNextPosition()
        {
            _position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            return _position;
        }

        private void CreateMajorField()
        {
            const string label = SemVerTooltip.Major;
            Target.major = CreateCoreField(label, Target.major);
            if (CreateIncrementButton(label)) Target.IncrementMajor();
        }

        private void CreateMinorField()
        {
            const string label = SemVerTooltip.Minor;
            Target.minor = CreateCoreField(label, Target.minor);
            if (CreateIncrementButton(label)) Target.IncrementMinor();
        }

        private void CreatePatchField()
        {
            const string label = SemVerTooltip.Patch;
            Target.patch = CreateCoreField(label, Target.patch);
            if (CreateIncrementButton(label)) Target.IncrementPatch();
        }

        private uint CreateCoreField(string label, uint version)
        {
            uint newVersionUint;
            try
            {
                var position = GetNextPosition();
                position.width -= IncrementButtonWidth + EditorGUIUtility.standardVerticalSpacing;
                var content = CreateGuiContentWithTooltip(label);
                var newVersionInt = EditorGUI.IntField(position, content, (int) version);
                newVersionUint = Convert.ToUInt32(newVersionInt);
            }
            catch (OverflowException)
            {
                Debug.LogWarning("A version must not be negative");
                newVersionUint = version;
            }

            return newVersionUint;
        }

        private static GUIContent CreateGuiContentWithTooltip(string label)
        {
            return new GUIContent(label, SemVerTooltip.Field[label]);
        }

        private bool CreateIncrementButton(string version)
        {
            var buttonName = GUID.Generate().ToString();
            GUI.SetNextControlName(buttonName);
            var position = _position;
            position.width = IncrementButtonWidth;
            position.x += _position.width - IncrementButtonWidth;
            var content = new GUIContent("+", SemVerTooltip.Increment[version]);
            var isClicked = GUI.Button(position, content);
            if (isClicked) GUI.FocusControl(buttonName);
            return isClicked;
        }

        private void CreatePreReleaseField()
        {
            var position = GetNextPosition();
            var content = CreateGuiContentWithTooltip(SemVerTooltip.PreRelease);
            Target.preRelease = EditorGUI.TextField(position, content, Target.preRelease);
        }

        private void CreateBuildField()
        {
            var position = GetNextPosition();
            var content = CreateGuiContentWithTooltip(SemVerTooltip.Build);
            if (DrawAutoBuildPopup)
            {
                var popupPosition = position;
                var popupWidth = EditorGUIUtility.labelWidth + 120f;
                popupPosition.width = popupWidth;
                var selected = EditorGUI.EnumPopup(popupPosition, content, Target.autoBuild);
                Target.autoBuild = (SemVerAutoBuild.Type) selected;
                var textPosition = position;
                textPosition.x = popupWidth + EditorGUIUtility.standardVerticalSpacing;
                textPosition.width -= textPosition.x - 14f;
                if (SemVerAutoBuild.Instances[Target.autoBuild] is SemVerAutoBuild.ReadOnly)
                {
                    var guiEnabled = GUI.enabled;
                    GUI.enabled = false;
                    EditorGUI.TextField(textPosition, Target.Build);
                    GUI.enabled = guiEnabled;
                }
                else
                {
                    Target.Build = EditorGUI.TextField(textPosition, Target.Build);
                }
            }
            else
            {
                Target.autoBuild = SemVerAutoBuild.Type.Manual;
                Target.Build = EditorGUI.TextField(position, content, Target.Build);
            }
        }

        private SemVer Validate()
        {
            var result = Target.Validate();
            if (result.IsValid) return Target;
            foreach (var message in result.Errors)
            {
                EditorGUILayout.HelpBox(message, MessageType.Warning);
            }

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            var fixButtonName = GUID.Generate().ToString();
            GUI.SetNextControlName(fixButtonName);
            var isFixed = GUILayout.Button("Fix", GUILayout.Width(40));
            if (isFixed)
            {
                GUI.FocusControl(fixButtonName);
            }

            GUILayout.EndHorizontal();
            return isFixed ? result.Corrected : Target;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return _position.yMax - _yMin;
        }
    }
}