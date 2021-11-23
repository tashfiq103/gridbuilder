namespace Project.Data.Grid
{
    using UnityEngine;
    using UnityEditor;

    [CustomEditor(typeof(GridDataAsset))]
    public class GridDataAssetEditor : Editor
    {
        #region Private Variables

        private GridDataAsset _reference;

        private SerializedProperty _name;
        private SerializedProperty _row;
        private SerializedProperty _column;
        private SerializedProperty _colors;

        #endregion

        #region Editor

        private void OnEnable()
        {
            _reference = (GridDataAsset)target;

            if (_reference == null)
                return;

            _name = serializedObject.FindProperty("_name");
            _row = serializedObject.FindProperty("_row");
            _column = serializedObject.FindProperty("_column");
            _colors = serializedObject.FindProperty("_colors");
        }

        public override void OnInspectorGUI()
        {
            CoreEditorModule.ShowScriptReference(serializedObject);
            CoreEditorModule.DrawHorizontalLine();

            serializedObject.Update();

            EditorGUILayout.PropertyField(_name);
            CoreEditorModule.DrawHorizontalLine();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                EditorGUILayout.PropertyField(_row);
                EditorGUILayout.PropertyField(_column);
            }
            EditorGUILayout.EndVertical();

            if (_colors.arraySize > 6)
            {

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                {
                    EditorGUILayout.HelpBox("As per documentation, the color should not exceed more than '6'", MessageType.Warning);

                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(_colors, true);
                    if (EditorGUI.EndChangeCheck())
                    {
                        CheckForProperorderOfColorRules();
                    }
                }
                EditorGUILayout.EndVertical();

            }
            else
            {
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(_colors, true);
                if (EditorGUI.EndChangeCheck())
                {
                    CheckForProperorderOfColorRules();
                }
            }
            serializedObject.ApplyModifiedProperties();
        }

        #endregion

        #region Configuretion

        private void CheckForProperorderOfColorRules() {

            _colors.serializedObject.ApplyModifiedProperties();
            int numberOfColor = _colors.arraySize;
            for (int i = 0; i < numberOfColor; i++)
            {
                int numberOfRules = _colors.GetArrayElementAtIndex(i).FindPropertyRelative("_colorSpriteForGroup").arraySize;
                for (int j = 0; j < numberOfRules - 1; j++)
                {
                    int currentValue = _colors.GetArrayElementAtIndex(i).FindPropertyRelative("_colorSpriteForGroup").GetArrayElementAtIndex(j).FindPropertyRelative("_groupSizeGreaterThan").intValue;
                    int nextValue = _colors.GetArrayElementAtIndex(i).FindPropertyRelative("_colorSpriteForGroup").GetArrayElementAtIndex(j + 1).FindPropertyRelative("_groupSizeGreaterThan").intValue;

                    if (nextValue <= currentValue)
                    {
                        nextValue = currentValue + 1;
                    }

                    _colors.GetArrayElementAtIndex(i).FindPropertyRelative("_colorSpriteForGroup").GetArrayElementAtIndex(j + 1).FindPropertyRelative("_groupSizeGreaterThan").intValue = nextValue;
                    _colors.GetArrayElementAtIndex(i).FindPropertyRelative("_colorSpriteForGroup").GetArrayElementAtIndex(j + 1).FindPropertyRelative("_groupSizeGreaterThan").serializedObject.ApplyModifiedProperties();

                    //Debug.LogWarning(string.Format("MinGroupSize has to be greater than previous size. Enforcuing to be greater than previous for Color[{0}]_Rules[{1}]", i, j+ 1));
                }
                _colors.GetArrayElementAtIndex(i).FindPropertyRelative("_colorSpriteForGroup").serializedObject.ApplyModifiedProperties();
            }

            _colors.serializedObject.ApplyModifiedProperties();
        }

        #endregion
    }
}

