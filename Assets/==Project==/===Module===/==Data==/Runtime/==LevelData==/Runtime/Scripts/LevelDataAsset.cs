namespace Project.Data.LevelData
{
    using UnityEngine;
    using com.faith.core;

    [CreateAssetMenu(fileName = "LevelDataAsset", menuName =  GameConstant.GAME_NAME + "/LevelData/LevelDataAsset")]
    public class LevelDataAsset : ScriptableObject
    {
        #region Public Variables

        public bool IsBonusLevel { get { return _isBonusLevel; } }
        public SceneReference LevelScene { get { return _levelScene; } }

        #endregion

        #region Private Variables

        [SerializeField] private bool _isBonusLevel;
        [SerializeField] private SceneReference _levelScene;


        #endregion
    }
}

