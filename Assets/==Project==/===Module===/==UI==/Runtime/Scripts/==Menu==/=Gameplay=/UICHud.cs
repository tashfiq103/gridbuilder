namespace Project.UI
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using DG.Tweening;
    using TMPro;
    using Project.Data.PlayableArea;

    public class UICHud : UICanvas
    {
        #region CustomVariables

        private class ObjectiveBlockInfo
        {
            public ObjectiveBlockAsset ObjectiveBlockAssetReference { get; private set; }
            public int RemainingNumberOfObjective { get; private set; }

            public ObjectiveBlockInfo(ObjectiveBlockAsset objectiveBlockAsset, int remainingNumber)
            {
                ObjectiveBlockAssetReference    = objectiveBlockAsset;
                RemainingNumberOfObjective      = remainingNumber;
            }

            public void Update(int value)
            {
                RemainingNumberOfObjective = value;
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
        [SerializeField] private Image          _objectiveIcon;

        private GridDataAsset _currentGridDataAsset;
        private List<ObjectiveBlockInfo> _listOfObjectiveBlockInfo;

        #endregion

        #region Configuretion

        private void OnUpdatingRemainingNumberOfMove(int value)
        {
            _remainingNumberOfMoveText.text = value.ToString();
        }

        private void OnUpdatingRemainingNumberOfObjective(ObjectiveBlockAsset objectiveBlockAsset, int value)
        {

        }

        #endregion

        #region Override Method

        protected override void OnCanvasEnabled()
        {
            int levelIndex          = _gameManager.LevelDataManagerReference.GetLevelIndex;
            _currentGridDataAsset   = _gameManager.GridDataManagerReference.GridsData[levelIndex];

            OnUpdatingRemainingNumberOfMove(_currentGridDataAsset.NumberOfAvailableMove);
            _gameManager.GridDataManagerReference.OnPassingRemainingNumberOfMove += OnUpdatingRemainingNumberOfMove;

            
            _listOfObjectiveBlockInfo = new List<ObjectiveBlockInfo>();
            int numberOfObjective = _currentGridDataAsset.ObjectiveBlocks.Count;
            for (int i = 0; i < numberOfObjective; i++)
            {
                _listOfObjectiveBlockInfo.Add(new ObjectiveBlockInfo(_currentGridDataAsset.ObjectiveBlocks[i], _currentGridDataAsset.GetNumberObjectiveBlock(_currentGridDataAsset.ObjectiveBlocks[i])));
            }

            _moveInfoRectTransform.DOScale(1, 0.5f);
            _objectiveRectTransform.DOScale(1, 0.5f);
        }

        protected override void OnCavasDisabled()
        {
            _gameManager.GridDataManagerReference.OnPassingRemainingNumberOfMove -= OnUpdatingRemainingNumberOfMove;
        }

        #endregion


    }
}

