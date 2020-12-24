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
            nameTextBox.Validating += NameTextBox_Validating;
            aboutTextBox.Validating += AboutTextBox_Validating;
        }

        private void AboutTextBox_Validating(object sender, CancelEventArgs e)
        {
            if (String.IsNullOrEmpty(nameTextBox.Text))
            {
                errorProviderName.SetError(nameTextBox, "Не указано название!");
            }
            else
            {
                errorProviderName.Clear();
            }
        }

        private void NameTextBox_Validating(object sender, CancelEventArgs e)
        {
            if (String.IsNullOrEmpty(aboutTextBox.Text))
            {
                errorProviderAbout.SetError(aboutTextBox, "Не указано описание!");
            }
            else
            {
                errorProviderAbout.Clear();
            }
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

        private async void загрузитьВТаблицуДанныеИзВнешнегоИсточникаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Вызываем форму с выбором количества животных
            FormCount formCount = new FormCount();
            var progress = new Progress<int>(i => progressBarDownload.Value = i);
            if (formCount.ShowDialog() == DialogResult.OK)
            {
                toolStripStatusLabelDownload.Text = "Загрузка...";
                //Для облегчения работы сбрасываем на время update DataSource из DataGrip
                object obj = this.animalsDataGridView.DataSource;
                this.animalsDataGridView.DataSource = null;
                //Вызывем updater в асинхронном варианте
                await Task.Run(() =>
                {
                    AnimalsUpdater animalsUpdater = new AnimalsUpdater(rgrNetDataSet.Animal, progress, formCount.CountAnimal);
                    animalsUpdater.DoWork();
                });
                //Возвращает DataSource
                this.animalBindingSource.EndEdit();
                this.animalsDataGridView.DataSource = obj;
                this.animalBindingSource.EndEdit();
                //Загружаем из таблицы данные в базу данных
                this.animalTableAdapter.Update(this.rgrNetDataSet.Animal);
                //Заполняем из базы данных DataGrip
                this.animalTableAdapter.Fill(this.rgrNetDataSet.Animal);
                progressBarDownload.Value = 0;
                toolStripStatusLabelDownload.Text = "Загрузка завершена!";
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
            if (this.ValidateChildren())
            {
                try
                {
                    this.animalBindingSource.EndEdit();
                    this.animalTableAdapter.Update(this.rgrNetDataSet.Animal);
                }
                catch (SqlException)
                {
                    toolStripStatusLabelDownload.Text = "Такое животное уже есть в базе данных!";
                }

            }
        }
    }
}
