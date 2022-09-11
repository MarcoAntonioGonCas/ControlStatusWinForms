using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ControlStatus
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


        public BorderControlStatusClick(Control container, int tamaBorde, Type typo = null)
        {
            Control.ControlCollection controlsChild = container.Controls;

            List<Control> lstControls = new List<Control>();

            foreach (Control control in controlsChild)
            {
                if (typo == null)
                {
                    if (control is Button)
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

            InicializaColoresIni(controlsChild);
            this._pnlPointers = new List<Panel>();
            this._ctrlActive = new List<Control>();

            this._pnlUnique = CreaPanel();
            this._controls = lstControls.ToArray();
            this.Multiple = false;
            this._lugarBorde = DirecccionBorde.Abajo;


            this._bordeAutomatico = true;
            this._tipoColorAutomatico = TipoColorAutomatico.ColorTexto;
            this._colorBorde = Color.Black;

            this._tipoBorde = DashStyle.Solid;

        }

        public BorderControlStatusClick(Control container, int tamaBorder, bool multiple, Type typo = null) : this(container, tamaBorder, typo)
        {
            this.Multiple = multiple;

        }
        public BorderControlStatusClick(Control container, int tamaBorder, bool multiple, bool colorAuto, DashStyle estiloBorde, Type typo = null) : this(container, tamaBorder, typo)
        {


            this._tipoBorde = estiloBorde;
            this._bordeAutomatico = colorAuto;
            this.Multiple = multiple;
        }

        private void InicializaColoresIni(Control.ControlCollection controls)
        {
            List<Color> colores = new List<Color>();

            foreach(Control item in controls)
            {
                colores.Add(item.BackColor);
            }

            this._coloresIni = colores.ToArray();

        }
        private void LimpiaBackGroundColors()
        {
            if(this._controls.Length == this._coloresIni.Length)
            {
                for(int i = 0; i < _controls.Length; i++)
                {
                    _controls[i].BackColor = _coloresIni[i];
                }
            }
        }
        private void EliminaHandles()
        {
            Array.ForEach(_controls, control =>
            {
                control.Click -= BordeUnique_Click;
                control.Click -= BordeMultiple_Click;

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

            if (_lugarBorde == DirecccionBorde.Abajo ||
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

        private GraphicsPath ObtienePahtLineaCentral(Rectangle rec, bool horizontal)
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
            gPath.AddLine(iniLinea, finLinea);
            gPath.CloseFigure();

            return gPath;
        }

        private void paintBackControl_Paint(object sender, PaintEventArgs args)
        {
            args.Graphics.Clear(ColorBackActive);

        }
        private void paintPanel_Paint(object sender, PaintEventArgs args)
        {
            //MessageBox.Show("pintando");
            Panel pnl = (Panel)sender;
            Graphics grap = args.Graphics;


            if (pnl.Parent.BackColor != Color.Transparent)
                grap.Clear(pnl.Parent.BackColor);


            GraphicsPath path;

            if (_lugarBorde == DirecccionBorde.Abajo ||
                _lugarBorde == DirecccionBorde.Arriba)
                path = ObtienePahtLineaCentral(pnl.ClientRectangle, true);
            else
                path = ObtienePahtLineaCentral(pnl.ClientRectangle, false);


            Color color;
            if (_bordeAutomatico)
            {
                if (_tipoColorAutomatico == TipoColorAutomatico.ColorTexto)

                    color = pnl.Parent.ForeColor;
                else
                    color = pnl.Parent.BackColor;
            }
            else
            {
                color = _colorBorde;
            }

            int menor = (pnl.Width < pnl.Height) ?
                pnl.Width :
                pnl.Height;

            using (Pen pen = new Pen(color, TipoBorde != DashStyle.Solid ? TamaBorde / 2 :
                menor
               ))
            {
                pen.DashStyle = _tipoBorde;
                grap.DrawPath(pen, path);
            }



        }
        private void ActivaBordeUnico()
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
            if(BackActive)
                BackColorActiveUnico(control);


        }
        private void BackColorActiveUnico(Control control)
        {

            this.LimpiaBackGroundColors();
            control.BackColor = this.ColorBackActive;
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

            if(BackActive)
                BackColorActiveMultiple(control);

            if (control.Controls.Count > 0)
            {
                foreach (var item in control.Controls)
                {
                    if (item is Panel)
                    {
                        ((Panel)item).Paint -= paintPanel_Paint;
                        _pnlPointers.Remove((Panel)item);
                        control.Controls.Remove((Panel)item);
                        break;
                    }
                }
                return;
            }

            Panel pnl = CreaPanel();
            control.Controls.Add(pnl);
            _pnlPointers.Add(pnl);
        }
        private void BackColorActiveMultiple(Control ctr)
        {
            Control controlExist = this._ctrlActive.FirstOrDefault(c => c == ctr);
            

            if (controlExist == null)
            {

                ctr.BackColor = ColorBackActive;
                _ctrlActive.Add(ctr);
            }
            else
            {

                for(int i = 0; i < _controls.Length; i++)
                {
                    if ( _controls[i] == controlExist )
                    {
                        _controls[i].BackColor = _coloresIni[i];
                        break;
                    }
                }
                _ctrlActive.Remove(controlExist);
                ctr.BackColor = Color.Transparent;

            }
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

            LimpiaBackGroundColors();
            this._pnlUnique = CreaPanel();

            if (_mutiple)
            {
                ActivaBordeMultiple();
            }
            else
            {
                ActivaBordeUnico();
            }
        }


        #region Propiedades


        private Panel _pnlUnique; //panel unico sirve para el modo no multiple
        private List<Panel> _pnlPointers; //punteros a paneles para el modo multiple
        private List<Control> _ctrlActive; //controlesActivos para el fondo multiple
        private Color[] _coloresIni; //colores iniciales de los controles
        private Control[] _controls; // controles a los cuales se les pondra el borde
        private int _tamaBorde; //tamaño del borde

        private DirecccionBorde _lugarBorde;//Lugar del borde Arriba,Abajo,Izquierda,derecha

        private bool _mutiple; // selecciona multiple
        private bool _bordeAutomatico; //Borde multple
        private bool _backActive; //color de fondo al seleccionar activao

        private Color _colorBackActive;
        private Color _colorBorde;

        private DashStyle _tipoBorde;
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
        public bool BordeAutomatico
        {
            get => _bordeAutomatico;
            set
            {
                _bordeAutomatico = value;

            }
        }

        public Color ColorBorde
        {
            get => _colorBorde;
            set
            {
                _bordeAutomatico = false;
                _colorBorde = value;

            }
        }


        public DashStyle TipoBorde
        {
            get => _tipoBorde;
            set
            {
                _tipoBorde = value;

            }
        }

        public bool BackActive
        {
            get
            {
                return _backActive;
            }

            set
            {
                _backActive = value;
            }
        }

        public Color ColorBackActive
        {
            get
            {
                return _colorBackActive;
            }

            set
            {
                _colorBackActive = value;
            }
        }

        #endregion
    }
}
