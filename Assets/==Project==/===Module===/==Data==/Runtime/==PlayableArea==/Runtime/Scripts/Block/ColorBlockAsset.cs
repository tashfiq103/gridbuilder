namespace Project.Data.PlayableArea
{
    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu(fileName = "ColorBlockAsset", menuName = GameConstant.GAME_NAME + "/PlayableArea/Block/ColorBlockAsset")]
    public class ColorBlockAsset : InteractableBlockAsset
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

        public List<GridColorGroup> ColorSpriteForGroup { get { return _colorSpriteForGroup; } }

        #endregion

        #region Private Variables

        [SerializeField] private List<GridColorGroup> _colorSpriteForGroup;

        #endregion
    }
}

