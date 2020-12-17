using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Net_rgr
{
    public partial class FormCount : Form
    {

        public int CountAnimal { get; set; }

        public FormCount()
        {
            InitializeComponent();
            CountAnimal = (int)countAnimals.Value;
        }

        private void countAnimals_ValueChanged(object sender, EventArgs e)
        {
            CountAnimal = (int) countAnimals.Value;
        }
    }
}
