namespace Project.Module.Grid
{
    using UnityEngine;
    using UnityEngine.Events;
    using Toolset.GameEvent.Raycast;

    public class UserInputOnColorGrid : Raycast2DEventFromCamera
    {

        #region Public Varibles

        public bool IsAcceptingInput { get; private set; }

        #endregion

        #region Private Variables

        private UnityAction<Grid> OnPassingTheGridInfo;


        #endregion

        #region Override Method

        protected override void OnRaycastHit(RaycastHit2D raycastHit2D)
        {

        }

        protected override void OnRaycastingEnd()
        {

        }

        protected override void OnRaycastingStart()
        {

        }

        protected override void RaycastHitOnTouch(RaycastHit2D raycastHit2D)
        {
            if(IsAcceptingInput)
                OnPassingTheGridInfo.Invoke(raycastHit2D.collider.GetComponent<Grid>());
            
        }

        protected override void RaycastHitOnTouchDown(RaycastHit2D raycastHit2D)
        {

        }

        protected override void RaycastHitOnTouchUp(RaycastHit2D raycastHit2D)
        {

        }

        #endregion

        #region Public Callback

        public void Initialize(UnityAction<Grid> OnPassingTheGridInfo)
        {
            this.OnPassingTheGridInfo = OnPassingTheGridInfo;
            IsAcceptingInput = true;
            StartRayCasting();
        }

        public void RestoreToDefault() {

            IsAcceptingInput = false;
            StopRaycasting();
        }

        public void SetInputStatus(bool status)
        {
            IsAcceptingInput = status;
        }

        #endregion
    }
}
