namespace Project.Data.PlayableArea
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using Project.Data;


    [CreateAssetMenu(fileName = "GridDataManager", menuName = GameConstant.GAME_NAME + "/PlayableArea/Grid/GridDataManager")]
    public class GridDataManager : ScriptableObject
    {
        
        #region Public Variables

        public event Action<int>    OnPassingRemainingNumberOfMove;
        public event Action<ObjectiveBlockAsset, int> OnRemainingNumberOfObjective;

        #endregion


        #region Public Callback

        public void PassRemainingNumberOfMove(int value)
        {
            OnPassingRemainingNumberOfMove?.Invoke(value);
        }

        public void PassRemainingNumberOfObjective(ObjectiveBlockAsset objectiveBlockAsset, int value)
        {
            OnRemainingNumberOfObjective?.Invoke(objectiveBlockAsset, value);
        }

        #endregion
    }
}

