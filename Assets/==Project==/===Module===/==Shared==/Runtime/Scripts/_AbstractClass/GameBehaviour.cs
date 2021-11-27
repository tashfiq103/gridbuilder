namespace Project.Shared
{

    using System;
    using Toolset.GameEvent;

    public abstract class GameBehaviour : GameEventListener
    {
        #region Public Variables

        public bool IsRegistered { get; private set; }

        public event Action OnEnabledEvent;
        public event Action OnDisabledEvent;

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
                _gameManager.OnLevelCompleteEvent.RegisterEvent(this, OnLevelCompleted);
                _gameManager.OnLevelFailedEvent.RegisterEvent(this, OnLevelFailed);
            }

            OnEnabledEvent?.Invoke();
        }

        protected virtual void OnDisable()
        {
            if (IsRegistered)
            {
                _gameManager.OnSceneLoadedEvent.UnregisterEvent(this, OnSceneLoaded);
                _gameManager.OnDataLoadedEvent.UnregisterEvent(this, OnDataLoaded);
                _gameManager.OnLevelStartedEvent.UnregisterEvent(this, OnLevelStarted);
                _gameManager.OnLevelCompleteEvent.UnregisterEvent(this, OnLevelCompleted);
                _gameManager.OnLevelFailedEvent.UnregisterEvent(this, OnLevelFailed);
            }

            OnDisabledEvent?.Invoke();
        }

        #endregion

        #region Virtual Method

        protected virtual void OnSceneLoaded() { }
        protected virtual void OnDataLoaded() { }
        protected virtual void OnLevelStarted() { }
        protected virtual void OnLevelCompleted() { }
        protected virtual void OnLevelFailed() { }

        #endregion

    }
}

