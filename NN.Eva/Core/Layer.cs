﻿using NN.Eva.Services;
using System;
using System.Collections.Generic;
using NN.Eva.Core.Database;
using NN.Eva.Models;

namespace NN.Eva.Core
{
    public class Layer
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        private List<Neuron> _neuronList = new List<Neuron>();

        private Layer() { }

        public Layer(int neuronCount, int weightCount, int layerNumber, FileManager fileManager)
        {
            double offsetValue = 0.5;
            double offsetWeight = -1;

            for (int i = 0; i < neuronCount; i++)
            {
                double[] weights = fileManager.LoadMemory(layerNumber, i, ref offsetValue, ref offsetWeight);
                Neuron neuron = new Neuron(weights, offsetValue, offsetWeight, ActivationFunction.Sigmoid);

                _neuronList.Add(neuron);
            }
        }

        public Layer(int neuronCount, int weightCount, int layerNumber, FileManager fileManager, string memoryPath)
        {
            double offsetValue = 0.5;
            double offsetWeight = -1;

            for (int i = 0; i < neuronCount; i++)
            {
                double[] weights = fileManager.LoadMemory(layerNumber, i, memoryPath, ref offsetValue, ref offsetWeight);
                Neuron neuron = new Neuron(weights, offsetValue, offsetWeight, ActivationFunction.Sigmoid);

                _neuronList.Add(neuron);
            }
        }

        #region Handling

        public double[] Handle(double[] data)
        {
            double[] layerResultVector = new double[_neuronList.Count];

            for (int i = 0; i < layerResultVector.Length; i++)
            {
                layerResultVector[i] = _neuronList[i].Handle(data);
            }

            return layerResultVector;
        }

        #endregion

        #region Teaching

        #region Error calculating

        public void CalcErrorAsOut(double[] rightAnwsersSet)
        {
            for (int i = 0; i < _neuronList.Count; i++)
            {
                _neuronList[i].CalcErrorForOutNeuron(rightAnwsersSet[i]);
            }
        }

        public void CalcErrorAsHidden(double[][] nextLayerWeights, double[] nextLayerErrors)
        {
            for (int i = 0; i < _neuronList.Count; i++)
            {
                _neuronList[i].CalcErrorForHiddenNeuron(i, nextLayerWeights, nextLayerErrors);
            }
        }

        #endregion

        #region Weights changing

        public void ChangeWeights(double learnSpeed, double[] anwsersFromPrewLayer)
        {
            for (int i = 0; i < _neuronList.Count; i++)
            {
                _neuronList[i].ChangeWeights(learnSpeed, anwsersFromPrewLayer);
            }
        }

        #endregion

        public double[] GetLastAnswers()
        {
            double[] lastAnwsers = new double[_neuronList.Count];

            for (int i = 0; i < _neuronList.Count; i++)
            {
                lastAnwsers[i] = _neuronList[i].GetLastAnswer();
            }

            return lastAnwsers;
        }

        public double[][] GetWeights()
        {
            double[][] weights = new double[_neuronList.Count][];

            for (int i = 0; i < _neuronList.Count; i++)
            {
                weights[i] = _neuronList[i].GetWeights();
            }

            return weights;
        }

        public double[] GetErrors()
        {
            double[] errors = new double[_neuronList.Count];

            for (int i = 0; i < _neuronList.Count; i++)
            {
                errors[i] = _neuronList[i].GetError();
            }

            return errors;
        }

        #endregion

        #region Memory operations

        public void SaveMemory(FileManager fileManager, int layerNumber, string path)
        {
            for (int i = 0; i < _neuronList.Count; i++)
            {
                _neuronList[i].SaveMemory(fileManager, layerNumber, i, path);
            }
        }

        #region Database operations

        public void SaveMemoryToDB(Guid networkId, Guid userId, int number, DBInserter dbInserter)
        {
            // Saving layer info:
            dbInserter.InsertLayer(Id, userId, networkId, number);

            // Saving neurons info:
            for (int i = 0; i < _neuronList.Count; i++)
            {
                _neuronList[i].SaveMemoryToDB(Id, userId, i, dbInserter);
            }
        }

        public void DBMemoryAbort(Guid networkId, DBDeleter dbDeleter)
        {
            // Aborting saving of neurons:
            for (int i = 0; i < _neuronList.Count; i++)
            {
                _neuronList[i].DBMemoryAbort(Id, dbDeleter);
            }

            // Aborting saving of layer:
            dbDeleter.DeleteFromTableLayers(networkId);
        }

        #endregion

        #endregion

        public bool IsMemoryEquals(NetworkStructure netStructure, int currentLayerNumber)
        {
            // Check for equals count of neurons:
            if (_neuronList.Count != netStructure.NeuronsByLayers[currentLayerNumber])
            {
                return false;
            }

            // Check for correct count of weights on each neuron:
            // *If this is first layer check equals with input vector:
            if (currentLayerNumber == 0)
            {
                for (int i = 0; i < _neuronList.Count; i++)
                {
                    if (!_neuronList[i].IsMemoryEquals(netStructure.InputVectorLength))
                    {
                        return false;
                    }
                }
            }
            else
            {
                for (int i = 0; i < _neuronList.Count; i++)
                {
                    if (!_neuronList[i].IsMemoryEquals(netStructure.NeuronsByLayers[currentLayerNumber - 1]))
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
