namespace Project.Data.PlayableArea
{
    using UnityEngine;
    using UnityEditor;

    [CanEditMultipleObjects]
    [CustomEditor(typeof(GridDataAsset))]
    public class GridDataAssetEditor : Editor
    {
        #region Private Variables

        private GridDataAsset _reference;

        private SerializedProperty _colorForColorGrid;
        private SerializedProperty _colorForObjective;

        private SerializedProperty _name;
        private SerializedProperty _row;
        private SerializedProperty _column;
        private SerializedProperty _objectiveBlock;
        private SerializedProperty _colorBlocks;
        private SerializedProperty _marker;
        private SerializedProperty _gridLayout;

        private Vector2 _scrollPosition;
        private Texture2D _1PixelWhiteTexture;
        private Texture2D _1PixelTextureForColor;
        private Texture2D _1PixelTextureForObjective;
        private GUIStyle _buttonStyle;
        #endregion

        #region Configuretion

        private Texture2D GetBackground(GridDataAsset.Marker marker)
        {
            switch (marker)
            {
                case GridDataAsset.Marker.Color:
                    return _1PixelTextureForColor;
                case GridDataAsset.Marker.Objective:
                    return _1PixelTextureForObjective;
                default:
                    return _1PixelWhiteTexture;
            }
        }

        private void MakeTextureForColorGrid()
        {
            _1PixelTextureForColor = new Texture2D(1, 1);
            _1PixelTextureForColor.SetPixel(0, 0, _colorForColorGrid.colorValue);
            _1PixelTextureForColor.Apply();
        }

        private void MakeTextureForObjectiveGrid()
        {
            _1PixelTextureForObjective = new Texture2D(1, 1);
            _1PixelTextureForObjective.SetPixel(0, 0, _colorForObjective.colorValue);
            _1PixelTextureForObjective.Apply();
        }

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
            _objectiveBlock = serializedObject.FindProperty("_objectiveBlock");
            _colorBlocks = serializedObject.FindProperty("_colorBlocks");
            _marker = serializedObject.FindProperty("_marker");
            _gridLayout = serializedObject.FindProperty("_gridLayout");

            _colorForColorGrid = serializedObject.FindProperty("_colorForColorGrid");
            _colorForObjective = serializedObject.FindProperty("_colorForObjective");

            _1PixelWhiteTexture = new Texture2D(1, 1);
            _1PixelWhiteTexture.SetPixel(0, 0, Color.white);
            _1PixelWhiteTexture.Apply();


            MakeTextureForColorGrid();
            MakeTextureForObjectiveGrid();

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
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(_row);
                EditorGUILayout.PropertyField(_column);
                if (EditorGUI.EndChangeCheck())
                {
                    _row.serializedObject.ApplyModifiedProperties();
                    _column.serializedObject.ApplyModifiedProperties();
                    _gridLayout.arraySize = _row.intValue * _column.intValue;
                    _gridLayout.serializedObject.ApplyModifiedProperties();
                }
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.PropertyField(_objectiveBlock);
            if (_colorBlocks.arraySize > 6)
            {

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                {
                    EditorGUILayout.HelpBox("As per documentation, the color should not exceed more than '6'", MessageType.Warning);
                    EditorGUILayout.PropertyField(_colorBlocks, true);
                }
                EditorGUILayout.EndVertical();

            }
            else
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                {
                    EditorGUILayout.PropertyField(_colorBlocks, true);
                }
                EditorGUILayout.EndVertical();
            }

            CoreEditorModule.DrawHorizontalLine();
            EditorGUILayout.PropertyField(_marker);

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(_colorForColorGrid);
            if (EditorGUI.EndChangeCheck())
            {
                _colorForColorGrid.serializedObject.ApplyModifiedProperties();
                MakeTextureForColorGrid();
            }

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(_colorForObjective);
            if (EditorGUI.EndChangeCheck())
            {
                _colorForObjective.serializedObject.ApplyModifiedProperties();
                MakeTextureForObjectiveGrid();
            }

            CoreEditorModule.DrawHorizontalLine();
            int row = _row.intValue;
            int column = _column.intValue;
            int size = row * column;
            if (_gridLayout.arraySize != size)
            {
                _gridLayout.arraySize = size;
                _gridLayout.serializedObject.ApplyModifiedProperties();
            }

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            {
                EditorGUILayout.BeginVertical();
                {
                    float screenWidth = Screen.width;
                    float blockDimension = Mathf.Clamp(screenWidth / column, 80, 120);
                    _buttonStyle = new GUIStyle(GUI.skin.button);
                    _buttonStyle.fixedWidth = blockDimension;
                    _buttonStyle.fixedHeight = blockDimension;
                    for (int i = 0; i < row; i++)
                    {
                        EditorGUILayout.BeginHorizontal();
                        {
                            for (int j = 0; j < column; j++)
                            {
                                int index = (i * column) + j;

                                _buttonStyle.normal.background = GetBackground((GridDataAsset.Marker)_gridLayout.GetArrayElementAtIndex(index).enumValueIndex);

                                if (GUILayout.Button(
                                        string.Format("Grid({0},{1})\n{2}", i, j, (GridDataAsset.Marker)_gridLayout.GetArrayElementAtIndex(index).enumValueIndex),
                                        _buttonStyle,
                                        GUILayout.Width(blockDimension),
                                        GUILayout.Height(blockDimension)
                                    ))
                                {
                                    _gridLayout.GetArrayElementAtIndex(index).enumValueIndex = _marker.enumValueIndex;
                                    _gridLayout.GetArrayElementAtIndex(index).serializedObject.ApplyModifiedProperties();

                                    _gridLayout.serializedObject.ApplyModifiedProperties();
                                }
                            }
                        }
                        EditorGUILayout.EndHorizontal();

                    }
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndScrollView();

            _gridLayout.serializedObject.ApplyModifiedProperties();

            serializedObject.ApplyModifiedProperties();
        }

        #endregion

    }
}

