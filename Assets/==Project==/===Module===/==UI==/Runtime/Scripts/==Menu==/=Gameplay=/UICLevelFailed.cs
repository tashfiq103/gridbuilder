namespace Project.UI
{
    using UnityEngine;
    using UnityEngine.UI;
    using DG.Tweening;

    public class UICLevelFailed : UICanvas
    {
        #region Private Variables

        [SerializeField] private Image _background;
        [SerializeField] private RectTransform _levelFailedRctTransform;
        [SerializeField] private Button _restartButton;


        #endregion

        #region Override Method

        protected override void Awake()
        {
            base.Awake();

            _restartButton.onClick.AddListener(() =>
            {
                _gameManager.RestartLevel();
            });
        }

        protected override void OnCanvasEnabled()
        {
            DOVirtual.Float(0f, 1, 0.5f, (value) =>
            {
                _background.color = new Color(1, 1, 1, value);
            });
            _levelFailedRctTransform.DOScale(1, 0.5f);
            DOVirtual.DelayedCall(
                    0.5f,
                    () =>
                    {
                        _restartButton.transform.DOScale(1, 0.5f);
                    }
                );
        }

        protected override void OnCavasDisabled()
        {

        }

        protected override void OnLevelFailed()
        {
            base.OnLevelFailed();

            SetCanvasVisibility(true);
        }

        #endregion
    }
}

