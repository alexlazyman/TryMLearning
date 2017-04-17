﻿namespace TryMLearning.Model
{
    public class AlgorithmParameterValue
    {
        public int AlgorithmParameterValueId { get; set; }

        public int AlgorithmParameterId { get; set; }

        public int AlgorithmSessionId { get; set; }

        public int? IntValue { get; set; }

        public double? DoubleValue { get; set; }

        public string StringValue { get; set; }
    }
}