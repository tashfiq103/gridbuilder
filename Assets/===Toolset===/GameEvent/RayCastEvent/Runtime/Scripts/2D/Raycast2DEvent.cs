namespace Toolset.GameEvent.Raycast
{
    using System;
    using UnityEngine;
    using com.faith.core;

    public abstract class Raycast2DEvent : MonoBehaviour
    {
        #region Public Variables

        public bool IsRaycasting { get { return _updateThreadForRayCasting.IsUpdateThreadRunning; } }

        public event Action OnRaycastingStartEvent;
        public event Action OnRaycastingEndEvent;
        public event Action<RaycastHit2D> OnRaycastHitEvent;

        #endregion

        #region Private Variables

#if UNITY_EDITOR
        [Header("Parameter  :   Editor")]
        [SerializeField] private bool _showDebugRay = false;
        [SerializeField] private FloatReference _durationOfEachRay;
        [SerializeField] private Color _colorOfDebugRay = Color.blue;
#endif



        [Header("Parameter")]
        [SerializeField] protected bool _alwaysRaycast = false;
        [Space(5.0f)]
        [SerializeField, Range(0.1f, 1f)] private float _rayCastFrequency = 0.5f;
        [SerializeField] private LayerMask _rayCastingLayer;
        [SerializeField] private FloatReference _maxDistanceForRay;

        private BatchedUpdateThread _updateThreadForRayCasting;
        private LayerMask _defaultRayCastingLayer;
        private int _rayCastUpdateFrequency;

        #endregion

        #region Mono Behaviour

        protected virtual void Awake()
        {
            _updateThreadForRayCasting = new BatchedUpdateThread(()=> {
                Raycaster();
            });
            _defaultRayCastingLayer = _rayCastingLayer;
            _rayCastUpdateFrequency = (int) Mathf.Lerp(60, 1, _rayCastFrequency);
        }

        protected virtual void OnEnable()
        {
            OnRaycastingStartEvent  += OnRaycastingStart;
            OnRaycastHitEvent       += OnRaycastHit;
            OnRaycastingEndEvent    += OnRaycastingEnd;
        }

        protected virtual void OnDisable()
        {
            OnRaycastingStartEvent  -= OnRaycastingStart;
            OnRaycastHitEvent       -= OnRaycastHit;
            OnRaycastingEndEvent    -= OnRaycastingEnd;
        }

        #endregion

        #region Configuretion

        protected RaycastHit2D Raycaster()
        {
            Vector3 origin = GetOrigin();
            Vector3 direction = GetDirection();
            RaycastHit2D raycastHit = Physics2D.Raycast(origin, direction, _maxDistanceForRay.Value, _rayCastingLayer);
            if (raycastHit.collider != null)
            {
                OnRaycastHitEvent.Invoke(raycastHit);
            }

#if UNITY_EDITOR
            if (_showDebugRay)
                Debug.DrawRay(origin, direction * _maxDistanceForRay.Value, _colorOfDebugRay, _durationOfEachRay.Value);
#endif

            return raycastHit;
        }

        #endregion

        #region Public Variables

        public void StartRayCasting()
        {
            if(_alwaysRaycast)
                _updateThreadForRayCasting.StartUpdate(_rayCastUpdateFrequency);

            OnRaycastingStartEvent.Invoke();
        }

        public void StopRaycasting()
        {
            if (_alwaysRaycast)
                _updateThreadForRayCasting.StopUpdate();

            OnRaycastingEndEvent.Invoke();
        }

        public void ChangeLayerMask(LayerMask layerMask)
        {
            _rayCastingLayer = layerMask;
        }

        public void ResetLayerMask()
        {
            _rayCastingLayer = _defaultRayCastingLayer;
        }

        #endregion

        #region Abstract Method

        protected abstract Vector3 GetOrigin();
        protected abstract Vector3 GetDirection();
        protected abstract void OnRaycastHit(RaycastHit2D raycastHit2D);
        protected abstract void OnRaycastingStart();
        protected abstract void OnRaycastingEnd();

        #endregion

    }
}

