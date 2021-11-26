namespace Project.Data.PlayableArea
{
    using UnityEngine;
    using UnityEditor;

    [CanEditMultipleObjects]
    [CustomEditor(typeof(ColorBlockAsset))]
    public class ColorBlockAssetEditor : Editor
    {
        #region Private Variables

        private ColorBlockAsset _reference;

        private SerializedProperty _name;
        private SerializedProperty _gravity;
        private SerializedProperty _defaultColorSprite;
        private SerializedProperty _colorSpriteForGroup;

        #endregion

        #region Editor

        private void OnEnable()
        {
            _reference = (ColorBlockAsset)target;

            if (_reference == null)
                return;

            _name = serializedObject.FindProperty("_name");
            _gravity = serializedObject.FindProperty("_gravity");
            _defaultColorSprite = serializedObject.FindProperty("_defaultColorSprite");
            _colorSpriteForGroup = serializedObject.FindProperty("_colorSpriteForGroup");

        }

        public override void OnInspectorGUI()
        {
            CoreEditorModule.ShowScriptReference(serializedObject);
            CoreEditorModule.DrawHorizontalLine();

            serializedObject.Update();

            EditorGUILayout.PropertyField(_name);
            CoreEditorModule.DrawHorizontalLine();

            EditorGUILayout.BeginVertical(GUI.skin.box);
            {
                EditorGUILayout.PropertyField(_defaultColorSprite);
                EditorGUILayout.PropertyField(_gravity);
            }
            EditorGUILayout.EndVertical();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(_colorSpriteForGroup);
            if(EditorGUI.EndChangeCheck())
            {
                CheckForProperorderOfColorRules();
            }

            serializedObject.ApplyModifiedProperties();
        }

        #endregion

        #region Configuretion

        private void CheckForProperorderOfColorRules()
        {

            _colorSpriteForGroup.serializedObject.ApplyModifiedProperties();
            int numberOfRules = _colorSpriteForGroup.arraySize;
            
            for (int i = 0; i < numberOfRules - 1; i++)
            {
                int currentValue= _colorSpriteForGroup.GetArrayElementAtIndex(i).FindPropertyRelative("_groupSizeGreaterThan").intValue;
                int nextValue   = _colorSpriteForGroup.GetArrayElementAtIndex(i + 1).FindPropertyRelative("_groupSizeGreaterThan").intValue;

                if (nextValue <= currentValue)
                {
                    nextValue = currentValue + 1;
                }

                _colorSpriteForGroup.GetArrayElementAtIndex(i + 1).FindPropertyRelative("_groupSizeGreaterThan").intValue = nextValue;
                _colorSpriteForGroup.GetArrayElementAtIndex(i + 1).FindPropertyRelative("_groupSizeGreaterThan").serializedObject.ApplyModifiedProperties();

            }
            
            _colorSpriteForGroup.serializedObject.ApplyModifiedProperties();
        }

        #endregion
    }
}

