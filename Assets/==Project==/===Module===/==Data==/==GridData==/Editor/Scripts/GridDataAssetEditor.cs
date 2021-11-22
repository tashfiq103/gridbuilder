namespace Project.Data.Grid
{
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
                    EditorGUILayout.PropertyField(_colors, true);
                }
                EditorGUILayout.EndVertical();

            }
            else
                EditorGUILayout.PropertyField(_colors);

            serializedObject.ApplyModifiedProperties();
        }

        #endregion
    }
}

