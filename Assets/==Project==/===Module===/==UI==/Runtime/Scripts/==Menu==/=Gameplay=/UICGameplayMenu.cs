namespace Project.UI
{
    using UnityEngine;

    public class UICGameplayMenu : UICanvas
    {
        #region Private Variables

        [SerializeField] private UICHud _hudReference;

        #endregion

        #region Override Method

        protected override void OnLevelStarted()
        {
            base.OnLevelStarted();

            SetCanvasVisibility(true);
        }

        protected override void OnCanvasEnabled()
        {
            _hudReference.SetCanvasVisibility(true);
        }

        protected override void OnCavasDisabled()
        {

        }

        #endregion
    }
}

