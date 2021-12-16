﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace SimplePaint
{
    internal class ControlMoverOrResizer
    {
        private  bool _moving;
        private  Point _cursorStartPoint;
        private  bool _moveIsInterNal;
        public bool _resizing { get; private set; }
        private  Size _currentControlStartSize;
        private  bool MouseIsInLeftEdge { get; set; }
        private bool MouseIsInRightEdge { get; set; }
        private bool MouseIsInTopEdge { get; set; }
        private bool MouseIsInBottomEdge { get; set; }
        private FormPaint form;

        private enum MoveOrResize
        {
            Move,
            Resize,
            MoveAndResize
        }

        public ControlMoverOrResizer(FormPaint form)
        {
            this.form = form;
        }

        private MoveOrResize WorkType { get; set; }

        public  void Init(Control control)
        {
            Init(control, control);
        }

        public void Init(Control control, Control container)
        {
            _moving = false;
            _resizing = false;
            _moveIsInterNal = false;
            _cursorStartPoint = Point.Empty;
            MouseIsInLeftEdge = false;
            MouseIsInLeftEdge = false;
            MouseIsInRightEdge = false;
            MouseIsInTopEdge = false;
            MouseIsInBottomEdge = false;
            WorkType = MoveOrResize.Resize;
            control.MouseDown += (sender, e) => StartMovingOrResizing(control, e);
            control.MouseUp += (sender, e) => StopDragOrResizing(control);
            control.MouseMove += (sender, e) => MoveControl(container, e);
        }

        private  void UpdateMouseEdgeProperties(Control control, Point mouseLocationInControl)
        {
            if (WorkType == MoveOrResize.Move)
            {
                return;
            }
            MouseIsInLeftEdge = false;//Math.Abs(mouseLocationInControl.X) <= 5;
            MouseIsInRightEdge = Math.Abs(mouseLocationInControl.X - control.Width) <= 5;
            MouseIsInTopEdge = false;//Math.Abs(mouseLocationInControl.Y) <= 5;
            MouseIsInBottomEdge = Math.Abs(mouseLocationInControl.Y - control.Height) <= 5;
        }

        private  void UpdateMouseCursor(Control control)
        {
            if (WorkType == MoveOrResize.Move)
            {
                return;
            }
            if (MouseIsInLeftEdge)
            {
                if (MouseIsInTopEdge)
                {
                    control.Cursor = Cursors.SizeNWSE;
                }
                else if (MouseIsInBottomEdge)
                {
                    control.Cursor = Cursors.SizeNESW;
                }
                else
                {
                    control.Cursor = Cursors.SizeWE;
                }
            }
            else if (MouseIsInRightEdge)
            {
                if (MouseIsInTopEdge)
                {
                    control.Cursor = Cursors.SizeNESW;
                }
                else if (MouseIsInBottomEdge)
                {
                    control.Cursor = Cursors.SizeNWSE;
                }
                else
                {
                    control.Cursor = Cursors.SizeWE;
                }
            }
            else if (MouseIsInTopEdge || MouseIsInBottomEdge)
            {
                control.Cursor = Cursors.SizeNS;
            }
            else
            {
                control.Cursor = form.cursor;
            }
        }

        private  void StartMovingOrResizing(Control control, MouseEventArgs e)
        {
            if (_moving || _resizing)
            {
                return;
            }
            if (WorkType!=MoveOrResize.Move &&
                (MouseIsInRightEdge || MouseIsInLeftEdge || MouseIsInTopEdge || MouseIsInBottomEdge))
            {
                _resizing = true;
                _currentControlStartSize = control.Size;
            }
            else if (WorkType!=MoveOrResize.Resize)
            {
                _moving = true;
                control.Cursor = Cursors.Hand;
            }
            _cursorStartPoint = new Point(e.X, e.Y);
            control.Capture = true;
        }

        private  void MoveControl(Control control, MouseEventArgs e)
        {
            if (!_resizing && ! _moving)
            {
                UpdateMouseEdgeProperties(control, new Point(e.X, e.Y));
                UpdateMouseCursor(control);
            }
            if (_resizing)
            {
                if (MouseIsInLeftEdge)
                {
                    if (MouseIsInTopEdge)
                    {
                        if (control.Width >= 10)
                        {
                            control.Width -= (e.X - _cursorStartPoint.X);
                            control.Left += (e.X - _cursorStartPoint.X);
                        }
                        if (control.Height >= 10)
                        {
                            control.Height -= (e.Y - _cursorStartPoint.Y);
                            control.Top += (e.Y - _cursorStartPoint.Y);
                        }
                    }
                    else if (MouseIsInBottomEdge)
                    {
                        if (control.Width >= 10)
                        {
                            control.Width -= (e.X - _cursorStartPoint.X);
                            control.Left += (e.X - _cursorStartPoint.X);
                        }
                        if (control.Height >= 10)
                        control.Height = (e.Y - _cursorStartPoint.Y) + _currentControlStartSize.Height;
                    }
                    else
                    {
                        if (control.Width >= 10)
                        {
                            control.Width -= (e.X - _cursorStartPoint.X);
                            control.Left += (e.X - _cursorStartPoint.X);
                        }
                    }
                }
                else if (MouseIsInRightEdge)
                {
                    if (MouseIsInTopEdge)
                    {
                        if (control.Width >= 10)
                        control.Width = (e.X - _cursorStartPoint.X) + _currentControlStartSize.Width;
                        if (control.Height >= 10)
                        {
                            control.Height -= (e.Y - _cursorStartPoint.Y);
                            control.Top += (e.Y - _cursorStartPoint.Y);
                        }
                    }
                    else if (MouseIsInBottomEdge)
                    {
                        if (control.Width >= 10)
                        control.Width = (e.X - _cursorStartPoint.X) + _currentControlStartSize.Width;
                        if (control.Height >= 10)
                        control.Height = (e.Y - _cursorStartPoint.Y) + _currentControlStartSize.Height;
                    }
                    else
                    {
                        if (control.Width >= 10)
                        control.Width = (e.X - _cursorStartPoint.X)+_currentControlStartSize.Width;
                    }
                }
                else if (MouseIsInTopEdge)
                {
                    if (control.Height >= 10)
                    {
                        control.Height -= (e.Y - _cursorStartPoint.Y);
                        control.Top += (e.Y - _cursorStartPoint.Y);
                    }
                }
                else if (MouseIsInBottomEdge)
                {
                    if (control.Height >= 10)
                        control.Height = (e.Y - _cursorStartPoint.Y) + _currentControlStartSize.Height;
                }
                else
                {
                     StopDragOrResizing(control);
                }
            }
            else if (_moving)
            {
                _moveIsInterNal = !_moveIsInterNal;
                if (!_moveIsInterNal)
                {
                    int x = (e.X - _cursorStartPoint.X) + control.Left;
                    int y = (e.Y - _cursorStartPoint.Y) + control.Top;
                    control.Location = new Point(x, y);
                }
            }
        }

        private  void StopDragOrResizing(Control control)
        {
            _resizing = false;
            _moving = false;
            control.Capture = false;
            UpdateMouseCursor(control);
        }

        #region Save And Load

        private  List<Control> GetAllChildControls(Control control, List<Control> list)
        {
            List<Control> controls = control.Controls.Cast<Control>().ToList();
            list.AddRange(controls);
            return controls.SelectMany(ctrl => GetAllChildControls(ctrl, list)).ToList();
        }

        private string GetSizeAndPositionOfControlsToString(Control container)
        {
            List<Control> controls = new List<Control>();
            GetAllChildControls(container, controls);
            CultureInfo cultureInfo = new CultureInfo("en");
            string info = string.Empty;
            foreach (Control control in controls)
            {
                info += control.Name + ":" + control.Left.ToString(cultureInfo) + "," + control.Top.ToString(cultureInfo) + "," +
                        control.Width.ToString(cultureInfo) + "," + control.Height.ToString(cultureInfo) + "*";
            }
            return info;
        }
        private void SetSizeAndPositionOfControlsFromString(Control container, string controlsInfoStr)
        {
            List<Control> controls = new List<Control>();
            GetAllChildControls(container, controls);
            string[] controlsInfo = controlsInfoStr.Split(new []{"*"},StringSplitOptions.RemoveEmptyEntries );
            Dictionary<string, string> controlsInfoDictionary = new Dictionary<string, string>();
            foreach (string controlInfo in controlsInfo)
            {
                string[] info = controlInfo.Split(new [] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                controlsInfoDictionary.Add(info[0], info[1]);
            }
            foreach (Control control in controls)
            {
                string propertiesStr;
                controlsInfoDictionary.TryGetValue(control.Name, out propertiesStr);
                string[] properties = propertiesStr.Split(new [] { "," }, StringSplitOptions.RemoveEmptyEntries);
                if (properties.Length == 4)
                {
                    control.Left = int.Parse(properties[0]);
                    control.Top = int.Parse(properties[1]);
                    control.Width = int.Parse(properties[2]);
                    control.Height = int.Parse(properties[3]);
                }
            }
        }

        #endregion
    }
}