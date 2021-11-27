namespace Project.Data.PlayableArea
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;
    using com.faith.core;

    [CanEditMultipleObjects]
    [CustomEditor(typeof(GridDataAsset))]
    public class GridDataAssetEditor : Editor
    {
        #region Private Variables

        private GridDataAsset _reference;

        private SerializedProperty _gridBuildOption;
        private SerializedProperty _selectedInteractableBlockIndex;
        private SerializedProperty _selectedInteractableBlock;
        private SerializedProperty _showGridNumber;

        private SerializedProperty _name;
        private SerializedProperty _row;
        private SerializedProperty _column;
        private SerializedProperty _numberOfAvailableMove;
        private SerializedProperty _objectiveBlocks;
        private SerializedProperty _colorBlocks;
        private SerializedProperty _gridLayout;

        private Vector2 _scrollPosition;
        private GUIStyle _buttonStyle;
        #endregion

        #region CustomGUI

        private void HeaderGUI()
        {
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
                EditorGUILayout.PropertyField(_numberOfAvailableMove);
            }
            EditorGUILayout.EndVertical();

            EditorGUI.BeginChangeCheck();
            {
                EditorGUILayout.PropertyField(_objectiveBlocks);
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
            }
            if (EditorGUI.EndChangeCheck())
            {
                _selectedInteractableBlock.objectReferenceValue = null;
                _selectedInteractableBlock.serializedObject.ApplyModifiedProperties();
            }
            
        }

        private void GridBuilderGUI()
        {
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
                    bool showGrid = _showGridNumber.boolValue;
                    float screenWidth = Screen.width;
                    float blockDimension = Mathf.Clamp(screenWidth / column, 40,60);
                    Texture2D defaultTexture = GUI.skin.button.normal.background;
                    _buttonStyle = new GUIStyle(GUI.skin.button);
                    _buttonStyle.fixedWidth = blockDimension;
                    _buttonStyle.fixedHeight = blockDimension;
                    
                    for (int i = row - 1; i >= 0; i--)
                    {
                        EditorGUILayout.BeginHorizontal();
                        {
                            for (int j = column - 1; j >= 0; j--)
                            {
                                EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.Height(blockDimension + (showGrid ? 20 : 0)));
                                {
                                    int index = (i * column) + j;

                                    if (_gridLayout.GetArrayElementAtIndex(index).objectReferenceValue == null)
                                        _buttonStyle.normal.background = defaultTexture;
                                    else
                                    {

                                        InteractableBlockAsset interactableBlockAsset = (InteractableBlockAsset) System.Convert.ChangeType(_reference.GridLayout[index], _reference.GridLayout[index].GetType());
                                        _buttonStyle.normal.background = interactableBlockAsset.DefaulColorSprite.texture;
                                    }

                                    if (GUILayout.Button(
                                            "",
                                            _buttonStyle,
                                            GUILayout.Width(blockDimension),
                                            GUILayout.Height(blockDimension)
                                        ))
                                    {
                                        _gridLayout.GetArrayElementAtIndex(index).objectReferenceValue = _selectedInteractableBlock.objectReferenceValue;
                                        _gridLayout.GetArrayElementAtIndex(index).serializedObject.ApplyModifiedProperties();

                                        _gridLayout.serializedObject.ApplyModifiedProperties();
                                    }

                                    if (showGrid)
                                        EditorGUILayout.LabelField(string.Format("Grid({0},{1})", i, j), EditorStyles.boldLabel, GUILayout.Width(blockDimension), GUILayout.Height(20));

                                }
                                EditorGUILayout.EndVertical();

                                
                            }
                        }
                        EditorGUILayout.EndHorizontal();

                    }
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndScrollView();

            _gridLayout.serializedObject.ApplyModifiedProperties();
        }

        private void GridBuilderOptionGUI()
        {
            CoreEditorModule.DrawHorizontalLine();

            EditorGUILayout.BeginVertical(GUI.skin.box);
            {
                EditorGUILayout.PropertyField(_showGridNumber);
                _gridBuildOption.intValue = EditorGUILayout.Popup("Build Option", _gridBuildOption.intValue, new string[] { "Random", "Custom" });
            }
            EditorGUILayout.EndVertical();
            CoreEditorModule.DrawHorizontalLine();

            int numberOfColorBlock = _colorBlocks.arraySize;
            int numberOfObjectiveBlock = _objectiveBlocks.arraySize;
            int totalNumberOfBlock = numberOfColorBlock + numberOfObjectiveBlock;

            switch (_gridBuildOption.intValue)
            {
                case 0:

                    if (GUILayout.Button("Generate Random Grid"))
                    {
                        if (_colorBlocks.arraySize >= 1)
                        {
                            int row = _row.intValue;
                            int column = _column.intValue;
                            int size = row * column;

                            _gridLayout.arraySize = size;
                            _gridLayout.serializedObject.ApplyModifiedProperties();

                            float probabilityOfColorBlock = numberOfColorBlock / ((float)totalNumberOfBlock);
                            float probabilityOfObjectiveBlock = numberOfObjectiveBlock / ((float)totalNumberOfBlock);

                            for (int i = 0; i < size; i++)
                            {
                                if (probabilityOfColorBlock > probabilityOfObjectiveBlock)
                                {
                                    if (Random.Range(0f, 1f) <= probabilityOfColorBlock)
                                        _gridLayout.GetArrayElementAtIndex(i).objectReferenceValue = _reference.ColorBlocks[Random.Range(0, numberOfColorBlock)];
                                    else
                                        _gridLayout.GetArrayElementAtIndex(i).objectReferenceValue = _reference.ObjectiveBlocks[Random.Range(0, numberOfObjectiveBlock)];
                                }
                                else {
                                    if (Random.Range(0f, 1f) <= probabilityOfObjectiveBlock)
                                        _gridLayout.GetArrayElementAtIndex(i).objectReferenceValue = _reference.ObjectiveBlocks[Random.Range(0, numberOfObjectiveBlock)];
                                    else
                                        _gridLayout.GetArrayElementAtIndex(i).objectReferenceValue = _reference.ColorBlocks[Random.Range(0, numberOfColorBlock)];
                                }
                                _gridLayout.GetArrayElementAtIndex(i).serializedObject.ApplyModifiedProperties();
                            }

                            _gridLayout.serializedObject.ApplyModifiedProperties();
                        }
                        else
                            Debug.LogError("Must have at least one color block");
                    }

                    GridBuilderGUI();

                    break;

                case 1:

                    if (numberOfColorBlock > 0 && totalNumberOfBlock > 0)
                    {
                        List<string> blockOption = new List<string>();

                        for (int i = 0; i < numberOfColorBlock; i++)
                        {
                            if (_reference.ColorBlocks[i] != null)
                            {
                                string name = (string.IsNullOrEmpty(_reference.ColorBlocks[i].Name)) ? string.Format("({0})_NameNotFound", i) : _reference.ColorBlocks[i].Name;
                                blockOption.Add(string.Format("CB : {0}", name));
                            }
                        }

                        for (int i = 0; i < numberOfObjectiveBlock; i++)
                        {
                            if (_reference.ObjectiveBlocks[i] != null)
                            {
                                string name = (string.IsNullOrEmpty(_reference.ObjectiveBlocks[i].Name)) ? string.Format("({0})_NameNotFound", i) : _reference.ObjectiveBlocks[i].Name;
                                blockOption.Add(string.Format("OB : {0}", name));
                            }
                        }

                        blockOption.Add("NONE");

                        EditorGUILayout.BeginHorizontal(GUI.skin.box);
                        {
                            EditorGUI.BeginChangeCheck();
                            _selectedInteractableBlockIndex.intValue = EditorGUILayout.Popup("Select", _selectedInteractableBlockIndex.intValue, blockOption.ToArray());
                            if (EditorGUI.EndChangeCheck())
                            {
                                _selectedInteractableBlockIndex.serializedObject.ApplyModifiedProperties();
                                if (_selectedInteractableBlockIndex.intValue < numberOfColorBlock)
                                {
                                    _selectedInteractableBlock.objectReferenceValue = _colorBlocks.GetArrayElementAtIndex(_selectedInteractableBlockIndex.intValue).objectReferenceValue;
                                    _selectedInteractableBlock.serializedObject.ApplyModifiedProperties();
                                }
                                else if (_selectedInteractableBlockIndex.intValue >= numberOfColorBlock && _selectedInteractableBlockIndex.intValue < totalNumberOfBlock)
                                {
                                    _selectedInteractableBlock.objectReferenceValue = _objectiveBlocks.GetArrayElementAtIndex(_selectedInteractableBlockIndex.intValue - numberOfColorBlock).objectReferenceValue;
                                    _selectedInteractableBlock.serializedObject.ApplyModifiedProperties();
                                }
                                else
                                {

                                    _selectedInteractableBlock.objectReferenceValue = null;
                                    _selectedInteractableBlock.serializedObject.ApplyModifiedProperties();
                                }
                            }

                            if (GUILayout.Button("Fill Empty (Color)", GUILayout.Width(120)))
                            {
                                FillEmptyWithColor();
                            }

                            if (GUILayout.Button("Reset Grid", GUILayout.Width(80)))
                            {
                                ResetGrid();
                            }
                        }
                        EditorGUILayout.EndHorizontal();

                        if (_selectedInteractableBlock.objectReferenceValue == null)
                            EditorGUILayout.HelpBox("You must select an option to change grid value", MessageType.Error);

                        GridBuilderGUI();
                    }
                    else
                    {
                        EditorGUILayout.HelpBox("You must assigned at least one color block to customly build the grid", MessageType.Error);
                    }

                    

                    break;
            }

        }

        #endregion

        #region Configuretion

        private void ResetGrid()
        {
            int sizeOfGrid = _gridLayout.arraySize;
            for (int i = 0; i < sizeOfGrid; i++)
            {
                _gridLayout.GetArrayElementAtIndex(i).objectReferenceValue = null;
                _gridLayout.GetArrayElementAtIndex(i).serializedObject.ApplyModifiedProperties();
            }

            _gridLayout.serializedObject.ApplyModifiedProperties();
        }

        private void FillEmptyWithColor()
        {
            int numberOfColorBlock = _colorBlocks.arraySize;
            int sizeOfGrid = _gridLayout.arraySize;
            for (int i = 0; i < sizeOfGrid; i++)
            {
                if (_gridLayout.GetArrayElementAtIndex(i).objectReferenceValue == null)
                {
                    _gridLayout.GetArrayElementAtIndex(i).objectReferenceValue = _reference.ColorBlocks[Random.Range(0, numberOfColorBlock)];
                    _gridLayout.GetArrayElementAtIndex(i).serializedObject.ApplyModifiedProperties();
                }
            }

            _gridLayout.serializedObject.ApplyModifiedProperties();
        }

        #endregion

        #region Editor

        private void OnEnable()
        {
            _reference = (GridDataAsset)target;

            if (_reference == null)
                return;

            _gridBuildOption = serializedObject.FindProperty("_gridBuildOption");
            _selectedInteractableBlock = serializedObject.FindProperty("_selectedInteractableBlock");
            _selectedInteractableBlockIndex = serializedObject.FindProperty("_selectedInteractableBlockIndex");
            _showGridNumber = serializedObject.FindProperty("_showGridNumber");

            _name = serializedObject.FindProperty("_name");
            _row = serializedObject.FindProperty("_row");
            _column = serializedObject.FindProperty("_column");
            _numberOfAvailableMove = serializedObject.FindProperty("_numberOfAvailableMove");
            _objectiveBlocks = serializedObject.FindProperty("_objectiveBlocks");
            _colorBlocks = serializedObject.FindProperty("_colorBlocks");
            _gridLayout = serializedObject.FindProperty("_gridLayout");



        }


        public override void OnInspectorGUI()
        {
            CoreEditorModule.ShowScriptReference(serializedObject);
            CoreEditorModule.DrawHorizontalLine();

            serializedObject.Update();

            HeaderGUI();

            GridBuilderOptionGUI();

            serializedObject.ApplyModifiedProperties();
        }

        #endregion

    }
}

