namespace Project.Shared
{
    using UnityEngine;
    using UnityEngine.Events;
    using Project.Data;
    using Project.Data.LevelData;
    using Project.Data.CurrencyData;
    using Project.Data.Grid;
    using Toolset.GameEvent;
    using com.faith.core;
    

    [DefaultExecutionOrder(GameConstant.EXECUTION_ORDER_GAME_MANAGER)]
    public class GameManager : GameEventListener
    {
        #region Public Variables

        public static GameManager Instance { get; private set; }


        public CurrencyDataManager CurrencyDataManagerReference { get { return _currencyDataManager; } }
        public GridDataManager GridDataManagerReference { get { return _gridDataManager; } }
        public LevelDataManager LevelDataManagerReference { get { return _levelDataManager; } }

        public GameEvent OnDataLoadedEvent { get { return _OnDataLoadedEvent; } }
        public GameEvent OnSceneLoadedEvent { get { return _OnSceneLoadedEvent; } }
        public GameEvent OnLevelStartedEvent { get { return _OnLevelStartedEvent; } }
        public GameEvent OnLevelEndedEvent { get { return _OnLevelEndedEvent; } }
        public GameEvent OnRequestingToShowBonusLevelPopUpEvent { get { return _OnRequestingToShowBonusLevelPopUpEvent; } }
        public GameEvent OnShowingCharacterUnlockPopUpEvent { get { return _OnShowingCharacterUnlockPopUpEvent; } }
        public GameEvent OnShowingCharacterSkinShopEvent { get { return _OnShowingCharacterSkinShopEvent; } }

        #endregion

        #region

        [Header("LevelData")]
        [SerializeField] private CurrencyDataManager _currencyDataManager;
        [SerializeField] private GridDataManager _gridDataManager;
        [SerializeField] private LevelDataManager _levelDataManager;

        [Header("StateEvent")]
        [SerializeField] private GameEvent _OnDataLoadedEvent;
        [SerializeField] private GameEvent _OnSceneLoadedEvent;
        [SerializeField] private GameEvent _OnLevelStartedEvent;
        [SerializeField] private GameEvent _OnLevelEndedEvent;
        [SerializeField] private GameEvent _OnRequestingToShowBonusLevelPopUpEvent;
        [SerializeField] private GameEvent _OnShowingCharacterUnlockPopUpEvent;
        [SerializeField] private GameEvent _OnShowingCharacterSkinShopEvent;

        #endregion

        #region Mono Behaviour

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        #endregion

        #region Configuretion

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void OnGameStart()
        {
            GameManager gameManager = Instantiate(Resources.Load<GameManager>("GameManager"));

            gameManager.CurrencyDataManagerReference.Initialize();
            gameManager.LevelDataManagerReference.Initialization(() =>
            {

                gameManager.OnDataLoadedEvent.Raise();
                gameManager.OnSceneLoadedEvent.Raise();
            });
        }

        #endregion

        #region Public Callback

        public void LoadNextLevel()
        {
            LevelDataManagerReference.UpdateLevelProgressionDataOnLevelComplete();
            int levelIndex = LevelDataManagerReference.GetLevelIndex;
            LoadLevel(
                    LevelDataManagerReference.LevelInformationReference[levelIndex].LevelScene
                );
        }

        public void SkipLevel()
        {
            do
            {
                LevelDataManagerReference.UpdateLevelProgressionDataOnLevelComplete();

            } while (LevelDataManagerReference.LevelInformationReference[LevelDataManagerReference.GetLevelIndex].IsBonusLevel);

            LoadLevel(
                    LevelDataManagerReference.LevelInformationReference[LevelDataManagerReference.GetLevelIndex].LevelScene
                );
        }

        public void LoadLevel(
            SceneReference sceneReference,
            UnityAction<float> OnLoadingProgress = null,
            UnityAction OnSceneLoaded = null,
            float initialDelayToInvokeOnSceneLoaded = -1)
        {

            initialDelayToInvokeOnSceneLoaded = initialDelayToInvokeOnSceneLoaded == -1 ? 0.1f : initialDelayToInvokeOnSceneLoaded;

            sceneReference.LoadScene(
                OnUpdatingProgression: OnLoadingProgress,
                OnSceneLoaded: () => {
                    _OnSceneLoadedEvent.Raise();
                    OnSceneLoaded?.Invoke();
                },
                initalDelayToInvokeOnSceneLoaded: initialDelayToInvokeOnSceneLoaded);


        }

        #endregion

    }

}
