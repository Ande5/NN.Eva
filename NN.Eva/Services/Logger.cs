﻿using NN.Eva.Models;
using System;
using System.IO;

namespace NN.Eva.Services
{
    public class Logger
    {
        private string _trainLogsDirectoryName;
        private string _errorLogsDirectoryName;

        public Logger()
        {
            _trainLogsDirectoryName = ".logs/trainLogs";
            _errorLogsDirectoryName = ".logs/errorLogs";
        }

        public Logger(string trainLogsDirectoryName, string errorLogsDirectoryName)
        {
            _trainLogsDirectoryName = trainLogsDirectoryName;
            _errorLogsDirectoryName = errorLogsDirectoryName;
        }

        public void LogTrainResults(int testPassed, int testFailed, int iteration)
        {
            // Check for existing this logs-directory:
            if (!Directory.Exists(_trainLogsDirectoryName))
            {
                Directory.CreateDirectory(_trainLogsDirectoryName);
            }

            // Save log:
            using (StreamWriter fileWriter = new StreamWriter(_trainLogsDirectoryName + "/" + iteration + ".txt"))
            {
                fileWriter.WriteLine("Test passed: " + testPassed);
                fileWriter.WriteLine("Test failed: " + testFailed);
                fileWriter.WriteLine("Percent learned: {0:f2}", (double)testPassed * 100 / (testPassed + testFailed));
            }

            Console.WriteLine("Learn statistic logs saved in {0}!", _trainLogsDirectoryName);
        }

        public void LogWarning(WarningType warningType)
        {
            WriteLog(GetWarningTextByType(warningType));
        }

        public void LogError(ErrorType errorType, Exception ex)
        {
            WriteLog(GetErrorTextByType(errorType), ex.Message);
        }

        public void LogError(ErrorType errorType, string errorText = "")
        {
            WriteLog(GetErrorTextByType(errorType), errorText);
        }

        private string GetWarningTextByType(WarningType warningType)
        {
            string warningText;

            switch (warningType)
            {
                case WarningType.UsingUnsafeTrainingMode:
                    warningText = "You are using unsafe training mode! Make sure that your input-vectors lengths are equals with Network's ones!";
                    break;
                default:
                    warningText = "";
                    break;
            }

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("[WARNING] " + warningText);
            Console.ForegroundColor = ConsoleColor.Gray;

            return warningText + "\n";
        }

        private string GetErrorTextByType(ErrorType errorType)
        {
            string errorText;

            switch (errorType)
            {
                case ErrorType.MemoryMissing:
                    errorText = "Memory is missing!";
                    break;
                case ErrorType.MemoryInitializeError:
                    errorText = "Memory initialize error! Check compliance the network structure and the memory-file!";
                    break;
                case ErrorType.MemoryGenerateError:
                    errorText = "Memory generate error!";
                    break;
                case ErrorType.SetMissing:
                    errorText = "One or more train set is missing!";
                    break;
                case ErrorType.TrainError:
                    errorText = "Network training error!";
                    break;
                case ErrorType.DBConnectionError:
                    errorText = "Database connection error!";
                    break;
                case ErrorType.DBInsertError:
                    errorText = "Database inserting error!";
                    break;
                case ErrorType.DBDeleteError:
                    errorText = "Database deleting error!";
                    break;
                case ErrorType.DBMemoryLoadError:
                    errorText = "Database loading error!";
                    break;
                case ErrorType.NonEqualsInputLengths:
                    errorText = "Length of network's input vector and length of training dataset's vector is not equals!";
                    break;
                case ErrorType.OperationWithNonexistentNetwork:
                    errorText = "Error in operation with nonexistent network! Please, create the Network first!";
                    break;
                case ErrorType.UnknownError:
                default:
                    errorText = "Unknown error!";
                    break;
            }

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("[ERROR] " + errorText);
            Console.ForegroundColor = ConsoleColor.Gray;

            return errorText + "\n";
        }

        private void WriteLog(string systemErrorText, string additionalErrorText = "")
        {
            // Check for existing this logs - directory:
            if (!Directory.Exists(_errorLogsDirectoryName))
            {
                Directory.CreateDirectory(_errorLogsDirectoryName);
            }

            // Save log:
            try
            {
                using (StreamWriter fileWriter = new StreamWriter(_errorLogsDirectoryName + "/"
                                                                                          + DateTime.Now.Day + "_"
                                                                                          + DateTime.Now.Month + "_"
                                                                                          + DateTime.Now.Year + "_"
                                                                                          + DateTime.Now.Ticks +
                                                                                          ".txt"))
                {
                    fileWriter.WriteLine("\nTime: " + DateTime.Now);
                    fileWriter.WriteLine("System error text: " + systemErrorText);

                    if (additionalErrorText != "")
                    {
                        fileWriter.Write("Additional error text: " + additionalErrorText);
                    }
                }

                Console.WriteLine("Error log saved in {0}!", _errorLogsDirectoryName);
            }
            catch { }
        }
    }
}
