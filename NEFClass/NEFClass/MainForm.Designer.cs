namespace NEFClass
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dataLoadDialog = new System.Windows.Forms.OpenFileDialog();
            this.learnGroupBox = new System.Windows.Forms.GroupBox();
            this.checkButton = new System.Windows.Forms.Button();
            this.trainButton = new System.Windows.Forms.Button();
            this.label10 = new System.Windows.Forms.Label();
            this.rulesAlgoComboBox = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.maxRulesInput = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.sectionsDataGridView = new System.Windows.Forms.DataGridView();
            this.var = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.varsec = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label6 = new System.Windows.Forms.Label();
            this.classesListView = new System.Windows.Forms.ListView();
            this.learningDatasetClassesCountTextBox = new System.Windows.Forms.TextBox();
            this.learningDatasetAttrsCountTextBox = new System.Windows.Forms.TextBox();
            this.learningDatasetSizeTextBox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.selectLearningDatasetFileButton = new System.Windows.Forms.Button();
            this.learningDatasetFileTExtBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.trainDatasetFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.checkFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.logTextBox = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.maxEpochsInput = new System.Windows.Forms.NumericUpDown();
            this.optimizationSpeedTextBox = new System.Windows.Forms.TextBox();
            this.accTextBox = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.learnGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.maxRulesInput)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sectionsDataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.maxEpochsInput)).BeginInit();
            this.SuspendLayout();
            // 
            // learnGroupBox
            // 
            this.learnGroupBox.Controls.Add(this.accTextBox);
            this.learnGroupBox.Controls.Add(this.label12);
            this.learnGroupBox.Controls.Add(this.optimizationSpeedTextBox);
            this.learnGroupBox.Controls.Add(this.maxEpochsInput);
            this.learnGroupBox.Controls.Add(this.label11);
            this.learnGroupBox.Controls.Add(this.checkButton);
            this.learnGroupBox.Controls.Add(this.trainButton);
            this.learnGroupBox.Controls.Add(this.label10);
            this.learnGroupBox.Controls.Add(this.rulesAlgoComboBox);
            this.learnGroupBox.Controls.Add(this.label9);
            this.learnGroupBox.Controls.Add(this.maxRulesInput);
            this.learnGroupBox.Controls.Add(this.label8);
            this.learnGroupBox.Controls.Add(this.label7);
            this.learnGroupBox.Controls.Add(this.sectionsDataGridView);
            this.learnGroupBox.Controls.Add(this.label6);
            this.learnGroupBox.Controls.Add(this.classesListView);
            this.learnGroupBox.Controls.Add(this.learningDatasetClassesCountTextBox);
            this.learnGroupBox.Controls.Add(this.learningDatasetAttrsCountTextBox);
            this.learnGroupBox.Controls.Add(this.learningDatasetSizeTextBox);
            this.learnGroupBox.Controls.Add(this.label5);
            this.learnGroupBox.Controls.Add(this.label4);
            this.learnGroupBox.Controls.Add(this.label3);
            this.learnGroupBox.Controls.Add(this.label2);
            this.learnGroupBox.Controls.Add(this.selectLearningDatasetFileButton);
            this.learnGroupBox.Controls.Add(this.learningDatasetFileTExtBox);
            this.learnGroupBox.Controls.Add(this.label1);
            this.learnGroupBox.Location = new System.Drawing.Point(12, 12);
            this.learnGroupBox.Name = "learnGroupBox";
            this.learnGroupBox.Size = new System.Drawing.Size(710, 243);
            this.learnGroupBox.TabIndex = 1;
            this.learnGroupBox.TabStop = false;
            this.learnGroupBox.Text = "Обучение";
            // 
            // checkButton
            // 
            this.checkButton.Enabled = false;
            this.checkButton.Location = new System.Drawing.Point(538, 193);
            this.checkButton.Name = "checkButton";
            this.checkButton.Size = new System.Drawing.Size(166, 43);
            this.checkButton.TabIndex = 24;
            this.checkButton.Text = "Проверка";
            this.checkButton.UseVisualStyleBackColor = true;
            this.checkButton.Click += new System.EventHandler(this.checkButton_Click);
            // 
            // trainButton
            // 
            this.trainButton.Enabled = false;
            this.trainButton.Location = new System.Drawing.Point(367, 193);
            this.trainButton.Name = "trainButton";
            this.trainButton.Size = new System.Drawing.Size(165, 43);
            this.trainButton.TabIndex = 23;
            this.trainButton.Text = "Обучение";
            this.trainButton.UseVisualStyleBackColor = true;
            this.trainButton.Click += new System.EventHandler(this.trainButton_Click);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(449, 114);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(128, 13);
            this.label10.TabIndex = 19;
            this.label10.Text = "Скорость оптимизации:";
            // 
            // rulesAlgoComboBox
            // 
            this.rulesAlgoComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.rulesAlgoComboBox.FormattingEnabled = true;
            this.rulesAlgoComboBox.Items.AddRange(new object[] {
            "Простое",
            "Лучшее",
            "Лучшее для класса"});
            this.rulesAlgoComboBox.Location = new System.Drawing.Point(583, 84);
            this.rulesAlgoComboBox.Name = "rulesAlgoComboBox";
            this.rulesAlgoComboBox.Size = new System.Drawing.Size(121, 21);
            this.rulesAlgoComboBox.TabIndex = 18;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(480, 88);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(97, 13);
            this.label9.TabIndex = 17;
            this.label9.Text = "Обучение правил:";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // maxRulesInput
            // 
            this.maxRulesInput.Location = new System.Drawing.Point(583, 58);
            this.maxRulesInput.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.maxRulesInput.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.maxRulesInput.Name = "maxRulesInput";
            this.maxRulesInput.Size = new System.Drawing.Size(66, 20);
            this.maxRulesInput.TabIndex = 16;
            this.maxRulesInput.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.maxRulesInput.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(429, 53);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(148, 26);
            this.label8.TabIndex = 15;
            this.label8.Text = "Максимальное количество \r\nнейронов правил:";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(15, 140);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(49, 13);
            this.label7.TabIndex = 14;
            this.label7.Text = "Классы:";
            // 
            // sectionsDataGridView
            // 
            this.sectionsDataGridView.AllowUserToAddRows = false;
            this.sectionsDataGridView.AllowUserToDeleteRows = false;
            this.sectionsDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.sectionsDataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle5;
            this.sectionsDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.sectionsDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.var,
            this.varsec});
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle8.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle8.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle8.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle8.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.sectionsDataGridView.DefaultCellStyle = dataGridViewCellStyle8;
            this.sectionsDataGridView.Location = new System.Drawing.Point(238, 58);
            this.sectionsDataGridView.Name = "sectionsDataGridView";
            this.sectionsDataGridView.RowHeadersVisible = false;
            this.sectionsDataGridView.Size = new System.Drawing.Size(123, 178);
            this.sectionsDataGridView.TabIndex = 13;
            // 
            // var
            // 
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.var.DefaultCellStyle = dataGridViewCellStyle6;
            this.var.HeaderText = "Var";
            this.var.Name = "var";
            this.var.ReadOnly = true;
            // 
            // varsec
            // 
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.varsec.DefaultCellStyle = dataGridViewCellStyle7;
            this.varsec.HeaderText = "Parts";
            this.varsec.Name = "varsec";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(225, 42);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(176, 13);
            this.label6.TabIndex = 12;
            this.label6.Text = "Разбиения входных переменных:";
            // 
            // classesListView
            // 
            this.classesListView.FullRowSelect = true;
            this.classesListView.Location = new System.Drawing.Point(70, 140);
            this.classesListView.Name = "classesListView";
            this.classesListView.Size = new System.Drawing.Size(132, 97);
            this.classesListView.TabIndex = 11;
            this.classesListView.UseCompatibleStateImageBehavior = false;
            this.classesListView.View = System.Windows.Forms.View.List;
            // 
            // learningDatasetClassesCountTextBox
            // 
            this.learningDatasetClassesCountTextBox.Location = new System.Drawing.Point(145, 114);
            this.learningDatasetClassesCountTextBox.Name = "learningDatasetClassesCountTextBox";
            this.learningDatasetClassesCountTextBox.ReadOnly = true;
            this.learningDatasetClassesCountTextBox.Size = new System.Drawing.Size(57, 20);
            this.learningDatasetClassesCountTextBox.TabIndex = 10;
            // 
            // learningDatasetAttrsCountTextBox
            // 
            this.learningDatasetAttrsCountTextBox.Location = new System.Drawing.Point(145, 88);
            this.learningDatasetAttrsCountTextBox.Name = "learningDatasetAttrsCountTextBox";
            this.learningDatasetAttrsCountTextBox.ReadOnly = true;
            this.learningDatasetAttrsCountTextBox.Size = new System.Drawing.Size(57, 20);
            this.learningDatasetAttrsCountTextBox.TabIndex = 9;
            // 
            // learningDatasetSizeTextBox
            // 
            this.learningDatasetSizeTextBox.Location = new System.Drawing.Point(145, 62);
            this.learningDatasetSizeTextBox.Name = "learningDatasetSizeTextBox";
            this.learningDatasetSizeTextBox.ReadOnly = true;
            this.learningDatasetSizeTextBox.Size = new System.Drawing.Size(57, 20);
            this.learningDatasetSizeTextBox.TabIndex = 8;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(15, 117);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(111, 13);
            this.label5.TabIndex = 7;
            this.label5.Text = "Количество классов";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(15, 91);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(120, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Количество атрибутов";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 65);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(46, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Размер";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(140, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Характеристики выборки:";
            // 
            // selectLearningDatasetFileButton
            // 
            this.selectLearningDatasetFileButton.Location = new System.Drawing.Point(642, 17);
            this.selectLearningDatasetFileButton.Name = "selectLearningDatasetFileButton";
            this.selectLearningDatasetFileButton.Size = new System.Drawing.Size(62, 23);
            this.selectLearningDatasetFileButton.TabIndex = 3;
            this.selectLearningDatasetFileButton.Text = "...";
            this.selectLearningDatasetFileButton.UseVisualStyleBackColor = true;
            this.selectLearningDatasetFileButton.Click += new System.EventHandler(this.selectLearningDatasetFileButton_Click);
            // 
            // learningDatasetFileTExtBox
            // 
            this.learningDatasetFileTExtBox.Location = new System.Drawing.Point(125, 19);
            this.learningDatasetFileTExtBox.Name = "learningDatasetFileTExtBox";
            this.learningDatasetFileTExtBox.ReadOnly = true;
            this.learningDatasetFileTExtBox.Size = new System.Drawing.Size(510, 20);
            this.learningDatasetFileTExtBox.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(113, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Обучающая выборка";
            // 
            // checkFileDialog
            // 
            this.checkFileDialog.FileName = "openFileDialog1";
            // 
            // logTextBox
            // 
            this.logTextBox.BackColor = System.Drawing.Color.White;
            this.logTextBox.Location = new System.Drawing.Point(12, 261);
            this.logTextBox.Multiline = true;
            this.logTextBox.Name = "logTextBox";
            this.logTextBox.ReadOnly = true;
            this.logTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.logTextBox.Size = new System.Drawing.Size(710, 155);
            this.logTextBox.TabIndex = 2;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(408, 140);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(169, 13);
            this.label11.TabIndex = 26;
            this.label11.Text = "Максимальное число итераций:";
            // 
            // maxEpochsInput
            // 
            this.maxEpochsInput.Location = new System.Drawing.Point(583, 138);
            this.maxEpochsInput.Maximum = new decimal(new int[] {
            10000000,
            0,
            0,
            0});
            this.maxEpochsInput.Name = "maxEpochsInput";
            this.maxEpochsInput.Size = new System.Drawing.Size(121, 20);
            this.maxEpochsInput.TabIndex = 27;
            this.maxEpochsInput.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.maxEpochsInput.Value = new decimal(new int[] {
            500000,
            0,
            0,
            0});
            // 
            // optimizationSpeedTextBox
            // 
            this.optimizationSpeedTextBox.Location = new System.Drawing.Point(584, 112);
            this.optimizationSpeedTextBox.Name = "optimizationSpeedTextBox";
            this.optimizationSpeedTextBox.Size = new System.Drawing.Size(120, 20);
            this.optimizationSpeedTextBox.TabIndex = 28;
            this.optimizationSpeedTextBox.Text = "0,003";
            this.optimizationSpeedTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // accTextBox
            // 
            this.accTextBox.Location = new System.Drawing.Point(584, 164);
            this.accTextBox.Name = "accTextBox";
            this.accTextBox.Size = new System.Drawing.Size(120, 20);
            this.accTextBox.TabIndex = 30;
            this.accTextBox.Text = "0,0001";
            this.accTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(457, 167);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(120, 13);
            this.label12.TabIndex = 29;
            this.label12.Text = "Точность сходимости:";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(734, 428);
            this.Controls.Add(this.logTextBox);
            this.Controls.Add(this.learnGroupBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "NEFClass";
            this.learnGroupBox.ResumeLayout(false);
            this.learnGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.maxRulesInput)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sectionsDataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.maxEpochsInput)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog dataLoadDialog;
        private System.Windows.Forms.GroupBox learnGroupBox;
        private System.Windows.Forms.Button selectLearningDatasetFileButton;
        private System.Windows.Forms.TextBox learningDatasetFileTExtBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.OpenFileDialog trainDatasetFileDialog;
        private System.Windows.Forms.TextBox learningDatasetClassesCountTextBox;
        private System.Windows.Forms.TextBox learningDatasetAttrsCountTextBox;
        private System.Windows.Forms.TextBox learningDatasetSizeTextBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListView classesListView;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.DataGridView sectionsDataGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn var;
        private System.Windows.Forms.DataGridViewTextBoxColumn varsec;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown maxRulesInput;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ComboBox rulesAlgoComboBox;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Button trainButton;
        private System.Windows.Forms.Button checkButton;
        private System.Windows.Forms.OpenFileDialog checkFileDialog;
        private System.Windows.Forms.TextBox logTextBox;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.NumericUpDown maxEpochsInput;
        private System.Windows.Forms.TextBox optimizationSpeedTextBox;
        private System.Windows.Forms.TextBox accTextBox;
        private System.Windows.Forms.Label label12;
    }
}

