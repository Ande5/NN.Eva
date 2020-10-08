﻿using System;
using System.Diagnostics;
using NN.Eva.Core;
using NN.Eva.Models;
using NN.Eva.Models.Database;
using NN.Eva.Services;

namespace NN.Eva
{
    public class ServiceEvaNN
    {
        private FileManager _fileManager;
        private NetworksTeacher _networkTeacher;

        /// <summary>
        /// Creating FeedForward - Neural Network
        /// </summary>
        /// <param name="memoryFolderName"></param>
        /// <param name="networkStructure"></param>
        /// <param name="testDatasetPath"></param>
        /// <returns>Returns success result of network creating</returns>
        public bool CreateNetwork(string memoryFolderName, NetworkStructure networkStructure,
                                    string testDatasetPath = null)
        {
            _fileManager = new FileManager(networkStructure, memoryFolderName);

            if(_fileManager.IsMemoryLoadCorrect)
            {
                _networkTeacher = new NetworksTeacher(networkStructure, _fileManager);

                if (testDatasetPath != null)
                {
                    _networkTeacher.TestVectors = _fileManager.LoadTestDataset(testDatasetPath);
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Training FeedForward - NeuralNetwork
        /// </summary>
        /// <param name="trainingConfiguration"></param>
        /// <param name="iterationToPause"></param>
        /// <param name="printLearnStatistic"></param>
        /// <param name="processPriorityClass"></param>
        /// <param name="unsafeTrainingMode"></param>
        public void Train(TrainingConfiguration trainingConfiguration,
                          int iterationToPause = 100,
                          bool printLearnStatistic = false,
                          ProcessPriorityClass processPriorityClass = ProcessPriorityClass.Normal,
                          bool unsafeTrainingMode = false)
        {
            trainingConfiguration.MemoryFolder = trainingConfiguration.MemoryFolder == "" ? "Memory" : trainingConfiguration.MemoryFolder;

            // Start process timer:
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            // Set the process priority class:
            Process thisProc = Process.GetCurrentProcess();
            thisProc.PriorityClass = processPriorityClass;

            if (_networkTeacher.CheckMemory(trainingConfiguration.MemoryFolder))
            {
                _networkTeacher.TrainNet(trainingConfiguration, iterationToPause, unsafeTrainingMode);

                if (printLearnStatistic)
                {
                    _networkTeacher.PrintLearningStatistic(trainingConfiguration, true);
                }
            }
            else
            {
                Console.WriteLine("Train failed! Invalid memory!");
            }

            // Stopping timer and print spend time in [HH:MM:SS]:
            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;

            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}", ts.Hours, ts.Minutes, ts.Seconds);
            Console.WriteLine("Time spend: " + elapsedTime);
        }

        /// <summary>
        /// Handling double-vector data
        /// </summary>
        /// <param name="data"></param>
        /// <returns>Vector-classes</returns>
        public double[] Handle(double[] data)
        {
            try
            {
                return _networkTeacher.Handle(data);
            }
            catch
            {
                return null;
            }
        }

        public void CalculateStatistic(TrainingConfiguration trainingConfig)
        {
            _networkTeacher.PrintLearningStatistic(trainingConfig, true);
        }

        /// <summary>
        /// Backuping network's memory to db OR/AND local folder
        /// </summary>
        /// <param name="memoryFolder"></param>
        /// <param name="dbConfig"></param>
        /// <param name="networkStructureInfo"></param>
        /// <returns>State of operation success</returns>
        public bool BackupMemory(string memoryFolder, DatabaseConfig dbConfig = null, string networkStructureInfo = "no information")
        {
            try
            {
                if (_networkTeacher.CheckMemory(memoryFolder))
                {
                    _networkTeacher.BackupMemory(memoryFolder, ".memory_backups", dbConfig, networkStructureInfo);  
                }
                else
                {
                    return false;
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Aborting network's memory from database
        /// </summary>
        /// <param name="dbConnection"></param>
        /// <returns>State of operation success</returns>
        public bool DBMemoryAbort(DatabaseConfig dbConfig)
        {
            try
            {
                _networkTeacher.DBMemoryAbort(dbConfig);

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Loading network's memory from database
        /// </summary>
        /// <param name="dbConfig"></param>
        /// <param name="networkID"></param>
        /// <param name="destinationMemoryFilePath"></param>
        /// <returns></returns>
        public bool DBMemoryLoad(DatabaseConfig dbConfig, Guid networkID, string destinationMemoryFilePath)
        {
            try
            {
                _networkTeacher.DBMemoryLoad(dbConfig, networkID, destinationMemoryFilePath);

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
