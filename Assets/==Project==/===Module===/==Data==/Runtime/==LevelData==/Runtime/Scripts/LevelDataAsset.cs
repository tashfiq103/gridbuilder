namespace Project.Data.LevelData
{
    using UnityEngine;
    using com.faith.core;
    using Project.Data.PlayableArea;

    [CreateAssetMenu(fileName = "LevelDataAsset", menuName =  GameConstant.GAME_NAME + "/LevelData/LevelDataAsset")]
    public class LevelDataAsset : ScriptableObject
    {
        #region Public Variables

        public bool IsBonusLevel { get { return _isBonusLevel; } }
        public SceneReference LevelScene { get { return _levelScene; } }
        public GridDataAsset GridData { get { return _gridData; } }

        #endregion

        #region Private Variables

        [SerializeField] private bool _isBonusLevel;
        [SerializeField] private SceneReference _levelScene;
        [SerializeField] private GridDataAsset _gridData;

        #endregion
    }
}

