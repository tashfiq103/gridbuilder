namespace Project.Module.Grid
{
    using UnityEngine;
    using DG.Tweening;

    public abstract class Grid : MonoBehaviour
    {
        #region Public Variables

        public int Row { get; private set; }
        public int Column { get; private set; }
        public int Index { get; private set; }
        public Sprite ColorOfGrid { get; private set; }

        #endregion

        #region Private Variables

        [SerializeField] private SpriteRenderer _spriteRendererReference;


        #endregion

        #region Abstract Method

        protected abstract void OnPassingIdentity(int row, int column, int index);

        #endregion

        #region Public Callback

        public void Initialize(int row, int column, int index, Sprite gridColorSprite, Vector3 localPosition)
        {
            ChangeGridColor(gridColorSprite);
            UpdateGridInfo(row, column, index);
            transform.localPosition = localPosition;
            Appear();
        }

        public void UpdateGridInfo(int row, int column, int index)
        {
            Row = row;
            Column = column;
            Index = index;

            OnPassingIdentity(row, column, index);
        }


        public void ChangeGridColor(Sprite gridColorSprite)
        {
            ColorOfGrid = gridColorSprite;
            _spriteRendererReference.sprite = ColorOfGrid;
        }

        public void Appear() {

            transform.localScale = Vector3.zero;
            transform.DOScale(1, 0.5f);
        }

        public void Disappear()
        {
            transform.DOScale(0, 0.5f);
        }


        #endregion
    }
}

