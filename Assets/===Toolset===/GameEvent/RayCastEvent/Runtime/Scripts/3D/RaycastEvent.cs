namespace Toolset.GameEvent.Raycast
{
    using System;
    using UnityEngine;
    using com.faith.core;

    public abstract class RaycastEvent : MonoBehaviour
    {
        #region Public Variables

        public bool IsRaycasting { get { return _updateThreadForRayCasting.IsUpdateThreadRunning; } }

        public event Action OnRaycastingStartEvent;
        public event Action OnRaycastingEndEvent;
        public event Action<RaycastHit> OnRaycastHitEvent;

        #endregion

        #region Private Variables

#if UNITY_EDITOR
        [Header("Parameter  :   Editor")]
        [SerializeField] private bool _showDebugRay = false;
        [SerializeField] private FloatReference _durationOfEachRay;
        [SerializeField] private Color _colorOfDebugRay = Color.blue;
#endif



        [Header("Parameter")]
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
            _updateThreadForRayCasting = new BatchedUpdateThread(Raycaster);
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
            OnRaycastingStartEvent -= OnRaycastingStart;
            OnRaycastHitEvent -= OnRaycastHit;
            OnRaycastingEndEvent -= OnRaycastingEnd;
        }

        #endregion

        #region Configuretion

        private void Raycaster()
        {
            Ray ray = GetRay();
            RaycastHit raycastHit;
            if (Physics.Raycast(ray, out raycastHit, _maxDistanceForRay.Value, _rayCastingLayer))
            {
                OnRaycastHitEvent.Invoke(raycastHit);
            }

#if UNITY_EDITOR
            if (_showDebugRay)
                Debug.DrawRay(ray.origin, ray.direction * _maxDistanceForRay.Value, _colorOfDebugRay, _durationOfEachRay.Value); 
#endif
        }

        #endregion

        #region Public Variables

        public void StartRayCasting()
        {
            _updateThreadForRayCasting.StartUpdate(_rayCastUpdateFrequency);
            OnRaycastingStartEvent.Invoke();
        }

        public void StopRaycasting()
        {
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

        protected abstract Ray GetRay();
        protected abstract void OnRaycastHit(RaycastHit raycastHit);
        protected abstract void OnRaycastingStart();
        protected abstract void OnRaycastingEnd();

        #endregion

    }
}

