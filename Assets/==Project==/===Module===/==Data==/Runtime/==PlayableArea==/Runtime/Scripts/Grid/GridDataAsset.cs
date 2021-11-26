namespace Project.Data.PlayableArea
{
    using System.Collections.Generic;
    using UnityEngine;
    using Project.Data;
    using Toolset.Enum;

    [CreateAssetMenu(fileName = "GridDataAsset", menuName = GameConstant.GAME_NAME + "/PlayableArea/Grid/GridDataAsset")]
    public class GridDataAsset : EnumAsset
    {

        #region Public Variables

        public int NumberOfColorBlock { get { return _colorBlocks.Count; } }
        public int NumberOfObjectiveBlock { get { return _objectiveBlocks.Count; } }
        public int Row { get { return _row; } }
        public int Column { get { return _column; } }
        public List<ObjectiveBlockAsset> ObjectiveBlocks { get { return _objectiveBlocks; } }
        public List<ColorBlockAsset> ColorBlocks { get { return _colorBlocks; } }
        public List<Object> GridLayout { get { return _gridLayout; } }


        #endregion

        #region Private Variables

#if UNITY_EDITOR

        [SerializeField] private int _gridBuildOption = 0;
        [SerializeField] private int _selectedInteractableBlockIndex;
        [SerializeField] private InteractableBlockAsset _selectedInteractableBlock;
        [SerializeField] private bool _showGridNumber;

#endif

        [SerializeField, Range(1,10)]   private int _row = 2;
        [SerializeField, Range(1,10)]  private int _column = 2;
        [SerializeField] private List<ObjectiveBlockAsset> _objectiveBlocks;
        [SerializeField] private List<ColorBlockAsset> _colorBlocks;
        [SerializeField] private List<Object> _gridLayout;

        #endregion

        #region Public Callback

        public Sprite GetRandomDefaultColorSprite()
        {
            return _colorBlocks[Random.Range(0, NumberOfColorBlock)].DefaulColorSprite;
        }

        public int GetColorBlockIndex(ColorBlockAsset colorBlockAsset)
        {
            for (int i = 0; i < NumberOfColorBlock; i++)
            {
                if (_colorBlocks[i] == colorBlockAsset)
                    return i;
            }

            return -1;
        }

        public int GetObjectiveBlockIndex(ObjectiveBlockAsset objectiveBlockAsset)
        {
            for (int i = 0; i < NumberOfObjectiveBlock; i++)
            {
                if (_objectiveBlocks[i] == objectiveBlockAsset)
                    return i;
            }

            return -1;
        }

        #endregion
    }
}

