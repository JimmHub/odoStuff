namespace EmguTest
{
    partial class Form1
    {
        /// <summary>
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Обязательный метод для поддержки конструктора - не изменяйте
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.deviceSwitchButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.fpsLabel = new System.Windows.Forms.Label();
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            this.timer3 = new System.Windows.Forms.Timer(this.components);
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.memsTestOutputTimer = new System.Windows.Forms.Timer(this.components);
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.MEMSRotationTabPage = new System.Windows.Forms.TabPage();
            this.showMEMSFormButton = new System.Windows.Forms.Button();
            this.adoptiveFilterCheckBox = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.accMagnetFilterTrackBar = new System.Windows.Forms.TrackBar();
            this.gyroCheckBox = new System.Windows.Forms.CheckBox();
            this.accMagnetCheckBox = new System.Windows.Forms.CheckBox();
            this.logMEMSRichTextBox = new System.Windows.Forms.RichTextBox();
            this.featureDetectionTabPage = new System.Windows.Forms.TabPage();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.calibrationStatusLabel = new System.Windows.Forms.Label();
            this.imageIdxLabel = new System.Windows.Forms.Label();
            this.PrevCalibButton = new System.Windows.Forms.Button();
            this.nextCalibButton = new System.Windows.Forms.Button();
            this.calibPictureBoxUndist = new System.Windows.Forms.PictureBox();
            this.calibPictureBoxOriginal = new System.Windows.Forms.PictureBox();
            this.monoCameraCalibrateButton = new System.Windows.Forms.Button();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.useMapTransformCheckBox = new System.Windows.Forms.CheckBox();
            this.mapTransformApplyButton = new System.Windows.Forms.Button();
            this.label16 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.mapScaleTextBox = new System.Windows.Forms.TextBox();
            this.mapYShiftTextBox = new System.Windows.Forms.TextBox();
            this.mapXShiftTextBox = new System.Windows.Forms.TextBox();
            this.stereoCalibFolderTextBox = new System.Windows.Forms.TextBox();
            this.calibrateFromGrabbedListButton = new System.Windows.Forms.Button();
            this.stereoCalibUseRectificationCheckBox = new System.Windows.Forms.CheckBox();
            this.stereoCalibDrawLinesCheckBox = new System.Windows.Forms.CheckBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.stereoCalibPrevButton = new System.Windows.Forms.Button();
            this.stereoCalibNextButton = new System.Windows.Forms.Button();
            this.stereoImageNumLabel = new System.Windows.Forms.Label();
            this.stereoCalibrationStatusLabel = new System.Windows.Forms.Label();
            this.stereoCalibrateButton = new System.Windows.Forms.Button();
            this.rightStereoCalibPictureBox = new System.Windows.Forms.PictureBox();
            this.leftStereoCalibPictureBox = new System.Windows.Forms.PictureBox();
            this.rightStereoOriginalPictureBox = new System.Windows.Forms.PictureBox();
            this.leftStereoOriginalPictureBox = new System.Windows.Forms.PictureBox();
            this.calibratedStereoCaptureTabPage = new System.Windows.Forms.TabPage();
            this.label12 = new System.Windows.Forms.Label();
            this.prefixCalibListTextBox = new System.Windows.Forms.TextBox();
            this.saveGrabbedCalibrationListToButton = new System.Windows.Forms.Button();
            this.calibListCountLabel = new System.Windows.Forms.Label();
            this.clearCalibrationListButton = new System.Windows.Forms.Button();
            this.grabFrameForCalibrationButton = new System.Windows.Forms.Button();
            this.resumeStereoCapButton = new System.Windows.Forms.Button();
            this.changeRightCamCapButton = new System.Windows.Forms.Button();
            this.changeLeftCamCapButton = new System.Windows.Forms.Button();
            this.fileCapRadioButton = new System.Windows.Forms.RadioButton();
            this.camCapRadioButton = new System.Windows.Forms.RadioButton();
            this.label11 = new System.Windows.Forms.Label();
            this.stereoFileNameTextBox = new System.Windows.Forms.TextBox();
            this.rightCaptureTextBox = new System.Windows.Forms.TextBox();
            this.leftCaptureTextBox = new System.Windows.Forms.TextBox();
            this.stereoCapProgressTrackBar = new System.Windows.Forms.TrackBar();
            this.pauseStereoCapButton = new System.Windows.Forms.Button();
            this.startStereoCapButton = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.calibStereoCapRightPictureBox = new System.Windows.Forms.PictureBox();
            this.calibStereoCapLeftPictureBox = new System.Windows.Forms.PictureBox();
            this.stereoCapTimer = new System.Windows.Forms.Timer(this.components);
            this.stereoStreamRenderTimer = new System.Windows.Forms.Timer(this.components);
            this.stereoCalibListSaveFolderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.useCalibratedStereoRenderCheckBox = new System.Windows.Forms.CheckBox();
            this.syncDataSourcePathTextBox = new System.Windows.Forms.TextBox();
            this.testSyncDataSourceStartButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.MEMSRotationTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.accMagnetFilterTrackBar)).BeginInit();
            this.featureDetectionTabPage.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.calibPictureBoxUndist)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.calibPictureBoxOriginal)).BeginInit();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rightStereoCalibPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.leftStereoCalibPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rightStereoOriginalPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.leftStereoOriginalPictureBox)).BeginInit();
            this.calibratedStereoCaptureTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.stereoCapProgressTrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.calibStereoCapRightPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.calibStereoCapLeftPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(46, 55);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(469, 368);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // timer1
            // 
            this.timer1.Interval = 1;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // deviceSwitchButton
            // 
            this.deviceSwitchButton.Location = new System.Drawing.Point(46, 26);
            this.deviceSwitchButton.Name = "deviceSwitchButton";
            this.deviceSwitchButton.Size = new System.Drawing.Size(75, 23);
            this.deviceSwitchButton.TabIndex = 1;
            this.deviceSwitchButton.Text = "GPU";
            this.deviceSwitchButton.UseVisualStyleBackColor = true;
            this.deviceSwitchButton.Click += new System.EventHandler(this.deviceSwitchButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(158, 31);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(33, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "FPS: ";
            // 
            // fpsLabel
            // 
            this.fpsLabel.AutoSize = true;
            this.fpsLabel.Location = new System.Drawing.Point(197, 31);
            this.fpsLabel.Name = "fpsLabel";
            this.fpsLabel.Size = new System.Drawing.Size(13, 13);
            this.fpsLabel.TabIndex = 3;
            this.fpsLabel.Text = "0";
            // 
            // timer2
            // 
            this.timer2.Tick += new System.EventHandler(this.timer2_Tick);
            // 
            // timer3
            // 
            this.timer3.Interval = 1;
            this.timer3.Tick += new System.EventHandler(this.timer3_Tick);
            // 
            // pictureBox2
            // 
            this.pictureBox2.Location = new System.Drawing.Point(523, 55);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(469, 368);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox2.TabIndex = 4;
            this.pictureBox2.TabStop = false;
            this.pictureBox2.Click += new System.EventHandler(this.pictureBox2_Click);
            // 
            // memsTestOutputTimer
            // 
            this.memsTestOutputTimer.Interval = 1;
            this.memsTestOutputTimer.Tick += new System.EventHandler(this.memsTestOutputTimer_Tick);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.MEMSRotationTabPage);
            this.tabControl1.Controls.Add(this.featureDetectionTabPage);
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.calibratedStereoCaptureTabPage);
            this.tabControl1.Location = new System.Drawing.Point(10, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1029, 738);
            this.tabControl1.TabIndex = 6;
            // 
            // MEMSRotationTabPage
            // 
            this.MEMSRotationTabPage.Controls.Add(this.showMEMSFormButton);
            this.MEMSRotationTabPage.Controls.Add(this.adoptiveFilterCheckBox);
            this.MEMSRotationTabPage.Controls.Add(this.label2);
            this.MEMSRotationTabPage.Controls.Add(this.accMagnetFilterTrackBar);
            this.MEMSRotationTabPage.Controls.Add(this.gyroCheckBox);
            this.MEMSRotationTabPage.Controls.Add(this.accMagnetCheckBox);
            this.MEMSRotationTabPage.Controls.Add(this.logMEMSRichTextBox);
            this.MEMSRotationTabPage.Location = new System.Drawing.Point(4, 22);
            this.MEMSRotationTabPage.Name = "MEMSRotationTabPage";
            this.MEMSRotationTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.MEMSRotationTabPage.Size = new System.Drawing.Size(1021, 712);
            this.MEMSRotationTabPage.TabIndex = 1;
            this.MEMSRotationTabPage.Text = "MEMSOrientation";
            this.MEMSRotationTabPage.UseVisualStyleBackColor = true;
            // 
            // showMEMSFormButton
            // 
            this.showMEMSFormButton.Location = new System.Drawing.Point(6, 463);
            this.showMEMSFormButton.Name = "showMEMSFormButton";
            this.showMEMSFormButton.Size = new System.Drawing.Size(168, 46);
            this.showMEMSFormButton.TabIndex = 12;
            this.showMEMSFormButton.Text = "show MEMS form";
            this.showMEMSFormButton.UseVisualStyleBackColor = true;
            this.showMEMSFormButton.Click += new System.EventHandler(this.showMEMSFormButton_Click);
            // 
            // adoptiveFilterCheckBox
            // 
            this.adoptiveFilterCheckBox.AutoSize = true;
            this.adoptiveFilterCheckBox.Checked = true;
            this.adoptiveFilterCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.adoptiveFilterCheckBox.Location = new System.Drawing.Point(201, 340);
            this.adoptiveFilterCheckBox.Name = "adoptiveFilterCheckBox";
            this.adoptiveFilterCheckBox.Size = new System.Drawing.Size(89, 17);
            this.adoptiveFilterCheckBox.TabIndex = 11;
            this.adoptiveFilterCheckBox.Text = "adoptive filter";
            this.adoptiveFilterCheckBox.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(172, 382);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(83, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "accMagnet filter";
            // 
            // accMagnetFilterTrackBar
            // 
            this.accMagnetFilterTrackBar.Location = new System.Drawing.Point(6, 398);
            this.accMagnetFilterTrackBar.Maximum = 1000;
            this.accMagnetFilterTrackBar.Name = "accMagnetFilterTrackBar";
            this.accMagnetFilterTrackBar.Size = new System.Drawing.Size(419, 45);
            this.accMagnetFilterTrackBar.TabIndex = 9;
            this.accMagnetFilterTrackBar.Value = 100;
            this.accMagnetFilterTrackBar.Scroll += new System.EventHandler(this.accMagnetFilterTrackBar_Scroll);
            // 
            // gyroCheckBox
            // 
            this.gyroCheckBox.AutoSize = true;
            this.gyroCheckBox.Checked = true;
            this.gyroCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.gyroCheckBox.Location = new System.Drawing.Point(6, 363);
            this.gyroCheckBox.Name = "gyroCheckBox";
            this.gyroCheckBox.Size = new System.Drawing.Size(75, 17);
            this.gyroCheckBox.TabIndex = 8;
            this.gyroCheckBox.Text = "gyroscope";
            this.gyroCheckBox.UseVisualStyleBackColor = true;
            // 
            // accMagnetCheckBox
            // 
            this.accMagnetCheckBox.AutoSize = true;
            this.accMagnetCheckBox.Checked = true;
            this.accMagnetCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.accMagnetCheckBox.Location = new System.Drawing.Point(6, 340);
            this.accMagnetCheckBox.Name = "accMagnetCheckBox";
            this.accMagnetCheckBox.Size = new System.Drawing.Size(79, 17);
            this.accMagnetCheckBox.TabIndex = 7;
            this.accMagnetCheckBox.Text = "accmagnet";
            this.accMagnetCheckBox.UseVisualStyleBackColor = true;
            // 
            // logMEMSRichTextBox
            // 
            this.logMEMSRichTextBox.Location = new System.Drawing.Point(6, 6);
            this.logMEMSRichTextBox.Name = "logMEMSRichTextBox";
            this.logMEMSRichTextBox.Size = new System.Drawing.Size(998, 317);
            this.logMEMSRichTextBox.TabIndex = 6;
            this.logMEMSRichTextBox.Text = "";
            // 
            // featureDetectionTabPage
            // 
            this.featureDetectionTabPage.Controls.Add(this.pictureBox1);
            this.featureDetectionTabPage.Controls.Add(this.deviceSwitchButton);
            this.featureDetectionTabPage.Controls.Add(this.fpsLabel);
            this.featureDetectionTabPage.Controls.Add(this.pictureBox2);
            this.featureDetectionTabPage.Controls.Add(this.label1);
            this.featureDetectionTabPage.Location = new System.Drawing.Point(4, 22);
            this.featureDetectionTabPage.Name = "featureDetectionTabPage";
            this.featureDetectionTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.featureDetectionTabPage.Size = new System.Drawing.Size(1021, 712);
            this.featureDetectionTabPage.TabIndex = 0;
            this.featureDetectionTabPage.Text = "featureDetection";
            this.featureDetectionTabPage.UseVisualStyleBackColor = true;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.label4);
            this.tabPage1.Controls.Add(this.label3);
            this.tabPage1.Controls.Add(this.calibrationStatusLabel);
            this.tabPage1.Controls.Add(this.imageIdxLabel);
            this.tabPage1.Controls.Add(this.PrevCalibButton);
            this.tabPage1.Controls.Add(this.nextCalibButton);
            this.tabPage1.Controls.Add(this.calibPictureBoxUndist);
            this.tabPage1.Controls.Add(this.calibPictureBoxOriginal);
            this.tabPage1.Controls.Add(this.monoCameraCalibrateButton);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1021, 712);
            this.tabPage1.TabIndex = 2;
            this.tabPage1.Text = "Mono calibration";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(484, 99);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(59, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "undistorted";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(21, 99);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(40, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "original";
            // 
            // calibrationStatusLabel
            // 
            this.calibrationStatusLabel.AutoSize = true;
            this.calibrationStatusLabel.Location = new System.Drawing.Point(355, 26);
            this.calibrationStatusLabel.Name = "calibrationStatusLabel";
            this.calibrationStatusLabel.Size = new System.Drawing.Size(31, 13);
            this.calibrationStatusLabel.TabIndex = 6;
            this.calibrationStatusLabel.Text = "none";
            // 
            // imageIdxLabel
            // 
            this.imageIdxLabel.AutoSize = true;
            this.imageIdxLabel.Location = new System.Drawing.Point(644, 21);
            this.imageIdxLabel.Name = "imageIdxLabel";
            this.imageIdxLabel.Size = new System.Drawing.Size(16, 13);
            this.imageIdxLabel.TabIndex = 5;
            this.imageIdxLabel.Text = "-1";
            // 
            // PrevCalibButton
            // 
            this.PrevCalibButton.Location = new System.Drawing.Point(537, 6);
            this.PrevCalibButton.Name = "PrevCalibButton";
            this.PrevCalibButton.Size = new System.Drawing.Size(91, 43);
            this.PrevCalibButton.TabIndex = 4;
            this.PrevCalibButton.Text = "Prev";
            this.PrevCalibButton.UseVisualStyleBackColor = true;
            this.PrevCalibButton.Click += new System.EventHandler(this.PrevCalibButton_Click);
            // 
            // nextCalibButton
            // 
            this.nextCalibButton.Location = new System.Drawing.Point(685, 6);
            this.nextCalibButton.Name = "nextCalibButton";
            this.nextCalibButton.Size = new System.Drawing.Size(91, 43);
            this.nextCalibButton.TabIndex = 3;
            this.nextCalibButton.Text = "Next";
            this.nextCalibButton.UseVisualStyleBackColor = true;
            this.nextCalibButton.Click += new System.EventHandler(this.nextCalibButton_Click);
            // 
            // calibPictureBoxUndist
            // 
            this.calibPictureBoxUndist.Location = new System.Drawing.Point(487, 115);
            this.calibPictureBoxUndist.Name = "calibPictureBoxUndist";
            this.calibPictureBoxUndist.Size = new System.Drawing.Size(420, 420);
            this.calibPictureBoxUndist.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.calibPictureBoxUndist.TabIndex = 2;
            this.calibPictureBoxUndist.TabStop = false;
            // 
            // calibPictureBoxOriginal
            // 
            this.calibPictureBoxOriginal.Location = new System.Drawing.Point(24, 115);
            this.calibPictureBoxOriginal.Name = "calibPictureBoxOriginal";
            this.calibPictureBoxOriginal.Size = new System.Drawing.Size(420, 420);
            this.calibPictureBoxOriginal.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.calibPictureBoxOriginal.TabIndex = 1;
            this.calibPictureBoxOriginal.TabStop = false;
            // 
            // monoCameraCalibrateButton
            // 
            this.monoCameraCalibrateButton.Location = new System.Drawing.Point(24, 26);
            this.monoCameraCalibrateButton.Name = "monoCameraCalibrateButton";
            this.monoCameraCalibrateButton.Size = new System.Drawing.Size(308, 23);
            this.monoCameraCalibrateButton.TabIndex = 0;
            this.monoCameraCalibrateButton.Text = "monoCameraCalibrateButton";
            this.monoCameraCalibrateButton.UseVisualStyleBackColor = true;
            this.monoCameraCalibrateButton.Click += new System.EventHandler(this.monoCameraCalibrateButton_Click);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.useMapTransformCheckBox);
            this.tabPage2.Controls.Add(this.mapTransformApplyButton);
            this.tabPage2.Controls.Add(this.label16);
            this.tabPage2.Controls.Add(this.label15);
            this.tabPage2.Controls.Add(this.label14);
            this.tabPage2.Controls.Add(this.mapScaleTextBox);
            this.tabPage2.Controls.Add(this.mapYShiftTextBox);
            this.tabPage2.Controls.Add(this.mapXShiftTextBox);
            this.tabPage2.Controls.Add(this.stereoCalibFolderTextBox);
            this.tabPage2.Controls.Add(this.calibrateFromGrabbedListButton);
            this.tabPage2.Controls.Add(this.stereoCalibUseRectificationCheckBox);
            this.tabPage2.Controls.Add(this.stereoCalibDrawLinesCheckBox);
            this.tabPage2.Controls.Add(this.label8);
            this.tabPage2.Controls.Add(this.label7);
            this.tabPage2.Controls.Add(this.label6);
            this.tabPage2.Controls.Add(this.label5);
            this.tabPage2.Controls.Add(this.stereoCalibPrevButton);
            this.tabPage2.Controls.Add(this.stereoCalibNextButton);
            this.tabPage2.Controls.Add(this.stereoImageNumLabel);
            this.tabPage2.Controls.Add(this.stereoCalibrationStatusLabel);
            this.tabPage2.Controls.Add(this.stereoCalibrateButton);
            this.tabPage2.Controls.Add(this.rightStereoCalibPictureBox);
            this.tabPage2.Controls.Add(this.leftStereoCalibPictureBox);
            this.tabPage2.Controls.Add(this.rightStereoOriginalPictureBox);
            this.tabPage2.Controls.Add(this.leftStereoOriginalPictureBox);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(1021, 712);
            this.tabPage2.TabIndex = 3;
            this.tabPage2.Text = "Stereo calibration";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // useMapTransformCheckBox
            // 
            this.useMapTransformCheckBox.AutoSize = true;
            this.useMapTransformCheckBox.Location = new System.Drawing.Point(868, 421);
            this.useMapTransformCheckBox.Name = "useMapTransformCheckBox";
            this.useMapTransformCheckBox.Size = new System.Drawing.Size(112, 17);
            this.useMapTransformCheckBox.TabIndex = 27;
            this.useMapTransformCheckBox.Text = "use map transform";
            this.useMapTransformCheckBox.UseVisualStyleBackColor = true;
            this.useMapTransformCheckBox.CheckedChanged += new System.EventHandler(this.useMapTransformCheckBox_CheckedChanged);
            // 
            // mapTransformApplyButton
            // 
            this.mapTransformApplyButton.Location = new System.Drawing.Point(892, 522);
            this.mapTransformApplyButton.Name = "mapTransformApplyButton";
            this.mapTransformApplyButton.Size = new System.Drawing.Size(88, 32);
            this.mapTransformApplyButton.TabIndex = 26;
            this.mapTransformApplyButton.Text = "apply";
            this.mapTransformApplyButton.UseVisualStyleBackColor = true;
            this.mapTransformApplyButton.Click += new System.EventHandler(this.mapTransformApplyButton_Click);
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(871, 500);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(35, 13);
            this.label16.TabIndex = 25;
            this.label16.Text = "scale:";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(867, 474);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(39, 13);
            this.label15.TabIndex = 24;
            this.label15.Text = "Y shift:";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(867, 448);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(39, 13);
            this.label14.TabIndex = 23;
            this.label14.Text = "X shift:";
            // 
            // mapScaleTextBox
            // 
            this.mapScaleTextBox.Location = new System.Drawing.Point(910, 496);
            this.mapScaleTextBox.Name = "mapScaleTextBox";
            this.mapScaleTextBox.Size = new System.Drawing.Size(105, 20);
            this.mapScaleTextBox.TabIndex = 21;
            this.mapScaleTextBox.Text = "1.0";
            // 
            // mapYShiftTextBox
            // 
            this.mapYShiftTextBox.Location = new System.Drawing.Point(910, 470);
            this.mapYShiftTextBox.Name = "mapYShiftTextBox";
            this.mapYShiftTextBox.Size = new System.Drawing.Size(105, 20);
            this.mapYShiftTextBox.TabIndex = 20;
            this.mapYShiftTextBox.Text = "0";
            // 
            // mapXShiftTextBox
            // 
            this.mapXShiftTextBox.Location = new System.Drawing.Point(910, 444);
            this.mapXShiftTextBox.Name = "mapXShiftTextBox";
            this.mapXShiftTextBox.Size = new System.Drawing.Size(105, 20);
            this.mapXShiftTextBox.TabIndex = 19;
            this.mapXShiftTextBox.Text = "0";
            // 
            // stereoCalibFolderTextBox
            // 
            this.stereoCalibFolderTextBox.Location = new System.Drawing.Point(866, 112);
            this.stereoCalibFolderTextBox.Name = "stereoCalibFolderTextBox";
            this.stereoCalibFolderTextBox.Size = new System.Drawing.Size(149, 20);
            this.stereoCalibFolderTextBox.TabIndex = 18;
            // 
            // calibrateFromGrabbedListButton
            // 
            this.calibrateFromGrabbedListButton.Location = new System.Drawing.Point(883, 185);
            this.calibrateFromGrabbedListButton.Name = "calibrateFromGrabbedListButton";
            this.calibrateFromGrabbedListButton.Size = new System.Drawing.Size(118, 41);
            this.calibrateFromGrabbedListButton.TabIndex = 17;
            this.calibrateFromGrabbedListButton.Text = "calibrate from list";
            this.calibrateFromGrabbedListButton.UseVisualStyleBackColor = true;
            this.calibrateFromGrabbedListButton.Click += new System.EventHandler(this.calibrateFromGrabbedListButton_Click);
            // 
            // stereoCalibUseRectificationCheckBox
            // 
            this.stereoCalibUseRectificationCheckBox.AutoSize = true;
            this.stereoCalibUseRectificationCheckBox.Location = new System.Drawing.Point(870, 371);
            this.stereoCalibUseRectificationCheckBox.Name = "stereoCalibUseRectificationCheckBox";
            this.stereoCalibUseRectificationCheckBox.Size = new System.Drawing.Size(100, 17);
            this.stereoCalibUseRectificationCheckBox.TabIndex = 16;
            this.stereoCalibUseRectificationCheckBox.Text = "use rectification";
            this.stereoCalibUseRectificationCheckBox.UseVisualStyleBackColor = true;
            this.stereoCalibUseRectificationCheckBox.CheckedChanged += new System.EventHandler(this.stereoCalibUseRectificationCheckBox_CheckedChanged);
            // 
            // stereoCalibDrawLinesCheckBox
            // 
            this.stereoCalibDrawLinesCheckBox.AutoSize = true;
            this.stereoCalibDrawLinesCheckBox.Location = new System.Drawing.Point(870, 348);
            this.stereoCalibDrawLinesCheckBox.Name = "stereoCalibDrawLinesCheckBox";
            this.stereoCalibDrawLinesCheckBox.Size = new System.Drawing.Size(73, 17);
            this.stereoCalibDrawLinesCheckBox.TabIndex = 15;
            this.stereoCalibDrawLinesCheckBox.Text = "draw lines";
            this.stereoCalibDrawLinesCheckBox.UseVisualStyleBackColor = true;
            this.stereoCalibDrawLinesCheckBox.CheckedChanged += new System.EventHandler(this.stereoCalibDrawLinesCheckBox_CheckedChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(3, 461);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(29, 13);
            this.label8.TabIndex = 14;
            this.label8.Text = "calib";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(3, 129);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(24, 13);
            this.label7.TabIndex = 13;
            this.label7.Text = "orig";
            this.label7.Click += new System.EventHandler(this.label7_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(457, 13);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(27, 13);
            this.label6.TabIndex = 12;
            this.label6.Text = "right";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(30, 13);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(21, 13);
            this.label5.TabIndex = 11;
            this.label5.Text = "left";
            // 
            // stereoCalibPrevButton
            // 
            this.stereoCalibPrevButton.Location = new System.Drawing.Point(870, 266);
            this.stereoCalibPrevButton.Name = "stereoCalibPrevButton";
            this.stereoCalibPrevButton.Size = new System.Drawing.Size(45, 54);
            this.stereoCalibPrevButton.TabIndex = 10;
            this.stereoCalibPrevButton.Text = "prev";
            this.stereoCalibPrevButton.UseVisualStyleBackColor = true;
            this.stereoCalibPrevButton.Click += new System.EventHandler(this.stereoCalibPrevButton_Click);
            // 
            // stereoCalibNextButton
            // 
            this.stereoCalibNextButton.Location = new System.Drawing.Point(943, 266);
            this.stereoCalibNextButton.Name = "stereoCalibNextButton";
            this.stereoCalibNextButton.Size = new System.Drawing.Size(45, 54);
            this.stereoCalibNextButton.TabIndex = 9;
            this.stereoCalibNextButton.Text = "next";
            this.stereoCalibNextButton.UseVisualStyleBackColor = true;
            this.stereoCalibNextButton.Click += new System.EventHandler(this.stereoCalibNextButton_Click);
            // 
            // stereoImageNumLabel
            // 
            this.stereoImageNumLabel.AutoSize = true;
            this.stereoImageNumLabel.Location = new System.Drawing.Point(921, 287);
            this.stereoImageNumLabel.Name = "stereoImageNumLabel";
            this.stereoImageNumLabel.Size = new System.Drawing.Size(16, 13);
            this.stereoImageNumLabel.TabIndex = 8;
            this.stereoImageNumLabel.Text = "-1";
            // 
            // stereoCalibrationStatusLabel
            // 
            this.stereoCalibrationStatusLabel.AutoSize = true;
            this.stereoCalibrationStatusLabel.Location = new System.Drawing.Point(880, 238);
            this.stereoCalibrationStatusLabel.Name = "stereoCalibrationStatusLabel";
            this.stereoCalibrationStatusLabel.Size = new System.Drawing.Size(35, 13);
            this.stereoCalibrationStatusLabel.TabIndex = 7;
            this.stereoCalibrationStatusLabel.Text = "status";
            this.stereoCalibrationStatusLabel.TextChanged += new System.EventHandler(this.stereoCalibrationStatusLabel_TextChanged);
            // 
            // stereoCalibrateButton
            // 
            this.stereoCalibrateButton.Location = new System.Drawing.Point(883, 138);
            this.stereoCalibrateButton.Name = "stereoCalibrateButton";
            this.stereoCalibrateButton.Size = new System.Drawing.Size(118, 41);
            this.stereoCalibrateButton.TabIndex = 6;
            this.stereoCalibrateButton.Text = "calibrate from folder";
            this.stereoCalibrateButton.UseVisualStyleBackColor = true;
            this.stereoCalibrateButton.Click += new System.EventHandler(this.stereoCalibrateButton_Click);
            // 
            // rightStereoCalibPictureBox
            // 
            this.rightStereoCalibPictureBox.Location = new System.Drawing.Point(460, 335);
            this.rightStereoCalibPictureBox.Name = "rightStereoCalibPictureBox";
            this.rightStereoCalibPictureBox.Size = new System.Drawing.Size(400, 300);
            this.rightStereoCalibPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.rightStereoCalibPictureBox.TabIndex = 5;
            this.rightStereoCalibPictureBox.TabStop = false;
            // 
            // leftStereoCalibPictureBox
            // 
            this.leftStereoCalibPictureBox.Location = new System.Drawing.Point(33, 335);
            this.leftStereoCalibPictureBox.Name = "leftStereoCalibPictureBox";
            this.leftStereoCalibPictureBox.Size = new System.Drawing.Size(400, 300);
            this.leftStereoCalibPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.leftStereoCalibPictureBox.TabIndex = 4;
            this.leftStereoCalibPictureBox.TabStop = false;
            // 
            // rightStereoOriginalPictureBox
            // 
            this.rightStereoOriginalPictureBox.Location = new System.Drawing.Point(460, 29);
            this.rightStereoOriginalPictureBox.Name = "rightStereoOriginalPictureBox";
            this.rightStereoOriginalPictureBox.Size = new System.Drawing.Size(400, 300);
            this.rightStereoOriginalPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.rightStereoOriginalPictureBox.TabIndex = 3;
            this.rightStereoOriginalPictureBox.TabStop = false;
            this.rightStereoOriginalPictureBox.Click += new System.EventHandler(this.pictureBox4_Click);
            // 
            // leftStereoOriginalPictureBox
            // 
            this.leftStereoOriginalPictureBox.Location = new System.Drawing.Point(33, 29);
            this.leftStereoOriginalPictureBox.Name = "leftStereoOriginalPictureBox";
            this.leftStereoOriginalPictureBox.Size = new System.Drawing.Size(400, 300);
            this.leftStereoOriginalPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.leftStereoOriginalPictureBox.TabIndex = 2;
            this.leftStereoOriginalPictureBox.TabStop = false;
            // 
            // calibratedStereoCaptureTabPage
            // 
            this.calibratedStereoCaptureTabPage.Controls.Add(this.testSyncDataSourceStartButton);
            this.calibratedStereoCaptureTabPage.Controls.Add(this.syncDataSourcePathTextBox);
            this.calibratedStereoCaptureTabPage.Controls.Add(this.useCalibratedStereoRenderCheckBox);
            this.calibratedStereoCaptureTabPage.Controls.Add(this.label12);
            this.calibratedStereoCaptureTabPage.Controls.Add(this.prefixCalibListTextBox);
            this.calibratedStereoCaptureTabPage.Controls.Add(this.saveGrabbedCalibrationListToButton);
            this.calibratedStereoCaptureTabPage.Controls.Add(this.calibListCountLabel);
            this.calibratedStereoCaptureTabPage.Controls.Add(this.clearCalibrationListButton);
            this.calibratedStereoCaptureTabPage.Controls.Add(this.grabFrameForCalibrationButton);
            this.calibratedStereoCaptureTabPage.Controls.Add(this.resumeStereoCapButton);
            this.calibratedStereoCaptureTabPage.Controls.Add(this.changeRightCamCapButton);
            this.calibratedStereoCaptureTabPage.Controls.Add(this.changeLeftCamCapButton);
            this.calibratedStereoCaptureTabPage.Controls.Add(this.fileCapRadioButton);
            this.calibratedStereoCaptureTabPage.Controls.Add(this.camCapRadioButton);
            this.calibratedStereoCaptureTabPage.Controls.Add(this.label11);
            this.calibratedStereoCaptureTabPage.Controls.Add(this.stereoFileNameTextBox);
            this.calibratedStereoCaptureTabPage.Controls.Add(this.rightCaptureTextBox);
            this.calibratedStereoCaptureTabPage.Controls.Add(this.leftCaptureTextBox);
            this.calibratedStereoCaptureTabPage.Controls.Add(this.stereoCapProgressTrackBar);
            this.calibratedStereoCaptureTabPage.Controls.Add(this.pauseStereoCapButton);
            this.calibratedStereoCaptureTabPage.Controls.Add(this.startStereoCapButton);
            this.calibratedStereoCaptureTabPage.Controls.Add(this.label9);
            this.calibratedStereoCaptureTabPage.Controls.Add(this.label10);
            this.calibratedStereoCaptureTabPage.Controls.Add(this.calibStereoCapRightPictureBox);
            this.calibratedStereoCaptureTabPage.Controls.Add(this.calibStereoCapLeftPictureBox);
            this.calibratedStereoCaptureTabPage.Location = new System.Drawing.Point(4, 22);
            this.calibratedStereoCaptureTabPage.Name = "calibratedStereoCaptureTabPage";
            this.calibratedStereoCaptureTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.calibratedStereoCaptureTabPage.Size = new System.Drawing.Size(1021, 712);
            this.calibratedStereoCaptureTabPage.TabIndex = 4;
            this.calibratedStereoCaptureTabPage.Text = "calibratedStereoCaptureT";
            this.calibratedStereoCaptureTabPage.UseVisualStyleBackColor = true;
            this.calibratedStereoCaptureTabPage.Click += new System.EventHandler(this.calibratedStereoCaptureTabPage_Click);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(300, 579);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(35, 13);
            this.label12.TabIndex = 35;
            this.label12.Text = "prefix:";
            // 
            // prefixCalibListTextBox
            // 
            this.prefixCalibListTextBox.Location = new System.Drawing.Point(341, 576);
            this.prefixCalibListTextBox.Name = "prefixCalibListTextBox";
            this.prefixCalibListTextBox.Size = new System.Drawing.Size(87, 20);
            this.prefixCalibListTextBox.TabIndex = 34;
            this.prefixCalibListTextBox.Text = "stereo";
            // 
            // saveGrabbedCalibrationListToButton
            // 
            this.saveGrabbedCalibrationListToButton.Location = new System.Drawing.Point(98, 568);
            this.saveGrabbedCalibrationListToButton.Name = "saveGrabbedCalibrationListToButton";
            this.saveGrabbedCalibrationListToButton.Size = new System.Drawing.Size(196, 35);
            this.saveGrabbedCalibrationListToButton.TabIndex = 33;
            this.saveGrabbedCalibrationListToButton.Text = "Save grabbed calibration list";
            this.saveGrabbedCalibrationListToButton.UseVisualStyleBackColor = true;
            this.saveGrabbedCalibrationListToButton.Click += new System.EventHandler(this.saveGrabbedCalibrationListToButton_Click);
            // 
            // calibListCountLabel
            // 
            this.calibListCountLabel.AutoSize = true;
            this.calibListCountLabel.Location = new System.Drawing.Point(191, 518);
            this.calibListCountLabel.Name = "calibListCountLabel";
            this.calibListCountLabel.Size = new System.Drawing.Size(13, 13);
            this.calibListCountLabel.TabIndex = 31;
            this.calibListCountLabel.Text = "0";
            // 
            // clearCalibrationListButton
            // 
            this.clearCalibrationListButton.Location = new System.Drawing.Point(210, 507);
            this.clearCalibrationListButton.Name = "clearCalibrationListButton";
            this.clearCalibrationListButton.Size = new System.Drawing.Size(84, 35);
            this.clearCalibrationListButton.TabIndex = 30;
            this.clearCalibrationListButton.Text = "Clear grabbed calibration list";
            this.clearCalibrationListButton.UseVisualStyleBackColor = true;
            this.clearCalibrationListButton.Click += new System.EventHandler(this.clearCalibrationListButton_Click);
            // 
            // grabFrameForCalibrationButton
            // 
            this.grabFrameForCalibrationButton.Location = new System.Drawing.Point(98, 507);
            this.grabFrameForCalibrationButton.Name = "grabFrameForCalibrationButton";
            this.grabFrameForCalibrationButton.Size = new System.Drawing.Size(84, 35);
            this.grabFrameForCalibrationButton.TabIndex = 29;
            this.grabFrameForCalibrationButton.Text = "Grab for calibrate";
            this.grabFrameForCalibrationButton.UseVisualStyleBackColor = true;
            this.grabFrameForCalibrationButton.Click += new System.EventHandler(this.grabFrameForCalibrationButton_Click);
            // 
            // resumeStereoCapButton
            // 
            this.resumeStereoCapButton.Location = new System.Drawing.Point(289, 71);
            this.resumeStereoCapButton.Name = "resumeStereoCapButton";
            this.resumeStereoCapButton.Size = new System.Drawing.Size(75, 23);
            this.resumeStereoCapButton.TabIndex = 28;
            this.resumeStereoCapButton.Text = "Resume";
            this.resumeStereoCapButton.UseVisualStyleBackColor = true;
            this.resumeStereoCapButton.Click += new System.EventHandler(this.resumeStereoCapButton_Click);
            // 
            // changeRightCamCapButton
            // 
            this.changeRightCamCapButton.Location = new System.Drawing.Point(661, 166);
            this.changeRightCamCapButton.Name = "changeRightCamCapButton";
            this.changeRightCamCapButton.Size = new System.Drawing.Size(75, 23);
            this.changeRightCamCapButton.TabIndex = 27;
            this.changeRightCamCapButton.Text = "change";
            this.changeRightCamCapButton.UseVisualStyleBackColor = true;
            this.changeRightCamCapButton.Click += new System.EventHandler(this.changeRightCamCapButton_Click);
            // 
            // changeLeftCamCapButton
            // 
            this.changeLeftCamCapButton.Location = new System.Drawing.Point(228, 167);
            this.changeLeftCamCapButton.Name = "changeLeftCamCapButton";
            this.changeLeftCamCapButton.Size = new System.Drawing.Size(75, 23);
            this.changeLeftCamCapButton.TabIndex = 26;
            this.changeLeftCamCapButton.Text = "change";
            this.changeLeftCamCapButton.UseVisualStyleBackColor = true;
            this.changeLeftCamCapButton.Click += new System.EventHandler(this.changeLeftCamCapButton_Click);
            // 
            // fileCapRadioButton
            // 
            this.fileCapRadioButton.AutoSize = true;
            this.fileCapRadioButton.Checked = true;
            this.fileCapRadioButton.Location = new System.Drawing.Point(586, 78);
            this.fileCapRadioButton.Name = "fileCapRadioButton";
            this.fileCapRadioButton.Size = new System.Drawing.Size(116, 17);
            this.fileCapRadioButton.TabIndex = 25;
            this.fileCapRadioButton.TabStop = true;
            this.fileCapRadioButton.Text = "fileCapRadioButton";
            this.fileCapRadioButton.UseVisualStyleBackColor = true;
            // 
            // camCapRadioButton
            // 
            this.camCapRadioButton.AutoSize = true;
            this.camCapRadioButton.Location = new System.Drawing.Point(586, 55);
            this.camCapRadioButton.Name = "camCapRadioButton";
            this.camCapRadioButton.Size = new System.Drawing.Size(123, 17);
            this.camCapRadioButton.TabIndex = 24;
            this.camCapRadioButton.Text = "camCapRadioButton";
            this.camCapRadioButton.UseVisualStyleBackColor = true;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(489, 32);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(91, 13);
            this.label11.TabIndex = 23;
            this.label11.Text = "stereo movie path";
            // 
            // stereoFileNameTextBox
            // 
            this.stereoFileNameTextBox.Location = new System.Drawing.Point(586, 29);
            this.stereoFileNameTextBox.Name = "stereoFileNameTextBox";
            this.stereoFileNameTextBox.Size = new System.Drawing.Size(339, 20);
            this.stereoFileNameTextBox.TabIndex = 22;
            // 
            // rightCaptureTextBox
            // 
            this.rightCaptureTextBox.Location = new System.Drawing.Point(555, 169);
            this.rightCaptureTextBox.Name = "rightCaptureTextBox";
            this.rightCaptureTextBox.Size = new System.Drawing.Size(100, 20);
            this.rightCaptureTextBox.TabIndex = 21;
            this.rightCaptureTextBox.Text = "1";
            // 
            // leftCaptureTextBox
            // 
            this.leftCaptureTextBox.Location = new System.Drawing.Point(122, 169);
            this.leftCaptureTextBox.Name = "leftCaptureTextBox";
            this.leftCaptureTextBox.Size = new System.Drawing.Size(100, 20);
            this.leftCaptureTextBox.TabIndex = 20;
            this.leftCaptureTextBox.Text = "0";
            // 
            // stereoCapProgressTrackBar
            // 
            this.stereoCapProgressTrackBar.Location = new System.Drawing.Point(98, 113);
            this.stereoCapProgressTrackBar.Name = "stereoCapProgressTrackBar";
            this.stereoCapProgressTrackBar.Size = new System.Drawing.Size(827, 45);
            this.stereoCapProgressTrackBar.TabIndex = 19;
            // 
            // pauseStereoCapButton
            // 
            this.pauseStereoCapButton.Location = new System.Drawing.Point(194, 71);
            this.pauseStereoCapButton.Name = "pauseStereoCapButton";
            this.pauseStereoCapButton.Size = new System.Drawing.Size(75, 23);
            this.pauseStereoCapButton.TabIndex = 18;
            this.pauseStereoCapButton.Text = "Pause";
            this.pauseStereoCapButton.UseVisualStyleBackColor = true;
            this.pauseStereoCapButton.Click += new System.EventHandler(this.pauseStereoCapButton_Click);
            // 
            // startStereoCapButton
            // 
            this.startStereoCapButton.Location = new System.Drawing.Point(98, 71);
            this.startStereoCapButton.Name = "startStereoCapButton";
            this.startStereoCapButton.Size = new System.Drawing.Size(75, 23);
            this.startStereoCapButton.TabIndex = 17;
            this.startStereoCapButton.Text = "Start";
            this.startStereoCapButton.UseVisualStyleBackColor = true;
            this.startStereoCapButton.Click += new System.EventHandler(this.startStereoCapButton_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(522, 169);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(27, 13);
            this.label9.TabIndex = 16;
            this.label9.Text = "right";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(95, 169);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(21, 13);
            this.label10.TabIndex = 15;
            this.label10.Text = "left";
            // 
            // calibStereoCapRightPictureBox
            // 
            this.calibStereoCapRightPictureBox.Location = new System.Drawing.Point(525, 201);
            this.calibStereoCapRightPictureBox.Name = "calibStereoCapRightPictureBox";
            this.calibStereoCapRightPictureBox.Size = new System.Drawing.Size(400, 300);
            this.calibStereoCapRightPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.calibStereoCapRightPictureBox.TabIndex = 14;
            this.calibStereoCapRightPictureBox.TabStop = false;
            // 
            // calibStereoCapLeftPictureBox
            // 
            this.calibStereoCapLeftPictureBox.Location = new System.Drawing.Point(98, 201);
            this.calibStereoCapLeftPictureBox.Name = "calibStereoCapLeftPictureBox";
            this.calibStereoCapLeftPictureBox.Size = new System.Drawing.Size(400, 300);
            this.calibStereoCapLeftPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.calibStereoCapLeftPictureBox.TabIndex = 13;
            this.calibStereoCapLeftPictureBox.TabStop = false;
            // 
            // stereoCapTimer
            // 
            this.stereoCapTimer.Interval = 1;
            this.stereoCapTimer.Tick += new System.EventHandler(this.stereoCapTimer_Tick);
            // 
            // stereoStreamRenderTimer
            // 
            this.stereoStreamRenderTimer.Interval = 1;
            this.stereoStreamRenderTimer.Tick += new System.EventHandler(this.stereoStreamRenderTimer_Tick);
            // 
            // useCalibratedStereoRenderCheckBox
            // 
            this.useCalibratedStereoRenderCheckBox.AutoSize = true;
            this.useCalibratedStereoRenderCheckBox.Location = new System.Drawing.Point(331, 507);
            this.useCalibratedStereoRenderCheckBox.Name = "useCalibratedStereoRenderCheckBox";
            this.useCalibratedStereoRenderCheckBox.Size = new System.Drawing.Size(92, 17);
            this.useCalibratedStereoRenderCheckBox.TabIndex = 36;
            this.useCalibratedStereoRenderCheckBox.Text = "use calibrated";
            this.useCalibratedStereoRenderCheckBox.UseVisualStyleBackColor = true;
            // 
            // syncDataSourcePathTextBox
            // 
            this.syncDataSourcePathTextBox.Location = new System.Drawing.Point(555, 576);
            this.syncDataSourcePathTextBox.Name = "syncDataSourcePathTextBox";
            this.syncDataSourcePathTextBox.Size = new System.Drawing.Size(212, 20);
            this.syncDataSourcePathTextBox.TabIndex = 37;
            // 
            // testSyncDataSourceStartButton
            // 
            this.testSyncDataSourceStartButton.Location = new System.Drawing.Point(806, 579);
            this.testSyncDataSourceStartButton.Name = "testSyncDataSourceStartButton";
            this.testSyncDataSourceStartButton.Size = new System.Drawing.Size(75, 23);
            this.testSyncDataSourceStartButton.TabIndex = 38;
            this.testSyncDataSourceStartButton.Text = "testSyncDataSource";
            this.testSyncDataSourceStartButton.UseVisualStyleBackColor = true;
            this.testSyncDataSourceStartButton.Click += new System.EventHandler(this.testSyncDataSourceStartButton_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1051, 677);
            this.Controls.Add(this.tabControl1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.MEMSRotationTabPage.ResumeLayout(false);
            this.MEMSRotationTabPage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.accMagnetFilterTrackBar)).EndInit();
            this.featureDetectionTabPage.ResumeLayout(false);
            this.featureDetectionTabPage.PerformLayout();
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.calibPictureBoxUndist)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.calibPictureBoxOriginal)).EndInit();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rightStereoCalibPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.leftStereoCalibPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rightStereoOriginalPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.leftStereoOriginalPictureBox)).EndInit();
            this.calibratedStereoCaptureTabPage.ResumeLayout(false);
            this.calibratedStereoCaptureTabPage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.stereoCapProgressTrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.calibStereoCapRightPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.calibStereoCapLeftPictureBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button deviceSwitchButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label fpsLabel;
        private System.Windows.Forms.Timer timer2;
        private System.Windows.Forms.Timer timer3;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Timer memsTestOutputTimer;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage featureDetectionTabPage;
        private System.Windows.Forms.TabPage MEMSRotationTabPage;
        private System.Windows.Forms.RichTextBox logMEMSRichTextBox;
        private System.Windows.Forms.Timer stereoCapTimer;
        private System.Windows.Forms.CheckBox gyroCheckBox;
        private System.Windows.Forms.CheckBox accMagnetCheckBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TrackBar accMagnetFilterTrackBar;
        private System.Windows.Forms.CheckBox adoptiveFilterCheckBox;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Button monoCameraCalibrateButton;
        private System.Windows.Forms.Button PrevCalibButton;
        private System.Windows.Forms.Button nextCalibButton;
        private System.Windows.Forms.PictureBox calibPictureBoxUndist;
        private System.Windows.Forms.PictureBox calibPictureBoxOriginal;
        private System.Windows.Forms.Label imageIdxLabel;
        private System.Windows.Forms.Label calibrationStatusLabel;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.PictureBox rightStereoOriginalPictureBox;
        private System.Windows.Forms.PictureBox leftStereoOriginalPictureBox;
        private System.Windows.Forms.PictureBox rightStereoCalibPictureBox;
        private System.Windows.Forms.PictureBox leftStereoCalibPictureBox;
        private System.Windows.Forms.Label stereoCalibrationStatusLabel;
        private System.Windows.Forms.Button stereoCalibrateButton;
        private System.Windows.Forms.Button stereoCalibPrevButton;
        private System.Windows.Forms.Button stereoCalibNextButton;
        private System.Windows.Forms.Label stereoImageNumLabel;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.CheckBox stereoCalibDrawLinesCheckBox;
        private System.Windows.Forms.CheckBox stereoCalibUseRectificationCheckBox;
        private System.Windows.Forms.TabPage calibratedStereoCaptureTabPage;
        private System.Windows.Forms.TrackBar stereoCapProgressTrackBar;
        private System.Windows.Forms.Button pauseStereoCapButton;
        private System.Windows.Forms.Button startStereoCapButton;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.PictureBox calibStereoCapRightPictureBox;
        private System.Windows.Forms.PictureBox calibStereoCapLeftPictureBox;
        private System.Windows.Forms.TextBox rightCaptureTextBox;
        private System.Windows.Forms.TextBox leftCaptureTextBox;
        private System.Windows.Forms.RadioButton fileCapRadioButton;
        private System.Windows.Forms.RadioButton camCapRadioButton;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox stereoFileNameTextBox;
        private System.Windows.Forms.Button changeRightCamCapButton;
        private System.Windows.Forms.Button changeLeftCamCapButton;
        private System.Windows.Forms.Timer stereoStreamRenderTimer;
        private System.Windows.Forms.Button resumeStereoCapButton;
        private System.Windows.Forms.Button grabFrameForCalibrationButton;
        private System.Windows.Forms.Label calibListCountLabel;
        private System.Windows.Forms.Button clearCalibrationListButton;
        private System.Windows.Forms.Button calibrateFromGrabbedListButton;
        private System.Windows.Forms.TextBox stereoCalibFolderTextBox;
        private System.Windows.Forms.Button saveGrabbedCalibrationListToButton;
        private System.Windows.Forms.FolderBrowserDialog stereoCalibListSaveFolderBrowserDialog;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox prefixCalibListTextBox;
        private System.Windows.Forms.CheckBox useMapTransformCheckBox;
        private System.Windows.Forms.Button mapTransformApplyButton;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox mapScaleTextBox;
        private System.Windows.Forms.TextBox mapYShiftTextBox;
        private System.Windows.Forms.TextBox mapXShiftTextBox;
        private System.Windows.Forms.Button showMEMSFormButton;
        private System.Windows.Forms.CheckBox useCalibratedStereoRenderCheckBox;
        private System.Windows.Forms.Button testSyncDataSourceStartButton;
        private System.Windows.Forms.TextBox syncDataSourcePathTextBox;
    }
}

