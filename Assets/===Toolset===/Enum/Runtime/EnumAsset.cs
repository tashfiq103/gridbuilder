namespace Toolset.Enum
{
    using UnityEngine;

    public abstract class EnumAsset : ScriptableObject
    {
        #region Public Variables

        public string Name { get { return _name; } }

        #endregion

        #region Private Varaibles

        [SerializeField] private string _name;

        #endregion
    }
}


