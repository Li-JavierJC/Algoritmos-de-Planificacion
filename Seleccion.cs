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
    public partial class Seleccion : Form
    {
        List<int> listaOpciones = new List<int>();

        public delegate void pasar(List<int> listaOpciones);
        public event pasar pasado;

        public Seleccion()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            for(int i=0; i < checkedListBox1.CheckedIndices.Count; i++)
            {
                listaOpciones.Add(checkedListBox1.CheckedIndices[i]);
            }

            pasado(listaOpciones);
            this.Hide();
        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }
    }
}
