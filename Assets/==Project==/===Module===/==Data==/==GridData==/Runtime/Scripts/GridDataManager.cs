namespace Project.Data.Grid
{
    using System.Collections.Generic;
    using UnityEngine;
    using Project.Data;

    [CreateAssetMenu(fileName = "GridDataManager", menuName = GameConstant.GAME_NAME + "/Grid/GridDataManager")]
    public class GridDataManager : ScriptableObject
    {
        #region Public Variables

        public List<GridDataAsset> GridsData { get { return _listOfGridData; } }

        #endregion

        #region Private Variables

        [SerializeField] private List<GridDataAsset> _listOfGridData;

        #endregion
    }
}

