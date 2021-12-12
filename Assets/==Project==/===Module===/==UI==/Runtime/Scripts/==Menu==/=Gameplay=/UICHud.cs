namespace Project.UI
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;
    using DG.Tweening;
    using TMPro;
    using Project.Data.PlayableArea;

    public class UICHud : UICanvas
    {
        #region CustomVariables

        private class ObjectiveBlockInfo
        {
            public int InfoIndex { get; private set; }
            public ObjectiveBlockAsset  ObjectiveBlockAssetReference { get; private set; }
            public int                  RemainingNumberOfObjective { get; private set; }

            private UnityAction<int> OnPassingTheValue;

            public ObjectiveBlockInfo(int infoIndex,ObjectiveBlockAsset objectiveBlockAsset, int remainingNumber, UnityAction<int> OnPassingTheValue)
            {
                InfoIndex = infoIndex;
                ObjectiveBlockAssetReference    = objectiveBlockAsset;
                RemainingNumberOfObjective      = remainingNumber;
                this.OnPassingTheValue          = OnPassingTheValue;
            }

            public void Update(int value)
            {
                RemainingNumberOfObjective = value;
                OnPassingTheValue?.Invoke(InfoIndex);
            }
        }

        #endregion

        #region Private Variables


        [Header("Parameter  :   UI")]
        [SerializeField] private RectTransform _moveInfoRectTransform;
        [SerializeField] private RectTransform _objectiveRectTransform;

        [Space(5.0f)]
        [SerializeField] private TextMeshProUGUI _remainingNumberOfMoveText;
        [SerializeField] private TextMeshProUGUI _remainingNumberOfObjectiveText;
        [SerializeField] private TextMeshProUGUI _levelNumberText;
        [SerializeField] private Image          _objectiveIcon;
        [SerializeField] private float          _delayOnSwapingObjective;

        private bool _isShowingObjective;
        private bool _isResetTime;
        private int _indexOfCurrentInfo;
        private GridDataAsset _currentGridDataAsset;
        private List<ObjectiveBlockInfo> _listOfObjectiveBlockInfo;

        #endregion

        #region Configuretion

        private void OnUpdatingUIRemainingNumberOfMove(int value)
        {
            _remainingNumberOfMoveText.text = value.ToString();
        }

        private void OnUpdatingUIForObjective(int infoIndex)
        {
            _indexOfCurrentInfo = infoIndex;
            _isResetTime = true;
        }

        private void OnUpdatingRemainingNumberOfObjective(ObjectiveBlockAsset objectiveBlockAsset, int value)
        {
            foreach (ObjectiveBlockInfo objectiveBlockInfo in _listOfObjectiveBlockInfo)
            {
                if (objectiveBlockInfo.ObjectiveBlockAssetReference == objectiveBlockAsset)
                {
                    objectiveBlockInfo.Update(value);
                }
            }
        }

        private IEnumerator ShowingObjective()
        {
            _indexOfCurrentInfo = 0;
            _isShowingObjective = true;
            _isResetTime = true;
            float deltaTime;
            float remainingTimeToSwap = 0;

            while (_isShowingObjective)
            {
                if (_isResetTime || remainingTimeToSwap <= 0)
                {
                    _objectiveIcon.sprite = _listOfObjectiveBlockInfo[_indexOfCurrentInfo].ObjectiveBlockAssetReference.DefaulColorSprite;
                    _remainingNumberOfObjectiveText.text = _listOfObjectiveBlockInfo[_indexOfCurrentInfo].RemainingNumberOfObjective.ToString();

                    _indexOfCurrentInfo++;
                    if (_indexOfCurrentInfo >= _listOfObjectiveBlockInfo.Count)
                        _indexOfCurrentInfo = 0;

                    remainingTimeToSwap = _delayOnSwapingObjective;
                    _isResetTime = false;
                }

                deltaTime = Time.deltaTime;
                remainingTimeToSwap -= deltaTime;
                yield return new WaitForSeconds(deltaTime);
            }
        }

        #endregion

        #region Override Method

        protected override void OnCanvasEnabled()
        {
            int incrementalLevelIndex    = _gameManager.LevelDataManagerReference.GetIncrementalLevelIndex;
            _levelNumberText.text       = string.Format("LEVEL\n{0}{1}", incrementalLevelIndex <= 9 ? "0" : "", incrementalLevelIndex); 

            int levelIndex              = _gameManager.LevelDataManagerReference.GetLevelIndex;
            
            _currentGridDataAsset       = _gameManager.LevelDataManagerReference.LevelInformationReference[levelIndex].GridData;

            OnUpdatingUIRemainingNumberOfMove(_currentGridDataAsset.NumberOfAvailableMove);
            _gameManager.GridDataManagerReference.OnPassingRemainingNumberOfMove += OnUpdatingUIRemainingNumberOfMove;

            
            _listOfObjectiveBlockInfo = new List<ObjectiveBlockInfo>();
            int numberOfObjective = _currentGridDataAsset.ObjectiveBlocks.Count;
            for (int i = 0; i < numberOfObjective; i++)
            {
                _listOfObjectiveBlockInfo.Add(
                    new ObjectiveBlockInfo(
                        i,
                        _currentGridDataAsset.ObjectiveBlocks[i],
                        _currentGridDataAsset.GetNumberObjectiveBlock(_currentGridDataAsset.ObjectiveBlocks[i]),
                        OnUpdatingUIForObjective
                        ));
            }
            _gameManager.GridDataManagerReference.OnRemainingNumberOfObjective += OnUpdatingRemainingNumberOfObjective;

            _levelNumberText.rectTransform.DOScale(1, 0.5f);
            _moveInfoRectTransform.DOScale(1, 0.5f);
            _objectiveRectTransform.DOScale(1, 0.5f);

            StartCoroutine(ShowingObjective());
        }

        protected override void OnCavasDisabled()
        {
            _gameManager.GridDataManagerReference.OnPassingRemainingNumberOfMove    -= OnUpdatingUIRemainingNumberOfMove;
            _gameManager.GridDataManagerReference.OnRemainingNumberOfObjective      -= OnUpdatingRemainingNumberOfObjective;

            _isShowingObjective = false;
        }


        protected override void OnLevelCompleted()
        {
            base.OnLevelCompleted();

            SetCanvasVisibility(false);
        }

        protected override void OnLevelFailed()
        {
            base.OnLevelFailed();

            SetCanvasVisibility(false);
        }

        #endregion


    }
}

