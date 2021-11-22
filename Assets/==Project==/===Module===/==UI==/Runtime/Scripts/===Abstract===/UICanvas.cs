namespace Project.UI
{
    using UnityEngine;
    using Project.Shared;

    public abstract class UICanvas : GameBehaviour
    {
        #region Private Variables

        [SerializeField] protected GameObject _rootObject;

        #endregion

        #region Mono Behaviour

        protected virtual void Awake()
        {
            OnEnabledEvent  += OnCavasDisabled;
            OnDisabledEvent += OnCavasDisabled;
        }

        protected virtual void OnDestroy()
        {
            OnEnabledEvent  -= OnCavasDisabled;
            OnDisabledEvent -= OnCavasDisabled;
        }

        #endregion

        #region Abstract Method

        protected abstract void OnCanvasEnabled();
        protected abstract void OnCavasDisabled();

        #endregion

        #region Public Callback

        public void SetCanvasVisibility(bool value)
        {
            _rootObject.SetActive(value);
        }

        #endregion
    }
}

