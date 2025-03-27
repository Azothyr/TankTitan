using UnityEngine;

namespace Primitives
{
    [CreateAssetMenu (fileName = "FloatData", menuName = "Data/Primitive/FloatData")]
    public class FloatData : ScriptableObject
    {
        private string _saveKey;
    
        [SerializeField] private bool zeroOnEnable;
        [SerializeField] private float objectValue;

        public float Value
        {
            get => objectValue;
            set => objectValue = value;
        }

        private void Awake() => _saveKey = name;

        private void OnEnable() => Value = zeroOnEnable ? 0 : Value;

        public void Set(float num) => Value = num;
        public void Set(FloatData otherDataObj) => Value = otherDataObj.Value;

        public void IncrementValue() => ++Value;
    
        public void DecrementValue() => --Value;
    
        public void AdjustValue(int num) => Value += num;

        public float GetSavedValue()
        {
            var key = name;
            Value = (PlayerPrefs.HasKey(key)) ? PlayerPrefs.GetInt(key) : 0;
            return Value;
        }
    
        public void SaveCurrentValue()
        {
            PlayerPrefs.SetFloat(_saveKey, Value);
            PlayerPrefs.Save();
        }
    
        public override string ToString() => base.ToString() + $": {Value}";
    
        public static implicit operator float(FloatData data) => data.Value;

        public static FloatData operator --(FloatData data)
        {
            data.Value--;
            return data;
        }

        public static FloatData operator ++(FloatData data)
        {
            data.Value++;
            return data;
        }
    
        public static FloatData operator +(FloatData data, int other)
        {
            data.Value += other;
            return data;
        }

        public static FloatData operator -(FloatData data, int other)
        {
            data.Value -= other;
            return data;
        }

        public static FloatData operator *(FloatData data, int scalar)
        {
            data.Value *= scalar;
            return data;
        }

        public static FloatData operator /(FloatData data, int scalar)
        {
            data.Value /= scalar;
            return data;
        }
    
        public static bool operator ==(FloatData data, float other) => data != null && Mathf.Approximately(data.Value, other);
        public static bool operator !=(FloatData data, float other) => data != null && !Mathf.Approximately(data.Value, other);
        public static bool operator >(FloatData data, float other) => data.Value > other;
        public static bool operator <(FloatData data, float other) => data.Value < other;
        public static bool operator >=(FloatData data, float other) => data.Value >= other;
        public static bool operator <=(FloatData data, float other) => data.Value <= other;

        public override bool Equals(object obj)
        {
            switch (obj)
            {
                case FloatData otherData:
                    return Mathf.Approximately(Value, otherData.Value);
                case float otherValue:
                    return Mathf.Approximately(Value, otherValue);
                default:
                    return false;
            }
        }

        public override int GetHashCode() => Value.GetHashCode();
    }
}
