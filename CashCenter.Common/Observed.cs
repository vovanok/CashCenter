﻿using System;

namespace CashCenter.Common
{
    public class Observed<T>
    {
        private object locker = new object();

        private T value;

        public T Value
        {
            get
            {
                return value;
            }
            set
            {
                lock(locker)
                {
                    this.value = value;
                }

                OnChange?.Invoke(value);
            }
        }

        public event Action<T> OnChange;

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
