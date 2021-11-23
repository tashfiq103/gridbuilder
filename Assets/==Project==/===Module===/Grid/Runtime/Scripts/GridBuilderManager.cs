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

            int levelIndex                  = _gameManager.LevelDataManagerReference.GetLevelIndex;
            _gridDataAssetForCurrentLevel   = _gameManager.GridDataManagerReference.GridsData[levelIndex];

            FillColorGrid();

            _userInputOnColorGrid.StartRayCasting();
        }

        protected override void OnLevelEnded()
        {
            base.OnLevelEnded();

            _userInputOnColorGrid.StopRaycasting();
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

        #endregion

        #endregion ALL SELF DECLEAR FUNCTIONS

    }
}
