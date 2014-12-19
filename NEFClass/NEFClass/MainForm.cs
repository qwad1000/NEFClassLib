using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

using NEFClassLib;
using Logging;

namespace NEFClass
{
    public partial class MainForm : Form
    {
        NCDataSet trainDataset;
        NEFClassNetwork network;

        public MainForm()
        {
            InitializeComponent();

            string datasetsDir = Path.Combine(Directory.GetCurrentDirectory(), "datasets");

            this.trainDatasetFileDialog.InitialDirectory = datasetsDir;
            this.checkFileDialog.InitialDirectory = datasetsDir;

            this.rulesAlgoComboBox.DataSource = new string[3] { 
                TrainConfiguration.TRAIN_RULES_SIMPLE,
                TrainConfiguration.TRAIN_RULES_BEST,
                TrainConfiguration.TRAIN_RULES_BEST_FOR_CLASS
            };

            Log.RegisterHandler(new TextBoxLogHandler(logTextBox));
        }

        private void selectLearningDatasetFileButton_Click(object sender, EventArgs e)
        {
            if (trainDatasetFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string filename = trainDatasetFileDialog.FileName;
                
                learningDatasetFileTExtBox.Text = filename;
                trainDataset = new NCDataSet(filename);

                learningDatasetSizeTextBox.Text = trainDataset.Length.ToString();
                learningDatasetAttrsCountTextBox.Text = trainDataset.Dimension.ToString();
                learningDatasetClassesCountTextBox.Text = trainDataset.GetClassesList().Length.ToString();

                classesListView.Clear();
                foreach (string cls in trainDataset.GetClassesList())
                    classesListView.Items.Add(cls);

                sectionsDataGridView.Rows.Clear();
                for (int i = 0; i < trainDataset.Dimension; ++i)
                {
                    sectionsDataGridView.Rows.Add();
                    sectionsDataGridView.Rows[i].Cells[0].Value = "x" + (i + 1).ToString();
                    sectionsDataGridView.Rows[i].Cells[1].Value = 3;
                }

                trainButton.Enabled = true;
            }
        }

        private void DoTrain()
        {
            TrainConfiguration config = new TrainConfiguration();

            config.DoOptimization = true;

            config.RuleNodesMax = (int)maxRulesInput.Value;
            config.FuzzyPartsCount = new int[trainDataset.Dimension];
            for (int i = 0; i < trainDataset.Dimension; ++i)
                config.FuzzyPartsCount[i] = Convert.ToInt32(this.sectionsDataGridView[1, i].Value);

            config.RulesTrainAlgo = rulesAlgoComboBox.SelectedValue.ToString();
            config.OptimizationSpeed = Convert.ToDouble(optimizationSpeedTextBox.Text.Replace(".", ","));
            config.MaxIterations = Convert.ToInt32(maxEpochsInput.Value);
            config.Accuracy = Convert.ToDouble(accTextBox.Text); ;

            network = new NEFClassNetwork(trainDataset, config);

            checkButton.Enabled = true;
        }

        private void trainButton_Click(object sender, EventArgs e)
        {
            logTextBox.Clear();

            DoTrain();
            MessageBox.Show("Обучение выполнено.");
        }

        private void checkButton_Click(object sender, EventArgs e)
        {
            if (checkFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string filename = checkFileDialog.FileName;

                NCDataSet dataset = new NCDataSet(filename);

                int goodCount = 0;
                double rmse = 0;
                for (int i = 0; i < dataset.Length; ++i)
                {
                    string result = network.Classify(dataset[i]);
                    if (dataset[i].Class == result)
                        ++goodCount;

                    double[] desiredOutput = new double[dataset.GetClassesList().Length];
                    desiredOutput[network.GetClassIndex(dataset[i].Class)] = 1;
                    double[] output = network.GetOutput(dataset[i]);

                    for (int z = 0; z < output.Length; ++z)
                        rmse += (desiredOutput[z] - output[z]) * (desiredOutput[z] - output[z]);
                }

                rmse /= dataset.Length;

                string results = String.Format("Размер выборки: {0}\nТочность: {1}%\nНеудачных попыток: {2}\nError: {3}", dataset.Length, (goodCount * 100.0 / dataset.Length).ToString("0.##"), dataset.Length - goodCount, rmse.ToString("0.##"));
                MessageBox.Show(results, "Результаты");
            }
        }
    }
}
