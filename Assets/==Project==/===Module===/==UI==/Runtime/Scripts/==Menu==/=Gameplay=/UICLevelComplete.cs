namespace Project.UI
{
    using UnityEngine;
    using UnityEngine.UI;
    using DG.Tweening;

    public class UICLevelComplete : UICanvas
    {
        #region Private Variables

        [SerializeField] private Image _background;
        [SerializeField] private RectTransform _levelCompleteRibonRectTransform;
        [SerializeField] private RectTransform _levelCompleteText;
        [SerializeField] private Button _nextButton;

        [Space(5.0f)]
        [SerializeField] private ParticleSystem _celebretionParticle;

        private Tween _tweenForLevelCompleteText;

        #endregion

        #region Override Method

        protected override void Awake()
        {
            base.Awake();

            _nextButton.onClick.AddListener(() =>
            {
                _tweenForLevelCompleteText.Kill();

                _gameManager.LoadNextLevel();
            });
        }

        protected override void OnCanvasEnabled()
        {
            
        }

        protected override void OnCavasDisabled()
        {
            
        }

        protected override void OnLevelCompleted()
        {
            base.OnLevelCompleted();

            SetCanvasVisibility(true);

            DOVirtual.Float(0f, 1, 0.5f, (value) =>
            {
                _background.color = new Color(1, 1, 1, value);
            });

            _levelCompleteRibonRectTransform.DOScale(1, 0.5f);
            DOVirtual.DelayedCall(
                    0.5f,
                    () =>
                    {
                        _celebretionParticle.Play();
                        _tweenForLevelCompleteText = _levelCompleteText.DOScale(1.125f, 0.25f).SetLoops(-1, LoopType.Yoyo);
                        _nextButton.transform.DOScale(1, 0.5f);
                    }
                );
        }

        #endregion


    }
}

