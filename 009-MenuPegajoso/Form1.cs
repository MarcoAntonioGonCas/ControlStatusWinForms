using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
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

            _buttonsActive = new BorderControlStatus(pnlConteBtn, 3,true,typeof(Button));


            _buttonsActive.EstiloBorde = DashStyle.Solid;
        }
         
    
        private void button1_Click_1(object sender, EventArgs e)
        {
            _buttonsActive.TamaBorde = 10;
        }

        private void button2_Click(object sender, EventArgs e)
        {

            
        }

        private void button3_Click(object sender, EventArgs e)
        {

           
        }

        private void button4_Click(object sender, EventArgs e)
        {

           
        }

        private void button5_Click(object sender, EventArgs e)
        {

            _buttonsActive.Multiple = false;
        }
    }
}
