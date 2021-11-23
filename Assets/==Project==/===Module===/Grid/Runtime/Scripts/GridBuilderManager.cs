namespace Project.Module.Grid
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Project.Shared;
    using Project.Data.Grid;

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

            FillGridWithColor(true);

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
        [SerializeField] private UserInputOnColorGrid _userInputOnColorGrid;
        [SerializeField] private GameObject _colorGridPrefab;

        [SerializeField] private GridDataAsset   _gridDataAssetForCurrentLevel;
        [SerializeField] private List<Grid>      _listOfColorOnGrid;

        [SerializeField] private Dictionary<Grid, int> _gridMapingForPossibleSolution;
        [SerializeField] private List<List<Grid>> _listOfSolution;

        #endregion

        #region Configuretion

        private bool CheckIfDeadlockCondition()
        {
            bool isDeadLock = true;
            int row = _gridDataAssetForCurrentLevel.Row;
            int column = _gridDataAssetForCurrentLevel.Column;

            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < column; j++)
                {
                    int index = (i * column) + j;
                    ColorGrid colorGrid = (ColorGrid)_listOfColorOnGrid[index];

                    if (colorGrid != null)
                    {
                        //if : upper row is valid
                        if ((i + 1) < row)
                        {
                            int upperRowIndex = ((i + 1) * row) + j;
                            ColorGrid colorGridOnUpperRow = (ColorGrid)_listOfColorOnGrid[upperRowIndex];

                            if (colorGridOnUpperRow != null)
                            {
                                if (colorGrid.ColorIndex == colorGridOnUpperRow.ColorIndex)
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
                            ColorGrid colorGridOnNextColumn = (ColorGrid)_listOfColorOnGrid[nextColumnIndex];

                            if (colorGridOnNextColumn != null)
                            {
                                if (colorGrid.ColorIndex == colorGridOnNextColumn.ColorIndex)
                                {
                                    isDeadLock = false;
                                    break;
                                }
                            }
                        }
                    }

                    if (!isDeadLock)
                        break;
                }
            }

            if (isDeadLock)
                Debug.LogWarning(string.Format("Deadlock Found"));

            return isDeadLock;
        }

        private void FillGridWithColor(bool create)
        {
            _listOfColorOnGrid = new List<Grid>();

            do
            {
                int row = _gridDataAssetForCurrentLevel.Row;
                int column = _gridDataAssetForCurrentLevel.Column;

                float x = -(row / 2.0f) + 0.5f;
                for (int i = 0; i < row; i++)
                {
                    float y = -(column / 2.0f) + 0.5f;
                    for (int j = 0; j < _gridDataAssetForCurrentLevel.Column; j++)
                    {
                        int gridColorIndex = Random.Range(0, _gridDataAssetForCurrentLevel.NumberOfColor);
                        int index = (i * column) + j;

                        if (create)
                        {
                            Grid grid = Instantiate(_colorGridPrefab, transform).GetComponent<Grid>();
#if UNITY_EDITOR
                            grid.gameObject.name = string.Format(
                                "Grid[{0},{1}]_Index({2})_ColorIndex(3)",
                                i,
                                j,
                                index,
                                gridColorIndex);
#endif

                            grid.Initialize(
                                i,
                                j,
                                index,
                                gridColorIndex,
                                _gridDataAssetForCurrentLevel.Colors[gridColorIndex].DefaulColorSprite,
                                new Vector3(y, x, 0));
                            _listOfColorOnGrid.Add(grid);
                        }
                        else {
#if UNITY_EDITOR
                            _listOfColorOnGrid[index].gameObject.name = string.Format(
                                "Grid[{0},{1}]_Index({2})_ColorIndex(3)",
                                i,
                                j,
                                index,
                                gridColorIndex);
#endif
                            _listOfColorOnGrid[index].Initialize(
                                    i,
                                    j,
                                    index,
                                    gridColorIndex,
                                    _gridDataAssetForCurrentLevel.Colors[gridColorIndex].DefaulColorSprite,
                                    new Vector3(y, x, 0)
                                );
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
            _gridMapingForPossibleSolution  = new Dictionary<Grid, int>();
            _listOfSolution = new List<List<Grid>>();

            int row = _gridDataAssetForCurrentLevel.Row;
            int column = _gridDataAssetForCurrentLevel.Column;

            //--------
            //[ ][ ][ ][ ]
            //[ ][ ][ ][ ]
            //[ ][ ][ ][ ]
            //[ ][ ][ ][ ]
            //------
            //Mapping The Group
            for (int i = 0; i < row - 1; i++)
            {
                for (int j = 0; j < column - 1 ; j++)
                {
                    int index = (i * column) + j;
                    int nextRowIndex = ((i + 1) * column) + j;
                    int nextColumnIndex = (i * column) + (j + 1);




                    //ORDER IS IMPORTANT
                    Debug.Log(string.Format("Index = {0}, IndexInNextColumn = {1}", index, nextColumnIndex));
                    CheckAndMap(index, nextColumnIndex);
                    Debug.Log(string.Format("Index = {0}, IndexInNextRow = {1}", index, nextRowIndex));
                    CheckAndMap(index, nextRowIndex);

                }
            }

            Debug.Log(string.Format("NumberOfSolution = {0}", _listOfSolution.Count));
            for (int i = 0; i < _listOfSolution.Count; i++)
            {
                Debug.Log(string.Format("Solution[{0}] : GroupSize = {1}", i, _listOfSolution[i].Count));
            }

            ColorTheGroup();
        }

        private void CheckAndMap(int index, int nextIndex)
        {
            //--------
            //[ ][ ][ ][ ]
            //[ ][ ][ ][ ]
            //[ ][ ][ ][ ]
            //[ ][ ][ ][ ]
            //------

            if (_listOfColorOnGrid[index].ColorIndex == _listOfColorOnGrid[nextIndex].ColorIndex)
            {

                int indexOnList = -1;

                
                if (!_gridMapingForPossibleSolution.ContainsKey(_listOfColorOnGrid[nextIndex]))
                {
                    //if : The next is not include in any group


                    if (!_gridMapingForPossibleSolution.ContainsKey(_listOfColorOnGrid[index]))
                    {
                        //if : Me myself not in any group

                        indexOnList = _listOfSolution.Count;
                        _listOfSolution.Add(new List<Grid>());
                        _listOfSolution[indexOnList] = new List<Grid>();
                        _listOfSolution[indexOnList].Add(_listOfColorOnGrid[index]);
                        _gridMapingForPossibleSolution.Add(_listOfColorOnGrid[index], indexOnList);
                    }
                    else
                    {
                        if (!_gridMapingForPossibleSolution.TryGetValue(_listOfColorOnGrid[index], out indexOnList))
                        {
                            Debug.LogError("Value not found");
                        }
                    }

                    _gridMapingForPossibleSolution.Add(_listOfColorOnGrid[nextIndex], indexOnList);
                    _listOfSolution[indexOnList].Add(_listOfColorOnGrid[nextIndex]);
                }
                else {
                    //if : Next is already in group
                    if (!_gridMapingForPossibleSolution.TryGetValue(_listOfColorOnGrid[nextIndex], out indexOnList))
                    {
                        Debug.LogError("Value not found");
                    }

                    if (_gridMapingForPossibleSolution.ContainsKey(_listOfColorOnGrid[index]))
                    {
                        //Merge
                        Debug.Log("Merge");
                        //Removing From Dictionary
                        foreach (Grid grid in _listOfSolution[index])
                            _gridMapingForPossibleSolution.Remove(grid);

                        foreach (Grid grid in _listOfSolution[nextIndex])
                            _gridMapingForPossibleSolution.Remove(grid);


                        _listOfSolution[index].AddRange(_listOfSolution[nextIndex]);
                        _listOfSolution.RemoveAt(nextIndex);

                        foreach (Grid grid in _listOfSolution[index])
                            _gridMapingForPossibleSolution.Add(grid, index);
                    }
                    else {

                        _gridMapingForPossibleSolution.Add(_listOfColorOnGrid[index], indexOnList);
                        _listOfSolution[indexOnList].Add(_listOfColorOnGrid[index]);
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
                int colorIndex          = _listOfSolution[i][0].ColorIndex;

                int colorSpriteIndex = -1;
                Sprite colorOfGrid      = _gridDataAssetForCurrentLevel.Colors[colorIndex].DefaulColorSprite;

                for (int j = 0; j < _gridDataAssetForCurrentLevel.Colors[colorIndex].ColorSpriteForGroup.Count ; j++)
                {
                    if (sizeOfGroup > _gridDataAssetForCurrentLevel.Colors[colorIndex].ColorSpriteForGroup[j].MinNumberOfGroupSize)
                    {
                        colorSpriteIndex = j;
                    }
                }

                if (colorSpriteIndex != -1)
                    colorOfGrid = _gridDataAssetForCurrentLevel.Colors[colorIndex].ColorSpriteForGroup[colorSpriteIndex].ColorSprite;

                for (int j = 0; j < sizeOfGroup; j++)
                {
                    _listOfSolution[i][j].ChangeGridColorVarient(colorOfGrid);
                }
            }
        }

        private void OnRecievingTheTouchedColorGrid(Grid touchedGrid)
        {
            Debug.Log(string.Format(
                "Touced Grid({0},{1}) : Index = {2} : ColorIndex = {3}",
                touchedGrid.Row,
                touchedGrid.Column,
                touchedGrid.Index,
                touchedGrid.ColorIndex));

            
        }

        #endregion

        #endregion ALL SELF DECLEAR FUNCTIONS

    }
}
