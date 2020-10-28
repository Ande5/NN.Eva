﻿using System;

namespace NN.Eva.Core.ResilientPropagation.Layers
{
    public class LayerRProp
    {
        protected int _inputsCount = 0;

        public int InputsCount => _inputsCount;

        protected int _neuronsCount = 0;

        public int NeuronsCount => _neuronsCount;

        protected NeuronRProp[] _neurons;

        public NeuronRProp[] Neurons => _neurons;

        protected double[] _output;

        public double[] Output => _output;

        protected LayerRProp(int neuronsCount, int inputsCount)
        {
            _inputsCount = Math.Max(1, inputsCount);
            _neuronsCount = Math.Max(1, neuronsCount);
            _neurons = new NeuronRProp[_neuronsCount];
        }

        public double[] Compute(double[] input)
        {
            // local variable to avoid mutlithread conflicts
            double[] output = new double[_neuronsCount];

            // compute each neuron
            for (int i = 0; i < _neurons.Length; i++)
                output[i] = _neurons[i].Compute(input);

            // assign output property as well (works correctly for single threaded usage)
            _output = output;

            return output;
        }

    }
}
