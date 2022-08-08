﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _009_MenuPegajoso
{
    public enum TipoColorAutomatico
    {
        ColorTexto,
        ColorDeFondo
    }
    public enum DirecccionBorde
    {
        Abajo,
        Arriba,
        Izquierda,
        Derecha
    }
    public class BorderControlStatusClick
    {
        

        public BorderControlStatusClick( Control container, int tamaBorde, Type typo = null)
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

            _tamaBorde = tamaBorde;

            this._pnlPointers = new List<Panel>();
            this._pnlUnique = CreaPanel();
            this._controls = lstControls.ToArray();
            this.Multiple = false;
            this._lugarBorde = DirecccionBorde.Abajo;


            this._colorAuto = true;
            this._tipoColorAutomatico = TipoColorAutomatico.ColorTexto;
            this._colorManual = Color.Black;
            
            this._estiloBorde = DashStyle.Solid;

        }
       
        public BorderControlStatusClick(Control container, int tamaBorder, bool multiple, Type typo=null) : this(container, tamaBorder,typo)
        {
            this.Multiple = multiple;
            
        }
        public BorderControlStatusClick(Control container, int tamaBorder, bool multiple,bool colorAuto,DashStyle estiloBorde, Type typo=null) : this(container, tamaBorder,typo)
        {

            
            this._estiloBorde = estiloBorde;
            this._colorAuto = colorAuto;
            
            
            this.Multiple = multiple;
        }
       
        private void EliminaHandles()
        {
            Array.ForEach(_controls, button =>
            {
                button.Click -= BordeUnique_Click;
            });
            Array.ForEach(_controls, button =>
            {
                button.Click -= BordeMultiple_Click;
            });
            
        }
        private void EliminaChildControls()
        {
            Array.ForEach(_controls, control =>
            {
                control.Controls.Clear();
            });
        }
        
        
        private DockStyle DirecADock(DirecccionBorde dire)
        {
            DockStyle dock = DockStyle.Bottom;
            switch (dire)
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
               throw new NullReferenceException("El panel enviado es nulo");
            
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

            pnl.Paint += paintPanel_Paint;
                        
            return pnl;
        }

        private GraphicsPath ObtienePahtLineaCentral(Rectangle rec,bool horizontal)
        {
            GraphicsPath gPath = new GraphicsPath();

            Point iniLinea = new Point();
            Point finLinea = new Point();
            
            if (horizontal)
            {
                iniLinea.X = rec.X;
                iniLinea.Y = rec.Bottom / 2;
                
                finLinea.X = rec.Right;
                finLinea.Y = rec.Bottom / 2;

            }
            else
            {
                iniLinea.X = rec.Right / 2;
                iniLinea.Y = rec.Y;

                finLinea.X = rec.Right / 2;
                iniLinea.Y = rec.Bottom;
            }
            gPath.StartFigure();
            gPath.AddLine(iniLinea,finLinea);
            gPath.CloseFigure();

            return gPath;
        }
        private void paintPanel_Paint(object sender, PaintEventArgs args)
        {
            //MessageBox.Show("pintando");
            Panel pnl = (Panel)sender;  
            Graphics grap=args.Graphics;

            if(pnl.Parent.BackColor != Color.Transparent)
                grap.Clear(pnl.Parent.BackColor);
            
            
            GraphicsPath path;

            if (_lugarBorde == DirecccionBorde.Abajo ||
                _lugarBorde == DirecccionBorde.Arriba)
                path = ObtienePahtLineaCentral(pnl.ClientRectangle, true);
            else
                path = ObtienePahtLineaCentral(pnl.ClientRectangle, false);
            
             
            Color color;
            if (_colorAuto)
            {
                if (_tipoColorAutomatico == TipoColorAutomatico.ColorTexto)
                    
                    color = pnl.Parent.ForeColor;
                else
                    color = pnl.Parent.BackColor;
            }
            else 
                color = _colorManual;
            
            using (Pen pen = new Pen(color,pnl.Width<pnl.Height?pnl.Width:pnl.Height))
            {
                pen.DashStyle = _estiloBorde;
                grap.DrawPath(pen,path);
            }
                
        }
        private void ActivaBordeUnique()
        {
            Array.ForEach(_controls, button =>
            {
                button.Click += BordeUnique_Click;
            });

        }
        private void BordeUnique_Click(object sender, EventArgs e)
        {
            if (Multiple) return;
            
            Control control = (Control)sender;            
            control.Controls.Add(_pnlUnique);
        }


        private void ActivaBordeMultiple()
        {
            Array.ForEach(_controls, button =>
            {
                button.Click += BordeMultiple_Click;
            });
        }
        private void BordeMultiple_Click(Object sender, EventArgs e)
        {
            if (!Multiple) return;

            Control control = (Control)sender;
            if (control.Controls.Count > 0)
            {
                control.Controls.Clear();
                return;
            }

            Panel pnl = CreaPanel();
            control.Controls.Add(pnl);
            _pnlPointers.Add(pnl);
        }
        public void Update()
        {
            if (Block) return;
            if (_controls == null)
            {
                return;
            }
            EliminaHandles();
            EliminaChildControls();
            this._pnlPointers.ForEach(pointer =>
            {
                pointer.Paint -= paintPanel_Paint;
            });
            this._pnlUnique.Paint -= paintPanel_Paint;
            this._pnlPointers.Clear();
            
            this._pnlUnique = CreaPanel();

            if (_mutiple)
            {
                ActivaBordeMultiple();
            }
            else
            {
                ActivaBordeUnique();
            }
        }


        #region Propiedades

 
        private Panel _pnlUnique;
        private List<Panel> _pnlPointers;
        private Control[] _controls;
        private int _tamaBorde;
        private bool _mutiple;
        private DirecccionBorde _lugarBorde;
        private bool _colorAuto;
        private Color _colorManual;
        private DashStyle _estiloBorde;
        private TipoColorAutomatico _tipoColorAutomatico;
 
        public TipoColorAutomatico TipoColorAutomatico
        {
            get => _tipoColorAutomatico;
            set
            {
                _tipoColorAutomatico = value;
            }
        }

        public DirecccionBorde LugarBorde
        {
            get => _lugarBorde;
            set
            {
                _lugarBorde = value;
                ActualizaPanel(_pnlUnique);
                _pnlPointers.ForEach(pnl => ActualizaPanel(pnl));
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

        public bool Block = false;
        public int TamaBorde
        {
            get => _tamaBorde;
            set
            {
                _tamaBorde = value;
                ActualizaPanel(_pnlUnique);
                _pnlPointers.ForEach(panel => ActualizaPanel(panel));
            }
        }
        public bool ColorAuto
        {
            get => _colorAuto;
            set
            {
                _colorAuto = value; 
               
            }
        }

        public Color ColorManual
        {
            get => _colorManual;
            set
            {
                _colorManual = value;
                
            }
        }
        

        public DashStyle EstiloBorde
        {
            get => _estiloBorde;
            set
            {
                _estiloBorde = value;

            }
        }

        #endregion
    }
}
