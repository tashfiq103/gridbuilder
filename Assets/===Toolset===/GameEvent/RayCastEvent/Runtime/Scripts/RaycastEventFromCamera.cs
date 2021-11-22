namespace Toolset.GameEvent.Raycast
{
    using UnityEngine;
    using com.faith.gameplay.service;

    public abstract class RaycastEventFromCamera : RaycastEvent
    {

        #region Protected Variables

        protected bool _IsTouchingTheScreen { get; private set; }

        #endregion

        #region Private Variables

        private Camera  _mainCameraReference;
        private Vector3 _touchPosition;

        #endregion

        #region Mono Behaviour

        protected override void Awake()
        {
            base.Awake();

            _mainCameraReference = Camera.main;

        }

        protected override void OnEnable()
        {
            base.OnEnable();

            OnRaycastingStartEvent  += EnableTouch;
            OnRaycastingEndEvent    += DisableTouch;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            OnRaycastingStartEvent  -= EnableTouch;
            OnRaycastingEndEvent    -= DisableTouch;
        }

        #endregion

        #region Configuretion

        private void EnableTouch() {

            GlobalTouchController.Instance.EnableTouchController();

            GlobalTouchController.Instance.OnTouchDown += OnTouchDown;
            GlobalTouchController.Instance.OnTouch += OnTouch;
            GlobalTouchController.Instance.OnTouchUp += OnTouchUp;
        }

        private void DisableTouch() {

            GlobalTouchController.Instance.OnTouchDown -= OnTouchDown;
            GlobalTouchController.Instance.OnTouch -= OnTouch;
            GlobalTouchController.Instance.OnTouchUp -= OnTouchUp;

            GlobalTouchController.Instance.DisableTouchController();
        }

        private void OnTouchDown(Vector3 touchPosition, int touchIndex)
        {
            _touchPosition = touchPosition;
            _IsTouchingTheScreen = true;
        }

        private void OnTouch(Vector3 touchPosition, int touchIndex)
        {
            _touchPosition = touchPosition;
        }

        private void OnTouchUp(Vector3 touchPosition, int touchIndex)
        {
            _touchPosition = touchPosition;
            _IsTouchingTheScreen = false;
        }

        #endregion

        #region Abstract Method


        protected override Ray GetRay()
        {
            return _mainCameraReference.ScreenPointToRay(_touchPosition);
        }


        #endregion

    }
}

