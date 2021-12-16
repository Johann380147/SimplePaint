using Commands;
using ExternalForms;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;



namespace SimplePaint
{
    public partial class FormPaint : Form
    {
        #region Constructor

        public FormPaint()
        {
            InitializeComponent();
            StartUp startup = new StartUp(); startup.ShowDialog();
            
            mainBmp = new Bitmap(pbCanvas.Width, pbCanvas.Height);
            pbCanvas.Image = mainBmp;
            undoRedo.InsertUndoBitmapData(ImageHandler.StoreCurrentBitmap(mainBmp));
            
            comboFillShapes.SelectedIndex = 0;
            cursor = CursorResourceLoader.LoadEmbeddedCursor(Properties.Resources.pencil_cursor);
            pbCanvas.Cursor = cursor;
            c = new ControlMoverOrResizer(this); c.Init(pbCanvas);
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.panelBottom.Region = new System.Drawing.Region(new Rectangle(panelBottom.ClientRectangle.Location,
                new Size(panelBottom.ClientSize.Width - 20, panelBottom.ClientSize.Height)));
            menuStrip1.Renderer = new MyRenderer();

            tools = Tools.stationery; stationery = Stationery.pencil;
            generalPen.SetLineCap(LineCap.Round, LineCap.Round, DashCap.Round);
            transPen.SetLineCap(LineCap.Round, LineCap.Round, DashCap.Round);
            hatchPen.SetLineCap(LineCap.Round, LineCap.Round, DashCap.Round);

            toolStripPictureSize.Text = pbCanvas.Size.Width.ToString() + " x " + pbCanvas.Size.Height.ToString() + "px";

            #region Event Subs
            this.MouseWheel += (sender, e) => MainPanel.Focus();

            MainPanel.MouseWheel += (sender, e) => MainPanel.Invalidate();
            MainPanel.Scroll += (sender, e) => MainPanel.Invalidate();

            ZoomBar.MouseWheel += (s, e) =>
            {
                HandledMouseEventArgs ee = (HandledMouseEventArgs)e;
                ee.Handled = true;
                MainPanel.Focus();
            };

            comboSize.MouseWheel += (s, e) =>
            {
                HandledMouseEventArgs ee = (HandledMouseEventArgs)e;
                ee.Handled = true;
                MainPanel.Focus();
            };

            t.Tick += (s, a) =>
            {
                //MainPanel.AutoScrollPosition = new Point(pbCanvas.PointToClient(MousePosition).X - MainPanel.AutoScrollPosition.X,
                //    pbCanvas.PointToClient(MousePosition).Y - MainPanel.AutoScrollPosition.Y);
                MainPanel.AutoScrollPosition = new Point(pbCanvas.PointToClient(MousePosition).X,
                    pbCanvas.PointToClient(MousePosition).Y);
                
                t.Stop();
            };
            #endregion

            ToolTipInit();

            for (count = 1; count < 8; count++)
            {
                control = Controls.Find("btnCustom" + count.ToString(), true);
                controlList[count - 1] = control[0];
            }
            count = 0;
        }

        #endregion

        #region PictureBox Events
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {

            if(mainBmp == null)
            {
                mainBmp = new Bitmap(pbCanvas.Width, pbCanvas.Height);
                pbCanvas.Image = mainBmp;
                undoRedo.InsertUndoBitmapData(ImageHandler.StoreCurrentBitmap(mainBmp));
            }
            if (e.Button == MouseButtons.Left)
            {
                isLeftDown = true;
                saved = false;
                startPoint = e.Location;
                endPoint = e.Location;

                
                brush.Color = color;
                brushFill.Color = colorInner;
                if (whichColor == 1)
                {
                    generalPen.Color = color;
                    transBrush.Color = Color.FromArgb(128, color);
                }
                else if (whichColor == 2)
                {
                    generalPen.Color = colorInner;
                    transBrush.Color = Color.FromArgb(128, colorInner);
                }
                else
                {
                    generalPen.Color = btnColor3.BackColor;
                    transBrush.Color = Color.FromArgb(128, btnColor3.BackColor);
                }
                if (stationery == Stationery.eraser)
                {
                    generalPen.Color = Color.FromArgb(0, Color.White);
                }

                transPen.Width = generalPen.Width = hatchPen.Width = size;
                transPen.Brush = transBrush;
                hatchPen.Brush = new HatchBrush(style, foreHatchColor, backHatchColor);
                graphicsPath = new GraphicsPath();
                if (isZoomed)
                    graphicsZoom = new GraphicsPath();

            }
            if (e.Button == MouseButtons.Right)
            {
                if (cropRectangle.Size != new Size(0, 0))
                {
                    isRightDown = true;
                    mouseDisplacement.Width = e.Location.X - cropRectangle.X;
                    mouseDisplacement.Height = e.Location.Y - cropRectangle.Y;
                    moveBitMap = new Bitmap(pbCanvas.Image).Clone(cropRectangle, mainBmp.PixelFormat);

                    using (Graphics g = Graphics.FromImage(pbCanvas.Image))
                    {
                        g.FillRectangle(Brushes.White, cropRectangle);
                    }

                    if(isZoomed)
                    {
                        float reciprocal = 1 / zoomFactor;
                        Rectangle r = new Rectangle(new Point((int)(cropRectangle.X * reciprocal), (int)(cropRectangle.Y * reciprocal)),
                            new Size((int)(cropRectangle.Width * reciprocal), (int)(cropRectangle.Height * reciprocal)));

                        moveBitMapZoom = new Bitmap(mainBmp).Clone(r, mainBmp.PixelFormat);

                        using(Graphics d = Graphics.FromImage(mainBmp))
                        {
                            d.FillRectangle(Brushes.White, r);
                        }
                    }
                }
            }
            

        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (hatchPen.Brush != null)
            {
                hatchPen.Brush.Dispose();
            }
            t.Stop();

            #region Drawing Shapes & Stationery
            if (isLeftDown)
            {
                endPoint = e.Location;
                isLeftDown = false;

                if (tools == Tools.stationery)
                {
                    using (Graphics g = Graphics.FromImage(pbCanvas.Image))
                    {
                        g.SmoothingMode = SmoothingMode.HighQuality;

                        if (stationery == Stationery.eraser || stationery == Stationery.pencil || stationery == Stationery.brush)
                        {
                            if (stationery == Stationery.eraser)
                            {
                                g.CompositingMode = CompositingMode.SourceCopy;
                                g.DrawPath(generalPen, graphicsPath);
                            }
                            else
                            {
                                g.DrawPath(generalPen, graphicsPath);
                            }
                        }
                        else if (stationery == Stationery.transBrush)
                        {
                            g.DrawPath(transPen, graphicsPath);
                        }
                        else if (stationery == Stationery.custom)
                        {
                            g.DrawPath(hatchPen, graphicsPath);
                        }
                    }
                    if (isZoomed)
                    {
                        using (Graphics g = Graphics.FromImage(mainBmp))
                        {
                            g.SmoothingMode = SmoothingMode.HighQuality;

                            if (stationery == Stationery.eraser || stationery == Stationery.pencil || stationery == Stationery.brush)
                            {
                                if(stationery == Stationery.eraser)
                                {
                                    g.CompositingMode = CompositingMode.SourceCopy;
                                    g.DrawPath(generalPen, graphicsZoom);
                                }
                                else
                                {
                                    g.DrawPath(generalPen, graphicsZoom);
                                }
                            }
                            else if (stationery == Stationery.transBrush)
                            {
                                g.DrawPath(transPen, graphicsZoom);
                            }
                            else if (stationery == Stationery.custom)
                            {
                                g.DrawPath(hatchPen, graphicsZoom);
                            }
                        }
                        graphicsZoom.Dispose();
                    }
                    
                    graphicsPath.Dispose();
                }

                if (tools == Tools.shapes)
                {
                    using (Graphics g = Graphics.FromImage(pbCanvas.Image))
                    {
                        g.SmoothingMode = SmoothingMode.HighQuality;

                        #region none
                        if (shapeFillMode == ShapeFillMode.none)
                        {
                            if (shapes == Shapes.line)
                            {
                                g.DrawLine(new Pen(brush, size), startPoint, endPoint);

                            }
                            else if (shapes == Shapes.rectangle)
                            {
                                Rectangle r = new Rectangle(ImageHandler.RectAlgorithm(startPoint, endPoint).Item1,
                                      ImageHandler.RectAlgorithm(startPoint, endPoint).Item2);

                                g.DrawRectangle(new Pen(brush, size), r);
                                
                            }
                            else if (shapes == Shapes.circle)
                            {
                                g.DrawEllipse(new Pen(brush, size), startPoint.X, startPoint.Y,
                                    endPoint.X - startPoint.X, endPoint.Y - startPoint.Y);
                            }

                            else if (shapes == Shapes.polygon || shapes == Shapes.triangle ||
                                     shapes == Shapes.diamond || shapes == Shapes.hexagon ||
                                     shapes == Shapes.rightTriangle || shapes == Shapes.arrowUp ||
                                     shapes == Shapes.arrowDown || shapes == Shapes.arrowLeft ||
                                     shapes == Shapes.arrowRight)
                            {
                                g.DrawPolygon(new Pen(brush, size), pointPolygon);
                            }
                        }
                        #endregion

                        #region solid
                        else if (shapeFillMode == ShapeFillMode.solid)
                        {
                            if (shapes == Shapes.line)
                            {
                                g.DrawLine(new Pen(brush, size), startPoint, endPoint);

                            }
                            else if (shapes == Shapes.rectangle)
                            {
                                Rectangle r = new Rectangle(ImageHandler.RectAlgorithm(startPoint, endPoint).Item1,
                                         ImageHandler.RectAlgorithm(startPoint, endPoint).Item2);

                                g.FillRectangle(brushFill, r);
                                g.DrawRectangle(new Pen(brush, size), r);
                                
                            }
                            else if (shapes == Shapes.circle)
                            {
                                g.FillEllipse(brushFill, startPoint.X, startPoint.Y,
                                    endPoint.X - startPoint.X, endPoint.Y - startPoint.Y);
                                g.DrawEllipse(new Pen(brush, size), startPoint.X, startPoint.Y,
                                    endPoint.X - startPoint.X, endPoint.Y - startPoint.Y);
                            }

                            else if (shapes == Shapes.polygon || shapes == Shapes.triangle ||
                                     shapes == Shapes.diamond || shapes == Shapes.hexagon ||
                                     shapes == Shapes.rightTriangle || shapes == Shapes.arrowUp ||
                                     shapes == Shapes.arrowDown || shapes == Shapes.arrowLeft ||
                                     shapes == Shapes.arrowRight)
                            {
                                g.FillPolygon(brushFill, pointPolygon);
                                g.DrawPolygon(new Pen(brush, size), pointPolygon);
                            }
                        }
                        #endregion
                            
                        #region gradient
                        else if (shapeFillMode == ShapeFillMode.gradient)
                        {
                            try
                            {
                                if (shapes == Shapes.rectangle)
                                {
                                    Rectangle r = new Rectangle(ImageHandler.RectAlgorithm(startPoint, endPoint).Item1,
                                         ImageHandler.RectAlgorithm(startPoint, endPoint).Item2);

                                    if (startPoint.X < endPoint.X && startPoint.Y < endPoint.Y)
                                    {
                                        using (gradientBrush = new LinearGradientBrush(
                                        new Point(0, 0), new Point(endPoint.X, endPoint.Y),
                                        colorInner, btnColor3.BackColor))
                                        {
                                            g.FillRectangle(gradientBrush, r);
                                            g.DrawRectangle(new Pen(brush, size), r);
                                        }
                                    }
                                    else if (startPoint.X < endPoint.X && startPoint.Y > endPoint.Y)
                                    {
                                        using (gradientBrush = new LinearGradientBrush(
                                        new Point(0, 0), new Point(endPoint.X, startPoint.Y),
                                        colorInner, btnColor3.BackColor))
                                        {
                                            g.FillRectangle(gradientBrush, r);
                                            g.DrawRectangle(new Pen(brush, size), r);
                                        }
                                    }
                                    else if (endPoint.X < startPoint.X && endPoint.Y < startPoint.Y)
                                    {
                                        using (gradientBrush = new LinearGradientBrush(
                                        new Point(0, 0), new Point(startPoint.X, startPoint.Y),
                                        colorInner, btnColor3.BackColor))
                                        {
                                            g.FillRectangle(gradientBrush, r);
                                            g.DrawRectangle(new Pen(brush, size), r);
                                        }
                                    }
                                    else
                                    {
                                        using (gradientBrush = new LinearGradientBrush(
                                        new Point(0, 0), new Point(startPoint.X, endPoint.Y),
                                        colorInner, btnColor3.BackColor))
                                        {
                                            g.FillRectangle(gradientBrush, r);
                                            g.DrawRectangle(new Pen(brush, size), r);
                                        }
                                    }
                                }

                                else
                                {
                                    using (gradientBrush = new LinearGradientBrush(
                                        startPoint, endPoint,
                                        colorInner, btnColor3.BackColor))
                                    {
                                        if (shapes == Shapes.line)
                                        {
                                            g.DrawLine(new Pen(gradientBrush, size), startPoint, endPoint);

                                        }

                                        else if (shapes == Shapes.circle)
                                        {
                                            g.FillEllipse(gradientBrush, startPoint.X, startPoint.Y,
                                                endPoint.X - startPoint.X, endPoint.Y - startPoint.Y);
                                            g.DrawEllipse(new Pen(brush, size), startPoint.X, startPoint.Y,
                                                endPoint.X - startPoint.X, endPoint.Y - startPoint.Y);
                                        }

                                        else if (shapes == Shapes.polygon || shapes == Shapes.triangle ||
                                                 shapes == Shapes.diamond || shapes == Shapes.hexagon ||
                                                 shapes == Shapes.rightTriangle || shapes == Shapes.arrowUp ||
                                                 shapes == Shapes.arrowDown || shapes == Shapes.arrowLeft ||
                                                 shapes == Shapes.arrowRight)
                                        {
                                            g.FillPolygon(gradientBrush, pointPolygon);
                                            g.DrawPolygon(new Pen(brush, size), pointPolygon);
                                        }
                                    }
                                }
                            }
                            catch(OutOfMemoryException) { }
                        }
                        #endregion

                        
                    }
                    if(isZoomed)
                    {
                        startPoint.X *= (int)(1 / zoomFactor);
                        startPoint.Y *= (int)(1 / zoomFactor);
                        endPoint.X *= (int)(1 / zoomFactor);
                        endPoint.Y *= (int)(1 / zoomFactor);

                        for (int x = 0; x < pointPolygon.Length; x++)
                        {
                            pointPolygon[x].X *= (1 / zoomFactor);
                            pointPolygon[x].Y *= (1 / zoomFactor);
                        }

                        using (Graphics g = Graphics.FromImage(mainBmp))
                        {
                            g.SmoothingMode = SmoothingMode.HighQuality;

                            #region none
                            if (shapeFillMode == ShapeFillMode.none)
                            {
                                if (shapes == Shapes.line)
                                {
                                    g.DrawLine(new Pen(brush, size), startPoint, endPoint);

                                }
                                else if (shapes == Shapes.rectangle)
                                {
                                    Rectangle r = new Rectangle(ImageHandler.RectAlgorithm(startPoint, endPoint).Item1,
                                          ImageHandler.RectAlgorithm(startPoint, endPoint).Item2);

                                    g.DrawRectangle(new Pen(brush, size), r);

                                }
                                else if (shapes == Shapes.circle)
                                {
                                    g.DrawEllipse(new Pen(brush, size), startPoint.X, startPoint.Y,
                                        endPoint.X - startPoint.X, endPoint.Y - startPoint.Y);
                                }

                                else if (shapes == Shapes.polygon || shapes == Shapes.triangle ||
                                         shapes == Shapes.diamond || shapes == Shapes.hexagon ||
                                         shapes == Shapes.rightTriangle || shapes == Shapes.arrowUp ||
                                         shapes == Shapes.arrowDown || shapes == Shapes.arrowLeft ||
                                         shapes == Shapes.arrowRight)
                                {
                                    g.DrawPolygon(new Pen(brush, size), pointPolygon);
                                }
                            }
                            #endregion

                            #region solid
                            else if (shapeFillMode == ShapeFillMode.solid)
                            {
                                if (shapes == Shapes.line)
                                {
                                    g.DrawLine(new Pen(brush, size), startPoint, endPoint);

                                }
                                else if (shapes == Shapes.rectangle)
                                {
                                    Rectangle r = new Rectangle(ImageHandler.RectAlgorithm(startPoint, endPoint).Item1,
                                             ImageHandler.RectAlgorithm(startPoint, endPoint).Item2);

                                    g.FillRectangle(brushFill, r);
                                    g.DrawRectangle(new Pen(brush, size), r);

                                }
                                else if (shapes == Shapes.circle)
                                {
                                    g.FillEllipse(brushFill, startPoint.X, startPoint.Y,
                                        endPoint.X - startPoint.X, endPoint.Y - startPoint.Y);
                                    g.DrawEllipse(new Pen(brush, size), startPoint.X, startPoint.Y,
                                        endPoint.X - startPoint.X, endPoint.Y - startPoint.Y);
                                }

                                else if (shapes == Shapes.polygon || shapes == Shapes.triangle ||
                                         shapes == Shapes.diamond || shapes == Shapes.hexagon ||
                                         shapes == Shapes.rightTriangle || shapes == Shapes.arrowUp ||
                                         shapes == Shapes.arrowDown || shapes == Shapes.arrowLeft ||
                                         shapes == Shapes.arrowRight)
                                {
                                    g.FillPolygon(brushFill, pointPolygon);
                                    g.DrawPolygon(new Pen(brush, size), pointPolygon);
                                }
                            }
                            #endregion

                            #region gradient
                            else if (shapeFillMode == ShapeFillMode.gradient)
                            {
                                try
                                {
                                    if (shapes == Shapes.rectangle)
                                    {
                                        Rectangle r = new Rectangle(ImageHandler.RectAlgorithm(startPoint, endPoint).Item1,
                                             ImageHandler.RectAlgorithm(startPoint, endPoint).Item2);

                                        if (startPoint.X < endPoint.X && startPoint.Y < endPoint.Y)
                                        {
                                            using (gradientBrush = new LinearGradientBrush(
                                            new Point(0, 0), new Point(endPoint.X, endPoint.Y),
                                            colorInner, btnColor3.BackColor))
                                            {
                                                g.FillRectangle(gradientBrush, r);
                                                g.DrawRectangle(new Pen(brush, size), r);
                                            }
                                        }
                                        else if (startPoint.X < endPoint.X && startPoint.Y > endPoint.Y)
                                        {
                                            using (gradientBrush = new LinearGradientBrush(
                                            new Point(0, 0), new Point(endPoint.X, startPoint.Y),
                                            colorInner, btnColor3.BackColor))
                                            {
                                                g.FillRectangle(gradientBrush, r);
                                                g.DrawRectangle(new Pen(brush, size), r);
                                            }
                                        }
                                        else if (endPoint.X < startPoint.X && endPoint.Y < startPoint.Y)
                                        {
                                            using (gradientBrush = new LinearGradientBrush(
                                            new Point(0, 0), new Point(startPoint.X, startPoint.Y),
                                            colorInner, btnColor3.BackColor))
                                            {
                                                g.FillRectangle(gradientBrush, r);
                                                g.DrawRectangle(new Pen(brush, size), r);
                                            }
                                        }
                                        else
                                        {
                                            using (gradientBrush = new LinearGradientBrush(
                                            new Point(0, 0), new Point(startPoint.X, endPoint.Y),
                                            colorInner, btnColor3.BackColor))
                                            {
                                                g.FillRectangle(gradientBrush, r);
                                                g.DrawRectangle(new Pen(brush, size), r);
                                            }
                                        }
                                    }

                                    else
                                    {
                                        using (gradientBrush = new LinearGradientBrush(
                                            startPoint, endPoint,
                                            colorInner, btnColor3.BackColor))
                                        {
                                            if (shapes == Shapes.line)
                                            {
                                                g.DrawLine(new Pen(gradientBrush, size), startPoint, endPoint);

                                            }

                                            else if (shapes == Shapes.circle)
                                            {
                                                g.FillEllipse(gradientBrush, startPoint.X, startPoint.Y,
                                                    endPoint.X - startPoint.X, endPoint.Y - startPoint.Y);
                                                g.DrawEllipse(new Pen(brush, size), startPoint.X, startPoint.Y,
                                                    endPoint.X - startPoint.X, endPoint.Y - startPoint.Y);
                                            }

                                            else if (shapes == Shapes.polygon || shapes == Shapes.triangle ||
                                                     shapes == Shapes.diamond || shapes == Shapes.hexagon ||
                                                     shapes == Shapes.rightTriangle || shapes == Shapes.arrowUp ||
                                                     shapes == Shapes.arrowDown || shapes == Shapes.arrowLeft ||
                                                     shapes == Shapes.arrowRight)
                                            {
                                                g.FillPolygon(gradientBrush, pointPolygon);
                                                g.DrawPolygon(new Pen(brush, size), pointPolygon);
                                            }
                                        }
                                    }
                                }
                                catch (OutOfMemoryException) { }
                            }
                            #endregion


                        }
                    }
                    pbCanvas.Refresh();
                }
            }
            #endregion

            #region Tools
            if (tools == Tools.text)
            {
                using (Graphics g = Graphics.FromImage(pbCanvas.Image))
                {
                    g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                    g.DrawString(insertText, font, new SolidBrush(textColor), endPoint.X, endPoint.Y);
                }
                if(isZoomed)
                {
                    using (Graphics g = Graphics.FromImage(mainBmp))
                    {
                        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                        g.DrawString(insertText, font, new SolidBrush(textColor), endPoint.X * (1 / zoomFactor), endPoint.Y * (1 / zoomFactor));
                    }
                }
                pbCanvas.Refresh();
            }
            #endregion

            #region Moving area
            if (isRightDown)
            {
                isRightDown = false;
                using (Graphics g = Graphics.FromImage(pbCanvas.Image))
                {
                    g.DrawImage(moveBitMap, new Point(endPoint.X - mouseDisplacement.Width,
                      endPoint.Y - mouseDisplacement.Height));
                }
                if(isZoomed)
                {
                    using (Graphics g = Graphics.FromImage(mainBmp))
                    {
                        g.DrawImage(moveBitMapZoom, new PointF((endPoint.X - mouseDisplacement.Width) * (1 / zoomFactor),
                          (endPoint.Y - mouseDisplacement.Height) * (1 / zoomFactor)));
                    }
                    moveBitMapZoom.Dispose();
                }
                cropRectangle.Size = new Size(0, 0);
                moveBitMap.Dispose();
                pbCanvas.Refresh();
                undoRedo.InsertUndoBitmapData(ImageHandler.StoreCurrentBitmap(mainBmp));
            }
            #endregion

            #region Resizing picture box
            if (c._resizing == true)
            {
                if (pbCanvas.Image == null)
                {
                    mainBmp = new Bitmap(pbCanvas.Width, pbCanvas.Height);
                    pbCanvas.Image = mainBmp;
                }
                using (Bitmap temp = new Bitmap(pbCanvas.Image))
                {
                    if (pbCanvas.Image != null) 
                    {
                        pbCanvas.Image.Dispose(); 
                    }
                    
                    if(isZoomed)
                    {
                        pbCanvas.Image = new Bitmap(pbCanvas.Width, pbCanvas.Height);
                    }
                    else
                    {
                        mainBmp = new Bitmap(pbCanvas.Width, pbCanvas.Height);
                        pbCanvas.Image = mainBmp;
                    }
                    

                    using (Graphics g = Graphics.FromImage(pbCanvas.Image))
                    {
                        g.DrawImage(temp, new Point(0, 0));
                    }
                    
                    if(isZoomed)
                    {
                        using (Bitmap tmp = new Bitmap(mainBmp))
                        {
                            mainBmp.Dispose();
                            mainBmp = new Bitmap((int)(pbCanvas.Width * (1 / zoomFactor)), (int)(pbCanvas.Height * (1 / zoomFactor)));
                            using (Graphics g = Graphics.FromImage(mainBmp))
                            {
                                g.DrawImage(tmp, new Point(0, 0));
                            }
                        }
                    }
                    undoRedo.DeleteUndoRedoBitmapData();
                    undoRedo.InsertUndoBitmapData(ImageHandler.StoreCurrentBitmap(mainBmp));

                    pbCanvas.Refresh();
                }
                
            }
            #endregion


            if (tools != Tools.none && tools != Tools.pointer && !c._resizing)
            {
                undoRedo.InsertUndoBitmapData(ImageHandler.StoreCurrentBitmap(mainBmp));
            }
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            toolStripMousePoint.Text = e.Location.ToString() + "px";

            if (c._resizing)
            {
                if (e.Location.X - startPoint.X > 1 ||
                    e.Location.Y - startPoint.Y > 1 ||
                    startPoint.X - e.Location.X > 1 ||
                    startPoint.Y - e.Location.Y > 1)
                {
                    tooltip.Show(String.Format("{0}", pbCanvas.Size), pbCanvas, pbCanvas.PointToClient(Cursor.Position), 5000);
                    startPoint = e.Location;
                }
                t.Interval = 100;
                t.Start();

                return;
            }
            if (isLeftDown)
            {
                #region stationery

                if (tools == Tools.stationery)
                {
                    using (Graphics g = Graphics.FromImage(pbCanvas.Image))
                    {
                        g.SmoothingMode = SmoothingMode.HighQuality;


                        if (stationery == Stationery.pencil || stationery == Stationery.brush ||
                            stationery == Stationery.custom || stationery == Stationery.transBrush ||
                            stationery == Stationery.eraser)
                        {
                            graphicsPath.StartFigure();
                            graphicsPath.AddLine(startPoint, e.Location);
                        }
                        else if (stationery == Stationery.brush1)
                        {
                            g.DrawImage(sprayImage1, new Point(e.Location.X - 12, e.Location.Y - 12));
                        }
                        else if (stationery == Stationery.brush2)
                        {
                            g.DrawImage(sprayImage2, new Point(e.Location.X - 10, e.Location.Y - 10));
                        }

                        if (isZoomed)
                        {
                            using(Graphics d = Graphics.FromImage(mainBmp))
                            {
                                d.SmoothingMode = SmoothingMode.HighQuality;


                                if (stationery == Stationery.pencil || stationery == Stationery.brush ||
                                    stationery == Stationery.custom || stationery == Stationery.transBrush ||
                                    stationery == Stationery.eraser)
                                {
                                    graphicsZoom.StartFigure();
                                    graphicsZoom.AddLine(startPoint.X * (1 / zoomFactor), startPoint.Y * (1 / zoomFactor),
                                        e.Location.X * (1 / zoomFactor), e.Location.Y * (1 / zoomFactor));
                                }
                                else if (stationery == Stationery.brush1)
                                {
                                    d.DrawImage(sprayImage1, new PointF((float)((e.Location.X - 12) * (1 / zoomFactor)),
                                        (float)((e.Location.Y - 12)) * (1 / zoomFactor)));
                                }
                                else if (stationery == Stationery.brush2)
                                {
                                    d.DrawImage(sprayImage2, new PointF((float)((e.Location.X - 10) * (1 / zoomFactor)),
                                        (float)((e.Location.Y - 10)) * (1 / zoomFactor)));
                                }
                            }
                        }

                        startPoint = e.Location;
                    }
                }
                #endregion

                #region Shapes

                if (tools == Tools.shapes)
                {
                    endPoint = e.Location;
                }
                #endregion

                #region Crop

                if (tools == Tools.pointer)
                {
                    if (endPoint.X <= pbCanvas.Width && endPoint.Y <= pbCanvas.Height)
                        endPoint = e.Location; //cannot crop more than pictureBox's width/height
                    if (endPoint.X > pbCanvas.Width)
                    {
                        endPoint.X = pbCanvas.Width;
                    }
                    if (endPoint.Y > pbCanvas.Height)
                    {
                        endPoint.Y = pbCanvas.Height;
                    }
                    if (endPoint.X < (pbCanvas.Width - pbCanvas.Width))
                    {
                        endPoint.X = 0;
                    }
                    if (endPoint.Y < (pbCanvas.Height - pbCanvas.Height))
                    {
                        endPoint.Y = 0;
                    }
                }
                #endregion

                pbCanvas.Refresh();
            }
            if (isRightDown || tools == Tools.text)
            {
                endPoint = e.Location;
                pbCanvas.Refresh();
            }

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (c._resizing == true) return;
            if (isLeftDown)
            {
                if (tools == Tools.stationery)
                {
                    using (Graphics g = Graphics.FromImage(pbCanvas.Image))
                    {
                        if (stationery == Stationery.fill)
                        {
                            if (whichColor == 1)
                                g.Clear(color);
                            else if (whichColor == 2)
                                g.Clear(colorInner);
                            else
                                g.Clear(btnColor3.BackColor);
                        }
                        else if (stationery == Stationery.brush1)
                        {
                            g.DrawImage(sprayImage1, pbCanvas.PointToClient(new Point(Cursor.Position.X - 12, Cursor.Position.Y - 12)));
                        }
                        else if (stationery == Stationery.brush2)
                        {
                            g.DrawImage(sprayImage2, pbCanvas.PointToClient(new Point(Cursor.Position.X - 10, Cursor.Position.Y - 10)));
                        }
                    }
                    if (isZoomed)
                    {
                        using (Graphics d = Graphics.FromImage(mainBmp))
                        {
                            if (stationery == Stationery.fill)
                            {
                                if (whichColor == 1)
                                    d.Clear(color);
                                else if (whichColor == 2)
                                    d.Clear(colorInner);
                                else
                                    d.Clear(btnColor3.BackColor);
                            }
                            else if (stationery == Stationery.brush1)
                            {
                                Point tempPoint = new Point(pbCanvas.PointToClient(Cursor.Position).X, pbCanvas.PointToClient(Cursor.Position).Y);
                                
                                d.DrawImage(sprayImage1, new PointF((float)((tempPoint.X - 10) * (1 / zoomFactor)),
                                           (float)((tempPoint.Y - 10)) * (1 / zoomFactor)));

                            }
                            else if (stationery == Stationery.brush2)
                            {
                                Point tempPoint = new Point(pbCanvas.PointToClient(Cursor.Position).X, pbCanvas.PointToClient(Cursor.Position).Y);

                                d.DrawImage(sprayImage2, new PointF((float)((tempPoint.X - 10) * (1 / zoomFactor)),
                                           (float)((tempPoint.Y - 10)) * (1 / zoomFactor)));
                            }
                        }
                    }
                    pbCanvas.Invalidate();
                    startPoint = pbCanvas.PointToClient(Cursor.Position);
                }

                else if (tools == Tools.colorpicker)
                {
                    Bitmap picker = new Bitmap(pbCanvas.Width, pbCanvas.Height);
                    Rectangle r = pbCanvas.RectangleToScreen(pbCanvas.ClientRectangle);

                    using (Graphics g = Graphics.FromImage(picker))
                    {
                        g.CopyFromScreen(r.Location, Point.Empty, pbCanvas.Size);
                    }

                    if (whichColor == 1)
                    {
                        color = picker.GetPixel(startPoint.X, startPoint.Y);
                        btnColor.BackColor = color;
                    }
                    else if (whichColor == 2)
                    {
                        colorInner = picker.GetPixel(startPoint.X, startPoint.Y);
                        btnColor2.BackColor = colorInner;
                    }
                    else
                    {
                        btnColor3.BackColor = picker.GetPixel(startPoint.X, startPoint.Y);
                    }
                    picker.Dispose();
                }
            }

        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            saved = false;

            if (isLeftDown)
            {
                e.Graphics.SmoothingMode = SmoothingMode.HighQuality;

                #region Stationery
                if (tools == Tools.stationery)
                {
                    if (stationery == Stationery.eraser || stationery == Stationery.pencil || stationery == Stationery.brush)
                    {
                        if (stationery == Stationery.eraser)
                        {
                            using (Pen p = new Pen(Color.White, size))
                            {
                                p.SetLineCap(LineCap.Round, LineCap.Round, DashCap.Round);
                                e.Graphics.DrawPath(p, graphicsPath);
                            }
                            
                        }
                        else
                        {
                            e.Graphics.DrawPath(generalPen, graphicsPath);
                        }
                        
                    }
                    else if (stationery == Stationery.transBrush)
                    {
                        e.Graphics.DrawPath(transPen, graphicsPath);
                    }
                    else if (stationery == Stationery.custom)
                    {
                        e.Graphics.DrawPath(hatchPen, graphicsPath);
                    }
                }
                #endregion

                #region Shapes
                else if (tools == Tools.shapes)
                {
                    if (shapeFillMode == ShapeFillMode.none)
                    {
                        #region line
                        if (shapes == Shapes.line)
                        {
                            e.Graphics.DrawLine(new Pen(brush, size), startPoint, endPoint);
                        }
                        #endregion

                        #region rectangle
                        else if (shapes == Shapes.rectangle)
                        {
                            Rectangle r = new Rectangle(ImageHandler.RectAlgorithm(startPoint, endPoint).Item1,
                                ImageHandler.RectAlgorithm(startPoint,endPoint).Item2);

                            e.Graphics.DrawRectangle(new Pen(brush, size), r);

                        }
                        #endregion

                        #region circle
                        else if (shapes == Shapes.circle)
                        {
                            e.Graphics.DrawEllipse(new Pen(brush, size), startPoint.X, startPoint.Y,
                                endPoint.X - startPoint.X, endPoint.Y - startPoint.Y);
                        }
                        #endregion

                        #region pentagon
                        else if (shapes == Shapes.polygon)
                        {

                            PointF point2 = new PointF(startPoint.X + ((endPoint.X - startPoint.X) * 0.5f), startPoint.Y - 10);
                            PointF point1 = new PointF(startPoint.X, point2.Y + ((endPoint.Y - point2.Y) * 0.4f));
                            PointF point3 = new PointF(endPoint.X, point1.Y);
                            PointF point4 = new PointF(point2.X + ((endPoint.X - point2.X) * 0.6f), endPoint.Y);
                            PointF point5 = new PointF(startPoint.X + ((point2.X - startPoint.X) * 0.4f), endPoint.Y);

                            pointPolygon = new PointF[] { point1, point2, point3, point4, point5 };
                            e.Graphics.DrawPolygon(new Pen(brush, size), pointPolygon);
                        }
                        #endregion

                        #region triangle
                        else if (shapes == Shapes.triangle)
                        {
                            PointF point1 = new PointF(startPoint.X, pbCanvas.PointToClient(Cursor.Position).Y);
                            PointF point2 = new PointF(startPoint.X + ((endPoint.X - startPoint.X) * 0.5f), startPoint.Y - 10);
                            PointF point3 = new PointF(endPoint.X, endPoint.Y);

                            pointPolygon = new PointF[] { point1, point2, point3 };
                            e.Graphics.DrawPolygon(new Pen(brush, size), pointPolygon);
                        }
                        #endregion

                        #region diamond
                        else if (shapes == Shapes.diamond)
                        {
                            PointF point2 = new PointF(startPoint.X + ((endPoint.X - startPoint.X) * 0.5f), startPoint.Y - 10);
                            PointF point1 = new PointF(startPoint.X, point2.Y + ((endPoint.Y - point2.Y)) * 0.5f);
                            PointF point3 = new PointF(endPoint.X, point1.Y);
                            PointF point4 = new PointF(point2.X, endPoint.Y);

                            pointPolygon = new PointF[] { point1, point2, point3, point4 };
                            e.Graphics.DrawPolygon(new Pen(brush, size), pointPolygon);
                        }
                        #endregion

                        #region hexagon
                        else if (shapes == Shapes.hexagon)
                        {
                            PointF point2 = new PointF(startPoint.X + ((endPoint.X - startPoint.X) * 0.5f), startPoint.Y);
                            PointF point1 = new PointF(startPoint.X, point2.Y + ((endPoint.Y - point2.Y)) * 0.25f);
                            PointF point3 = new PointF(endPoint.X, point1.Y);
                            PointF point5 = new PointF(point2.X, endPoint.Y);
                            PointF point4 = new PointF(endPoint.X, point2.Y + ((endPoint.Y - point2.Y) * 0.75f));
                            PointF point6 = new PointF(point1.X, point4.Y);

                            pointPolygon = new PointF[] { point1, point2, point3, point4, point5, point6 };
                            e.Graphics.DrawPolygon(new Pen(brush, size), pointPolygon);
                        }
                        #endregion

                        #region right-angled Triangle
                        else if (shapes == Shapes.rightTriangle)
                        {
                            PointF point1 = new PointF(startPoint.X, startPoint.Y);
                            PointF point2 = new PointF(endPoint.X, endPoint.Y);
                            PointF point3 = new PointF(startPoint.X, endPoint.Y);

                            pointPolygon = new PointF[] { point1, point2, point3 };
                            e.Graphics.DrawPolygon(new Pen(brush, size), pointPolygon);
                        }
                        #endregion

                        #region arrow
                        if (shapes == Shapes.arrowUp)
                        {
                            PointF point1 = new PointF(startPoint.X, startPoint.Y + ((endPoint.Y - startPoint.Y) * 0.4f));
                            PointF point2 = new PointF(startPoint.X + ((endPoint.X - startPoint.X) * 0.5f), startPoint.Y);
                            PointF point3 = new PointF(endPoint.X, point1.Y);
                            PointF point4 = new PointF(point2.X + ((point3.X - point2.X)) * 0.5f, point3.Y);
                            PointF point5 = new PointF(point4.X, endPoint.Y);
                            PointF point7 = new PointF(point1.X + ((point2.X - point1.X) * 0.5f), point3.Y);
                            PointF point6 = new PointF(point7.X, point5.Y);

                            pointPolygon = new PointF[] { point1, point2, point3, point4, point5, point6, point7 };
                            e.Graphics.DrawPolygon(new Pen(brush, size), pointPolygon);
                        }

                        if (shapes == Shapes.arrowDown)
                        {
                            PointF point1 = new PointF(startPoint.X, startPoint.Y + ((endPoint.Y - startPoint.Y) * 0.6f));
                            PointF point6 = new PointF(endPoint.X, point1.Y);
                            PointF point7 = new PointF(point1.X + ((point6.X - point1.X) * 0.5f), endPoint.Y);
                            PointF point2 = new PointF(startPoint.X + ((point7.X - point1.X) * 0.5f), point1.Y);
                            PointF point5 = new PointF(point7.X + ((point6.X - point7.X) * 0.5f), point2.Y);
                            PointF point3 = new PointF(point2.X, startPoint.Y);
                            PointF point4 = new PointF(point5.X, point3.Y);

                            pointPolygon = new PointF[] { point1, point2, point3, point4, point5, point6, point7 };
                            e.Graphics.DrawPolygon(new Pen(brush, size), pointPolygon);
                        }

                        if (shapes == Shapes.arrowLeft)
                        {
                            PointF point2 = new PointF(startPoint.X + ((endPoint.X - startPoint.X) * 0.4f), startPoint.Y);
                            PointF point7 = new PointF(point2.X, endPoint.Y);
                            PointF point1 = new PointF(startPoint.X, point2.Y + ((point7.Y - point2.Y) * 0.5f));
                            PointF point4 = new PointF(endPoint.X, point2.Y + ((point1.Y - point2.Y) * 0.5f));
                            PointF point5 = new PointF(endPoint.X, point1.Y + ((point7.Y - point1.Y) * 0.5f));
                            PointF point3 = new PointF(point2.X, point4.Y);
                            PointF point6 = new PointF(point3.X, point5.Y);

                            pointPolygon = new PointF[] { point1, point2, point3, point4, point5, point6, point7 };
                            e.Graphics.DrawPolygon(new Pen(brush, size), pointPolygon);
                        }

                        if (shapes == Shapes.arrowRight)
                        {
                            PointF point3 = new PointF(startPoint.X + ((endPoint.X - startPoint.X) * 0.6f), startPoint.Y);
                            PointF point5 = new PointF(point3.X, endPoint.Y);
                            PointF point4 = new PointF(endPoint.X, startPoint.Y + ((endPoint.Y - startPoint.Y) * 0.5f));
                            PointF point1 = new PointF(startPoint.X, point3.Y + ((point4.Y - point3.Y) * 0.5f));
                            PointF point7 = new PointF(startPoint.X, point4.Y + ((point5.Y - point4.Y) * 0.5f));
                            PointF point2 = new PointF(point3.X, point1.Y);
                            PointF point6 = new PointF(point5.X, point7.Y);

                            pointPolygon = new PointF[] { point1, point2, point3, point4, point5, point6, point7 };
                            e.Graphics.DrawPolygon(new Pen(brush, size), pointPolygon);
                        }

                        #endregion
                    }
                    else if (shapeFillMode == ShapeFillMode.solid)
                    {
                        #region line
                        if (shapes == Shapes.line)
                        {
                            e.Graphics.DrawLine(new Pen(brush, size), startPoint, endPoint);
                        }
                        #endregion

                        #region rectangle
                        else if (shapes == Shapes.rectangle)
                        {
                            Rectangle r = new Rectangle(ImageHandler.RectAlgorithm(startPoint, endPoint).Item1,
                                   ImageHandler.RectAlgorithm(startPoint, endPoint).Item2);

                            e.Graphics.FillRectangle(brushFill, r);
                            e.Graphics.DrawRectangle(new Pen(brush, size), r);

                        }
                        #endregion

                        #region circle
                        else if (shapes == Shapes.circle)
                        {
                            e.Graphics.FillEllipse(brushFill, startPoint.X, startPoint.Y,
                                endPoint.X - startPoint.X, endPoint.Y - startPoint.Y);
                            e.Graphics.DrawEllipse(new Pen(brush, size), startPoint.X, startPoint.Y,
                                endPoint.X - startPoint.X, endPoint.Y - startPoint.Y);
                        }
                        #endregion

                        #region pentagon
                        else if (shapes == Shapes.polygon)
                        {

                            PointF point2 = new PointF(startPoint.X + ((endPoint.X - startPoint.X) * 0.5f), startPoint.Y - 10);
                            PointF point1 = new PointF(startPoint.X, point2.Y + ((endPoint.Y - point2.Y) * 0.4f));
                            PointF point3 = new PointF(endPoint.X, point1.Y);
                            PointF point4 = new PointF(point2.X + ((endPoint.X - point2.X) * 0.6f), endPoint.Y);
                            PointF point5 = new PointF(startPoint.X + ((point2.X - startPoint.X) * 0.4f), endPoint.Y);

                            pointPolygon = new PointF[] { point1, point2, point3, point4, point5 };
                            e.Graphics.FillPolygon(brushFill, pointPolygon);
                            e.Graphics.DrawPolygon(new Pen(brush, size), pointPolygon);
                        }
                        #endregion

                        #region triangle
                        else if (shapes == Shapes.triangle)
                        {
                            PointF point1 = new PointF(startPoint.X, pbCanvas.PointToClient(Cursor.Position).Y);
                            PointF point2 = new PointF(startPoint.X + ((endPoint.X - startPoint.X) * 0.5f), startPoint.Y - 10);
                            PointF point3 = new PointF(endPoint.X, endPoint.Y);

                            pointPolygon = new PointF[] { point1, point2, point3 };
                            e.Graphics.FillPolygon(brushFill, pointPolygon);
                            e.Graphics.DrawPolygon(new Pen(brush, size), pointPolygon);
                        }
                        #endregion

                        #region diamond
                        else if (shapes == Shapes.diamond)
                        {
                            PointF point2 = new PointF(startPoint.X + ((endPoint.X - startPoint.X) * 0.5f), startPoint.Y - 10);
                            PointF point1 = new PointF(startPoint.X, point2.Y + ((endPoint.Y - point2.Y)) * 0.5f);
                            PointF point3 = new PointF(endPoint.X, point1.Y);
                            PointF point4 = new PointF(point2.X, endPoint.Y);

                            pointPolygon = new PointF[] { point1, point2, point3, point4 };
                            e.Graphics.FillPolygon(brushFill, pointPolygon);
                            e.Graphics.DrawPolygon(new Pen(brush, size), pointPolygon);
                        }
                        #endregion

                        #region hexagon
                        else if (shapes == Shapes.hexagon)
                        {
                            PointF point2 = new PointF(startPoint.X + ((endPoint.X - startPoint.X) * 0.5f), startPoint.Y);
                            PointF point1 = new PointF(startPoint.X, point2.Y + ((endPoint.Y - point2.Y)) * 0.25f);
                            PointF point3 = new PointF(endPoint.X, point1.Y);
                            PointF point5 = new PointF(point2.X, endPoint.Y);
                            PointF point4 = new PointF(endPoint.X, point2.Y + ((endPoint.Y - point2.Y) * 0.75f));
                            PointF point6 = new PointF(point1.X, point4.Y);

                            pointPolygon = new PointF[] { point1, point2, point3, point4, point5, point6 };
                            e.Graphics.FillPolygon(brushFill, pointPolygon);
                            e.Graphics.DrawPolygon(new Pen(brush, size), pointPolygon);
                        }
                        #endregion

                        #region right-angled Triangle
                        else if (shapes == Shapes.rightTriangle)
                        {
                            PointF point1 = new PointF(startPoint.X, startPoint.Y);
                            PointF point2 = new PointF(endPoint.X, endPoint.Y);
                            PointF point3 = new PointF(startPoint.X, endPoint.Y);

                            pointPolygon = new PointF[] { point1, point2, point3 };
                            e.Graphics.FillPolygon(brushFill, pointPolygon);
                            e.Graphics.DrawPolygon(new Pen(brush, size), pointPolygon);
                        }
                        #endregion

                        #region arrow
                        if (shapes == Shapes.arrowUp)
                        {
                            PointF point1 = new PointF(startPoint.X, startPoint.Y + ((endPoint.Y - startPoint.Y) * 0.4f));
                            PointF point2 = new PointF(startPoint.X + ((endPoint.X - startPoint.X) * 0.5f), startPoint.Y);
                            PointF point3 = new PointF(endPoint.X, point1.Y);
                            PointF point4 = new PointF(point2.X + ((point3.X - point2.X)) * 0.5f, point3.Y);
                            PointF point5 = new PointF(point4.X, endPoint.Y);
                            PointF point7 = new PointF(point1.X + ((point2.X - point1.X) * 0.5f), point3.Y);
                            PointF point6 = new PointF(point7.X, point5.Y);

                            pointPolygon = new PointF[] { point1, point2, point3, point4, point5, point6, point7 };
                            e.Graphics.FillPolygon(brushFill, pointPolygon);
                            e.Graphics.DrawPolygon(new Pen(brush, size), pointPolygon);
                        }

                        if (shapes == Shapes.arrowDown)
                        {
                            PointF point1 = new PointF(startPoint.X, startPoint.Y + ((endPoint.Y - startPoint.Y) * 0.6f));
                            PointF point6 = new PointF(endPoint.X, point1.Y);
                            PointF point7 = new PointF(point1.X + ((point6.X - point1.X) * 0.5f), endPoint.Y);
                            PointF point2 = new PointF(startPoint.X + ((point7.X - point1.X) * 0.5f), point1.Y);
                            PointF point5 = new PointF(point7.X + ((point6.X - point7.X) * 0.5f), point2.Y);
                            PointF point3 = new PointF(point2.X, startPoint.Y);
                            PointF point4 = new PointF(point5.X, point3.Y);

                            pointPolygon = new PointF[] { point1, point2, point3, point4, point5, point6, point7 };
                            e.Graphics.FillPolygon(brushFill, pointPolygon);
                            e.Graphics.DrawPolygon(new Pen(brush, size), pointPolygon);
                        }

                        if (shapes == Shapes.arrowLeft)
                        {
                            PointF point2 = new PointF(startPoint.X + ((endPoint.X - startPoint.X) * 0.4f), startPoint.Y);
                            PointF point7 = new PointF(point2.X, endPoint.Y);
                            PointF point1 = new PointF(startPoint.X, point2.Y + ((point7.Y - point2.Y) * 0.5f));
                            PointF point4 = new PointF(endPoint.X, point2.Y + ((point1.Y - point2.Y) * 0.5f));
                            PointF point5 = new PointF(endPoint.X, point1.Y + ((point7.Y - point1.Y) * 0.5f));
                            PointF point3 = new PointF(point2.X, point4.Y);
                            PointF point6 = new PointF(point3.X, point5.Y);

                            pointPolygon = new PointF[] { point1, point2, point3, point4, point5, point6, point7 };
                            e.Graphics.FillPolygon(brushFill, pointPolygon);
                            e.Graphics.DrawPolygon(new Pen(brush, size), pointPolygon);
                        }

                        if (shapes == Shapes.arrowRight)
                        {
                            PointF point3 = new PointF(startPoint.X + ((endPoint.X - startPoint.X) * 0.6f), startPoint.Y);
                            PointF point5 = new PointF(point3.X, endPoint.Y);
                            PointF point4 = new PointF(endPoint.X, startPoint.Y + ((endPoint.Y - startPoint.Y) * 0.5f));
                            PointF point1 = new PointF(startPoint.X, point3.Y + ((point4.Y - point3.Y) * 0.5f));
                            PointF point7 = new PointF(startPoint.X, point4.Y + ((point5.Y - point4.Y) * 0.5f));
                            PointF point2 = new PointF(point3.X, point1.Y);
                            PointF point6 = new PointF(point5.X, point7.Y);

                            pointPolygon = new PointF[] { point1, point2, point3, point4, point5, point6, point7 };
                            e.Graphics.FillPolygon(brushFill, pointPolygon);
                            e.Graphics.DrawPolygon(new Pen(brush, size), pointPolygon);
                        }

                        #endregion
                    }
                    else if (shapeFillMode == ShapeFillMode.gradient)
                    {
                        try
                        {
                            if (shapes == Shapes.rectangle)
                            {
                                Rectangle r = new Rectangle(ImageHandler.RectAlgorithm(startPoint, endPoint).Item1,
                                   ImageHandler.RectAlgorithm(startPoint, endPoint).Item2);

                                if (startPoint.X < endPoint.X && startPoint.Y < endPoint.Y)
                                {
                                    using (gradientBrush = new LinearGradientBrush(new Point(0, 0), new Point(endPoint.X, endPoint.Y),
                                            colorInner, btnColor3.BackColor))
                                    {
                                        e.Graphics.FillRectangle(gradientBrush, r);
                                        e.Graphics.DrawRectangle(new Pen(brush, size), r);
                                    }
                                }
                                else if (startPoint.X < endPoint.X && startPoint.Y > endPoint.Y)
                                {
                                    using (gradientBrush = new LinearGradientBrush(
                                        new Point(0, 0), new Point(endPoint.X, startPoint.Y),
                                        colorInner, btnColor3.BackColor))
                                    {
                                        e.Graphics.FillRectangle(gradientBrush, r);
                                        e.Graphics.DrawRectangle(new Pen(brush, size), r);
                                    } 
                                }
                                else if (endPoint.X < startPoint.X && endPoint.Y < startPoint.Y)
                                {
                                    using (gradientBrush = new LinearGradientBrush(
                                        new Point(0, 0), new Point(startPoint.X, startPoint.Y),
                                        colorInner, btnColor3.BackColor))
                                    {
                                        e.Graphics.FillRectangle(gradientBrush, r);
                                        e.Graphics.DrawRectangle(new Pen(brush, size), r);
                                    }
                                }
                                else
                                {
                                    using (gradientBrush = new LinearGradientBrush(
                                        new Point(0, 0), new Point(startPoint.X, endPoint.Y),
                                        colorInner, btnColor3.BackColor))
                                    {
                                        e.Graphics.FillRectangle(gradientBrush, r);
                                        e.Graphics.DrawRectangle(new Pen(brush, size), r); 
                                    }
                                }

                            }
                            else
                            {
                                using (gradientBrush = new LinearGradientBrush(
                                startPoint, endPoint,
                                colorInner, btnColor3.BackColor))
                                {
                                    #region line
                                    if (shapes == Shapes.line)
                                    {
                                        e.Graphics.DrawLine(new Pen(gradientBrush, size), startPoint, endPoint);
                                    }
                                    #endregion

                                    #region circle
                                    else if (shapes == Shapes.circle)
                                    {
                                        e.Graphics.FillEllipse(gradientBrush, startPoint.X, startPoint.Y,
                                            endPoint.X - startPoint.X, endPoint.Y - startPoint.Y);
                                        e.Graphics.DrawEllipse(new Pen(brush, size), startPoint.X, startPoint.Y,
                                            endPoint.X - startPoint.X, endPoint.Y - startPoint.Y);
                                    }
                                    #endregion

                                    #region pentagon
                                    else if (shapes == Shapes.polygon)
                                    {
                                        PointF point2 = new PointF(startPoint.X + ((endPoint.X - startPoint.X) * 0.5f), startPoint.Y - 10);
                                        PointF point1 = new PointF(startPoint.X, point2.Y + ((endPoint.Y - point2.Y) * 0.4f));
                                        PointF point3 = new PointF(endPoint.X, point1.Y);
                                        PointF point4 = new PointF(point2.X + ((endPoint.X - point2.X) * 0.6f), endPoint.Y);
                                        PointF point5 = new PointF(startPoint.X + ((point2.X - startPoint.X) * 0.4f), endPoint.Y);

                                        pointPolygon = new PointF[] { point1, point2, point3, point4, point5 };
                                        e.Graphics.FillPolygon(gradientBrush, pointPolygon);
                                        e.Graphics.DrawPolygon(new Pen(brush, size), pointPolygon);
                                    }
                                    #endregion

                                    #region triangle
                                    else if (shapes == Shapes.triangle)
                                    {
                                        PointF point1 = new PointF(startPoint.X, pbCanvas.PointToClient(Cursor.Position).Y);
                                        PointF point2 = new PointF(startPoint.X + ((endPoint.X - startPoint.X) * 0.5f), startPoint.Y - 10);
                                        PointF point3 = new PointF(endPoint.X, endPoint.Y);

                                        pointPolygon = new PointF[] { point1, point2, point3 };
                                        e.Graphics.FillPolygon(gradientBrush, pointPolygon);
                                        e.Graphics.DrawPolygon(new Pen(brush, size), pointPolygon);
                                    }
                                    #endregion

                                    #region diamond
                                    else if (shapes == Shapes.diamond)
                                    {
                                        PointF point2 = new PointF(startPoint.X + ((endPoint.X - startPoint.X) * 0.5f), startPoint.Y - 10);
                                        PointF point1 = new PointF(startPoint.X, point2.Y + ((endPoint.Y - point2.Y)) * 0.5f);
                                        PointF point3 = new PointF(endPoint.X, point1.Y);
                                        PointF point4 = new PointF(point2.X, endPoint.Y);

                                        pointPolygon = new PointF[] { point1, point2, point3, point4 };
                                        e.Graphics.FillPolygon(gradientBrush, pointPolygon);
                                        e.Graphics.DrawPolygon(new Pen(brush, size), pointPolygon);
                                    }
                                    #endregion

                                    #region hexagon
                                    else if (shapes == Shapes.hexagon)
                                    {
                                        PointF point2 = new PointF(startPoint.X + ((endPoint.X - startPoint.X) * 0.5f), startPoint.Y);
                                        PointF point1 = new PointF(startPoint.X, point2.Y + ((endPoint.Y - point2.Y)) * 0.25f);
                                        PointF point3 = new PointF(endPoint.X, point1.Y);
                                        PointF point5 = new PointF(point2.X, endPoint.Y);
                                        PointF point4 = new PointF(endPoint.X, point2.Y + ((endPoint.Y - point2.Y) * 0.75f));
                                        PointF point6 = new PointF(point1.X, point4.Y);

                                        pointPolygon = new PointF[] { point1, point2, point3, point4, point5, point6 };
                                        e.Graphics.FillPolygon(gradientBrush, pointPolygon);
                                        e.Graphics.DrawPolygon(new Pen(brush, size), pointPolygon);
                                    }
                                    #endregion

                                    #region right-angled Triangle
                                    else if (shapes == Shapes.rightTriangle)
                                    {
                                        PointF point1 = new PointF(startPoint.X, startPoint.Y);
                                        PointF point2 = new PointF(endPoint.X, endPoint.Y);
                                        PointF point3 = new PointF(startPoint.X, endPoint.Y);

                                        pointPolygon = new PointF[] { point1, point2, point3 };
                                        e.Graphics.FillPolygon(gradientBrush, pointPolygon);
                                        e.Graphics.DrawPolygon(new Pen(brush, size), pointPolygon);
                                    }
                                    #endregion

                                    #region arrow
                                    if (shapes == Shapes.arrowUp)
                                    {
                                        PointF point1 = new PointF(startPoint.X, startPoint.Y + ((endPoint.Y - startPoint.Y) * 0.4f));
                                        PointF point2 = new PointF(startPoint.X + ((endPoint.X - startPoint.X) * 0.5f), startPoint.Y);
                                        PointF point3 = new PointF(endPoint.X, point1.Y);
                                        PointF point4 = new PointF(point2.X + ((point3.X - point2.X)) * 0.5f, point3.Y);
                                        PointF point5 = new PointF(point4.X, endPoint.Y);
                                        PointF point7 = new PointF(point1.X + ((point2.X - point1.X) * 0.5f), point3.Y);
                                        PointF point6 = new PointF(point7.X, point5.Y);

                                        pointPolygon = new PointF[] { point1, point2, point3, point4, point5, point6, point7 };
                                        e.Graphics.FillPolygon(gradientBrush, pointPolygon);
                                        e.Graphics.DrawPolygon(new Pen(brush, size), pointPolygon);
                                    }

                                    if (shapes == Shapes.arrowDown)
                                    {
                                        PointF point1 = new PointF(startPoint.X, startPoint.Y + ((endPoint.Y - startPoint.Y) * 0.6f));
                                        PointF point6 = new PointF(endPoint.X, point1.Y);
                                        PointF point7 = new PointF(point1.X + ((point6.X - point1.X) * 0.5f), endPoint.Y);
                                        PointF point2 = new PointF(startPoint.X + ((point7.X - point1.X) * 0.5f), point1.Y);
                                        PointF point5 = new PointF(point7.X + ((point6.X - point7.X) * 0.5f), point2.Y);
                                        PointF point3 = new PointF(point2.X, startPoint.Y);
                                        PointF point4 = new PointF(point5.X, point3.Y);

                                        pointPolygon = new PointF[] { point1, point2, point3, point4, point5, point6, point7 };
                                        e.Graphics.FillPolygon(gradientBrush, pointPolygon);
                                        e.Graphics.DrawPolygon(new Pen(brush, size), pointPolygon);
                                    }

                                    if (shapes == Shapes.arrowLeft)
                                    {
                                        PointF point2 = new PointF(startPoint.X + ((endPoint.X - startPoint.X) * 0.4f), startPoint.Y);
                                        PointF point7 = new PointF(point2.X, endPoint.Y);
                                        PointF point1 = new PointF(startPoint.X, point2.Y + ((point7.Y - point2.Y) * 0.5f));
                                        PointF point4 = new PointF(endPoint.X, point2.Y + ((point1.Y - point2.Y) * 0.5f));
                                        PointF point5 = new PointF(endPoint.X, point1.Y + ((point7.Y - point1.Y) * 0.5f));
                                        PointF point3 = new PointF(point2.X, point4.Y);
                                        PointF point6 = new PointF(point3.X, point5.Y);

                                        pointPolygon = new PointF[] { point1, point2, point3, point4, point5, point6, point7 };
                                        e.Graphics.FillPolygon(gradientBrush, pointPolygon);
                                        e.Graphics.DrawPolygon(new Pen(brush, size), pointPolygon);
                                    }

                                    if (shapes == Shapes.arrowRight)
                                    {
                                        PointF point3 = new PointF(startPoint.X + ((endPoint.X - startPoint.X) * 0.6f), startPoint.Y);
                                        PointF point5 = new PointF(point3.X, endPoint.Y);
                                        PointF point4 = new PointF(endPoint.X, startPoint.Y + ((endPoint.Y - startPoint.Y) * 0.5f));
                                        PointF point1 = new PointF(startPoint.X, point3.Y + ((point4.Y - point3.Y) * 0.5f));
                                        PointF point7 = new PointF(startPoint.X, point4.Y + ((point5.Y - point4.Y) * 0.5f));
                                        PointF point2 = new PointF(point3.X, point1.Y);
                                        PointF point6 = new PointF(point5.X, point7.Y);

                                        pointPolygon = new PointF[] { point1, point2, point3, point4, point5, point6, point7 };
                                        e.Graphics.FillPolygon(gradientBrush, pointPolygon);
                                        e.Graphics.DrawPolygon(new Pen(brush, size), pointPolygon);
                                    }

                                    #endregion
                                }
                            }
                        }
                        catch(OutOfMemoryException) { }
                    }
                }
                #endregion

                #region Pointer
                else if (tools == Tools.pointer)
                {
                    Rectangle r = new Rectangle(ImageHandler.RectAlgorithm(startPoint, endPoint).Item1,
                        ImageHandler.RectAlgorithm(startPoint, endPoint).Item2);

                    e.Graphics.DrawRectangle(new Pen(selectBrush), r);

                    cropRectangle.X = r.X; cropRectangle.Y = r.Y;
                    cropRectangle.Width = r.Width; cropRectangle.Height = r.Height;
                    
                }
                #endregion

            }

            if (isRightDown)
            {
                e.Graphics.DrawImage(moveBitMap, new Point(endPoint.X - mouseDisplacement.Width,
                      endPoint.Y - mouseDisplacement.Height));
            }

            if (tools == Tools.text)
            {
                e.Graphics.DrawString(insertText, font, new SolidBrush(textColor), pbCanvas.PointToClient(Cursor.Position));
            }

            if (!lblTitle.Text.Contains("*"))
            {
                lblTitle.Text += "*";
            }
        }

        private void pictureBox1_Resize(object sender, EventArgs e)
        {
            toolStripPictureSize.Text = pbCanvas.Size.Width.ToString() + " x " + pbCanvas.Size.Height.ToString() + "px";

            pictureDimensions.Size = new Size(pbCanvas.Width, pbCanvas.Height);
            pictureDimensions.Location = pbCanvas.Location;
            cropRectangle.Size = new Size(0, 0);
            
            pbBorder.Size = new Size(pbCanvas.Width + 2, pbCanvas.Height + 2);

        }

        private void pictureBox1_SizeChanged(object sender, EventArgs e)
        {
            MainPanel.Refresh();
        }
        #endregion

        #region Methods

        private void noBorder()
        {
            btnSelect.FlatAppearance.BorderSize = 0;
            btnCrop.FlatAppearance.BorderSize = 0;

            btnEraser.FlatAppearance.BorderSize = 0;
            btnPencil.FlatAppearance.BorderSize = 0;
            btnBrush.FlatAppearance.BorderSize = 0;
            btnFill.FlatAppearance.BorderSize = 0;
            btnText.FlatAppearance.BorderSize = 0;

            btnSpray1.FlatAppearance.BorderSize = 0;
            btnSpray2.FlatAppearance.BorderSize = 0;
            btnCustomBrush.FlatAppearance.BorderSize = 0;
            btnMarker.FlatAppearance.BorderSize = 0;
            btnColorPicker.FlatAppearance.BorderSize = 0;

            btnLine.BackColor = Color.Transparent;
            btnCircle.BackColor = Color.Transparent;
            btnRectangle.BackColor = Color.Transparent;
            btnDiamond.BackColor = Color.Transparent;
            btnRightTriangle.BackColor = Color.Transparent;
            btnTriangle.BackColor = Color.Transparent;
            btnPolygon.BackColor = Color.Transparent;
            btnHexagon.BackColor = Color.Transparent;
            btnArrowUp.BackColor = Color.Transparent;
            btnArrowDown.BackColor = Color.Transparent;
            btnArrowLeft.BackColor = Color.Transparent;
            btnArrowRight.BackColor = Color.Transparent;
        }

        private void CreatePictureBox()
        {
            pbCanvas = new PictureBox();
            this.pbCanvas.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.pbCanvas.BorderStyle = BorderStyle.FixedSingle;
            this.pbCanvas.Location = pictureDimensions.Location;
            this.pbCanvas.Name = "pictureBox1";
            this.pbCanvas.Size = new Size(pictureDimensions.Width, pictureDimensions.Height);
            this.pbCanvas.TabIndex = 0;
            this.pbCanvas.TabStop = false;
            this.pbCanvas.Cursor = cursor;
            this.pbCanvas.Click += new System.EventHandler(this.pictureBox1_Click);
            this.pbCanvas.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox1_Paint);
            this.pbCanvas.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseDown);
            this.pbCanvas.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseMove);
            this.pbCanvas.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseUp);
            this.pbCanvas.Resize += new EventHandler(this.pictureBox1_Resize);
            this.pbCanvas.SizeChanged += new EventHandler(this.pictureBox1_SizeChanged);
            MainPanel.Controls.Add(pbCanvas);
            pbCanvas.Image = mainBmp;
            pbCanvas.Refresh();

            c.Init(pbCanvas);
        }

        private void ToolTipInit()
        {
            tooltip.SetToolTip(btnDefault, "Default pointer");
            tooltip.SetToolTip(btnSelect, "Selection\n-Area selector for crop/deletion tool\n-Move area with right-click + drag");
            tooltip.SetToolTip(btnCrop, "Crop picture");
            tooltip.SetToolTip(btnDeleteArea, "Delete selected area");

            tooltip.SetToolTip(btnEraser, "Eraser\n-Erases part of image at mouse location");
            tooltip.SetToolTip(btnPencil, "Pencil");
            tooltip.SetToolTip(btnBrush, "Paint Brush");
            tooltip.SetToolTip(btnFill, "Fill canvas");
            tooltip.SetToolTip(btnColorPicker, "Color picker\n-Get color from previously drawn area");
            tooltip.SetToolTip(btnMarker, "Marker");
            tooltip.SetToolTip(btnText, "Insert Text");

            tooltip.SetToolTip(lblSize, "Width\n-Change width of selected tool(1~100)");
            tooltip.SetToolTip(lblFillShapes, "Fill mode for shapes");

            tooltip.SetToolTip(btnLine, "Line");
            tooltip.SetToolTip(btnCircle, "Circle");
            tooltip.SetToolTip(btnRectangle, "Rectangle");
            tooltip.SetToolTip(btnDiamond, "Diamond");
            tooltip.SetToolTip(btnTriangle, "Triangle");
            tooltip.SetToolTip(btnRightTriangle, "Right-angled Triangle");
            tooltip.SetToolTip(btnPolygon, "Pentagon");
            tooltip.SetToolTip(btnHexagon, "Hexagon");
            tooltip.SetToolTip(btnArrowUp, "Up Arrow");
            tooltip.SetToolTip(btnArrowDown, "Down Arrow");
            tooltip.SetToolTip(btnArrowRight, "Right Arrow");
            tooltip.SetToolTip(btnArrowLeft, "Left Arrow");

            string x = color.ToString().Replace("Color [", "");
            string y = colorInner.ToString().Replace("Color [", "");
            string z = btnColor3.BackColor.ToString().Replace("Color [", "");
            x = x.Replace("]", "");
            y = y.Replace("]", "");
            z = z.Replace("]", "");

            tooltip.SetToolTip(btnColor, x);
            tooltip.SetToolTip(btnColor2, y);
            tooltip.SetToolTip(btnColor3, z);
            tooltip.SetToolTip(btnCustomColor, "Custom color");

            tooltip.SetToolTip(btnSpray1, "Spray Paint(Black)");
            tooltip.SetToolTip(btnSpray2, "Spray Paint(Blue)");
            tooltip.SetToolTip(lblCustomBrush, "Customize your brush with various styles\nalong with the fore & back color");

            tooltip.SetToolTip(btnInvert, "Invert");
            tooltip.SetToolTip(btnGrayScale, "Gray scale");
            tooltip.SetToolTip(btnColorFilter, "Filter");
            tooltip.SetToolTip(btnGamma, "Gamma");
            tooltip.SetToolTip(btnBrightness, "Brightness");
            tooltip.SetToolTip(btnContrast, "Contrast");


            tooltip.SetToolTip(lblStationery, "Stationery\n-Standard tools");
            tooltip.SetToolTip(lblShapes, "Shapes\n-Select a shape to draw");
            tooltip.SetToolTip(lblOthers, "Size & color cannot be\nchanged for these tools");

        }

        #endregion

        #region Form Location

        bool moveForm = false;
        bool toNormal = false;
        Point coordi = new Point();

        private void panelTop_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Location.X >= 110)
            {
                moveForm = true;
                coordi = e.Location;
            }
            if (this.WindowState == FormWindowState.Maximized)
            {
                toNormal = true;
            }
        }

        private void panelTop_MouseUp(object sender, MouseEventArgs e)
        {
            moveForm = false;
            toNormal = false;
            if (MousePosition.Y <= 5)
            {
                this.WindowState = FormWindowState.Maximized;
            }
            if (MousePosition.X >= Screen.PrimaryScreen.Bounds.Width - 5)
            {

                this.Size = new Size(Screen.PrimaryScreen.Bounds.Width / 2, Screen.PrimaryScreen.WorkingArea.Height);
                this.Location = new Point(Screen.PrimaryScreen.Bounds.Width - this.Width, 0);
            }
            if (MousePosition.X <= 0)
            {
                this.Size = new Size(Screen.PrimaryScreen.Bounds.Width / 2, Screen.PrimaryScreen.WorkingArea.Height);
                this.Location = new Point(0, 0);
            }
        }

        private void panelTop_MouseMove(object sender, MouseEventArgs e)
        {
            if (toNormal)
            {
                this.WindowState = FormWindowState.Normal;
            }
            if (moveForm)
            {
                this.SetDesktopLocation(MousePosition.X - coordi.X, MousePosition.Y - coordi.Y);
            }
        }

        private void menuStrip1_DoubleClick(object sender, EventArgs e)
        {
            if (menuStrip1.PointToClient(Cursor.Position).X >= 110)
                btnMaximize.PerformClick();
        }

        private void lblTitle_MouseDown(object sender, MouseEventArgs e)
        {
            moveForm = true;
            coordi = new Point(e.Location.X + lblTitle.Left, e.Location.Y + lblTitle.Top);
            if (this.WindowState == FormWindowState.Maximized)
            {
                toNormal = true;
            }
        }

        private void lblTitle_MouseUp(object sender, MouseEventArgs e)
        {
            moveForm = false;
            toNormal = false;
            if (MousePosition.Y <= 5)
            {
                this.WindowState = FormWindowState.Maximized;
            }
            if (MousePosition.X >= Screen.PrimaryScreen.Bounds.Width - 5)
            {
                this.Size = new Size(Screen.PrimaryScreen.Bounds.Width / 2, Screen.PrimaryScreen.WorkingArea.Height);
                this.Location = new Point(Screen.PrimaryScreen.Bounds.Width - this.Width, 0);
            }
            if (MousePosition.X <= 0)
            {
                this.Size = new Size(Screen.PrimaryScreen.Bounds.Width / 2, Screen.PrimaryScreen.WorkingArea.Height);
                this.Location = new Point(0, 0);
            }
        }

        private void lblTitle_MouseMove(object sender, MouseEventArgs e)
        {
            if (toNormal)
            {
                this.WindowState = FormWindowState.Normal;
            }
            if (moveForm)
            {
                this.SetDesktopLocation(MousePosition.X - coordi.X, MousePosition.Y - coordi.Y);
            }
        }

        private void lblTitle_DoubleClick(object sender, EventArgs e)
        {
            btnMaximize.PerformClick();
        }

        private void panelTop_DoubleClick(object sender, EventArgs e)
        {
            btnMaximize.PerformClick();
        }

        #endregion

        #region Form Events
        private void FormPaint_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (mainBmp != null && saved == false)
            {
                e.Cancel = true;
                DialogResult dr = BasicSave.Show(path);

                if (dr == DialogResult.Yes)
                {
                    if (savedBefore == false)
                    {
                        saveAsToolStripMenuItem.PerformClick();
                        e.Cancel = false;
                    }
                    else
                    {
                        saveFileToolStripMenuItem.PerformClick();
                        e.Cancel = false;
                    }
                }
                else if (dr == DialogResult.No)
                {
                    e.Cancel = false;
                }
            }
        }

        private void FormPaint_Resize(object sender, EventArgs e)
        {
            pbCanvas.Invalidate();
            this.panelBottom.Region = new System.Drawing.Region(new Rectangle(panelBottom.ClientRectangle.Location,
                new Size(panelBottom.ClientSize.Width - 20, panelBottom.ClientSize.Height)));
        }

        private void FormPaint_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.N)
            {
                newToolStripMenuItem.PerformClick();
            }
            if (e.Control && e.KeyCode == Keys.S)
            {
                saveFileToolStripMenuItem.PerformClick();
            }

            if (e.KeyCode == Keys.F12)
            {
                saveAsToolStripMenuItem.PerformClick();
            }

            if (e.Control && e.KeyCode == Keys.O)
            {
                loadToolStripMenuItem.PerformClick();
            }

            if (e.KeyCode == Keys.Escape)
            {
                btnDefault.PerformClick();
            }
            
            if(e.Control && e.KeyCode == Keys.Z)
            {
                undoToolStripMenuItem.PerformClick();
            }

            if(e.Control && e.KeyCode == Keys.Y)
            {
                redoToolStripMenuItem.PerformClick();
            }

            if(e.Control && e.KeyCode == Keys.X)
            {
                cutToolStripMenuItem.PerformClick();
            }

            if (e.Control && e.KeyCode == Keys.C)
            {
                copyToolStripMenuItem.PerformClick();
            }

            if (e.Control && e.KeyCode == Keys.V)
            {
                pasteToolStripMenuItem.PerformClick();
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboSize.SelectedIndex != -1)
                size = Convert.ToInt32(comboSize.SelectedItem);
        }

        private void comboBox1_TextUpdate(object sender, EventArgs e)
        {
            int x = Int32.TryParse(comboSize.Text, out x) ? x : 0;

            if (x > 0 && x <= 100)
            {
                size = x;
                comboSize.Text = x.ToString();
            }
            else if (x <= 0)
            {
                comboSize.Text = Convert.ToString(1);
            }
            else if (x > 100) comboSize.Text = Convert.ToString(100);
        }

        private void comboFillShapes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboFillShapes.SelectedIndex == 0)
            {
                shapeFillMode = ShapeFillMode.none;
            }
            else if (comboFillShapes.SelectedIndex == 1)
            {
                shapeFillMode = ShapeFillMode.solid;
            }
            else if (comboFillShapes.SelectedIndex == 2)
            {
                shapeFillMode = ShapeFillMode.gradient;
            }
        }

        private void btnCustomize_Click(object sender, EventArgs e)
        {
            CustomBrush cb = new CustomBrush(style, foreHatchColor, backHatchColor);

            if (cb.ShowDialog() == DialogResult.OK)
            {
                style = cb.style;
                foreHatchColor = cb.foreHatchColor;
                backHatchColor = cb.backHatchColor;
            }
        }

        private void FormPaint_Shown(object sender, EventArgs e)
        {
            Application.DoEvents();
            saved = true;
            lblTitle.Text = "Untitled - Simple Paint";
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Rectangle rc = new Rectangle(this.ClientSize.Width - cGrip, this.ClientSize.Height - cGrip, cGrip, cGrip);
            ControlPaint.DrawSizeGrip(e.Graphics, this.BackColor, rc);
            base.OnPaint(e);
        }

        private const int cGrip = 16;
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x84)
            {
                Point pos = this.PointToClient(MousePosition);

                if (pos.X >= this.Width - cGrip && pos.Y >= this.Height - cGrip)
                {
                    m.Result = (IntPtr)17; // HTBOTTOMRIGHT
                    return;
                }
            }
            base.WndProc(ref m);
        }

        #endregion

        #region MenuEvents

        private void saveFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (savedBefore == false)
            {
                saveFileDialog1.Filter = "Png Image (*.png)|*.png|Jpeg Image (*.jpg)|*.jpg|Bitmap Image (*.bmp)|*.bmp";
                saveFileDialog1.Title = "Save Image File";
                saveFileDialog1.FileName = "Untitled";
                saveFileDialog1.ShowDialog();
            }
            else if (savedBefore == true && saved == false)
            {
                pbCanvas.Dispose();
                File.Delete(path);
                mainBmp.Save(path);
                CreatePictureBox();
                lblTitle.Text = Path.GetFileNameWithoutExtension(path) + " - Simple Paint";
                saved = true;

            }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "Png Image (*.png)|*.png|Jpeg Image (*.jpg)|*.jpg|Bitmap Image (*.bmp)|*.bmp";
            saveFileDialog1.Title = "Save Image File";
            saveFileDialog1.FileName = "Untitled";
            saveFileDialog1.ShowDialog();

            pbCanvas.Dispose();
            CreatePictureBox();
            lblTitle.Text = Path.GetFileNameWithoutExtension(path) + " - Simple Paint";
            saved = true;

        }

        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            if (pbCanvas.Image == null)
            {
                mainBmp = new Bitmap(pbCanvas.Width, pbCanvas.Height);
                pbCanvas.Image = mainBmp;
            }
            if (saveFileDialog1.FileName != "")
            {
                using (FileStream fs = (FileStream)saveFileDialog1.OpenFile())
                {
                    path = saveFileDialog1.FileName;
                    switch (saveFileDialog1.FilterIndex)
                    {
                        case 1:
                            mainBmp.Save(fs, ImageFormat.Png);
                            break;

                        case 2:
                            Bitmap blank = new Bitmap(mainBmp.Width, mainBmp.Height);
                            using (Graphics g = Graphics.FromImage(blank))
                            {
                                g.Clear(Color.White);
                                g.DrawImage(mainBmp, 0, 0, mainBmp.Width, mainBmp.Height);
                            }

                            blank.Save(fs, ImageFormat.Jpeg);
                            mainBmp.Dispose();
                            mainBmp = new Bitmap(blank);
                            blank.Dispose();
                            pbCanvas.Image = mainBmp;
                            break;

                        case 3:

                            Bitmap temp = new Bitmap(mainBmp.Width, mainBmp.Height);
                            using (Graphics g = Graphics.FromImage(temp))
                            {
                                g.Clear(Color.White);
                                g.DrawImage(mainBmp, 0, 0, mainBmp.Width, mainBmp.Height);
                            }

                            temp.Save(fs, ImageFormat.Bmp);
                            mainBmp.Dispose();
                            mainBmp = new Bitmap(temp);
                            temp.Dispose();
                            pbCanvas.Image = mainBmp;

                            break;
                    }

                    saved = true;
                    savedBefore = true;
                    lblTitle.Text = Path.GetFileNameWithoutExtension(path) + " - Simple Paint";
                }
            }
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mainBmp != null && !saved)
            {
                DialogResult dr = BasicSave.Show(path);


                if (dr == DialogResult.Cancel) { return; }
                if (dr == DialogResult.Yes)
                {
                    if (savedBefore == false)
                    {
                        saveAsToolStripMenuItem.PerformClick();
                    }
                    else
                    {
                        saveFileToolStripMenuItem.PerformClick();
                    }
                    mainBmp.Dispose();
                    mainBmp = new Bitmap(pbCanvas.Width, pbCanvas.Height);
                    pbCanvas.Image = mainBmp;

                    path = "";
                    lblTitle.Text = "Untitled - Simple Paint";
                    saved = true;
                    savedBefore = false;

                    undoRedo.DeleteUndoRedoBitmapData();
                    undoRedo.InsertUndoBitmapData(ImageHandler.StoreCurrentBitmap(mainBmp));
                }
                else if (dr == DialogResult.No)
                {
                    mainBmp.Dispose();
                    mainBmp = new Bitmap(pbCanvas.Width, pbCanvas.Height);
                    pbCanvas.Image = mainBmp;
                    pbCanvas.Invalidate();

                    path = "";
                    lblTitle.Text = "Untitled - Simple Paint";
                    saved = true;
                    savedBefore = false;

                    undoRedo.DeleteUndoRedoBitmapData();
                    undoRedo.InsertUndoBitmapData(ImageHandler.StoreCurrentBitmap(mainBmp));
                }


            }
            else if (saved)
            {
                mainBmp.Dispose();
                mainBmp = new Bitmap(pbCanvas.Width, pbCanvas.Height);
                pbCanvas.Image = mainBmp;
                pbCanvas.Invalidate();

                path = "";
                lblTitle.Text = "Untitled - Simple Paint";
                saved = true;
                savedBefore = false;
            }
            ClearChecked();
            zoom100.Checked = true;
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            #region Not Saved
            if (mainBmp != null && saved == false)
            {
                DialogResult dr = BasicSave.Show(path);

                if (dr == DialogResult.Cancel) return;

                if (dr == DialogResult.Yes)
                {
                    if (savedBefore == false)
                    {
                        saveAsToolStripMenuItem.PerformClick();
                    }
                    else
                    {
                        saveFileToolStripMenuItem.PerformClick();
                    }
                    if (lblTitle.Text.Contains("*"))
                    {
                        lblTitle.Text = lblTitle.Text.Replace("*", "");
                    }
                    saved = true;
                }

                using (OpenFileDialog op = new OpenFileDialog())
                {
                    op.Title = "Open Image";
                    op.Filter = "Png Image (*.png)|*.png|Jpeg Image (*.jpg)|*.jpg|Bitmap Image (*.bmp)|*.bmp|All Image Files(*.png,*.jpg,*bmp)|*.png;*.jpg;*.bmp";
                    op.FilterIndex = 4;

                    if (op.ShowDialog() == DialogResult.OK)
                    {
                        pbCanvas.Dispose();
                        if (mainBmp != null)
                        {
                            mainBmp.Dispose();
                        }
                        tempBitMap = new Bitmap(op.FileName);
                        mainBmp = new Bitmap(tempBitMap, tempBitMap.Size);
                        tempBitMap.Dispose();

                        pictureDimensions.Size = new Size(mainBmp.Width, mainBmp.Height);
                        CreatePictureBox();
                        pbBorder.Size = new Size(pbCanvas.Width + 2, pbCanvas.Height + 2);
                        pbBorder.SendToBack();
                        MainPanel.Refresh();

                        undoRedo.DeleteUndoRedoBitmapData();
                        undoRedo.InsertUndoBitmapData(ImageHandler.StoreCurrentBitmap(mainBmp));

                        Zoom.Text = "Zoom: 100%";
                        ZoomBar.CurrentTickPosition = 3;
                        toolStripPictureSize.Text = pbCanvas.Size.Width.ToString() + " x " + pbCanvas.Size.Height.ToString() + "px";

                        lblTitle.Text = Path.GetFileNameWithoutExtension(op.FileName) + " - Simple Paint";
                        path = op.FileName;
                        saved = true;
                        savedBefore = true;
                    }
                }
            }
            #endregion

            #region Saved
            else
            {
                using (OpenFileDialog op = new OpenFileDialog())
                {
                    op.Title = "Open Image";
                    op.Filter = "Png Image (*.png)|*.png|Jpeg Image (*.jpg)|*.jpg|Bitmap Image (*.bmp)|*.bmp|All Image Files(*.png,*.jpg,*bmp)|*.png;*.jpg;*.bmp";
                    op.FilterIndex = 4;

                    if (op.ShowDialog() == DialogResult.OK)
                    {
                        pbCanvas.Dispose();
                        if (mainBmp != null)
                        {
                            mainBmp.Dispose();
                        }
                        tempBitMap = new Bitmap(op.FileName);
                        mainBmp = new Bitmap(tempBitMap, tempBitMap.Size);
                        tempBitMap.Dispose();

                        pictureDimensions.Size = new Size(mainBmp.Width, mainBmp.Height);
                        CreatePictureBox();
                        pbBorder.Size = new Size(pbCanvas.Width + 2, pbCanvas.Height + 2);
                        pbBorder.SendToBack();
                        MainPanel.Refresh();

                        undoRedo.DeleteUndoRedoBitmapData();
                        undoRedo.InsertUndoBitmapData(ImageHandler.StoreCurrentBitmap(mainBmp));

                        Zoom.Text = "Zoom: 100%";
                        ZoomBar.CurrentTickPosition = 3;
                        toolStripPictureSize.Text = pbCanvas.Size.Width.ToString() + " x " + pbCanvas.Size.Height.ToString() + "px";

                        lblTitle.Text = Path.GetFileNameWithoutExtension(op.FileName) + " - Simple Paint";
                        path = op.FileName;
                        saved = true;
                        savedBefore = true;
                    }
                }
            }
            ClearChecked();
            zoom100.Checked = true;
            #endregion

            MainPanel.Refresh();
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutPaint ap = new AboutPaint();
            ap.Show();
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            undoRedo.Undo(mainBmp);
            if(!isZoomed)
            {
                pbCanvas.Size = new Size(mainBmp.Width, mainBmp.Height);
            }
            else if (isZoomed)
            {
                pbCanvas.Image = ImageHandler.PictureBoxZoom(mainBmp, zoomFactor);
                pbCanvas.Size = new Size(pbCanvas.Image.Width, pbCanvas.Height);
            }
            
            pbCanvas.Invalidate();
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            undoRedo.Redo(mainBmp);
            if (!isZoomed)
            {
                pbCanvas.Size = new Size(mainBmp.Width, mainBmp.Height);
            }
            else if (isZoomed)
            {
                pbCanvas.Image = ImageHandler.PictureBoxZoom(mainBmp, zoomFactor);
                pbCanvas.Size = new Size(pbCanvas.Image.Width, pbCanvas.Height);
            }
            pbCanvas.Invalidate();
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (cropRectangle.Width == 0 || cropRectangle.Height == 0) return;
            Bitmap temp = new Bitmap(mainBmp.Clone(cropRectangle, mainBmp.PixelFormat));
            Clipboard.SetImage(temp);
            btnDeleteArea.PerformClick();

        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (cropRectangle.Width == 0 && cropRectangle.Height == 0) return;

            Bitmap temp = new Bitmap(mainBmp.Clone(cropRectangle, mainBmp.PixelFormat));

            Clipboard.SetImage(temp);
            tooltip.Show("Saved to Clipboard!", pbCanvas, pbCanvas.PointToClient(Cursor.Position), 1500);

            cropRectangle.Width = 0; cropRectangle.Height = 0; cropRectangle.Location = Point.Empty;
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Clipboard.GetImage() == null ||
                mainBmp == null) return;
            
            mainBmp.Dispose();
            mainBmp = new Bitmap(Clipboard.GetImage());
            pbCanvas.Image = mainBmp;

            if(mainBmp.Width > pbCanvas.Width ||
                mainBmp.Height > pbCanvas.Height)
            pbCanvas.Size = new System.Drawing.Size(mainBmp.Width, mainBmp.Height);
            pbCanvas.Refresh();
            
            undoRedo.InsertUndoBitmapData(ImageHandler.StoreCurrentBitmap(mainBmp));
        }

        private void redFilterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btnFilterRed.PerformClick();
        }

        private void greenFilterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btnFilterGreen.PerformClick();
        }

        private void blueFilterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btnFilterBlue.PerformClick();
        }

        private void invertToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btnInvert.PerformClick();
        }

        private void grayscaleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btnGrayScale.PerformClick();
        }

        private void contrastToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btnContrast.PerformClick();
        }

        private void gammaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btnGamma.PerformClick();
        }

        private void brightnessToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btnBrightness.PerformClick();
        }

        #endregion

        #region Border Buttons & Styling
        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void btnMaximize_Click(object sender, EventArgs e)
        {
            if (!(this.WindowState == FormWindowState.Maximized))
                this.WindowState = FormWindowState.Maximized;
            else this.WindowState = FormWindowState.Normal;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void pbBorder_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawLine(new Pen(new SolidBrush(Color.FromArgb(150, 80, 80, 80)), 3),
                new Point(pbBorder.Width - 2, 1),
                new Point(pbBorder.Width - 2, pbBorder.Height - 1));
            e.Graphics.DrawLine(new Pen(new SolidBrush(Color.FromArgb(150, 80, 80, 80)), 3),
                new Point(1, pbBorder.Height - 2),
                new Point(pbBorder.Width - 1, pbBorder.Height - 2));
        }

        private class MyRenderer : ToolStripProfessionalRenderer
        {
            protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
            {
                if (!e.Item.Selected)
                {
                    Rectangle rc = new Rectangle(Point.Empty, e.Item.Size);
                    e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(44, 44, 44)), rc);
                }
                else
                {
                    Rectangle rc = new Rectangle(Point.Empty, e.Item.Size);
                    e.Graphics.FillRectangle(Brushes.DimGray, rc);
                }
            }

            protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e)
            {
                e.ArrowColor = Color.WhiteSmoke;
                base.OnRenderArrow(e);
            }

            protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
            {
                e.Graphics.Clear(Color.FromArgb(44, 44, 44));

                if (e.Item.Width > 200)
                {
                    e.Graphics.DrawLine(new Pen(Color.FromArgb(100, 100, 100)), new Point(60, 3), new Point(e.Item.Width, 3));
                }
                else
                {
                    e.Graphics.DrawLine(new Pen(Color.FromArgb(100, 100, 100)), new Point(35, 3), new Point(e.Item.Width, 3));
                }
            }

            protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
            {
                e.Graphics.Clear(Color.FromArgb(44, 44, 44));
            }

        }

        private void FormPaint_Paint(object sender, PaintEventArgs e)
        {
            if (this.WindowState != FormWindowState.Maximized)
            {
                this.DockPadding.Right = 1;
                this.DockPadding.Bottom = 1;
                Rectangle r = new Rectangle(0, 0, Width - 1, Height - 1);
                e.Graphics.DrawRectangle(Pens.RoyalBlue, r);
            }
        }

        #endregion

        #region ToolSelection

        #region Colors

        #region Normal Colors
        private void btnBlack_Click(object sender, EventArgs e)
        {
            if (whichColor == 1)
            {
                btnColor.BackColor = Color.Black;
            }
            else if (whichColor == 2)
            {
                btnColor2.BackColor = Color.Black;
            }
            else
            {
                btnColor3.BackColor = Color.Black;
            }
        }

        private void btnWhite_Click(object sender, EventArgs e)
        {
            if (whichColor == 1)
            {
                btnColor.BackColor = Color.White;
            }
            else if (whichColor == 2)
            {
                btnColor2.BackColor = Color.White;
            }
            else
            {
                btnColor3.BackColor = Color.White;
            }
        }

        private void btnRed_Click(object sender, EventArgs e)
        {
            if (whichColor == 1)
            {
                btnColor.BackColor = Color.Red;
            }
            else if (whichColor == 2)
            {
                btnColor2.BackColor = Color.Red;
            }
            else
            {
                btnColor3.BackColor = Color.Red;
            }
        }

        private void btnBlue_Click(object sender, EventArgs e)
        {
            if (whichColor == 1)
            {
                btnColor.BackColor = Color.Blue;
            }
            else if (whichColor == 2)
            {
                btnColor2.BackColor = Color.Blue;
            }
            else
            {
                btnColor3.BackColor = Color.Blue;
            }
        }

        private void btnGreen_Click(object sender, EventArgs e)
        {
            if (whichColor == 1)
            {
                btnColor.BackColor = Color.Green;
            }
            else if (whichColor == 2)
            {
                btnColor2.BackColor = Color.Green;
            }
            else
            {
                btnColor3.BackColor = Color.Green;
            }
        }

        private void btnYellow_Click(object sender, EventArgs e)
        {
            if (whichColor == 1)
            {
                btnColor.BackColor = Color.Yellow;
            }
            else if (whichColor == 2)
            {
                btnColor2.BackColor = Color.Yellow;
            }
            else
            {
                btnColor3.BackColor = Color.Yellow;
            }
        }

        private void btnOrange_Click(object sender, EventArgs e)
        {
            if (whichColor == 1)
            {
                btnColor.BackColor = Color.Orange;
            }
            else if (whichColor == 2)
            {
                btnColor2.BackColor = Color.Orange;
            }
            else
            {
                btnColor3.BackColor = Color.Orange;
            }
        }

        private void btnIndigo_Click(object sender, EventArgs e)
        {
            if (whichColor == 1)
            {
                btnColor.BackColor = Color.Indigo;
            }
            else if (whichColor == 2)
            {
                btnColor2.BackColor = Color.Indigo;
            }
            else
            {
                btnColor3.BackColor = Color.Indigo;
            }
        }

        private void btnBeige_Click(object sender, EventArgs e)
        {
            if (whichColor == 1)
            {
                btnColor.BackColor = Color.FromArgb(255, 192, 128);
            }
            else if (whichColor == 2)
            {
                btnColor2.BackColor = Color.FromArgb(255, 192, 128);
            }
            else
            {
                btnColor3.BackColor = Color.FromArgb(255, 192, 128);
            }
        }

        private void btnTurquoise_Click(object sender, EventArgs e)
        {
            if (whichColor == 1)
            {
                btnColor.BackColor = Color.Turquoise;
            }
            else if (whichColor == 2)
            {
                btnColor2.BackColor = Color.Turquoise;
            }
            else
            {
                btnColor3.BackColor = Color.Turquoise;
            }
        }

        private void btnMaroon_Click(object sender, EventArgs e)
        {
            if (whichColor == 1)
            {
                btnColor.BackColor = Color.Maroon;
            }
            else if (whichColor == 2)
            {
                btnColor2.BackColor = Color.Maroon;
            }
            else
            {
                btnColor3.BackColor = Color.Maroon;
            }
        }

        private void btnDarkPurple_Click(object sender, EventArgs e)
        {
            if (whichColor == 1)
            {
                btnColor.BackColor = Color.FromArgb(64, 0, 64);
            }
            else if (whichColor == 2)
            {
                btnColor2.BackColor = Color.FromArgb(64, 0, 64);
            }
            else
            {
                btnColor3.BackColor = Color.FromArgb(64, 0, 64);
            }
        }

        private void btnCadetBlue_Click(object sender, EventArgs e)
        {
            if (whichColor == 1)
            {
                btnColor.BackColor = Color.CadetBlue;
            }
            else if (whichColor == 2)
            {
                btnColor2.BackColor = Color.CadetBlue;
            }
            else
            {
                btnColor3.BackColor = Color.CadetBlue;
            }
        }

        private void btnDeepPink_Click(object sender, EventArgs e)
        {
            if (whichColor == 1)
            {
                btnColor.BackColor = Color.DeepPink;
            }
            else if (whichColor == 2)
            {
                btnColor2.BackColor = Color.DeepPink;
            }
            else
            {
                btnColor3.BackColor = Color.DeepPink;
            }
        }

        private void btnPink_Click(object sender, EventArgs e)
        {
            if (whichColor == 1)
            {
                btnColor.BackColor = Color.Pink;
            }
            else if (whichColor == 2)
            {
                btnColor2.BackColor = Color.Pink;
            }
            else
            {
                btnColor3.BackColor = Color.Pink;
            }
        }

        private void btnLightSteelBlue_Click(object sender, EventArgs e)
        {
            if (whichColor == 1)
            {
                btnColor.BackColor = Color.LightSteelBlue;
            }
            else if (whichColor == 2)
            {
                btnColor2.BackColor = Color.LightSteelBlue;
            }
            else
            {
                btnColor3.BackColor = Color.LightSteelBlue;
            }
        }

        private void btnCrimson_Click(object sender, EventArgs e)
        {
            if (whichColor == 1)
            {
                btnColor.BackColor = Color.Crimson;
            }
            else if (whichColor == 2)
            {
                btnColor2.BackColor = Color.Crimson;
            }
            else
            {
                btnColor3.BackColor = Color.Crimson;
            }
        }

        private void btnDarkKhaki_Click(object sender, EventArgs e)
        {
            if (whichColor == 1)
            {
                btnColor.BackColor = Color.DarkKhaki;
            }
            else if (whichColor == 2)
            {
                btnColor2.BackColor = Color.DarkKhaki;
            }
            else
            {
                btnColor3.BackColor = Color.DarkKhaki;
            }
        }

        #endregion

        #region Custom Colors
        private void btnCustomColor_Click(object sender, EventArgs e)
        {
            cColorDialog c = new cColorDialog();

            if (c.ShowDialog() == DialogResult.OK)
            {
                if (whichColor == 1)
                {
                    btnColor.BackColor = c.ColorSelected;
                }
                else if (whichColor == 2)
                {
                    btnColor2.BackColor = c.ColorSelected;
                }
                else
                {
                    btnColor3.BackColor = c.ColorSelected;
                }

                if (c.ColorSelected != Color.Transparent)
                {
                    if (count < 7)
                    {
                        controlList[count].BackColor = c.ColorSelected;
                        controlList[count].Enabled = true;
                        count++;
                    }
                    else
                    {
                        count = 0;
                        controlList[count].BackColor = c.ColorSelected;
                        count++;
                    }
                }
            }
        }

        private void btnCustom1_Click(object sender, EventArgs e)
        {
            if (whichColor == 1)
            {
                btnColor.BackColor = btnCustom1.BackColor;
            }
            else if (whichColor == 2)
            {
                btnColor2.BackColor = btnCustom1.BackColor;
            }
            else
            {
                btnColor3.BackColor = btnCustom1.BackColor;
            }
        }

        private void btnCustom2_Click(object sender, EventArgs e)
        {
            if (whichColor == 1)
            {
                btnColor.BackColor = btnCustom2.BackColor;
            }
            else if (whichColor == 2)
            {
                btnColor2.BackColor = btnCustom2.BackColor;
            }
            else
            {
                btnColor3.BackColor = btnCustom2.BackColor;
            }
        }

        private void btnCustom3_Click(object sender, EventArgs e)
        {
            if (whichColor == 1)
            {
                btnColor.BackColor = btnCustom3.BackColor;
            }
            else if (whichColor == 2)
            {
                btnColor2.BackColor = btnCustom3.BackColor;
            }
            else
            {
                btnColor3.BackColor = btnCustom3.BackColor;
            }
        }

        private void btnCustom4_Click(object sender, EventArgs e)
        {
            if (whichColor == 1)
            {
                btnColor.BackColor = btnCustom4.BackColor;
            }
            else if (whichColor == 2)
            {
                btnColor2.BackColor = btnCustom4.BackColor;
            }
            else
            {
                btnColor3.BackColor = btnCustom4.BackColor;
            }
        }

        private void btnCustom5_Click(object sender, EventArgs e)
        {
            if (whichColor == 1)
            {
                btnColor.BackColor = btnCustom5.BackColor;
            }
            else if (whichColor == 2)
            {
                btnColor2.BackColor = btnCustom5.BackColor;
            }
            else
            {
                btnColor3.BackColor = btnCustom5.BackColor;
            }
        }

        private void btnCustom6_Click(object sender, EventArgs e)
        {
            if (whichColor == 1)
            {
                btnColor.BackColor = btnCustom6.BackColor;
            }
            else if (whichColor == 2)
            {
                btnColor2.BackColor = btnCustom6.BackColor;
            }
            else
            {
                btnColor3.BackColor = btnCustom6.BackColor;
            }
        }

        private void btnCustom7_Click(object sender, EventArgs e)
        {
            if (whichColor == 1)
            {
                btnColor.BackColor = btnCustom7.BackColor;
            }
            else if (whichColor == 2)
            {
                btnColor2.BackColor = btnCustom7.BackColor;
            }
            else
            {
                btnColor3.BackColor = btnCustom7.BackColor;
            }
        }

        private void btnCustomColor_MouseEnter(object sender, EventArgs e)
        {
            btnCustomColor.ForeColor = Color.Black;
        }

        private void btnCustomColor_MouseLeave(object sender, EventArgs e)
        {
            btnCustomColor.ForeColor = Color.FromArgb(240, 240, 240);
        }

        #endregion

        private void btnColor_Click(object sender, EventArgs e)
        {
            whichColor = 1;
            lblColorSelect1.BackColor = Color.PowderBlue;
            lblColorSelect1.ForeColor = Color.Black;

            lblColorSelect3.BackColor = lblColorSelect2.BackColor = Color.Transparent;
            lblColorSelect3.ForeColor = lblColorSelect2.ForeColor = Color.WhiteSmoke;
        }

        private void btnColor2_Click(object sender, EventArgs e)
        {
            whichColor = 2;
            lblColorSelect2.BackColor = Color.PowderBlue;
            lblColorSelect2.ForeColor = Color.Black;
            lblColorSelect3.BackColor = lblColorSelect1.BackColor = Color.Transparent;
            lblColorSelect3.ForeColor = lblColorSelect1.ForeColor = Color.WhiteSmoke;
        }

        private void btnColor3_Click(object sender, EventArgs e)
        {
            whichColor = 3;
            lblColorSelect3.BackColor = Color.PowderBlue;
            lblColorSelect3.ForeColor = Color.Black;

            lblColorSelect2.BackColor = lblColorSelect1.BackColor = Color.Transparent;
            lblColorSelect2.ForeColor = lblColorSelect1.ForeColor = Color.WhiteSmoke;
        }

        private void btnColor_BackColorChanged(object sender, EventArgs e)
        {
            string x = btnColor.BackColor.ToString().Replace("Color [", "");
            x = x.Replace("]", "");
            tooltip.SetToolTip(btnColor, x);
            color = btnColor.BackColor;
        }

        private void btnColor2_BackColorChanged(object sender, EventArgs e)
        {
            string x = btnColor2.BackColor.ToString().Replace("Color [", "");
            x = x.Replace("]", "");
            tooltip.SetToolTip(btnColor2, x);
            colorInner = btnColor2.BackColor;
        }

        private void btnColor3_BackColorChanged(object sender, EventArgs e)
        {
            string x = btnColor3.BackColor.ToString().Replace("Color [", "");
            x = x.Replace("]", "");
            tooltip.SetToolTip(btnColor3, x);
        }

        #endregion

        #region Stationery
        private void btnEraser_Click(object sender, EventArgs e)
        {
            tools = Tools.stationery;
            stationery = Stationery.eraser;

            cursor = CursorResourceLoader.LoadEmbeddedCursor(Properties.Resources.eraser_cursor);
            pbCanvas.Cursor = cursor;

            noBorder();
            btnEraser.FlatAppearance.BorderSize = 1;

            comboSize.Items.Clear();
            comboSize.Items.Add("5"); comboSize.Items.Add("8"); comboSize.Items.Add("12"); comboSize.Items.Add("15"); comboSize.Items.Add("18");
            comboSize.SelectedIndex = 3;
        }

        private void btnPencil_Click(object sender, EventArgs e)
        {
            tools = Tools.stationery;
            stationery = Stationery.pencil;

            cursor = CursorResourceLoader.LoadEmbeddedCursor(Properties.Resources.pencil_cursor);
            pbCanvas.Cursor = cursor;

            noBorder();
            btnPencil.FlatAppearance.BorderSize = 1;

            comboSize.Items.Clear();
            comboSize.Items.Add("1"); comboSize.Items.Add("2"); comboSize.Items.Add("3");
            comboSize.SelectedIndex = 0;
        }

        private void btnBrush_Click(object sender, EventArgs e)
        {
            tools = Tools.stationery;
            stationery = Stationery.brush;

            cursor = CursorResourceLoader.LoadEmbeddedCursor(Properties.Resources.brush_cursor);
            pbCanvas.Cursor = cursor;

            noBorder();
            btnBrush.FlatAppearance.BorderSize = 1;

            comboSize.Items.Clear();
            comboSize.Items.Add("4"); comboSize.Items.Add("5"); comboSize.Items.Add("6");
            comboSize.Items.Add("7"); comboSize.Items.Add("8");
            comboSize.SelectedIndex = 1;
        }

        private void btnSpray1_Click(object sender, EventArgs e)
        {
            tools = Tools.stationery;
            stationery = Stationery.brush1;

            cursor = CursorResourceLoader.LoadEmbeddedCursor(Properties.Resources.spray_cursor);
            pbCanvas.Cursor = cursor;

            noBorder();
            btnSpray1.FlatAppearance.BorderSize = 1;

            comboSize.Items.Clear();
        }

        private void btnSpray2_Click(object sender, EventArgs e)
        {
            tools = Tools.stationery;
            stationery = Stationery.brush2;

            cursor = CursorResourceLoader.LoadEmbeddedCursor(Properties.Resources.spray_cursor);
            pbCanvas.Cursor = cursor;

            noBorder();
            btnSpray2.FlatAppearance.BorderSize = 1;

            comboSize.Items.Clear();
        }

        private void btnCustomBrush_Click(object sender, EventArgs e)
        {
            int x = 1;
            tools = Tools.stationery;
            stationery = Stationery.custom;

            cursor = CursorResourceLoader.LoadEmbeddedCursor(Properties.Resources.brush_cursor);
            pbCanvas.Cursor = cursor;

            noBorder();
            btnCustomBrush.FlatAppearance.BorderSize = 1;

            comboSize.Items.Clear();
            while (x < 21)
            {
                comboSize.Items.Add(x);
                x++;
            }

            comboSize.SelectedIndex = 7;
        }

        private void btnFill_Click(object sender, EventArgs e)
        {
            tools = Tools.stationery;
            stationery = Stationery.fill;

            cursor = CursorResourceLoader.LoadEmbeddedCursor(Properties.Resources.fill_cursor);
            pbCanvas.Cursor = cursor;

            noBorder();
            btnFill.FlatAppearance.BorderSize = 1;

            comboSize.Items.Clear();
        }

        private void btnMarker_Click(object sender, EventArgs e)
        {
            tools = Tools.stationery;
            stationery = Stationery.transBrush;

            cursor = CursorResourceLoader.LoadEmbeddedCursor(Properties.Resources.marker_cursor);
            pbCanvas.Cursor = cursor;

            noBorder();
            btnMarker.FlatAppearance.BorderSize = 1;

            comboSize.Items.Clear();
            comboSize.Items.Add("10"); comboSize.Items.Add("20"); comboSize.Items.Add("30");
            comboSize.Items.Add("40"); comboSize.Items.Add("50");
            comboSize.SelectedIndex = 1;
        }
        #endregion

        #region Shapes
        private void btnLine_Click(object sender, EventArgs e)
        {
            tools = Tools.shapes;
            shapes = Shapes.line;
            cursor = Cursors.Cross;
            pbCanvas.Cursor = cursor;

            noBorder();
            btnLine.BackColor = Color.PowderBlue;

            comboSize.Items.Clear();
            comboSize.Items.Add("1"); comboSize.Items.Add("3"); comboSize.Items.Add("5");
            comboSize.SelectedIndex = 1;
        }

        private void btnRectangle_Click(object sender, EventArgs e)
        {
            tools = Tools.shapes;
            shapes = Shapes.rectangle;
            cursor = Cursors.Cross;
            pbCanvas.Cursor = cursor;


            noBorder();
            btnRectangle.BackColor = Color.PowderBlue;

            comboSize.Items.Clear();
            comboSize.Items.Add("1"); comboSize.Items.Add("3"); comboSize.Items.Add("5");
            comboSize.SelectedIndex = 1;
        }

        private void btnCircle_Click(object sender, EventArgs e)
        {
            tools = Tools.shapes;
            shapes = Shapes.circle;
            cursor = Cursors.Cross;
            pbCanvas.Cursor = cursor;

            noBorder();
            btnCircle.BackColor = Color.PowderBlue;

            comboSize.Items.Clear();
            comboSize.Items.Add("1"); comboSize.Items.Add("3"); comboSize.Items.Add("5");
            comboSize.SelectedIndex = 1;
        }

        private void btnPolygon_Click(object sender, EventArgs e)
        {
            tools = Tools.shapes;
            shapes = Shapes.polygon;
            cursor = Cursors.Cross;
            pbCanvas.Cursor = cursor;

            noBorder();
            btnPolygon.BackColor = Color.PowderBlue;

            comboSize.Items.Clear();
            comboSize.Items.Add("1"); comboSize.Items.Add("3"); comboSize.Items.Add("5");
            comboSize.SelectedIndex = 1;
        }

        private void btnTriangle_Click(object sender, EventArgs e)
        {
            tools = Tools.shapes;
            shapes = Shapes.triangle;
            cursor = Cursors.Cross;
            pbCanvas.Cursor = cursor;

            noBorder();
            btnTriangle.BackColor = Color.PowderBlue;

            comboSize.Items.Clear();
            comboSize.Items.Add("1"); comboSize.Items.Add("3"); comboSize.Items.Add("5");
            comboSize.SelectedIndex = 1;
        }

        private void btnDiamond_Click(object sender, EventArgs e)
        {
            tools = Tools.shapes;
            shapes = Shapes.diamond;
            cursor = Cursors.Cross;
            pbCanvas.Cursor = cursor;

            noBorder();
            btnDiamond.BackColor = Color.PowderBlue;

            comboSize.Items.Clear();
            comboSize.Items.Add("1"); comboSize.Items.Add("3"); comboSize.Items.Add("5");
            comboSize.SelectedIndex = 1;
        }

        private void btnHexagon_Click(object sender, EventArgs e)
        {
            tools = Tools.shapes;
            shapes = Shapes.hexagon;
            cursor = Cursors.Cross;
            pbCanvas.Cursor = cursor;

            noBorder();
            btnHexagon.BackColor = Color.PowderBlue;

            comboSize.Items.Clear();
            comboSize.Items.Add("1"); comboSize.Items.Add("3"); comboSize.Items.Add("5");
            comboSize.SelectedIndex = 1;
        }

        private void btnRightTriangle_Click(object sender, EventArgs e)
        {
            tools = Tools.shapes;
            shapes = Shapes.rightTriangle;
            cursor = Cursors.Cross;
            pbCanvas.Cursor = cursor;

            noBorder();
            btnRightTriangle.BackColor = Color.PowderBlue;

            comboSize.Items.Clear();
            comboSize.Items.Add("1"); comboSize.Items.Add("3"); comboSize.Items.Add("5");
            comboSize.SelectedIndex = 1;
        }

        private void btnArrowUp_Click(object sender, EventArgs e)
        {
            tools = Tools.shapes;
            shapes = Shapes.arrowUp;
            cursor = Cursors.Cross;
            pbCanvas.Cursor = cursor;

            noBorder();
            btnArrowUp.BackColor = Color.PowderBlue;

            comboSize.Items.Clear();
            comboSize.Items.Add("1"); comboSize.Items.Add("3"); comboSize.Items.Add("5");
            comboSize.SelectedIndex = 1;
        }

        private void btnArrowDown_Click(object sender, EventArgs e)
        {
            tools = Tools.shapes;
            shapes = Shapes.arrowDown;
            cursor = Cursors.Cross;
            pbCanvas.Cursor = cursor;

            noBorder();
            btnArrowDown.BackColor = Color.PowderBlue;

            comboSize.Items.Clear();
            comboSize.Items.Add("1"); comboSize.Items.Add("3"); comboSize.Items.Add("5");
            comboSize.SelectedIndex = 1;

        }

        private void btnArrowLeft_Click(object sender, EventArgs e)
        {
            tools = Tools.shapes;
            shapes = Shapes.arrowLeft;
            cursor = Cursors.Cross;
            pbCanvas.Cursor = cursor;

            noBorder();
            btnArrowLeft.BackColor = Color.PowderBlue;

            comboSize.Items.Clear();
            comboSize.Items.Add("1"); comboSize.Items.Add("3"); comboSize.Items.Add("5");
            comboSize.SelectedIndex = 1;
        }

        private void btnArrowRight_Click(object sender, EventArgs e)
        {
            tools = Tools.shapes;
            shapes = Shapes.arrowRight;
            cursor = Cursors.Cross;
            pbCanvas.Cursor = cursor;

            noBorder();
            btnArrowRight.BackColor = Color.PowderBlue;

            comboSize.Items.Clear();
            comboSize.Items.Add("1"); comboSize.Items.Add("3"); comboSize.Items.Add("5");
            comboSize.SelectedIndex = 1;
        }

        #endregion

        #region Secondary Tools
        private void btnDefault_Click(object sender, EventArgs e)
        {
            tools = Tools.none;
            noBorder();
            cursor = Cursors.Default;
            pbCanvas.Invalidate();
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            tools = Tools.pointer;

            noBorder();
            btnSelect.FlatAppearance.BorderSize = 1;
            cursor = Cursors.Cross;
            pbCanvas.Cursor = cursor;
        }

        private void btnCrop_Click(object sender, EventArgs e)
        {
            if (cropRectangle.Width != 0 && cropRectangle.Height != 0)
            {

                Bitmap cropBitMap = new Bitmap(pbCanvas.Image).Clone(cropRectangle, mainBmp.PixelFormat);
                if (pbCanvas.Image != null) pbCanvas.Image.Dispose();

                if (!isZoomed)
                {
                    mainBmp = new Bitmap(pbCanvas.Width, pbCanvas.Height);
                    pbCanvas.Image = mainBmp;
                }
                else
                    pbCanvas.Image = new Bitmap(pbCanvas.Width, pbCanvas.Height);

                using (Graphics g = Graphics.FromImage(pbCanvas.Image))
                {
                    g.DrawImage(cropBitMap, cropRectangle.X, cropRectangle.Y);

                    cropBitMap.Dispose();
                }


                if (isZoomed)
                {
                    cropRectangle.X = (int)((float)cropRectangle.X * (1 / zoomFactor));
                    cropRectangle.Y = (int)((float)cropRectangle.Y * (1 / zoomFactor));
                    cropRectangle.Width *= (int)(1 / zoomFactor);
                    cropRectangle.Height *= (int)(1 / zoomFactor);
                    Size s = mainBmp.Size;

                    Bitmap crop = new Bitmap(mainBmp).Clone(cropRectangle, mainBmp.PixelFormat);
                    if (mainBmp != null) mainBmp.Dispose();
                    mainBmp = new Bitmap(s.Width, s.Height);

                    using (Graphics g = Graphics.FromImage(mainBmp))
                    {
                        g.DrawImage(crop, cropRectangle.X, cropRectangle.Y);

                        crop.Dispose();
                    }

                }

                pbCanvas.Refresh();
                undoRedo.InsertUndoBitmapData(ImageHandler.StoreCurrentBitmap(mainBmp));
                cropRectangle.Width = 0;
                cropRectangle.Height = 0;
            }

        }

        private void btnDeleteArea_Click(object sender, EventArgs e)
        {
            if (cropRectangle.Width != 0 && cropRectangle.Height != 0)
            {
                using (Graphics g = Graphics.FromImage(pbCanvas.Image))
                {
                    g.FillRectangle(new SolidBrush(pbCanvas.BackColor), cropRectangle);
                }
                if (isZoomed)
                {
                    cropRectangle.X = (int)((float)cropRectangle.X * (1 / zoomFactor));
                    cropRectangle.Y = (int)((float)cropRectangle.Y * (1 / zoomFactor));
                    cropRectangle.Width *= (int)(1 / zoomFactor);
                    cropRectangle.Height *= (int)(1 / zoomFactor);

                    using (Graphics g = Graphics.FromImage(mainBmp))
                    {
                        g.FillRectangle(new SolidBrush(pbCanvas.BackColor), cropRectangle);
                    }
                }
                undoRedo.InsertUndoBitmapData(ImageHandler.StoreCurrentBitmap(mainBmp));
            }
            pbCanvas.Refresh();
            cropRectangle.Width = 0;
            cropRectangle.Height = 0;
        }

        private void btnColorPicker_Click(object sender, EventArgs e)
        {
            tools = Tools.colorpicker;

            cursor = CursorResourceLoader.LoadEmbeddedCursor(Properties.Resources.colorpicker_cursor);
            pbCanvas.Cursor = cursor;

            noBorder();
            btnColorPicker.FlatAppearance.BorderSize = 1;
        }

        private void btnText_Click(object sender, EventArgs e)
        {
            if (text != null) text.ShowDialog();
            else
            {
                text = new AddText();
                text.ShowDialog();
            }

            if (text.DialogResult == DialogResult.OK)
            {
                if (pbCanvas.Image == null)
                {
                    mainBmp = new Bitmap(pbCanvas.Width, pbCanvas.Height);
                    pbCanvas.Image = mainBmp;
                }
                tools = Tools.text;
                noBorder();
                btnText.FlatAppearance.BorderSize = 1;
                cursor = Cursors.Cross;

                font = new Font(text.fontFamily, text.size, text.fontStyle);
                textColor = text.color;
                insertText = text.GetText();

            }
        }

        #endregion

        #endregion

        #region Image Processing

        #region Rotate/flip

        private void rotate90_Click(object sender, EventArgs e)
        {
            ImageHandler.RotateFlip(mainBmp, pbCanvas.Image, RotateFlipType.Rotate90FlipNone, isZoomed);

            pbCanvas.Size = new Size(pbCanvas.Image.Width, pbCanvas.Image.Height);
            //undoRedo.InsertUndoBitmapData(ImageHandler.StoreCurrentBitmap(mainBmp));
            pbCanvas.Refresh();
        }

        private void rotate180_Click(object sender, EventArgs e)
        {
            ImageHandler.RotateFlip(mainBmp, pbCanvas.Image, RotateFlipType.Rotate180FlipNone, isZoomed);

            undoRedo.InsertUndoBitmapData(ImageHandler.StoreCurrentBitmap(mainBmp));
            pbCanvas.Refresh();
        }

        private void rotate270_Click(object sender, EventArgs e)
        {
            ImageHandler.RotateFlip(mainBmp, pbCanvas.Image, RotateFlipType.Rotate270FlipNone, isZoomed);

            pbCanvas.Size = new Size(pbCanvas.Image.Width, pbCanvas.Image.Height);
            //undoRedo.InsertUndoBitmapData(ImageHandler.StoreCurrentBitmap(mainBmp));
            pbCanvas.Refresh();
        }

        private void flipHorizontalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ImageHandler.RotateFlip(mainBmp, pbCanvas.Image, RotateFlipType.RotateNoneFlipX, isZoomed);

            undoRedo.InsertUndoBitmapData(ImageHandler.StoreCurrentBitmap(mainBmp));
            pbCanvas.Refresh();
        }

        private void flipVerticalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ImageHandler.RotateFlip(mainBmp, pbCanvas.Image, RotateFlipType.RotateNoneFlipY, isZoomed);

            undoRedo.InsertUndoBitmapData(ImageHandler.StoreCurrentBitmap(mainBmp));
            pbCanvas.Refresh();
        }

        #endregion

        #region Zooming
        private void zoom50_Click(object sender, EventArgs e)
        {
            isZoomed = true;
            zoomFactor = 0.5f;

            pbCanvas.Image = ImageHandler.PictureBoxZoom(mainBmp, zoomFactor);
            pbCanvas.Size = pbCanvas.Image.Size;

            Zoom.Text = "Zoom: 50%";
            ZoomBar.CurrentTickPosition = 2;
            ClearChecked();
            zoom50.Checked = true;
        }

        private void zoom100_Click(object sender, EventArgs e)
        {
            isZoomed = false;
            zoomFactor = 1;

            pbCanvas.Image = mainBmp;
            pbCanvas.Size = pbCanvas.Image.Size;

            Zoom.Text = "Zoom: 100%";
            ZoomBar.CurrentTickPosition = 3;
            ClearChecked();
            zoom100.Checked = true;
        }

        private void zoom150_Click(object sender, EventArgs e)
        {
            isZoomed = true;
            zoomFactor = 1.5f;

            pbCanvas.Image = ImageHandler.PictureBoxZoom(mainBmp, zoomFactor);
            pbCanvas.Size = pbCanvas.Image.Size;

            Zoom.Text = "Zoom: 150%";
            ZoomBar.CurrentTickPosition = 4;
            ClearChecked();
            zoom150.Checked = true;
        }

        private void zoom200_Click(object sender, EventArgs e)
        {
            isZoomed = true;
            zoomFactor = 2f;

            pbCanvas.Image = ImageHandler.PictureBoxZoom(mainBmp, zoomFactor);
            pbCanvas.Size = pbCanvas.Image.Size;

            Zoom.Text = "Zoom: 200%";
            ZoomBar.CurrentTickPosition = 5;
            ClearChecked();
            zoom200.Checked = true;
        }

        private void zoom300_Click(object sender, EventArgs e)
        {
            isZoomed = true;
            zoomFactor = 3f;

            pbCanvas.Image = ImageHandler.PictureBoxZoom(mainBmp, zoomFactor);
            pbCanvas.Size = pbCanvas.Image.Size;

            Zoom.Text = "Zoom: 300%";
            ZoomBar.CurrentTickPosition = 6;
            ClearChecked();
            zoom300.Checked = true;
        }

        private void ZoomBar_Scroll(object sender, EventArgs e)
        {
            ClearChecked();

            zoomFactor = ImageHandler.ZoomHandler(ZoomBar.CurrentTickPosition).Item1;
            Zoom.Text = ImageHandler.ZoomHandler(ZoomBar.CurrentTickPosition).Item2;
            if (zoomFactor == 0.5f) zoom50.Checked = true;
            else if (zoomFactor == 1f) zoom100.Checked = true;
            else if (zoomFactor == 1.5f) zoom150.Checked = true;
            else if (zoomFactor == 2f) zoom200.Checked = true;
            else if (zoomFactor == 3) zoom300.Checked = true;

            if (zoomFactor == 1f)
            {
                isZoomed = false;
                pbCanvas.Image = mainBmp;
            }
            else
            {
                isZoomed = true;
                pbCanvas.Image = ImageHandler.PictureBoxZoom(mainBmp, zoomFactor);
            }

            pbCanvas.Size = pbCanvas.Image.Size;
        }

        private void ClearChecked()
        {
            zoom50.Checked = false;
            zoom100.Checked = false;
            zoom150.Checked = false;
            zoom200.Checked = false;
            zoom300.Checked = false;
        }

        #endregion

        private async void resizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ResizeImage resize = new ResizeImage(mainBmp.Size);

            if (resize.ShowDialog() == DialogResult.OK)
            {
                WaitScreen ws = new WaitScreen();
                ws.Show();
                using (Bitmap temp = new Bitmap(mainBmp))
                {
                    mainBmp.Dispose();
                    Image img = await Task.Run(() => ImageHandler.PictureResize(temp, resize.size));

                    mainBmp = new Bitmap(img);

                }

                if (isZoomed)
                {
                    pbCanvas.Image = ImageHandler.PictureBoxZoom(mainBmp, zoomFactor);
                    pbCanvas.Size = new Size(pbCanvas.Image.Width, pbCanvas.Image.Height);
                }
                else
                {
                    pbCanvas.Image = mainBmp;
                    pbCanvas.Size = new Size(mainBmp.Width, mainBmp.Height);
                }

                undoRedo.DeleteUndoRedoBitmapData();
                undoRedo.InsertUndoBitmapData(ImageHandler.StoreCurrentBitmap(mainBmp));

                ws.Dispose();

                saved = false;

            }
        }

        private async void btnInvert_Click(object sender, EventArgs e)
        {
            if (mainBmp != null)
            {
                WaitScreen ws = new WaitScreen();
                ws.Show();

                await Task.Run(() => ImageHandler.SetInvert(mainBmp));

                if (isZoomed)
                {
                    pbCanvas.Image = ImageHandler.PictureBoxZoom(mainBmp, zoomFactor);
                }
                else
                {
                    pbCanvas.Image = mainBmp;
                }

                undoRedo.InsertUndoBitmapData(ImageHandler.StoreCurrentBitmap(mainBmp));
                ws.Dispose();
            }

        }

        private async void btnGrayScale_Click(object sender, EventArgs e)
        {
            if (mainBmp != null)
            {
                WaitScreen ws = new WaitScreen();
                ws.Show();

                await Task.Run(() => ImageHandler.SetGrayscale(mainBmp));

                if (isZoomed)
                {
                    pbCanvas.Image = ImageHandler.PictureBoxZoom(mainBmp, zoomFactor);
                }
                else
                {
                    pbCanvas.Image = mainBmp;
                }
                undoRedo.InsertUndoBitmapData(ImageHandler.StoreCurrentBitmap(mainBmp));
                ws.Dispose();
            }
        }

        private void btnColorFilter_Click(object sender, EventArgs e)
        {
            if (ColorFilterWindow.Visible == true) ColorFilterWindow.Visible = false;
            else ColorFilterWindow.Visible = true;
        }

        private async void btnGamma_Click(object sender, EventArgs e)
        {
            if (mainBmp != null)
            {
                GammaForm gm = new GammaForm();
                if (gm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    float red = gm.red;
                    float green = gm.green;
                    float blue = gm.blue;
                    gm.Dispose();
                    WaitScreen ws = new WaitScreen();
                    ws.Show();

                    await Task.Run(() => ImageHandler.SetGamma(mainBmp, red, green, blue));

                    if (isZoomed)
                    {
                        pbCanvas.Image = ImageHandler.PictureBoxZoom(mainBmp, zoomFactor);
                    }
                    else
                    {
                        pbCanvas.Image = mainBmp;
                    }
                    undoRedo.InsertUndoBitmapData(ImageHandler.StoreCurrentBitmap(mainBmp));
                    ws.Dispose();
                }
            }
        }

        private async void btnBrightness_Click(object sender, EventArgs e)
        {
            if (mainBmp != null)
            {
                BrightnessForm bm = new BrightnessForm();
                if (bm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    float bright = bm.brightValue;
                    bm.Dispose();
                    WaitScreen ws = new WaitScreen();
                    ws.Show();

                    await Task.Run(() => ImageHandler.SetBrightness(mainBmp, (int)bright));

                    if (isZoomed)
                    {
                        pbCanvas.Image = ImageHandler.PictureBoxZoom(mainBmp, zoomFactor);
                    }
                    else
                    {
                        pbCanvas.Image = mainBmp;
                    }
                    undoRedo.InsertUndoBitmapData(ImageHandler.StoreCurrentBitmap(mainBmp));
                    ws.Dispose();
                }
            }
        }

        private async void btnFilterRed_Click(object sender, EventArgs e)
        {
            if (mainBmp != null)
            {
                WaitScreen ws = new WaitScreen();
                ws.Show();

                await Task.Run(() => ImageHandler.SetColorFilter(mainBmp, ImageHandler.ColorFilterTypes.Red));

                if (isZoomed)
                {
                    pbCanvas.Image = ImageHandler.PictureBoxZoom(mainBmp, zoomFactor);
                }
                else
                {
                    pbCanvas.Image = mainBmp;
                }
                undoRedo.InsertUndoBitmapData(ImageHandler.StoreCurrentBitmap(mainBmp));
                ws.Dispose();
            }
            ColorFilterWindow.Visible = false;
        }

        private async void btnFilterGreen_Click(object sender, EventArgs e)
        {
            if (mainBmp != null)
            {
                WaitScreen ws = new WaitScreen();
                ws.Show();

                await Task.Run(() => ImageHandler.SetColorFilter(mainBmp, ImageHandler.ColorFilterTypes.Green));

                if (isZoomed)
                {
                    pbCanvas.Image = ImageHandler.PictureBoxZoom(mainBmp, zoomFactor);
                }
                else
                {
                    pbCanvas.Image = mainBmp;
                }
                undoRedo.InsertUndoBitmapData(ImageHandler.StoreCurrentBitmap(mainBmp));
                ws.Dispose();
            }
            ColorFilterWindow.Visible = false;
        }

        private async void btnFilterBlue_Click(object sender, EventArgs e)
        {
            if (mainBmp != null)
            {
                WaitScreen ws = new WaitScreen();
                ws.Show();

                await Task.Run(() => ImageHandler.SetColorFilter(mainBmp, ImageHandler.ColorFilterTypes.Blue));

                if (isZoomed)
                {
                    pbCanvas.Image = ImageHandler.PictureBoxZoom(mainBmp, zoomFactor);
                }
                else
                {
                    pbCanvas.Image = mainBmp;
                }
                undoRedo.InsertUndoBitmapData(ImageHandler.StoreCurrentBitmap(mainBmp));
                ws.Dispose();
            }
            ColorFilterWindow.Visible = false;
        }

        private async void btnContrast_Click(object sender, EventArgs e)
        {
            if (mainBmp != null)
            {
                ContrastForm cm = new ContrastForm();
                if (cm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    double contrast = cm.contrastValue;
                    cm.Dispose();
                    WaitScreen ws = new WaitScreen();
                    ws.Show();

                    await Task.Run(() => ImageHandler.SetContrast(mainBmp, contrast));

                    if (isZoomed)
                    {
                        pbCanvas.Image = ImageHandler.PictureBoxZoom(mainBmp, zoomFactor);
                    }
                    else
                    {
                        pbCanvas.Image = mainBmp;
                    }
                    undoRedo.InsertUndoBitmapData(ImageHandler.StoreCurrentBitmap(mainBmp));
                    ws.Dispose();
                }
            }
        }

        #endregion
    }
}
