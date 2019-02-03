namespace FacebookPageMessegingApp
{
    partial class AutoMessageForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AutoMessageForm));
            this.autoMessageIdLabel = new System.Windows.Forms.Label();
            this.AdminCheckBox = new System.Windows.Forms.CheckBox();
            this.UserCheckBox = new System.Windows.Forms.CheckBox();
            this.imagePanel = new System.Windows.Forms.FlowLayoutPanel();
            this.targetMessageTextBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.saveButton = new System.Windows.Forms.Button();
            this.addImgBtn = new System.Windows.Forms.Button();
            this.keywordListBox = new System.Windows.Forms.ListBox();
            this.addKeywordButton = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.keywordTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.autoMessageTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.targetMsgListBox = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // autoMessageIdLabel
            // 
            this.autoMessageIdLabel.AutoSize = true;
            this.autoMessageIdLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.autoMessageIdLabel.Location = new System.Drawing.Point(451, 425);
            this.autoMessageIdLabel.Name = "autoMessageIdLabel";
            this.autoMessageIdLabel.Size = new System.Drawing.Size(0, 20);
            this.autoMessageIdLabel.TabIndex = 32;
            this.autoMessageIdLabel.Visible = false;
            // 
            // AdminCheckBox
            // 
            this.AdminCheckBox.AutoSize = true;
            this.AdminCheckBox.Location = new System.Drawing.Point(313, 421);
            this.AdminCheckBox.Name = "AdminCheckBox";
            this.AdminCheckBox.Size = new System.Drawing.Size(55, 17);
            this.AdminCheckBox.TabIndex = 31;
            this.AdminCheckBox.Text = "Admin";
            this.AdminCheckBox.UseVisualStyleBackColor = true;
            // 
            // UserCheckBox
            // 
            this.UserCheckBox.AutoSize = true;
            this.UserCheckBox.Location = new System.Drawing.Point(259, 421);
            this.UserCheckBox.Name = "UserCheckBox";
            this.UserCheckBox.Size = new System.Drawing.Size(48, 17);
            this.UserCheckBox.TabIndex = 30;
            this.UserCheckBox.Text = "User";
            this.UserCheckBox.UseVisualStyleBackColor = true;
            // 
            // imagePanel
            // 
            this.imagePanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.imagePanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.imagePanel.Location = new System.Drawing.Point(451, 36);
            this.imagePanel.Name = "imagePanel";
            this.imagePanel.Size = new System.Drawing.Size(342, 362);
            this.imagePanel.TabIndex = 29;
            // 
            // targetMessageTextBox
            // 
            this.targetMessageTextBox.Location = new System.Drawing.Point(255, 36);
            this.targetMessageTextBox.Name = "targetMessageTextBox";
            this.targetMessageTextBox.Size = new System.Drawing.Size(190, 20);
            this.targetMessageTextBox.TabIndex = 28;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(255, 13);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(38, 20);
            this.label4.TabIndex = 27;
            this.label4.Text = "Title";
            // 
            // saveButton
            // 
            this.saveButton.BackColor = System.Drawing.Color.SteelBlue;
            this.saveButton.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.saveButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.saveButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.saveButton.ForeColor = System.Drawing.Color.White;
            this.saveButton.Location = new System.Drawing.Point(679, 415);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(114, 27);
            this.saveButton.TabIndex = 26;
            this.saveButton.Text = "Save";
            this.saveButton.UseVisualStyleBackColor = false;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // addImgBtn
            // 
            this.addImgBtn.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.addImgBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.addImgBtn.Location = new System.Drawing.Point(451, 6);
            this.addImgBtn.Name = "addImgBtn";
            this.addImgBtn.Size = new System.Drawing.Size(114, 27);
            this.addImgBtn.TabIndex = 25;
            this.addImgBtn.Text = "Add Image";
            this.addImgBtn.UseVisualStyleBackColor = true;
            this.addImgBtn.Click += new System.EventHandler(this.addImgBtn_Click);
            // 
            // keywordListBox
            // 
            this.keywordListBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.keywordListBox.FormattingEnabled = true;
            this.keywordListBox.ItemHeight = 16;
            this.keywordListBox.Location = new System.Drawing.Point(255, 218);
            this.keywordListBox.Name = "keywordListBox";
            this.keywordListBox.Size = new System.Drawing.Size(190, 180);
            this.keywordListBox.TabIndex = 24;
            this.keywordListBox.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.keywordListBox_MouseDoubleClick);
            // 
            // addKeywordButton
            // 
            this.addKeywordButton.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.addKeywordButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.addKeywordButton.Location = new System.Drawing.Point(407, 182);
            this.addKeywordButton.Name = "addKeywordButton";
            this.addKeywordButton.Size = new System.Drawing.Size(38, 27);
            this.addKeywordButton.TabIndex = 23;
            this.addKeywordButton.Text = "Add";
            this.addKeywordButton.UseVisualStyleBackColor = true;
            this.addKeywordButton.Click += new System.EventHandler(this.addKeywordButton_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(255, 159);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(106, 20);
            this.label3.TabIndex = 22;
            this.label3.Text = "Add Keyword:";
            // 
            // keywordTextBox
            // 
            this.keywordTextBox.Location = new System.Drawing.Point(255, 186);
            this.keywordTextBox.Name = "keywordTextBox";
            this.keywordTextBox.Size = new System.Drawing.Size(145, 20);
            this.keywordTextBox.TabIndex = 21;
            this.keywordTextBox.MouseClick += new System.Windows.Forms.MouseEventHandler(this.keywordTextBox_MouseClick);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(255, 59);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(149, 20);
            this.label2.TabIndex = 20;
            this.label2.Text = "Set Auto Message :";
            // 
            // autoMessageTextBox
            // 
            this.autoMessageTextBox.Location = new System.Drawing.Point(255, 82);
            this.autoMessageTextBox.Multiline = true;
            this.autoMessageTextBox.Name = "autoMessageTextBox";
            this.autoMessageTextBox.Size = new System.Drawing.Size(190, 74);
            this.autoMessageTextBox.TabIndex = 19;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(8, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 20);
            this.label1.TabIndex = 18;
            this.label1.Text = "Title List:";
            // 
            // targetMsgListBox
            // 
            this.targetMsgListBox.BackColor = System.Drawing.SystemColors.Control;
            this.targetMsgListBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.targetMsgListBox.FormattingEnabled = true;
            this.targetMsgListBox.ItemHeight = 16;
            this.targetMsgListBox.Location = new System.Drawing.Point(14, 36);
            this.targetMsgListBox.Name = "targetMsgListBox";
            this.targetMsgListBox.Size = new System.Drawing.Size(235, 404);
            this.targetMsgListBox.TabIndex = 17;
            this.targetMsgListBox.SelectedIndexChanged += new System.EventHandler(this.targetMsgListBox_SelectedIndexChanged);
            // 
            // AutoMessageForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.autoMessageIdLabel);
            this.Controls.Add(this.AdminCheckBox);
            this.Controls.Add(this.UserCheckBox);
            this.Controls.Add(this.imagePanel);
            this.Controls.Add(this.targetMessageTextBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.addImgBtn);
            this.Controls.Add(this.keywordListBox);
            this.Controls.Add(this.addKeywordButton);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.keywordTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.autoMessageTextBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.targetMsgListBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "AutoMessageForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Auto Message Form";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label autoMessageIdLabel;
        private System.Windows.Forms.CheckBox AdminCheckBox;
        private System.Windows.Forms.CheckBox UserCheckBox;
        private System.Windows.Forms.FlowLayoutPanel imagePanel;
        private System.Windows.Forms.TextBox targetMessageTextBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Button addImgBtn;
        private System.Windows.Forms.ListBox keywordListBox;
        private System.Windows.Forms.Button addKeywordButton;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox keywordTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox autoMessageTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox targetMsgListBox;
    }
}