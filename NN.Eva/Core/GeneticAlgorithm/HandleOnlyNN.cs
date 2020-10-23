﻿using System.Collections.Generic;
using System.Linq;
using NN.Eva.Models;

namespace NN.Eva.Core.GeneticAlgorithm
{
    public class HandleOnlyNN
    {
        public List<HandleOnlyLayer> LayerList { get; set; } = new List<HandleOnlyLayer>();

        public HandleOnlyNN(List<double> weights, NetworkStructure networkStructure)
        {
            List<(int, int)> weightOnLayers = new List<(int, int)>();

            int index = 0;

            // Calculate weights count for the first layer:
            int countOfWeightsFirstLayer = networkStructure.NeuronsByLayers[0] * networkStructure.InputVectorLength;
            weightOnLayers.Add((index, countOfWeightsFirstLayer));
            index += countOfWeightsFirstLayer;

            // Calculate weights count for the other layers:
            for (int i = 1; i < networkStructure.NeuronsByLayers.Length; i++)
            {
                int countOfWeights = networkStructure.NeuronsByLayers[i] * networkStructure.NeuronsByLayers[i - 1];
                weightOnLayers.Add((index, countOfWeights));
                index += countOfWeights;
            }

            for (int i = 0; i < networkStructure.NeuronsByLayers.Length; i++)
            {
                LayerList.Add(new HandleOnlyLayer(weightOnLayers[i], weights, networkStructure.NeuronsByLayers[i]));
            }
        }

        /// <summary>
        /// Handling data
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public double[] Handle(double[] data)
        {
            double[] tempData = data;

            for (int i = 0; i < LayerList.Count; i++)
            {
                tempData = LayerList[i].Handle(tempData);
            }

            // There is one double value at the last handle
            return tempData;
        }
    }
}
