namespace Toolset.GameEvent.Raycast
{
    using UnityEngine;
    using com.faith.gameplay.service;

    public abstract class Raycast2DEventFromCamera : Raycast2DEvent
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

            _alwaysRaycast      = false;
            _mainCameraReference= Camera.main;

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

            RaycastHit2D raycastHit = Raycaster();
            if (raycastHit.collider != null)
                RaycastHitOnTouchDown(raycastHit);

            _IsTouchingTheScreen = true;
        }

        private void OnTouch(Vector3 touchPosition, int touchIndex)
        {
            _touchPosition = touchPosition;

            RaycastHit2D raycastHit = Raycaster();
            if (raycastHit.collider != null)
                RaycastHitOnTouch(raycastHit);
        }

        private void OnTouchUp(Vector3 touchPosition, int touchIndex)
        {
            _touchPosition = touchPosition;

            RaycastHit2D raycastHit = Raycaster();
            if (raycastHit.collider != null)
                RaycastHitOnTouchUp(raycastHit);

            _IsTouchingTheScreen = false;

            
        }
        #endregion

        #region Override Method

        protected override Vector3 GetOrigin()
        {
            return _mainCameraReference.ScreenToWorldPoint(_touchPosition);
        }

        protected override Vector3 GetDirection()
        {
            return Vector3.zero;
        }

        #endregion

        #region Abstract Method

        protected abstract void RaycastHitOnTouchDown(RaycastHit2D raycastHit2D);
        protected abstract void RaycastHitOnTouch(RaycastHit2D raycastHit2D);
        protected abstract void RaycastHitOnTouchUp(RaycastHit2D raycastHit2D);

        #endregion

    }
}

