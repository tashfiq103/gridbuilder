namespace Project.Data.LevelData
{
    using UnityEngine;
    using UnityEngine.Events;
    using com.faith.core;


    [CreateAssetMenu(fileName = "LevelDataManager", menuName = GameConstant.GAME_NAME + "/LevelDataManager")]
    public class LevelDataManager : ScriptableObject
    {
        #region Custom Variables

        [System.Serializable]
        public class LevelInformation
        {
            public bool IsBonusLevel { get { return _levelDataAsset.IsBonusLevel; } }
            public SceneReference LevelScene { get { return _levelDataAsset.LevelScene; } }

            [SerializeField] private LevelDataAsset _levelDataAsset;
        }

        #endregion

        #region Public Variables

        public LevelInformation[] LevelInformationReference { get { return _levelInformation; } }

        public int NumberOfLevel { get { return LevelInformationReference.Length; } }

        public int GetNumberOfAttemptOnThisLevel { get { return _numberOfAttempPerLevel.GetData(); } }
        public int GetLevelIndex { get { return _levelIndex.GetData(); } }
        public int GetIncrementalLevelIndex { get { return _incrementalLevelIndex.GetData() + 1; } }

        public bool IsNextLevelIsBonusLevel
        {
            get
            {
                int nextLevelIndex = _levelIndex.GetData() + 1;
                if (!IsValidLevelIndex(nextLevelIndex))
                    nextLevelIndex = 0;

                return LevelInformationReference[nextLevelIndex].IsBonusLevel;
            }
        }

        #endregion

        #region Private Variables

        private bool _isDataLoaded = false;
        private SavedData<int> _levelIndex;
        private SavedData<int> _incrementalLevelIndex;
        private SavedData<int> _numberOfAttempPerLevel;

        [Space(5.0f)]
        [SerializeField] private LevelInformation[]     _levelInformation;

        #endregion

        #region Configuretion

        private bool IsValidLevelIndex(int dayIndex)
        {

            if (dayIndex >= 0 && dayIndex < NumberOfLevel)
            {

                return true;
            }

            return false;
        }

        #endregion

        #region Public Callback

        public void Initialization(UnityAction OnLevelDataLoaded)
        {
            _isDataLoaded = false;

            _numberOfAttempPerLevel = new SavedData<int>(
                    "NUMBER_OF_ATTEMPT_PER_LEVEL",
                    0
                );

            _incrementalLevelIndex = new SavedData<int>(
                    "INCREMENTAL_LEVEL_INDEX",
                    0);
            _levelIndex = new SavedData<int>(
                "LEVEL_INDEX",
                0,
                (int value) => {

                    if (!_isDataLoaded)
                    {
                        _isDataLoaded = true;
                        OnLevelDataLoaded?.Invoke();
                    }
                });
        }

        

        public void UpdateLevelProgressionDataOnLevelFailed()
        {
            int numberOfAttempt = _numberOfAttempPerLevel.GetData();
            _numberOfAttempPerLevel.SetData(numberOfAttempt + 1);
        }

        public void UpdateLevelProgressionDataOnLevelComplete()
        {

            int nextLevel = _levelIndex.GetData() + 1;
            if (IsValidLevelIndex(nextLevel))
            {
                _levelIndex.SetData(nextLevel);
            }
            else
            {
                _levelIndex.SetData(0);
            }

            int currentData = _incrementalLevelIndex.GetData();
            _incrementalLevelIndex.SetData(currentData + 1);
            _numberOfAttempPerLevel.SetData(0);
        }

        

        #endregion

    }

}
