namespace Project.Data.CurrencyData
{
    using System;
    using UnityEngine;
    using com.faith.core;

    public class CurrencyDataManager    :   MonoBehaviour
    {
        #region Public Variables

        public SavedData<double> Currency { get; private set; }

        public event Action<double> OnCurrencyChangedEvent;

        #endregion

        #region Private Variables

        private bool _isInitialized;

        #endregion

        #region Public Variables

        public void Initialize()
        {
            if (!_isInitialized)
            {
                Currency = new SavedData<double>(
                    "CURRENCY",
                    0,
                    (value) =>
                    {
                        OnCurrencyChangedEvent?.Invoke(value);
                    }
                );
                _isInitialized = true;
            }
        }

        public void AddCurrency(double value)
        {
            Currency.SetData(Currency.GetData() + value);
        }

        public bool DeductCurrency(double value)
        {
            double currentValue = Currency.GetData();
            if (value <= currentValue)
            {
                Currency.SetData(currentValue - value);
                return true;
            }


            return false;
        }

        #endregion
    }
}

