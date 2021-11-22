namespace Project.Module.Grid
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Project.Shared;

    public class GridBuilderManager : GameBehaviour
    {
        #region ALL UNITY FUNCTIONS

        #endregion ALL UNITY FUNCTIONS

        //=================================   
        #region ALL OVERRIDING FUNCTIONS

        protected override void OnLevelStarted()
        {
            base.OnLevelStarted();


        }

        #endregion ALL OVERRIDING FUNCTIONS

        //=================================
        #region ALL SELF DECLEAR FUNCTIONS

        #region Private Variables

        [SerializeField] private GameObject _colorGridPrefab;

        #endregion

        #endregion ALL SELF DECLEAR FUNCTIONS

    }
}
