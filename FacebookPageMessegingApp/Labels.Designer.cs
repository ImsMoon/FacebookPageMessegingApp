
namespace FacebookPageMessegingApp
{
    partial class LabelManageForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LabelManageForm));
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.labelTableBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.database1DataSet = new Database1DataSet();
            this.btnCloseLabelBox = new System.Windows.Forms.Button();
            this.labelTableTableAdapter = new FacebookPageMessegingApp.DB.Database1DataSetTableAdapters.LabelTableTableAdapter();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.labelTableBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.database1DataSet)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(0, 1);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(491, 321);
            this.dataGridView1.TabIndex = 0;
            // 
            // labelTableBindingSource
            // 
            this.labelTableBindingSource.DataMember = "LabelTable";
            this.labelTableBindingSource.DataSource = this.database1DataSet;
            // 
            // database1DataSet
            // 
            this.database1DataSet.DataSetName = "Database1DataSet";
            this.database1DataSet.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // btnCloseLabelBox
            // 
            this.btnCloseLabelBox.Location = new System.Drawing.Point(386, 328);
            this.btnCloseLabelBox.Name = "btnCloseLabelBox";
            this.btnCloseLabelBox.Size = new System.Drawing.Size(75, 37);
            this.btnCloseLabelBox.TabIndex = 1;
            this.btnCloseLabelBox.Text = "OK";
            this.btnCloseLabelBox.UseVisualStyleBackColor = true;
            this.btnCloseLabelBox.Click += new System.EventHandler(this.btnCloseLabelBox_Click);
            // 
            // labelTableTableAdapter
            // 
            this.labelTableTableAdapter.ClearBeforeFill = true;
            // 
            // LabelManageForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(492, 385);
            this.Controls.Add(this.btnCloseLabelBox);
            this.Controls.Add(this.dataGridView1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LabelManageForm";
            this.Text = "Add & Manage Labels";
            this.Load += new System.EventHandler(this.LabelManageForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.labelTableBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.database1DataSet)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button btnCloseLabelBox;
        private Database1DataSet database1DataSet;
        private System.Windows.Forms.BindingSource labelTableBindingSource;
        private DB.Database1DataSetTableAdapters.LabelTableTableAdapter labelTableTableAdapter;
    }
}