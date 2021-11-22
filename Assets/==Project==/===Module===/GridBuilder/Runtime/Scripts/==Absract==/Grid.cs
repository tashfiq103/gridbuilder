namespace Project.Module.GridBuilder
{
    using UnityEngine;
    using DG.Tweening;

    public abstract class Grid : MonoBehaviour
    {
        #region Public Variables

        public int Row { get; private set; }
        public int Column { get; private set; }

        #endregion

        #region Private Variables

        [SerializeField] private SpriteRenderer _spriteRendererReference;


        #endregion

        #region Abstract Method

        protected abstract void OnInitialized(int row, int column);

        #endregion

        #region Public Callback

        public void Initialize(int row, int column, Sprite gridColorSprite, Vector3 localPosition)
        {
            Row = row;
            Column = column;

            ChangeGridColor(gridColorSprite);

            transform.localPosition = localPosition;
            Appear();

            OnInitialized(row, column);
        }

        public void ChangeGridColor(Sprite gridColorSprite)
        {
            _spriteRendererReference.sprite = gridColorSprite;
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

