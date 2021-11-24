namespace Project.Module.Grid
{
    using UnityEngine;

    public class ColorBlock : InteractableBlock
    {
        #region Public Variables

        public int ColorIndex { get; private set; }

        #endregion

        #region Abstract Method

        protected override void OnPassingIdentity(int row, int column, int index)
        {
            
        }

        #endregion

        #region Public Callback

        public void SetColorIndex(int colorIndex)
        {
            ColorIndex = colorIndex;
        }

        #endregion

    }
}

