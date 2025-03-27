using UnityEngine;

namespace Primitives
{
    [CreateAssetMenu(fileName = "IntData", menuName = "Data/Primitives/IntData")]
    public class IntData : ScriptableObject
    {
        private string _saveKey;
    
        [SerializeField] private bool zeroOnEnable;
        [SerializeField] private int objectValue;
    
        public int Value
        {
            get => objectValue;
            set => objectValue = value;
        }
    
        private void Awake()
        {
            Value = zeroOnEnable ? 0 : Value;
            _saveKey = name;
        }

        public void Set(int num) => Value = num;
        public void Set(IntData otherDataObj) => Value = otherDataObj.Value;
    
        public void Increment() => ++Value;
    
        public void Decrement() => --Value;
    
        public void AdjustValue(int num) => Value += num;

        public int GetSavedValue()
        {
            var key = name;
            Value = (PlayerPrefs.HasKey(key)) ? PlayerPrefs.GetInt(key) : 0;
            return Value;
        }
    
        public void SaveCurrentValue()
        {
            PlayerPrefs.SetInt(_saveKey, Value);
            PlayerPrefs.Save();
        }
    
        public override string ToString()
        {
            return base.ToString() + $": {Value}";
        }
    
        public static implicit operator int(IntData data)
        {
            return data.Value;
        }
    
        public static implicit operator float(IntData data)
        {
            return data.Value;
        }

        public static IntData operator --(IntData data)
        {
            data.Value--;
            return data;
        }

        public static IntData operator ++(IntData data)
        {
            data.Value++;
            return data;
        }
    
        public static IntData operator +(IntData data, int other)
        {
            data.Value += other;
            return data;
        }

        public static IntData operator -(IntData data, int other)
        {
            data.Value -= other;
            return data;
        }

        public static IntData operator *(IntData data, int scalar)
        {
            data.Value *= scalar;
            return data;
        }

        public static IntData operator /(IntData data, int scalar)
        {
            data.Value /= scalar;
            return data;
        } 
    
        public static bool operator ==(IntData data, int other)
        {
            return data != null && data.Value == other;
        }

        public static bool operator !=(IntData data, int other)
        {
            return data != null && data.Value != other;
        }

        public static bool operator >(IntData data, int other)
        {
            return data.Value > other;
        }

        public static bool operator <(IntData data, int other)
        {
            return data.Value < other;
        }

        public static bool operator >=(IntData data, int other)
        {
            return data.Value >= other;
        }

        public static bool operator <=(IntData data, int other)
        {
            return data.Value <= other;
        }

        public override bool Equals(object obj)
        {
            switch (obj)
            {
                case IntData otherData:
                    return Value == otherData.Value;
                case int otherValue:
                    return Value == otherValue;
                default:
                    return false;
            }
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }
}
