namespace Project.Module.PlayableArea
{
    using System.Collections.Generic;
    using UnityEngine;
    using Project.Shared;
    using Project.Data.PlayableArea;

    public class GridBuilderManager : GameBehaviour
    {
        #region ALL UNITY FUNCTIONS

        #endregion ALL UNITY FUNCTIONS

        //=================================   
        #region ALL OVERRIDING FUNCTIONS

        protected override void OnLevelStarted()
        {
            base.OnLevelStarted();

            int levelIndex = _gameManager.LevelDataManagerReference.GetLevelIndex;
            _gridDataAssetForCurrentLevel = _gameManager.GridDataManagerReference.GridsData[levelIndex];

            FillGridWithColor();

            _userInputOnColorGrid.Initialize(OnRecievingTheTouchedColorGrid);
        }

        protected override void OnLevelEnded()
        {
            base.OnLevelEnded();

            _userInputOnColorGrid.RestoreToDefault();
        }

        #endregion ALL OVERRIDING FUNCTIONS

        //=================================
        #region ALL SELF DECLEAR FUNCTIONS

        #region Private Variables

        [Header("Reference  :   External")]
        [SerializeField] private UserInputOnColorBlock _userInputOnColorGrid;
        [SerializeField] private GameObject _colorBlockPrefab;
        [SerializeField] private GameObject _objectiveBlockPrefab;


        private GridDataAsset               _gridDataAssetForCurrentLevel;

        [SerializeField] private List<InteractableBlock> _listOfBlock;

        //ColorBlock
        private List<ColorBlock>            _listOfColorBlock;

        private Dictionary<ColorBlock, int> _gridMapingForPossibleSolution;
        private List<List<ColorBlock>>      _listOfSolution;

        //ObjectiveBlock
        private List<ObjectiveBlock> _listOfObjectiveBlock;

        #endregion

        #region Configuretion

        private bool CheckIfDeadlockCondition()
        {
            bool isDeadLock = true;
            int row = _gridDataAssetForCurrentLevel.Row;
            int column = _gridDataAssetForCurrentLevel.Column;

            Debug.Log(string.Format("_listOfBlockCount = {0}", _listOfBlock.Count));

            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < column; j++)
                {
                    int index = (i * column) + j;

                    ColorBlock colorBlock;

                    Debug.Log(string.Format("Block({0})", index));

                    if (_listOfBlock[index].TryGetComponent<ColorBlock>(out colorBlock))
                    {
                        Debug.Log(string.Format("ColorBlock({0})", index));

                        //if : upper row is valid
                        if ((i + 1) < row)
                        {
                            int upperRowIndex = ((i + 1) * column) + j;
                            ColorBlock colorBlockOnUpperRow;

                            if (_listOfBlock[upperRowIndex].TryGetComponent<ColorBlock>(out colorBlockOnUpperRow))
                            {
                                Debug.Log(string.Format("Index = {0}, UpperRowIndex = {1}", index, upperRowIndex));
                                if (colorBlock.ColorIndex == colorBlockOnUpperRow.ColorIndex)
                                {
                                    isDeadLock = false;
                                    break;
                                }
                            }
                        }

                        //if : Next Column is valid
                        if ((j + 1) < column)
                        {
                            int nextColumnIndex = (i * column) + (j + 1);
                            ColorBlock colorBlockOnNextColumn;

                            if (_listOfBlock[nextColumnIndex].TryGetComponent<ColorBlock>(out colorBlockOnNextColumn))
                            {
                                Debug.Log(string.Format("Index = {0}, NextColumnIndex = {1}", index, nextColumnIndex));
                                if (colorBlock.ColorIndex == colorBlockOnNextColumn.ColorIndex)
                                {
                                    isDeadLock = false;
                                    break;
                                }
                            }
                        }
                    }

                    
                }

                if (!isDeadLock)
                    break;
            }

            if (isDeadLock)
                Debug.LogWarning(string.Format("Deadlock Found"));

            return isDeadLock;
        }

        private void FillGridWithColor()
        {
            
            do
            {
                if (_listOfBlock != null)
                {
                    foreach (InteractableBlock interactableBlock in _listOfBlock)
                        Destroy(interactableBlock.gameObject);
                }

                _listOfBlock = new List<InteractableBlock>();
                _listOfColorBlock = new List<ColorBlock>();
                _listOfObjectiveBlock = new List<ObjectiveBlock>();

                int row = _gridDataAssetForCurrentLevel.Row;
                int column = _gridDataAssetForCurrentLevel.Column;

                float x = -(row / 2.0f) + 0.5f;
                for (int i = 0; i < row; i++)
                {
                    float y = -(column / 2.0f) + 0.5f;
                    for (int j = 0; j < _gridDataAssetForCurrentLevel.Column; j++)
                    {
                        
                        int index = (i * column) + j;


                        switch (_gridDataAssetForCurrentLevel.GridLayout[index])
                        {
                            case GridDataAsset.Marker.Color:

                                int gridColorIndex = Random.Range(0, _gridDataAssetForCurrentLevel.NumberOfColor);
                                ColorBlock colorBlock = Instantiate(_colorBlockPrefab, transform).GetComponent<ColorBlock>();
                                colorBlock.transform.localPosition = new Vector3(y, row + 0.5f, 0);
                                colorBlock.Initialize(
                                    i,
                                    j,
                                    index,
                                    _gridDataAssetForCurrentLevel.ColorBlocks[gridColorIndex].DefaulColorSprite,
                                    new Vector3(y, x, 0));
                                colorBlock.SetColorIndex(gridColorIndex);
                                _listOfColorBlock.Add(colorBlock);
                                _listOfBlock.Add(colorBlock);
#if UNITY_EDITOR
                                colorBlock.gameObject.name = string.Format(
                                    "Block[{0},{1}]_Index({2}))",
                                    i,
                                    j,
                                    index);
#endif

                                break;

                            case GridDataAsset.Marker.Objective:

                                ObjectiveBlock objectiveBlock = Instantiate(_objectiveBlockPrefab, transform).GetComponent<ObjectiveBlock>();
                                objectiveBlock.transform.localPosition = new Vector3(y, row + 0.5f, 0);
                                objectiveBlock.Initialize(
                                    i,
                                    j,
                                    index,
                                    _gridDataAssetForCurrentLevel.ObjectiveBlock.DefaulColorSprite,
                                    new Vector3(y, x, 0));
                                _listOfObjectiveBlock.Add(objectiveBlock);
                                _listOfBlock.Add(objectiveBlock);
#if UNITY_EDITOR
                                objectiveBlock.gameObject.name = string.Format(
                                    "Block[{0},{1}]_Index({2}))",
                                    i,
                                    j,
                                    index);
#endif

                                break;
                        }
                        y++;
                    }
                    x++;
                }

            } while (CheckIfDeadlockCondition());

            CreateListOfPossibleSolution();
        }

        private void CreateListOfPossibleSolution()
        {
            _gridMapingForPossibleSolution  = new Dictionary<ColorBlock, int>();
            _listOfSolution = new List<List<ColorBlock>>();

            int row = _gridDataAssetForCurrentLevel.Row;
            int column = _gridDataAssetForCurrentLevel.Column;

            //--------
            //[ ][ ][ ][ ]
            //[ ][ ][ ][ ]
            //[ ][ ][ ][ ]
            //[ ][ ][ ][ ]
            //------
            //Mapping The Group
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < column ; j++)
                {
                    int index = (i * column) + j;
                    int nextRowIndex = ((i + 1) * column) + j;
                    int nextColumnIndex = (i * column) + (j + 1);

                    //ORDER IS IMPORTANT
                    if ((j + 1) < column)
                    {
                        CheckAndMap(index, nextColumnIndex);
                    }

                    if ((i + 1) < row)
                    {
                        CheckAndMap(index, nextRowIndex);
                    }
                }
            }

            ColorTheGroup();
        }

        private void CheckAndMap(int currentIndex, int nextIndex)
        {
            //--------
            //[ ][ ][ ][ ]
            //[ ][ ][ ][ ]
            //[ ][ ][ ][ ]
            //[ ][ ][ ][ ]
            //------

            ColorBlock currentColorBlock;
            ColorBlock nextColorBlock;
            if (_listOfBlock[currentIndex].TryGetComponent<ColorBlock>(out currentColorBlock) && _listOfBlock[nextIndex].TryGetComponent<ColorBlock>(out nextColorBlock))
            {
                if (currentColorBlock.ColorIndex == nextColorBlock.ColorIndex)
                {
                    int currentIndexOnSolutionList = -1;
                    int nextIndexOnSolutionList = -1;

                    bool isCurrentValueFound = _gridMapingForPossibleSolution.TryGetValue(currentColorBlock, out currentIndexOnSolutionList);
                    bool IsNextValueFound = _gridMapingForPossibleSolution.TryGetValue(nextColorBlock, out nextIndexOnSolutionList);

                    if (!IsNextValueFound)
                    {
                        //if : The next is not include in any group
                        if (!isCurrentValueFound)
                        {
                            //if : Current is also not in any group

                            currentIndexOnSolutionList = _listOfSolution.Count;
                            _listOfSolution.Add(new List<ColorBlock>());
                            _listOfSolution[currentIndexOnSolutionList] = new List<ColorBlock>();
                            _listOfSolution[currentIndexOnSolutionList].Add(currentColorBlock);
                            _gridMapingForPossibleSolution.Add(currentColorBlock, currentIndexOnSolutionList);
                        }

                        _listOfSolution[currentIndexOnSolutionList].Add(nextColorBlock);
                        _gridMapingForPossibleSolution.Add(nextColorBlock, currentIndexOnSolutionList);
                    }
                    else
                    {

                        if (!isCurrentValueFound)
                        {
                            //if : Next is in group but current is not in group
                            _listOfSolution[nextIndexOnSolutionList].Add(currentColorBlock);
                            _gridMapingForPossibleSolution.Add(currentColorBlock, nextIndexOnSolutionList);
                        }
                        else
                        {

                            //if : "Next and Current Both are in a group" && Not the same groupd -> Merge to next group

                            if (currentIndexOnSolutionList != nextIndexOnSolutionList)
                            {
                                int sizeOfOfCurrentSolution = _listOfSolution[currentIndexOnSolutionList].Count;

                                for (int i = 0; i < sizeOfOfCurrentSolution; i++)
                                {
                                    _listOfSolution[nextIndexOnSolutionList].Add(_listOfSolution[currentIndexOnSolutionList][i]);
                                    _gridMapingForPossibleSolution.Remove(_listOfSolution[currentIndexOnSolutionList][i]);
                                    _gridMapingForPossibleSolution.Add(_listOfSolution[currentIndexOnSolutionList][i], nextIndexOnSolutionList);
                                }

                                _listOfSolution[currentIndexOnSolutionList].Clear();
                            }
                        }
                    }
                }
            }
        }

        private void ColorTheGroup()
        {
            int numberOfSolutionList = _listOfSolution.Count;
            for (int i = 0; i < numberOfSolutionList; i++)
            {
                int sizeOfGroup         = _listOfSolution[i].Count;

                if (sizeOfGroup > 0)
                {
                    int colorIndex = _listOfSolution[i][0].ColorIndex;

                    int colorSpriteIndex = -1;
                    Sprite colorOfGrid = _gridDataAssetForCurrentLevel.ColorBlocks[colorIndex].DefaulColorSprite;

                    for (int j = 0; j < _gridDataAssetForCurrentLevel.ColorBlocks[colorIndex].ColorSpriteForGroup.Count; j++)
                    {
                        if (sizeOfGroup > _gridDataAssetForCurrentLevel.ColorBlocks[colorIndex].ColorSpriteForGroup[j].MinNumberOfGroupSize)
                        {
                            colorSpriteIndex = j;
                        }
                    }

                    if (colorSpriteIndex != -1)
                        colorOfGrid = _gridDataAssetForCurrentLevel.ColorBlocks[colorIndex].ColorSpriteForGroup[colorSpriteIndex].ColorSprite;

                    for (int j = 0; j < sizeOfGroup; j++)
                    {
                        _listOfSolution[i][j].ChangeSprite(colorOfGrid);
                    }
                }
            }
        }

        

        private void OnRecievingTheTouchedColorGrid(InteractableBlock touchedGrid)
        {
            Debug.Log(string.Format(
                "Touced Grid({0},{1}) : Index = {2}",
                touchedGrid.Row,
                touchedGrid.Column,
                touchedGrid.Index));

            
        }

        #endregion

        #endregion ALL SELF DECLEAR FUNCTIONS

    }
}
