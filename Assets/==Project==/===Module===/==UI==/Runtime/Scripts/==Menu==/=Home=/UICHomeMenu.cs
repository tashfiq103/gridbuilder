namespace Project.UI
{
    using UnityEngine;
    using UnityEngine.UI;

    public class UICHomeMenu : UICanvas
    {
        #region Private Variables

        [SerializeField] private Button _startButton;

        #endregion

        #region Mono Behaviour

        protected override void Awake()
        {
            base.Awake();

            _startButton.onClick.AddListener(() =>{

                SetCanvasVisibility(false);
                _gameManager.OnLevelStartedEvent.Raise();
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

