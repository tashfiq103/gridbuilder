namespace Project.Module.PlayableArea
{
    using System.Collections.Generic;
    using UnityEngine;
    using Project.Shared;
    using Project.Data.PlayableArea;
    using DG.Tweening;

    public class PlayableAreaController : GameBehaviour
    {
        #region ALL UNITY FUNCTIONS

        #endregion ALL UNITY FUNCTIONS

        //=================================   
        #region ALL OVERRIDING FUNCTIONS

        protected override void OnLevelStarted()
        {
            base.OnLevelStarted();

            int levelIndex                  = _gameManager.LevelDataManagerReference.GetLevelIndex;
            _gridDataAssetForCurrentLevel   = _gameManager.GridDataManagerReference.GridsData[levelIndex];
            RemainingNumberOfMove           = _gridDataAssetForCurrentLevel.NumberOfAvailableMove;

            FillTheWholeGridFromLayout();

            _userInputOnColorGrid.Initialize(OnRecievingTheTouchedColorGrid);
        }

        protected override void OnLevelCompleted()
        {
            base.OnLevelCompleted();

            _userInputOnColorGrid.RestoreToDefault();
        }

        protected override void OnLevelFailed()
        {
            base.OnLevelFailed();

            _userInputOnColorGrid.RestoreToDefault();
        }

        #endregion ALL OVERRIDING FUNCTIONS

        //=================================
        #region ALL SELF DECLEAR FUNCTIONS

        #region Public Variables

        public int RemainingNumberOfMove { get; private set; }

        #endregion

        #region Private Variables

        [Header("Reference  :   External")]
        [SerializeField] private UserInputOnColorBlock _userInputOnColorGrid;
        [SerializeField] private GameObject _colorBlockPrefab;
        [SerializeField] private GameObject _objectiveBlockPrefab;


        private GridDataAsset                           _gridDataAssetForCurrentLevel;
        private List<InteractableBlock>                 _listOfBlock;

        //ColorBlock
        private List<ColorBlock>                        _listOfColorBlock;
        private List<List<ColorBlock>>                  _listOfPossibleSolutionForColorBlock;
        private Dictionary<ColorBlock, int>             _colorBlockMapingForPossibleSolution;
        

        //ObjectiveBlock
        [SerializeField] private List<ObjectiveBlock>   _listOfObjectiveBlock;

        #endregion

        #region Configuretion

        private bool CheckIfDeadlockCondition()
        {
            bool isDeadLock = true;
            int row = _gridDataAssetForCurrentLevel.Row;
            int column = _gridDataAssetForCurrentLevel.Column;

            //Debug.Log(string.Format("_listOfBlockCount = {0}", _listOfBlock.Count));

            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < column; j++)
                {
                    int index = (i * column) + j;

                    ColorBlock colorBlock;

                    //Debug.Log(string.Format("Block({0})", index));

                    if (_listOfBlock[index] != null
                     && _listOfBlock[index].TryGetComponent<ColorBlock>(out colorBlock))
                    {
                        //Debug.Log(string.Format("ColorBlock({0})", index));

                        //if : upper row is valid
                        if ((i + 1) < row)
                        {
                            int upperRowIndex = ((i + 1) * column) + j;
                            ColorBlock colorBlockOnUpperRow;

                            if (_listOfBlock[upperRowIndex] != null
                             && _listOfBlock[upperRowIndex].TryGetComponent<ColorBlock>(out colorBlockOnUpperRow))
                            {
                                //Debug.Log(string.Format("Index = {0}, UpperRowIndex = {1}", index, upperRowIndex));
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

                            if (_listOfBlock[nextColumnIndex] != null
                             && _listOfBlock[nextColumnIndex].TryGetComponent<ColorBlock>(out colorBlockOnNextColumn))
                            {
                                //Debug.Log(string.Format("Index = {0}, NextColumnIndex = {1}", index, nextColumnIndex));
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

        private void FillTheWholeGridFromLayout()
        {
            do
            {
                if (_listOfBlock != null)
                {
                    foreach (InteractableBlock interactableBlock in _listOfBlock)
                    {
                        if (interactableBlock != null)
                            Destroy(interactableBlock.gameObject);
                    }
                }

                _listOfBlock = new List<InteractableBlock>();
                _listOfColorBlock = new List<ColorBlock>();
                _listOfObjectiveBlock = new List<ObjectiveBlock>();

                int row = _gridDataAssetForCurrentLevel.Row;
                int column = _gridDataAssetForCurrentLevel.Column;

                float y = -(row / 2.0f) + 0.5f;
                for (int i = 0; i < row; i++)
                {
                    float x = -(column / 2.0f) + 0.5f;
                    for (int j = 0; j < column; j++)
                    {
                        
                        int index = (i * column) + j;

                        if (_gridDataAssetForCurrentLevel.GridLayout[index].GetType() == typeof(ColorBlockAsset))
                        {
                            ColorBlockAsset colorBlockAsset = (ColorBlockAsset)System.Convert.ChangeType(_gridDataAssetForCurrentLevel.GridLayout[index], _gridDataAssetForCurrentLevel.GridLayout[index].GetType());
                            ColorBlock colorBlock = Instantiate(_colorBlockPrefab, transform).GetComponent<ColorBlock>();
                            colorBlock.transform.localPosition = new Vector3(x, row + 0.5f, 0);
                            colorBlock.Initialize(
                                i,
                                j,
                                index,
                                colorBlockAsset.Gravity,
                                colorBlockAsset.DefaulColorSprite,
                                new Vector3(x, y, 0));
                            colorBlock.SetColorIndex(_gridDataAssetForCurrentLevel.GetColorBlockIndex(colorBlockAsset));
                            _listOfColorBlock.Add(colorBlock);
                            _listOfBlock.Add(colorBlock);
#if UNITY_EDITOR
                            colorBlock.gameObject.name = string.Format(
                                "ColorBlock[{0},{1}]_Index({2}))",
                                i,
                                j,
                                index);
#endif
                        }
                        else if (_gridDataAssetForCurrentLevel.GridLayout[index].GetType() == typeof(ObjectiveBlockAsset)) {

                            ObjectiveBlockAsset objectiveBlockAsset = (ObjectiveBlockAsset)System.Convert.ChangeType(_gridDataAssetForCurrentLevel.GridLayout[index], _gridDataAssetForCurrentLevel.GridLayout[index].GetType());
                            ObjectiveBlock objectiveBlock = Instantiate(_objectiveBlockPrefab, transform).GetComponent<ObjectiveBlock>();
                            objectiveBlock.transform.localPosition = new Vector3(x, row + 0.5f, 0);
                            objectiveBlock.Initialize(
                                i,
                                j,
                                index,
                                objectiveBlockAsset.Gravity,
                                objectiveBlockAsset.DefaulColorSprite,
                                new Vector3(x, y, 0)) ;
                            _listOfObjectiveBlock.Add(objectiveBlock);
                            _listOfBlock.Add(objectiveBlock);
#if UNITY_EDITOR
                            objectiveBlock.gameObject.name = string.Format(
                                "ObjectiveBlock[{0},{1}]_Index({2}))",
                                i,
                                j,
                                index);
#endif
                        }
                        else {
                            Debug.LogError(string.Format("Type unknown. Type = {0}", _gridDataAssetForCurrentLevel.GridLayout[index].GetType()));
                        }
                        x++;
                    }
                    y++;
                }

            } while (CheckIfDeadlockCondition());

            CreateListOfPossibleSolution();
        }

        private void ShuffleTheWholeGrid() {

            _listOfColorBlock = new List<ColorBlock>();

            int row = _gridDataAssetForCurrentLevel.Row;
            int column = _gridDataAssetForCurrentLevel.Column;

            float y = -(row / 2.0f) + 0.5f;
            for (int i = 0; i < row; i++)
            {
                float x = -(column / 2.0f) + 0.5f;
                for (int j = 0; j < column; j++)
                {
                    int index = (i * column) + j;

                    ColorBlock colorBlock = null;
                    if (_listOfBlock[index] == null || (_listOfBlock[index] != null && _listOfBlock[index].TryGetComponent<ColorBlock>(out colorBlock)))
                    {
                        if (colorBlock != null)
                            Destroy(colorBlock.gameObject);

                        //if : Empty || ColorBlock
                        ColorBlockAsset colorBlockAsset = _gridDataAssetForCurrentLevel.ColorBlocks[Random.Range(0, _gridDataAssetForCurrentLevel.NumberOfColorBlock)];
                        colorBlock = Instantiate(_colorBlockPrefab, transform).GetComponent<ColorBlock>();
                        colorBlock.transform.localPosition = new Vector3(x, y, 0);
                        colorBlock.Initialize(
                            i,
                            j,
                            index,
                            colorBlockAsset.Gravity,
                            colorBlockAsset.DefaulColorSprite,
                            new Vector3(x, y, 0));
                        colorBlock.SetColorIndex(_gridDataAssetForCurrentLevel.GetColorBlockIndex(colorBlockAsset));
                        _listOfColorBlock.Add(colorBlock);
                        _listOfBlock[index] = colorBlock;

#if UNITY_EDITOR
                        colorBlock.gameObject.name = string.Format(
                            "ColorBlock[{0},{1}]_Index({2}))",
                            i,
                            j,
                            index);
#endif
                    }
                    x++;
                }
                y++;
            }
            
        }

        private void CreateListOfPossibleSolution()
        {
            _colorBlockMapingForPossibleSolution  = new Dictionary<ColorBlock, int>();
            _listOfPossibleSolutionForColorBlock = new List<List<ColorBlock>>();

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
            if (_listOfBlock[currentIndex] != null
             && _listOfBlock[nextIndex] != null
             && _listOfBlock[currentIndex].TryGetComponent<ColorBlock>(out currentColorBlock)
             && _listOfBlock[nextIndex].TryGetComponent<ColorBlock>(out nextColorBlock))
            {
                if (currentColorBlock.ColorIndex == nextColorBlock.ColorIndex)
                {
                    int currentIndexOnSolutionList = -1;
                    int nextIndexOnSolutionList = -1;

                    bool isCurrentValueFound = _colorBlockMapingForPossibleSolution.TryGetValue(currentColorBlock, out currentIndexOnSolutionList);
                    bool IsNextValueFound = _colorBlockMapingForPossibleSolution.TryGetValue(nextColorBlock, out nextIndexOnSolutionList);

                    if (!IsNextValueFound)
                    {
                        //if : The next is not include in any group
                        if (!isCurrentValueFound)
                        {
                            //if : Current is also not in any group

                            currentIndexOnSolutionList = _listOfPossibleSolutionForColorBlock.Count;
                            _listOfPossibleSolutionForColorBlock.Add(new List<ColorBlock>());
                            _listOfPossibleSolutionForColorBlock[currentIndexOnSolutionList] = new List<ColorBlock>();
                            _listOfPossibleSolutionForColorBlock[currentIndexOnSolutionList].Add(currentColorBlock);
                            _colorBlockMapingForPossibleSolution.Add(currentColorBlock, currentIndexOnSolutionList);
                        }

                        _listOfPossibleSolutionForColorBlock[currentIndexOnSolutionList].Add(nextColorBlock);
                        _colorBlockMapingForPossibleSolution.Add(nextColorBlock, currentIndexOnSolutionList);
                    }
                    else
                    {

                        if (!isCurrentValueFound)
                        {
                            //if : Next is in group but current is not in group
                            _listOfPossibleSolutionForColorBlock[nextIndexOnSolutionList].Add(currentColorBlock);
                            _colorBlockMapingForPossibleSolution.Add(currentColorBlock, nextIndexOnSolutionList);
                        }
                        else
                        {

                            //if : "Next and Current Both are in a group" && Not the same groupd -> Merge to next group

                            if (currentIndexOnSolutionList != nextIndexOnSolutionList)
                            {
                                int sizeOfOfCurrentSolution = _listOfPossibleSolutionForColorBlock[currentIndexOnSolutionList].Count;

                                for (int i = 0; i < sizeOfOfCurrentSolution; i++)
                                {
                                    _listOfPossibleSolutionForColorBlock[nextIndexOnSolutionList].Add(_listOfPossibleSolutionForColorBlock[currentIndexOnSolutionList][i]);
                                    _colorBlockMapingForPossibleSolution.Remove(_listOfPossibleSolutionForColorBlock[currentIndexOnSolutionList][i]);
                                    _colorBlockMapingForPossibleSolution.Add(_listOfPossibleSolutionForColorBlock[currentIndexOnSolutionList][i], nextIndexOnSolutionList);
                                }

                                _listOfPossibleSolutionForColorBlock[currentIndexOnSolutionList].Clear();
                            }
                        }
                    }
                }
            }
        }

        private void ColorTheGroup()
        {
            int numberOfSolutionList = _listOfPossibleSolutionForColorBlock.Count;
            for (int i = 0; i < numberOfSolutionList; i++)
            {
                int sizeOfGroup         = _listOfPossibleSolutionForColorBlock[i].Count;

                if (sizeOfGroup > 0)
                {
                    int colorIndex = _listOfPossibleSolutionForColorBlock[i][0].ColorIndex;

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
                        _listOfPossibleSolutionForColorBlock[i][j].ChangeSprite(colorOfGrid);
                    }
                }
            }
        }

        #endregion

        #region Configuretion   :   OnUserInteraction

        private ColorBlock FillTheGridWithColorBlock(int i, int j, int index)
        {
            int row = _gridDataAssetForCurrentLevel.Row;
            int column = _gridDataAssetForCurrentLevel.Column;

            float x = (-(column / 2.0f) + 0.5f) + Mathf.Lerp(0, column, ((float)j) / column);
            float y = (-(row / 2.0f) + 0.5f) + Mathf.Lerp(0, row, ((float)i) / row);

            int gridColorIndex = Random.Range(0, _gridDataAssetForCurrentLevel.NumberOfColorBlock);
            ColorBlock colorBlock = Instantiate(_colorBlockPrefab, transform).GetComponent<ColorBlock>();
            colorBlock.transform.localPosition = new Vector3(x, row + 0.5f, 0);
            colorBlock.Initialize(
                i,
                j,
                index,
                _gridDataAssetForCurrentLevel.ColorBlocks[gridColorIndex].Gravity,
                _gridDataAssetForCurrentLevel.ColorBlocks[gridColorIndex].DefaulColorSprite,
                new Vector3(x, y, 0));
            colorBlock.SetColorIndex(gridColorIndex);
            _listOfColorBlock.Add(colorBlock);
            _listOfBlock.Add(colorBlock);
#if UNITY_EDITOR
            colorBlock.gameObject.name = string.Format(
                "ColorBlock[{0},{1}]_Index({2}))",
                i,
                j,
                index);
#endif
            return colorBlock;
        }


        private void RefillTheGrid()
        {
            int row = _gridDataAssetForCurrentLevel.Row;
            int column = _gridDataAssetForCurrentLevel.Column;

            List<int> listOfShuffeledColumn = new List<int>();

            float posY = (-(row / 2.0f) + 0.5f);
            for (int i = 0; i < row; i++)
            {

                float posX = (-(column / 2.0f) + 0.5f);
                for (int j = 0; j < column; j++)
                {
                    int index   = (i * column) + j;

                    if (_listOfBlock[index] == null)
                    {
                        //if : Empty
                        for (int k = i + 1; k < row; k++)
                        {
                            int indexInNextRow = (k * column) + j;
                            if (_listOfBlock[indexInNextRow] != null)
                            {
                                InteractableBlock interactableBlock = null;
                                if (_listOfBlock[indexInNextRow].TryGetComponent<InteractableBlock>(out interactableBlock))
                                {
                                    if (interactableBlock.IsImpactByGravity)
                                    {
                                        _listOfBlock[index]         = _listOfBlock[indexInNextRow];
                                        _listOfBlock[indexInNextRow]= null;

                                        _listOfBlock[index].UpdateGridInfo(
                                                i,
                                                j,
                                                index
                                            );
                                        _listOfBlock[index].Move(
                                                new Vector3(
                                                        posX,
                                                        posY,
                                                        0
                                                    ),
                                                0.5f
                                            );
                                    }
                                }
                                break;
                            }
                        }
                    }
                    posX++;
                }
                posY++;
            }
        }

        private void TryToGetObjectiveBlock(int index)
        {
            ObjectiveBlock objectiveBlock = null;
            if (_listOfBlock[index] != null && _listOfBlock[index].TryGetComponent<ObjectiveBlock>(out objectiveBlock))
            {
                foreach (ObjectiveBlockAsset objectiveBlockAsset in _gridDataAssetForCurrentLevel.ObjectiveBlocks)
                {
                    if (objectiveBlockAsset.DefaulColorSprite == _listOfBlock[index].BlockImage)
                    {
                        int counter = 0;
                        foreach (ObjectiveBlock remainingObjectiveBlock in _listOfObjectiveBlock)
                        {
                            if (remainingObjectiveBlock.BlockImage == objectiveBlockAsset.DefaulColorSprite)
                                counter++;
                        }
                        _gameManager.GridDataManagerReference.PassRemainingNumberOfObjective(objectiveBlockAsset, counter - 1);
                    }
                }

                _listOfObjectiveBlock.Remove(objectiveBlock);
                _listOfBlock[index] = null;
                objectiveBlock.Disappear();

                
            }
        }

        private void OnTouchedSolution(List<ColorBlock> listOfSolution)
        {
            int sizeOfSolution = listOfSolution.Count;
            int row = _gridDataAssetForCurrentLevel.Row;
            int column = _gridDataAssetForCurrentLevel.Column;
            for (int i = 0; i < sizeOfSolution; i++)
            {
                int index = listOfSolution[i].Index;

                //Upper Row
                if ((listOfSolution[i].RowIndex + 1) < row)
                {
                    int upperRow = ((listOfSolution[i].RowIndex + 1) * column) + listOfSolution[i].ColumnIndex;
                    TryToGetObjectiveBlock(upperRow);
                }

                //Down Row
                if ((listOfSolution[i].RowIndex - 1) >= 0)
                {
                    int downRow = ((listOfSolution[i].RowIndex - 1) * column) + listOfSolution[i].ColumnIndex;
                    TryToGetObjectiveBlock(downRow);
                }

                //Next Column
                if ((listOfSolution[i].ColumnIndex + 1) < column)
                {
                    int nextColumn = (listOfSolution[i].RowIndex * column) + (listOfSolution[i].ColumnIndex + 1);
                    TryToGetObjectiveBlock(nextColumn);
                }

                //Previous Column
                if ((listOfSolution[i].ColumnIndex - 1) >= 0)
                {
                    int previousColumn = (listOfSolution[i].RowIndex * column) + (listOfSolution[i].ColumnIndex - 1);
                    TryToGetObjectiveBlock(previousColumn);
                }

                listOfSolution[i].Disappear();
                _listOfBlock[index] = null;
            }

        }

        private void OnRecievingTheTouchedColorGrid(InteractableBlock touchedGrid)
        {

            ColorBlock colorBlock;
            if (touchedGrid.TryGetComponent<ColorBlock>(out colorBlock))
            {
                Debug.Log(string.Format("Touched : ColorBlock = {0}", colorBlock.name));
                int solutionIndex = -1;
                if (_colorBlockMapingForPossibleSolution.TryGetValue(colorBlock, out solutionIndex))
                {
                    RemainingNumberOfMove--;
                    _gameManager.GridDataManagerReference.PassRemainingNumberOfMove(RemainingNumberOfMove);

                    _userInputOnColorGrid.SetInputStatus(false);
                    OnTouchedSolution(_listOfPossibleSolutionForColorBlock[solutionIndex]);

                    if (_listOfObjectiveBlock.Count <= 0)
                    {
                        DOVirtual.DelayedCall(
                                1,
                                () =>
                                {
                                    _gameManager.OnLevelCompleteEvent.Raise();
                                }
                            );
                        
                    }
                    else if (RemainingNumberOfMove <= 0)
                    {
                        DOVirtual.DelayedCall(
                                1,
                                () =>
                                {
                                    _gameManager.OnLevelFailedEvent.Raise();
                                }
                            );
                        
                    }
                    else {

                        RefillTheGrid();
                        while (CheckIfDeadlockCondition())
                        {
                            ShuffleTheWholeGrid();
                        }

                        CreateListOfPossibleSolution();

                        _userInputOnColorGrid.SetInputStatus(true);
                    }

                    
                }
                else {
                    Debug.LogError(string.Format("SolutionIndex = {0}", solutionIndex));
                }
            }
            else 
                Debug.LogError(string.Format("ColorBlock not found"));
            
        }

        #endregion

        #endregion ALL SELF DECLEAR FUNCTIONS

    }
}
