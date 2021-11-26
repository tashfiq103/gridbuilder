namespace Project.Data.PlayableArea
{
    using UnityEngine;
    using Toolset.Enum;

    public abstract class InteractableBlockAsset : EnumAsset
    {

        #region Public Variables

        public Sprite DefaulColorSprite { get { return _defaultColorSprite; } }
        public bool Gravity { get { return _gravity; } }

        #endregion

        #region Private Variables

        [SerializeField] protected Sprite _defaultColorSprite;
        [SerializeField] private bool _gravity = true;

        #endregion
    }
}

