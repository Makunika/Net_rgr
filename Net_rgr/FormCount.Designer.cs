
namespace Net_rgr
{
    partial class FormCount
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
            this.countAnimals = new System.Windows.Forms.NumericUpDown();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.countAnimals)).BeginInit();
            this.SuspendLayout();
            // 
            // countAnimals
            // 
            this.countAnimals.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.countAnimals.Location = new System.Drawing.Point(84, 37);
            this.countAnimals.Maximum = new decimal(new int[] {
            37000,
            0,
            0,
            0});
            this.countAnimals.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.countAnimals.Name = "countAnimals";
            this.countAnimals.Size = new System.Drawing.Size(120, 20);
            this.countAnimals.TabIndex = 0;
            this.countAnimals.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.countAnimals.ValueChanged += new System.EventHandler(this.countAnimals_ValueChanged);
            // 
            // button2
            // 
            this.button2.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button2.Location = new System.Drawing.Point(12, 84);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(98, 35);
            this.button2.TabIndex = 2;
            this.button2.Text = "Принять";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button1.Location = new System.Drawing.Point(180, 86);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(98, 35);
            this.button1.TabIndex = 3;
            this.button1.Text = "Отмена";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(81, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(119, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Количество животных";
            // 
            // FormCount
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(302, 133);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.countAnimals);
            this.Name = "FormCount";
            this.Text = "FormCount";
            ((System.ComponentModel.ISupportInitialize)(this.countAnimals)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NumericUpDown countAnimals;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
    }
}