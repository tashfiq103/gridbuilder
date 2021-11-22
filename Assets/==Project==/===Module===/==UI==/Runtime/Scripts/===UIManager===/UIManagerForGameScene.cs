namespace Project.UI
{
    using UnityEngine;

    public class UIManagerForGameScene : MonoBehaviour
    {
        #region Public Variables

        public static UIManagerForGameScene Instance { get; private set; }

        public UICHomeMenu HomeMenu { get { return _homeMenu; } }
        public UICGameplayMenu GameplayMenu { get { return _gameplayMenu; } }

        #endregion

        #region Private Variables

        [SerializeField] private UICHomeMenu _homeMenu;
        [SerializeField] private UICGameplayMenu _gameplayMenu;

        #endregion

        #region Mono Behaviour

        private void Awake()
        {
            Instance = this;
        }

        #endregion
    }
}


