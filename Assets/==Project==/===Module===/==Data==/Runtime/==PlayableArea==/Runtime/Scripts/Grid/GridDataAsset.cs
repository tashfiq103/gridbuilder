namespace Project.Data.PlayableArea
{
    using System.Collections.Generic;
    using UnityEngine;
    using Project.Data;
    using Toolset.Enum;

    [CreateAssetMenu(fileName = "GridDataAsset", menuName = GameConstant.GAME_NAME + "/PlayableArea/Grid/GridDataAsset")]
    public class GridDataAsset : EnumAsset
    {
        #region Custom Variables

        public enum Marker
        {
            Color,
            Objective
        }

        #endregion

        #region Public Variables

        public int NumberOfColor { get { return _colors.Count; } }
        public int Row { get { return _row; } }
        public int Column { get { return _column; } }
        public List<ColorBlockAsset> Colors { get { return _colors; } }
        public Marker[] GridLayout { get { return _gridLayout; } }

        #endregion

        #region Private Variables

#if UNITY_EDITOR

        [SerializeField] private Color _colorForColorGrid = new Color(0, 0.4538971f, 0.5566038f, 1);
        [SerializeField] private Color _colorForObjective = new Color(0.6698113f, 0.6382908f, 0, 1);

#endif

        [SerializeField, Range(2,10)]   private int _row = 2;
        [SerializeField, Range(2, 10)]  private int _column = 2;
        [SerializeField] private List<ColorBlockAsset> _colors;

        [SerializeField] private Marker _marker;
        [SerializeField] private Marker[] _gridLayout = new Marker[1];

        #endregion

        #region Public Callback

        public Sprite GetRandomDefaultColorSprite()
        {
            return _colors[Random.Range(0, NumberOfColor)].DefaulColorSprite;
        }

        #endregion
    }
}

