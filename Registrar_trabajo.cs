using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Planificacion
{
    public partial class Registrar_trabajo : Form
    {
        public delegate void pasar(int tiempo, int prioridad);
        public event pasar pasado;


        public Registrar_trabajo(int numProceso)
        {
            InitializeComponent();
            this.label2.Text = numProceso.ToString();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            pasado(Int32.Parse(textBox1.Text), Int32.Parse(textBox2.Text));
            this.Hide();
        }
    }
}
