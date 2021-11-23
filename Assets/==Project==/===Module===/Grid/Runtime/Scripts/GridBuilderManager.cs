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

            FillColorGrid();

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

        private GridDataAsset _gridDataAssetForCurrentLevel;
        private List<Grid> _listOfColorOnGrid = new List<Grid>();

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
                    int index = (i * row) + j;
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
                                if (colorGrid.ColorOfGrid == colorGridOnUpperRow.ColorOfGrid)
                                {
                                    isDeadLock = false;
                                    break;
                                }
                            }
                        }

                        //if : Next Column is valid
                        if ((j + 1) < column)
                        {
                            int nextColumnIndex = (i * row) + (j + 1);
                            ColorGrid colorGridOnNextColumn = (ColorGrid)_listOfColorOnGrid[nextColumnIndex];

                            if (colorGridOnNextColumn != null)
                            {

                            }
                        }
                    }

                    if (isDeadLock)
                        break;
                }
            }

            return isDeadLock;
        }

        private void FillColorGrid()
        {
            int row = _gridDataAssetForCurrentLevel.Row;
            int column = _gridDataAssetForCurrentLevel.Column;

            float x = -(row / 2.0f) + 0.5f;
            for (int i = 0; i < row; i++)
            {
                float y = -(column / 2.0f) + 0.5f;
                for (int j = 0; j < _gridDataAssetForCurrentLevel.Column; j++)
                {
                    int index = (i * row) + j;

                    Grid grid = Instantiate(_colorGridPrefab, transform).GetComponent<Grid>();
                    grid.Initialize(
                        i,
                        j,
                        index,
                        _gridDataAssetForCurrentLevel.GetRandomDefaultColorSprite(),
                        new Vector3(y, x, 0));

                    y++;
                }
                x++;
            }
        }

        private void OnRecievingTheTouchedColorGrid(Grid touchedGrid)
        {

        }

        #endregion

        #endregion ALL SELF DECLEAR FUNCTIONS

    }
}
