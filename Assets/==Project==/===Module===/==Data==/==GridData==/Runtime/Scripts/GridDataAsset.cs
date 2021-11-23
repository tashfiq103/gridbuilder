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
        public class GridColorGroup
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
        public class GridColor
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

        #region Public Variables

        public int NumberOfColor { get { return _colors.Count; } }
        public int Row { get { return _row; } }
        public int Column { get { return _column; } }
        public List<GridColor> Colors { get { return _colors; } }

        #endregion

        #region Private Variables

        [SerializeField, Range(2,10)]   private int _row = 2;
        [SerializeField, Range(2, 10)]  private int _column = 2;
        [SerializeField] private List<GridColor> _colors;

        #endregion

        #region Public Callback

        public Sprite GetRandomDefaultColorSprite()
        {
            return _colors[Random.Range(0, NumberOfColor)].DefaulColorSprite;
        }

        #endregion
    }
}

