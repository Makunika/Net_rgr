using Net_rgr.Modules;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Net_rgr
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
            progressBarDownload.Value = 0;
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            // TODO: данная строка кода позволяет загрузить данные в таблицу "rgrNetDataSet.Animal". При необходимости она может быть перемещена или удалена.
            this.animalTableAdapter.Fill(this.rgrNetDataSet.Animal);

        }

        private void сохранитьВФайлToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataRowView drw = (DataRowView)animalBindingSource.Current;
            rgrNetDataSet.AnimalRow ur = (rgrNetDataSet.AnimalRow)(drw.Row);
            SaveFileDialog sfd = new SaveFileDialog();
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                if (pictureBox1.Image != null)
                {
                    FileStream fs = new FileStream(sfd.FileName, FileMode.OpenOrCreate);
                    pictureBox1.Image.Save(fs, System.Drawing.Imaging.ImageFormat.Jpeg);
                    fs.Close();
                }
            }
        }

        private void загрузитьИзФайлаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataRowView drw = (DataRowView)animalBindingSource.Current;
            rgrNetDataSet.AnimalRow ur = (rgrNetDataSet.AnimalRow)(drw.Row);
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image = Image.FromFile(ofd.FileName);
            }
        }

        private void LoadData_Click(object sender, EventArgs e)
        {

        }

        private async void загрузитьВТаблицуДанныеИзВнешнегоИсточникаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormCount formCount = new FormCount();
            var progress = new Progress<int>(i => progressBarDownload.Value = i);
            if (formCount.ShowDialog() == DialogResult.OK)
            {
                object obj = this.filmsDataGridView.DataSource;
                this.filmsDataGridView.DataSource = null;
                //using (var con = new SqlConnection(Properties.Settings.Default.rgrNetConnectionString))
                //using (var cmd = new SqlCommand())
                //{
                //    cmd.CommandText = "DELETE FROM Animal";
                //    cmd.Connection = con;
                //    con.Open();
                //    int numberDeleted = cmd.ExecuteNonQuery();  // all rows deleted
                //}
                await Task.Run(() =>
                {
                    AnimalsUpdater animalsUpdater = new AnimalsUpdater(rgrNetDataSet.Animal, progress, formCount.CountAnimal);
                    animalsUpdater.DoWork();
                });
                this.animalBindingSource.EndEdit();
                this.filmsDataGridView.DataSource = obj;
                this.animalBindingSource.EndEdit();
                this.animalTableAdapter.Update(this.rgrNetDataSet.Animal);
                this.animalTableAdapter.Fill(this.rgrNetDataSet.Animal);
                progressBarDownload.Value = 0;
            }
        }

        private void загрухитьТаблицуФильмыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.animalTableAdapter.Fill(this.rgrNetDataSet.Animal);
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            this.animalBindingSource.EndEdit();
            this.animalTableAdapter.Update(this.rgrNetDataSet.Animal);
        }
    }
}
