using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEFClassLib
{
    public struct TrainConfiguration
    {
        public static string TRAIN_RULES_SIMPLE = "Простое";
        public static string TRAIN_RULES_BEST = "Лучшее";
        public static string TRAIN_RULES_BEST_FOR_CLASS = "Лучшее для класса";

        public int RuleNodesMax { get; set; }
        public bool DoOptimization { get; set; }
        public double OptimizationSpeed { get; set; }
        public int[] FuzzyPartsCount { get; set; }
        public string RulesTrainAlgo { get; set; }
        public int MaxIterations { get; set; }
        public double Accuracy { get; set; }
    }
}
