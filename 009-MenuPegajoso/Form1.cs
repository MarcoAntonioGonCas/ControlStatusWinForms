using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _009_MenuPegajoso
{
    public partial class Form1 : Form
    {
        BorderControlStatus _buttonsActive;
        

        public Form1()
        {
            InitializeComponent();

            _buttonsActive = new BorderControlStatus(pnlConteBtn, 5,true,typeof(Button));
        }
         
    
        private void button1_Click_1(object sender, EventArgs e)
        {
            _buttonsActive.LugarBorde = DirecccionBorde.Arriba;
        }

        private void button2_Click(object sender, EventArgs e)
        {

            _buttonsActive.LugarBorde = DirecccionBorde.Abajo;
        }

        private void button3_Click(object sender, EventArgs e)
        {

            _buttonsActive.LugarBorde = DirecccionBorde.Derecha;
        }

        private void button4_Click(object sender, EventArgs e)
        {

            _buttonsActive.LugarBorde = DirecccionBorde.Izquierda;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            ¿
        }
    }
}
