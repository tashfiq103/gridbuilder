namespace Project.Data.Grid
{
    using System.Collections.Generic;
    using UnityEngine;
    using Project.Data;

    [CreateAssetMenu(fileName = "GridDataAsset", menuName = GameConstant.GAME_NAME + "/Grid/GridDataAsset")]
    public class GridDataAsset : ScriptableObject
    {
        #region Custom Variables

        [System.Serializable]
        private class GridColor
        {
            #region Public Variables
            public Sprite ColorSprite { get { return _gridColorSprite; } }
            #endregion

            #region Private Variables
            [SerializeField] private Sprite _gridColorSprite;
            #endregion
        }

        [System.Serializable]
        private class GridData
        {
            [SerializeField] private GridColor _defaultColor;
            [SerializeField] private List<GridColor> _color;
        }

        #endregion

        #region Private Variables

        [SerializeField, Range(2,10)]   private int _row;
        [SerializeField, Range(2, 10)]  private int _column;
        [SerializeField] private List<Color> _colors;

        #endregion
    }
}

