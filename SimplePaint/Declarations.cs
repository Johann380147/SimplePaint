using System.Drawing;
using System.Drawing.Drawing2D;
using ExternalForms;
using System.Windows.Forms;
using System.Collections.Generic;
using Commands;

namespace SimplePaint
{
    partial class FormPaint
    {
        #region Size, etc

        private Rectangle cropRectangle = new Rectangle();
        private Rectangle pictureDimensions = new Rectangle(3, 3, 1080, 520);
        private Size mouseDisplacement;
        public Cursor cursor { get; private set; }

        #endregion

        #region Members

        private ControlMoverOrResizer c;
        private UndoRedo undoRedo = new UndoRedo();
        
        #endregion

        #region Points

        private bool isLeftDown = false;
        private bool isRightDown = false;
        private Point startPoint = Point.Empty;
        private Point endPoint = Point.Empty;
        private PointF[] pointPolygon;

        #endregion

        #region Style

        private AddText text;
        private string insertText;

        private HatchStyle style = HatchStyle.Horizontal;
        private Color color = Color.Black;
        private Color colorInner = Color.White;
        private Color foreHatchColor = Color.White;
        private Color backHatchColor = Color.Black;
        private Color textColor;
        private Font font;
        private byte whichColor = 1;

        #endregion

        #region Enums
        private enum Tools { stationery, shapes, pointer, colorpicker, text, none };
        private enum Stationery { eraser, pencil, fill, brush, brush1, brush2, custom, transBrush };
        private enum Shapes
        {
            line, rectangle, circle, triangle, polygon, diamond, hexagon, rightTriangle,
            arrowUp, arrowDown, arrowLeft, arrowRight
        };
        private enum ShapeFillMode { none, solid, gradient };

        private Tools tools;
        private Stationery stationery;
        private Shapes shapes;
        private ShapeFillMode shapeFillMode = ShapeFillMode.none;

        #endregion
        
        #region Bitmaps

        public Bitmap mainBmp { get; private set; }
        private Bitmap moveBitMap;
        private Bitmap moveBitMapZoom;
        private Bitmap tempBitMap;

        #endregion

        #region Brushes

        private Image sprayImage1 = SimplePaint.Properties.Resources.brushstroke1;
        private Image sprayImage2 = SimplePaint.Properties.Resources.brushstroke2;
        private SolidBrush brush = new SolidBrush(Color.Black);
        private SolidBrush brushFill = new SolidBrush(Color.Black);
        private SolidBrush transBrush = new SolidBrush(Color.Black);
        private HatchBrush selectBrush = new HatchBrush(HatchStyle.DashedVertical, Color.WhiteSmoke, Color.FromArgb(33, 33, 33));
        private LinearGradientBrush gradientBrush;
        private Pen generalPen = new Pen(Color.Black);
        private Pen transPen = new Pen(Color.Black);
        private Pen hatchPen = new Pen(Color.Black);
        private GraphicsPath graphicsPath;
        private GraphicsPath graphicsZoom;
        private int size = 1;

        #endregion

        #region Components

        private ToolTip tooltip = new ToolTip();
        private Timer t = new Timer();
        private Control[] control;
        private Control[] controlList = new Control[7];
        private int count;

        #endregion

        #region Menu variables
        
        private string path = "";
        private bool saved = true;
        private bool savedBefore = false;
        private float zoomFactor = 1;
        private bool isZoomed = false;
        
        #endregion


        List<Command> command = new List<Command>();
        CommandDrawGeneral drawGeneral = new CommandDrawGeneral();
    }
}
