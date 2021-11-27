namespace Project.Module.PlayableArea
{
    using UnityEngine;
    using DG.Tweening;

    public abstract class InteractableBlock : MonoBehaviour
    {
        #region Public Variables


        public int RowIndex { get; private set; }
        public int ColumnIndex { get; private set; }
        public int Index { get; private set; }
        public bool IsImpactByGravity { get; private set; }
        public Sprite BlockImage { get; private set; }

        #endregion

        #region Private Variables

        [SerializeField] private SpriteRenderer _spriteRendererReference;

#if UNITY_EDITOR

        [SerializeField] private int rowIndex;
        [SerializeField] private int columnIndex;
        [SerializeField] private int index;

#endif

#endregion

        #region Abstract Method

        protected abstract void OnPassingIdentity(int row, int column, int index);

        #endregion

        #region Public Callback

        public void Initialize(int row, int column, int index, bool gravity, Sprite sprite, Vector3 localPosition)
        {

            UpdateGridInfo(row, column, index);

            IsImpactByGravity = gravity;

            ChangeSprite(sprite);

            Appear();
            if (transform.localPosition != localPosition)
            {
                Move(localPosition, 0.5f);
            }

        }

        public void UpdateGridInfo(int row, int column, int index)
        {
#if UNITY_EDITOR

            rowIndex = row;
            columnIndex = column;
            this.index = index;

#endif

            RowIndex = row;
            ColumnIndex = column;
            Index = index;

            OnPassingIdentity(row, column, index);
        }


        public void ChangeSprite(Sprite gridColorSprite)
        {
            _spriteRendererReference.sprite = gridColorSprite;
            BlockImage = gridColorSprite;
        }

        public void Appear() {

            transform.localScale = Vector3.zero;
            transform.DOScale(1, 0.5f);
        }

        public void Disappear()
        {
            transform.DOScale(0, 0.5f);
            DOVirtual.DelayedCall(
                    0.5f,
                    () =>
                    {
                        Destroy(gameObject);
                    }
                );
        }

        public void Move(Vector3 localPosition, float duration)
        {
            transform.DOLocalMove(localPosition, duration);
        }

        #endregion
    }
}

