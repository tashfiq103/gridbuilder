namespace Project.UI
{
    using UnityEngine;
    using UnityEngine.UI;
    using DG.Tweening;

    public class UICHomeMenu : UICanvas
    {
        #region Private Variables

        [SerializeField] private Button _startButton;
        [SerializeField] private RectTransform _tapToStartRectTransform;

        private Tween _tweenForTapToStart;

        #endregion

        #region Mono Behaviour

        protected override void Awake()
        {
            base.Awake();

            _tweenForTapToStart = _tapToStartRectTransform.DOScale(1.125f, 1).SetLoops(-1, LoopType.Yoyo);

            _startButton.onClick.AddListener(() =>{

                _tweenForTapToStart.Kill();

                _tapToStartRectTransform.DOScale(0, 0.25f);
                DOVirtual.DelayedCall(
                        0.25f,
                        () =>
                        {
                            SetCanvasVisibility(false);
                            _gameManager.OnLevelStartedEvent.Raise();
                        });


            });
        }


        #endregion

        #region Override Method

        protected override void OnCanvasEnabled()
        {

        }

        protected override void OnCavasDisabled()
        {

        }

        #endregion


    }
}

