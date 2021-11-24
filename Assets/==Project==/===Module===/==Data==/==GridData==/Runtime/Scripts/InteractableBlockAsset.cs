namespace Project.Data.Grid
{
    using System.Collections.Generic;
    using UnityEngine;
    using Toolset.Enum;

    [CreateAssetMenu(fileName = "InteractableBlockAsset", menuName = GameConstant.GAME_NAME + "/Grid/InteractableBlockAsset")]
    public class InteractableBlockAsset : EnumAsset
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

            [SerializeField] private int _groupSizeGreaterThan;
            [SerializeField] private Sprite _gridColorSprite;

            #endregion
        }

        #endregion

        #region Public Variables

        public Sprite DefaulColorSprite { get { return _defaultColorSprite; } }
        public List<GridColorGroup> ColorSpriteForGroup { get { return _colorSpriteForGroup; } }

        #endregion

        #region Private Variables

        [SerializeField] private Sprite _defaultColorSprite;
        [SerializeField] private List<GridColorGroup> _colorSpriteForGroup;

        #endregion
    }
}

