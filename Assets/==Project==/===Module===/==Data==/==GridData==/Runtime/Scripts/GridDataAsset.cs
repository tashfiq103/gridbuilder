namespace Project.Data.Grid
{
    using System.Collections.Generic;
    using UnityEngine;
    using Project.Data;
    using Toolset.Enum;

    [CreateAssetMenu(fileName = "GridDataAsset", menuName = GameConstant.GAME_NAME + "/Grid/GridDataAsset")]
    public class GridDataAsset : EnumAsset
    {
        #region Custom Variables

        [System.Serializable]
        private class GridColorGroup
        {
            #region Public Variables

            public int MinNumberOfGroupSize { get { return _groupSizeGreaterThan + 1; } }
            public Sprite ColorSprite { get { return _gridColorSprite; } }

            #endregion

            #region Private Variables

            [SerializeField] private int            _groupSizeGreaterThan;
            [SerializeField] private Sprite         _gridColorSprite;

            #endregion
        }

        [System.Serializable]
        private class GridColor
        {
            #region Public Variables

            public Sprite               DefaulColorSprite { get { return _defaultColorSprite; } }
            public List<GridColorGroup> ColorSpriteForGroup { get { return _colorSpriteForGroup; } }

            #endregion

            #region Private Variables

            [SerializeField] private Sprite _defaultColorSprite;
            [SerializeField] private List<GridColorGroup> _colorSpriteForGroup;

            #endregion

        }

        #endregion

        #region Private Variables

        [SerializeField, Range(2,10)]   private int _row = 2;
        [SerializeField, Range(2, 10)]  private int _column = 2;
        [SerializeField] private List<GridColor> _colors;

        #endregion
    }
}

