namespace Task_Manager
{
    partial class Form1
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
            this.dgTasks = new System.Windows.Forms.DataGridView();
            this.cbPriority = new System.Windows.Forms.ComboBox();
            this.bEdit = new System.Windows.Forms.Button();
            this.bAdd = new System.Windows.Forms.Button();
            this.bDelete = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgTasks)).BeginInit();
            this.SuspendLayout();
            // 
            // dgTasks
            // 
            this.dgTasks.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgTasks.Location = new System.Drawing.Point(54, 46);
            this.dgTasks.Name = "dgTasks";
            this.dgTasks.RowHeadersWidth = 51;
            this.dgTasks.RowTemplate.Height = 24;
            this.dgTasks.Size = new System.Drawing.Size(278, 150);
            this.dgTasks.TabIndex = 0;
            this.dgTasks.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgTasks_CellContentClick);
            // 
            // cbPriority
            // 
            this.cbPriority.FormattingEnabled = true;
            this.cbPriority.Location = new System.Drawing.Point(385, 58);
            this.cbPriority.Name = "cbPriority";
            this.cbPriority.Size = new System.Drawing.Size(121, 24);
            this.cbPriority.TabIndex = 1;
            // 
            // bEdit
            // 
            this.bEdit.Location = new System.Drawing.Point(404, 101);
            this.bEdit.Name = "bEdit";
            this.bEdit.Size = new System.Drawing.Size(83, 25);
            this.bEdit.TabIndex = 2;
            this.bEdit.Text = "Edit";
            this.bEdit.UseVisualStyleBackColor = true;
            this.bEdit.Click += new System.EventHandler(this.bEdit_Click);
            // 
            // bAdd
            // 
            this.bAdd.Location = new System.Drawing.Point(404, 144);
            this.bAdd.Name = "bAdd";
            this.bAdd.Size = new System.Drawing.Size(83, 23);
            this.bAdd.TabIndex = 3;
            this.bAdd.Text = "Add";
            this.bAdd.UseVisualStyleBackColor = true;
            // 
            // bDelete
            // 
            this.bDelete.Location = new System.Drawing.Point(404, 185);
            this.bDelete.Name = "bDelete";
            this.bDelete.Size = new System.Drawing.Size(83, 23);
            this.bDelete.TabIndex = 4;
            this.bDelete.Text = "Delete";
            this.bDelete.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(704, 329);
            this.Controls.Add(this.bDelete);
            this.Controls.Add(this.bAdd);
            this.Controls.Add(this.bEdit);
            this.Controls.Add(this.cbPriority);
            this.Controls.Add(this.dgTasks);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgTasks)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgTasks;
        private System.Windows.Forms.ComboBox cbPriority;
        private System.Windows.Forms.Button bEdit;
        private System.Windows.Forms.Button bAdd;
        private System.Windows.Forms.Button bDelete;
    }
}

