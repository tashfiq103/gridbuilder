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
        public bool IsImpactByGravity { get { return _impactByGravity; } }

        #endregion

        #region Private Variables

        [SerializeField] private SpriteRenderer _spriteRendererReference;
        [SerializeField] private bool _impactByGravity = true;

        #endregion

        #region Abstract Method

        protected abstract void OnPassingIdentity(int row, int column, int index);

        #endregion

        #region Public Callback

        public void Initialize(int row, int column, int index, Sprite sprite, Vector3 localPosition)
        {
            UpdateGridInfo(row, column, index);

            ChangeSprite(sprite);

            Appear();
            if (transform.localPosition != localPosition)
            {
                transform.DOLocalMove(localPosition, 0.5f);
            }
            
        }

        public void UpdateGridInfo(int row, int column, int index)
        {
            RowIndex = row;
            ColumnIndex = column;
            Index = index;

            OnPassingIdentity(row, column, index);
        }


        public void ChangeSprite(Sprite gridColorSprite)
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
            DOVirtual.DelayedCall(
                    0.5f,
                    () =>
                    {
                        Destroy(gameObject);
                    }
                );
        }


        #endregion
    }
}

