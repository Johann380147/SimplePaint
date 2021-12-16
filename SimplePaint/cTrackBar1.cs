using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace SimplePaint
{
    public partial class cTrackBar : Control
    {
        private int numberTicks = 10;
        private Rectangle trackRectangle = new Rectangle();
        private Rectangle ticksRectangle = new Rectangle();
        private Rectangle thumbRectangle = new Rectangle();
        private int currentTickPosition = 3;
        private float tickSpace = 10;
        private bool thumbClicked = false;
        private TrackBarThumbState thumbState =
            TrackBarThumbState.Normal;
        
        public int CurrentTickPosition
        {
            get { return currentTickPosition; }
            set 
            {
                currentTickPosition = value;

                thumbRectangle.X = CurrentTickXCoordinate();
                Invalidate();
            }
        }
        public event EventHandler Scroll;
        public event EventHandler ValueChanged;

        public cTrackBar()
        {
            this.currentTickPosition = 3;
            this.Location = new Point(870, 0);
            this.Size = new Size(100, 28);
            this.numberTicks = 8;
            this.BackColor = Color.FromArgb(64, 64, 64);
            this.DoubleBuffered = true;
            // Calculate the initial sizes of the bar,  
            // thumb and ticks.
            SetupTrackBar();
        }

        // Calculate the sizes of the bar, thumb, and ticks rectangle. 
        private void SetupTrackBar()
        {
            if (!TrackBarRenderer.IsSupported)
                return;

            using (Graphics g = this.CreateGraphics())
            {
                // Calculate the size of the track bar.
                trackRectangle.X = ClientRectangle.X + 2;
                trackRectangle.Y = ClientRectangle.Y + 8;
                trackRectangle.Width = ClientRectangle.Width - 4;
                trackRectangle.Height = 4;

                // Calculate the size of the rectangle in which to  
                // draw the ticks.
                ticksRectangle.X = trackRectangle.X + 4;
                ticksRectangle.Y = trackRectangle.Y + 14;
                ticksRectangle.Width = trackRectangle.Width - 8;
                ticksRectangle.Height = 4;

                tickSpace = ((float)ticksRectangle.Width - 1) /
                    ((float)numberTicks - 1);

                // Calculate the size of the thumb.
                thumbRectangle.Size =
                    TrackBarRenderer.GetBottomPointingThumbSize(g,
                    TrackBarThumbState.Normal);

                thumbRectangle.X = CurrentTickXCoordinate();
                thumbRectangle.Y = trackRectangle.Y - 6;
            }
        }

        private int CurrentTickXCoordinate()
        {
            if (tickSpace == 0)
            {
                return 0;
            }
            else
            {
                return ((int)Math.Round(tickSpace) *
                    currentTickPosition) + 2;
            }
        }

        // Draw the track bar. 
        protected override void OnPaint(PaintEventArgs e)
        {
            if (!TrackBarRenderer.IsSupported)
            {
                return;
            }
            TrackBarRenderer.DrawHorizontalTrack(e.Graphics,
                trackRectangle);
            TrackBarRenderer.DrawBottomPointingThumb(e.Graphics,
                thumbRectangle, thumbState);
            TrackBarRenderer.DrawHorizontalTicks(e.Graphics,
                ticksRectangle, numberTicks, EdgeStyle.Sunken);
        }

        // Determine whether the user has clicked the track bar thumb. 
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (!TrackBarRenderer.IsSupported)
                return;

            if (this.thumbRectangle.Contains(e.Location))
            {
                thumbClicked = true;
                thumbState = TrackBarThumbState.Pressed;
            }

            if(this.ClientRectangle.Contains(e.Location))
            {
                if(e.Location.X < thumbRectangle.X)
                {
                    if (!(currentTickPosition == 0))
                        CurrentTickPosition--;
                    ValueChanged(this, e);
                }

                else if(e.Location.X > thumbRectangle.Right)
                {
                    if(!(currentTickPosition == 7))
                        CurrentTickPosition++;
                    ValueChanged(this, e);
                }
            }

            

            this.Invalidate();
        }

        // Redraw the track bar thumb if the user has moved it. 
        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (!TrackBarRenderer.IsSupported)
                return;

            if (thumbClicked == true)
            {
                if (e.Location.X > trackRectangle.X &&
                    e.Location.X < (trackRectangle.X +
                    trackRectangle.Width - thumbRectangle.Width))
                {
                    thumbClicked = false;
                    thumbState = TrackBarThumbState.Hot;
                    this.Invalidate();
                }

                thumbClicked = false;
            }
        }

        // Track cursor movements. 
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (!TrackBarRenderer.IsSupported)
                return;

            // The user is moving the thumb. 
            if (thumbClicked == true)
            {
                // Track movements to the next tick to the right, if  
                // the cursor has moved halfway to the next tick. 
                if (currentTickPosition < numberTicks - 1 &&
                    e.Location.X > CurrentTickXCoordinate() +
                    (int)(tickSpace))
                {
                    currentTickPosition++;
                    OnScroll(this, e);
                }

                // Track movements to the next tick to the left, if  
                // cursor has moved halfway to the next tick. 
                else if (currentTickPosition > 0 &&
                    e.Location.X < CurrentTickXCoordinate() -
                    (int)(tickSpace / 2))
                {
                    currentTickPosition--;
                    OnScroll(this, e);
                }

                thumbRectangle.X = CurrentTickXCoordinate();
            }

            // The cursor is passing over the track. 
            else
            {
                thumbState = thumbRectangle.Contains(e.Location) ?
                    TrackBarThumbState.Hot : TrackBarThumbState.Normal;
            }

            Invalidate();
        }

        protected virtual void OnScroll(object sender, EventArgs e)
        {
            EventHandler handler = this.Scroll;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnValueChanged(object sender, EventArgs e)
        {
            EventHandler handler = this.ValueChanged;

            if (handler != null)
            {
                handler(this, e);
            }
        }

    }



}
