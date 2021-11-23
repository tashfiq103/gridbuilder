namespace Project.Module.Grid
{
    using UnityEngine;
    using Toolset.GameEvent.Raycast;

    public class UserInputOnColorGrid : Raycast2DEventFromCamera
    {

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
            Grid grid = raycastHit2D.collider.GetComponent<Grid>();
            grid.Disappear();
        }

        protected override void RaycastHitOnTouchDown(RaycastHit2D raycastHit2D)
        {

        }

        protected override void RaycastHitOnTouchUp(RaycastHit2D raycastHit2D)
        {

        }

        #endregion


    }
}
