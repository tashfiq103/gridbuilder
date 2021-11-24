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
        [SerializeField] private UserInputOnColorBlock _userInputOnColorGrid;
        [SerializeField] private GameObject _colorGridPrefab;

        private GridDataAsset   _gridDataAssetForCurrentLevel;
        private List<ColorBlock>      _listOfColorOnGrid;

        private Dictionary<ColorBlock, int> _gridMapingForPossibleSolution;
        private List<List<ColorBlock>> _listOfSolution;

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
                    ColorBlock colorGrid = _listOfColorOnGrid[index];

                    if (colorGrid != null)
                    {
                        //if : upper row is valid
                        if ((i + 1) < row)
                        {
                            int upperRowIndex = ((i + 1) * row) + j;
                            ColorBlock colorGridOnUpperRow = _listOfColorOnGrid[upperRowIndex];

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
                            ColorBlock colorGridOnNextColumn = _listOfColorOnGrid[nextColumnIndex];

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
            _listOfColorOnGrid = new List<ColorBlock>();

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
                            ColorBlock grid = Instantiate(_colorGridPrefab, transform).GetComponent<ColorBlock>();
#if UNITY_EDITOR
                            grid.gameObject.name = string.Format(
                                "Grid[{0},{1}]_Index({2})_ColorIndex({3})",
                                i,
                                j,
                                index,
                                gridColorIndex);
#endif
                            grid.transform.localPosition = new Vector3(y, row + 0.5f, 0);
                            grid.Initialize(
                                i,
                                j,
                                index,
                                _gridDataAssetForCurrentLevel.Colors[gridColorIndex].DefaulColorSprite,
                                new Vector3(y, x, 0));
                            grid.SetColorIndex(gridColorIndex);
                            _listOfColorOnGrid.Add(grid);
                        }
                        else {
#if UNITY_EDITOR
                            _listOfColorOnGrid[index].gameObject.name = string.Format(
                                "Grid[{0},{1}]_Index({2})_ColorIndex({3})",
                                i,
                                j,
                                index,
                                gridColorIndex);
#endif
                            _listOfColorOnGrid[index].transform.localPosition = new Vector3(y, x, 0);
                            _listOfColorOnGrid[index].Initialize(
                                    i,
                                    j,
                                    index,
                                    _gridDataAssetForCurrentLevel.Colors[gridColorIndex].DefaulColorSprite,
                                    new Vector3(y, x, 0)
                                );
                            _listOfColorOnGrid[index].SetColorIndex(gridColorIndex);
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

            if (_listOfColorOnGrid[currentIndex].ColorIndex == _listOfColorOnGrid[nextIndex].ColorIndex)
            {
                Debug.Log(string.Format("MatchFound {0} == {1}. ColorIndex = {2}", currentIndex, nextIndex, _listOfColorOnGrid[currentIndex].ColorIndex));

                int currentIndexOnSolutionList = -1;
                int nextIndexOnSolutionList = -1;

                bool isCurrentValueFound    = _gridMapingForPossibleSolution.TryGetValue(_listOfColorOnGrid[currentIndex], out currentIndexOnSolutionList);
                bool IsNextValueFound       = _gridMapingForPossibleSolution.TryGetValue(_listOfColorOnGrid[nextIndex], out nextIndexOnSolutionList);

                if(isCurrentValueFound)
                    Debug.Log(string.Format("SolutionListIndex = {0}, for CurrentIndex = {1}", currentIndexOnSolutionList, currentIndex));

                if(IsNextValueFound)
                    Debug.Log(string.Format("SolutionListIndex = {0}, for NextIndex = {1}", nextIndexOnSolutionList, nextIndex));

                if (!IsNextValueFound)
                {
                    //if : The next is not include in any group
                    if (!isCurrentValueFound)
                    {
                        //if : Current is also not in any group

                        currentIndexOnSolutionList = _listOfSolution.Count;
                        _listOfSolution.Add(new List<ColorBlock>());
                        _listOfSolution[currentIndexOnSolutionList] = new List<ColorBlock>();
                        _listOfSolution[currentIndexOnSolutionList].Add(_listOfColorOnGrid[currentIndex]);
                        _gridMapingForPossibleSolution.Add(_listOfColorOnGrid[currentIndex], currentIndexOnSolutionList);
                    }

                    _listOfSolution[currentIndexOnSolutionList].Add(_listOfColorOnGrid[nextIndex]);
                    _gridMapingForPossibleSolution.Add(_listOfColorOnGrid[nextIndex], currentIndexOnSolutionList);
                }
                else {

                    if (!isCurrentValueFound)
                    {
                        //if : Next is in group but current is not in group
                        _listOfSolution[nextIndexOnSolutionList].Add(_listOfColorOnGrid[currentIndex]);
                        _gridMapingForPossibleSolution.Add(_listOfColorOnGrid[currentIndex], nextIndexOnSolutionList);
                    }
                    else {

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
                    Sprite colorOfGrid = _gridDataAssetForCurrentLevel.Colors[colorIndex].DefaulColorSprite;

                    for (int j = 0; j < _gridDataAssetForCurrentLevel.Colors[colorIndex].ColorSpriteForGroup.Count; j++)
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
                        _listOfSolution[i][j].ChangeSprite(colorOfGrid);
                    }
                }
            }
        }

        private void OnRecievingTheTouchedColorGrid(InteractableBlock touchedGrid)
        {
            Debug.Log(string.Format(
                "Touced Grid({0},{1}) : Index = {2} : ColorIndex = {3}",
                touchedGrid.Row,
                touchedGrid.Column,
                touchedGrid.Index));

            
        }

        #endregion

        #endregion ALL SELF DECLEAR FUNCTIONS

    }
}
