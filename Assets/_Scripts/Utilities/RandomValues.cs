using UnityEngine;

namespace IslandDefender.Utilities {

    [System.Serializable]
	public class RandomInt {

        public int MinValue;
        public int MaxValue;
        public int Value { get; private set; }

        public int Randomize() {
            Value = Random.Range(MinValue, MaxValue + 1);
            return Value;
        }
    }

    [System.Serializable]
    public class RandomFloat {

        public float MinValue;
        public float MaxValue;
        public float Value { get; private set; }

        public float Randomize() {
            Value = Random.Range(MinValue, MaxValue);
            return Value;
        }
    }
}