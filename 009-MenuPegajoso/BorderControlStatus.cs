using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _009_MenuPegajoso
{
    public enum DirecccionBorde
    {
        Abajo,
        Arriba,
        Izquierda,
        Derecha
    }
    public class BorderControlStatus
    {
        public BorderControlStatus( Control container, int tamaBorde, Type typo = null)
        {
            Control.ControlCollection controlsChild = container.Controls;
            List<Control> lstControls = new List<Control>();

            foreach (Control control in controlsChild)
            {
                if (typo == null)
                {
                    if(control is Button)
                    {
                        lstControls.Add(control);
                    }
                }
                else
                {
                    if (control.GetType().Name == typo.Name)
                    {
                       lstControls.Add(control);

                    }
                }
            }

            TamaBorde = tamaBorde;

            this._pnlPonters = new List<Panel>();
            this._pnlUnique = CreaPanel();
            this._controls = lstControls.ToArray();
            this.Multiple = false;
            this._lugarBorde = DirecccionBorde.Abajo;
        }
       
        public BorderControlStatus(Control container, int tamaBorder, bool multiple, Type typo=null) : this(container, tamaBorder,typo)
        {
            this.Multiple = multiple;
        }
       
        private void EliminaChildControls()
        {
            Array.ForEach(_controls, control =>
            {
                control.Controls.Clear();
            });
        }
        private void EliminaHandles()
        {
            Array.ForEach(_controls, button =>
            {
                
                button.Click -= new EventHandler(BordeUnique_Click);
            });
            Array.ForEach(_controls, button =>
            {
                button.Click -= new EventHandler(BordeMultiple_Click);
            });
            
        }
        
        
        private DockStyle DirecADock(DirecccionBorde dire)
        {
            DockStyle dock = DockStyle.Bottom;
            switch (_lugarBorde)
            {
                case DirecccionBorde.Abajo:
                    dock = DockStyle.Bottom;
                    
                    break;
                case DirecccionBorde.Arriba:
                    dock = DockStyle.Top;
                    
                    break;
                case DirecccionBorde.Derecha:
                    dock = DockStyle.Right;
                    
                    break;
                case DirecccionBorde.Izquierda:
                    dock = DockStyle.Left;
                    
                    break;



            }
            return dock;
        }
        private void ActualizaPanel(Panel pnl)
        {
            if (pnl == null)
            {
                throw new NullReferenceException("El panel enviado es nulo");
            }
            pnl.Dock = DirecADock(_lugarBorde);
            if (_lugarBorde == DirecccionBorde.Abajo ||
                _lugarBorde == DirecccionBorde.Arriba)
            {
                pnl.Height = TamaBorde;
                pnl.Width = 0;
            }
            else
            {
                pnl.Height = 0;
                pnl.Width = TamaBorde;
            }
        }
        private Panel CreaPanel()
        {
            Panel pnl = new Panel();

            pnl.Dock = DirecADock(_lugarBorde);

            if (_lugarBorde == DirecccionBorde.Abajo||
                _lugarBorde == DirecccionBorde.Arriba)
            {
                pnl.Height = TamaBorde;
            }
            else
            {
                pnl.Width = TamaBorde;
            }

            
            return pnl;
        }
       

        private void ActivaBordeUnique()
        {
            Array.ForEach(_controls, button =>
            {
                button.Click += new EventHandler(BordeUnique_Click);
            });

        }
        private void BordeUnique_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            _pnlUnique.BackColor = btn.ForeColor;
            btn.Controls.Add(_pnlUnique);
        }


        private void ActivaBordeMultiple()
        {
            Array.ForEach(_controls, button =>
            {
                button.Click += new EventHandler(BordeMultiple_Click);
            });
        }
        private void BordeMultiple_Click(Object sender, EventArgs e)
        {

            Control control = (Control)sender;
            if (control.Controls.Count > 0)
            {
                control.Controls.Clear();
                return;
            }

            Panel pnl = CreaPanel();

            pnl.BackColor = control.ForeColor;
            control.Controls.Add(pnl);
            _pnlPonters.Add(pnl);
        }
        public void Update()
        {
            if (_controls == null)
            {
               
                return;
            }
            EliminaChildControls();
            EliminaHandles();
            this._pnlPonters.Clear();

            if (_mutiple)
            {
                ActivaBordeMultiple();
            }
            else
            {
                ActivaBordeUnique();
            }
        }


        //Variables
        private Panel _pnlUnique;
        private List<Panel> _pnlPonters;
        private Control[] _controls;
        private bool _mutiple;
        private DirecccionBorde _lugarBorde;


        //Propiedades
        public DirecccionBorde LugarBorde
        {
            get => _lugarBorde;
            set
            {
                _lugarBorde = value;
                ActualizaPanel(_pnlUnique);
                _pnlPonters.ForEach(pnl => ActualizaPanel(pnl));
            }
        }
        public bool Multiple
        {
            get => _mutiple;
            set
            {
                _mutiple = value;
                Update();
            }
        }
        public int TamaBorde { get; set; }
    }
}
