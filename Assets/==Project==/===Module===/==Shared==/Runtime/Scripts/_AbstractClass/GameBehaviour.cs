namespace Project.Shared
{

    using Toolset.GameEvent;

    public abstract class GameBehaviour : GameEventListener
    {
        #region Public Variables

        public bool IsRegistered { get; private set; }

        #endregion

        #region Protected Variables

        protected GameManager _gameManager { get; private set; }

        #endregion

        #region Mono Behaviour

        protected virtual void OnEnable()
        {
            if (!IsRegistered)
            {
                IsRegistered = true;

                _gameManager = GameManager.Instance;

                _gameManager.OnSceneLoadedEvent.RegisterEvent(this, OnSceneLoaded);
                _gameManager.OnDataLoadedEvent.RegisterEvent(this, OnDataLoaded);
                _gameManager.OnLevelStartedEvent.RegisterEvent(this, OnLevelStarted);
                _gameManager.OnLevelEndedEvent.RegisterEvent(this, OnLevelEnded);
                _gameManager.OnRequestingToShowBonusLevelPopUpEvent.RegisterEvent(this, OnRequestingToShowBonusLevelPopUp);
                _gameManager.OnShowingCharacterUnlockPopUpEvent.RegisterEvent(this, OnShowingCharacterUnlockPopUp);
                _gameManager.OnShowingCharacterSkinShopEvent.RegisterEvent(this, OnShowingCharacterSkinShop);
            }

        }

        protected virtual void OnDisable()
        {
            if (IsRegistered)
            {
                _gameManager.OnSceneLoadedEvent.UnregisterEvent(this);
                _gameManager.OnDataLoadedEvent.UnregisterEvent(this);
                _gameManager.OnLevelStartedEvent.UnregisterEvent(this);
                _gameManager.OnLevelEndedEvent.UnregisterEvent(this);
                _gameManager.OnRequestingToShowBonusLevelPopUpEvent.UnregisterEvent(this);
                _gameManager.OnShowingCharacterUnlockPopUpEvent.UnregisterEvent(this);
                _gameManager.OnShowingCharacterSkinShopEvent.UnregisterEvent(this);
            }
        }

        #endregion

        #region Virtual Method

        protected virtual void OnSceneLoaded() { }
        protected virtual void OnDataLoaded() { }
        protected virtual void OnLevelStarted() { }
        protected virtual void OnLevelEnded() { }
        protected virtual void OnRequestingToShowBonusLevelPopUp() { }
        protected virtual void OnShowingCharacterUnlockPopUp() { }
        protected virtual void OnShowingCharacterSkinShop() { }

        #endregion

    }
}

