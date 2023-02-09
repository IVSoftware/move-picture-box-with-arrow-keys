﻿using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace move_picture_box_with_arrow_keys
{
    public partial class MainForm : Form , IMessageFilter
    {
        public MainForm()
        {
            InitializeComponent();
            Application.AddMessageFilter(this);
            Disposed += (sender, e) =>Application.RemoveMessageFilter(this);
            ArrowKeyPictureBox.CollisionDetected += onAnyCollisionDetected;
        }

        private void onAnyCollisionDetected(object sender, CollisionDetectedEventArgs e)
        {
        }

        const int WM_KEYDOWN = 0x0100;
        public bool PreFilterMessage(ref Message m)
        {
            base.WndProc(ref m);
            switch (m.Msg)
            {
                case WM_KEYDOWN:
                    foreach (var target in Controls.OfType<ArrowKeyPictureBox>())
                    {
                        target.MoveProgrammatically((Keys)m.WParam);
                    }
                    break;
            }
            return false;
        }
    }
    class ArrowKeyPictureBox : PictureBox
    {
        const int INCREMENT = 1;
        public void MoveProgrammatically(Keys direction)
        {
            Rectangle preview = Bounds;
            if (IsMoveTarget)
            {
                BringToFront();
                switch (direction)
                {
                    case Keys.Up:
                        if (Top <= 0)
                        {
                            return;
                        }
                        preview.Y -= INCREMENT;
                        if(detectCollision())
                        {
                            return;
                        }
                        Top -= INCREMENT;
                        break;
                    case Keys.Down:
                        if (Bottom >= Parent.Bottom)
                        {
                            return;
                        }
                        if (detectCollision())
                        {
                            return;
                        }
                        Top += INCREMENT;
                        break;
                    case Keys.Left:
                        if (Left <= 0)
                        {
                            return;
                        }
                        if (detectCollision())
                        {
                            return;
                        }
                        Left -= INCREMENT;
                        break;
                    case Keys.Right:
                        if (Right >= Parent.Right)
                        {
                            return;
                        }
                        if (detectCollision())
                        {
                            return;
                        }
                        Left += INCREMENT;
                        break;
                }
            }
            bool detectCollision()
            {
                foreach (Control control in Parent.Controls)
                {
                    if(!ReferenceEquals(this, control))
                    {
                        if(preview.IntersectsWith(control.Bounds))
                        {
                            CollisionDetected?.Invoke(
                                this, 
                                new CollisionDetectedEventArgs(control: control));
                            return true;
                        }
                    }
                }
                return false;
            }
        }
        public static event CollisionDetectedEventHandler CollisionDetected;
        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            foreach (var control in Parent.Controls.OfType<ArrowKeyPictureBox>()) 
            { 
                if(!ReferenceEquals(control, this)) control.IsMoveTarget= false;
            }
            IsMoveTarget= true;
        }
        bool _isMoveTarget = false;

        public bool IsMoveTarget
        {
            get => _isMoveTarget;
            set
            {
                if (!Equals(_isMoveTarget, value))
                {
                    _isMoveTarget = value;
                    BorderStyle = IsMoveTarget ?
                        BorderStyle.FixedSingle :
                        BorderStyle.None;
                }
            }
        }
    }

    delegate void CollisionDetectedEventHandler(Object sender, CollisionDetectedEventArgs e);
    class CollisionDetectedEventArgs : EventArgs
    {
        public CollisionDetectedEventArgs(Control control)
        {
            Control = control;
        }
        public Control Control { get; }
    }
}
