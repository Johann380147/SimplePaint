using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Commands
{
    public abstract class Command
    {
        private Color color = Color.Black;
        private Color color2 = Color.Black;
        private Color color3 = Color.Black;
        private Color foreColor = Color.White;
        private Color backColor = Color.Black;
        private int width = 1;
        private int whichColor = 1;
        private HatchStyle style = HatchStyle.Horizontal;

        #region Brushes

        private Image sprayImage1 = Commands.Properties.Resources.brushstroke1;
        private Image sprayImage2 = Commands.Properties.Resources.brushstroke2;
        private SolidBrush brush = new SolidBrush(Color.Black);
        private SolidBrush brushFill = new SolidBrush(Color.Black);
        private SolidBrush transBrush = new SolidBrush(Color.Black);
        private LinearGradientBrush gradientBrush;
        private Pen generalPen = new Pen(Color.Black);
        private Pen transPen = new Pen(Color.Black);
        private Pen hatchPen = new Pen(Color.Black);

        #endregion

        #region Properties
        public Color Color
        {
            get { return color; }
            set { color = value; }
        }
        public Color Color2
        {
            get { return color2; }
            set { color2 = value; }
        }
        public Color Color3
        {
            get { return color3; }
            set { color3 = value; }
        }
        public Color ForeColor
        {
            get { return foreColor; }
            set { foreColor = value; }
        }
        public Color BackColor
        {
            get { return backColor; }
            set { backColor = value; }
        }
        public HatchStyle Style
        {
            get { return style; }
            set { style = value; }
        }

        public int Width
        {
            get { return width; }
            set { width = value; }
        }
        public int WhichColor
        {
            get { return whichColor; }
            set { whichColor = value; }
        }

        public Pen GeneralPen
        {
            get { return generalPen; }
            set { generalPen = value; }
            
        }
        public Pen TransPen
        {
            get { return transPen; }
            set { transPen = value; }
        }
        public Pen HatchPen
        {
            get { return hatchPen; }
            set { hatchPen = value; }
        }
        public SolidBrush Brush
        {
            get { return brush; }
            set { brush = value; }
        }
        public SolidBrush BrushFill
        {
            get { return brushFill; }
            set { brushFill = value; }
        }
        public SolidBrush TransBrush
        {
            get { return transBrush; }
            set { transBrush = value; }
        }
        public LinearGradientBrush GradientBrush
        {
            get { return gradientBrush; }
            set { gradientBrush = value; }
        }
        public Image SprayImage1
        {
            get { return sprayImage1; }
            set { sprayImage1 = value; }
        }
        public Image SprayImage2
        {
            get { return sprayImage2; }
            set { sprayImage2 = value; }
        }

        #endregion

        public Command()
        {
            brush.Color = color;
            brushFill.Color = color2;
            if (whichColor == 1)
            {
                generalPen.Color = color;
                transBrush.Color = Color.FromArgb(128, color);
            }
            else if (whichColor == 2)
            {
                generalPen.Color = color2;
                transBrush.Color = Color.FromArgb(128, color2);
            }
            else
            {
                generalPen.Color = color3;
                transBrush.Color = Color.FromArgb(128, color3);
            }

            transPen.Width = generalPen.Width = hatchPen.Width = width;
            transPen.Brush = transBrush;
            hatchPen.Brush = new HatchBrush(style, foreColor, backColor);
        }

        public virtual void OnMouseDown(object sender, MouseEventArgs e) { }

        public virtual void OnMouseUp(Graphics g, MouseEventArgs e) { }

        public virtual void OnMouseMove(Graphics g, MouseEventArgs e, Point startPoint) { }

        public virtual void Paint(object sender, PaintEventArgs e) { }
    }
}
