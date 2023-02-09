using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace move_picture_box_with_arrow_keys
{
    public partial class MainForm : Form , IMessageFilter
    {
        public MainForm()
        {
            InitializeComponent();
            Application.AddMessageFilter(this);
            Disposed += (sender, e) => Application.RemoveMessageFilter(this);
            ArrowKeyPictureBox.CollisionDetected += onAnyCollisionDetected;
        }

        const int WM_KEYDOWN = 0x0100;
        public bool PreFilterMessage(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_KEYDOWN:
                    if (Controls.OfType<TextBoxBase>().Any(_ => _.Focused))
                    {   /* G T K */
                    }
                    else
                    {
                        Keys key = (Keys)m.WParam;
                        switch (key)
                        {
                            case Keys.Up:
                            case Keys.Down:
                            case Keys.Left:
                            case Keys.Right:
                                BeginInvoke(new Action(() =>
                                {
                                    foreach (var target in Controls.OfType<ArrowKeyPictureBox>())
                                    {
                                        target.MoveProgrammatically(key);
                                    }
                                }));
                                break;
                        }
                    }
                    break;
            }
            return false;
        }
        private void onAnyCollisionDetected(object sender, CollisionDetectedEventArgs e)
        {
            e.Cancel = !e.Control.Name.Contains("Portal");
            if (sender is Control control)
            {
                if (!_prevWarning.Equals(control.Bounds))
                {
                    richTextBox.SelectionColor = e.Cancel ? Color.Green : Color.Red;
                    richTextBox.Font = new Font(richTextBox.Font, FontStyle.Bold);
                    richTextBox.AppendText($"{Environment.NewLine}Collision{Environment.NewLine}");
                    richTextBox.Font = new Font(richTextBox.Font, FontStyle.Regular);
                    richTextBox.SelectionColor = Color.Chocolate;

                    List<string> builder = new List<string>();
                    builder.Add($"Moving: {control.Name}");
                    builder.Add( $"@ {control.Bounds}");
                    builder.Add($"Collided with: {e.Control.Name}");
                    builder.Add($"@ {e.Control.Bounds}");
                    richTextBox.AppendText($"{string.Join(Environment.NewLine, builder)}{Environment.NewLine}");
                    _prevWarning = control.Bounds;
                }
            }
        }
        Rectangle _prevWarning = new Rectangle();
    }
    class ArrowKeyPictureBox : PictureBox
    {
        const int INCREMENT = 1;
        public void MoveProgrammatically(Keys direction)
        {
            Rectangle preview = Bounds;
            if (IsCurrentMoveTarget)
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
                            var e = new CollisionDetectedEventArgs(control: control);
                            CollisionDetected?.Invoke(this, e);
                            return !e.Cancel;
                        }
                    }
                }
                return false;
            }
        }
        public static event CollisionDetectedEventHandler CollisionDetected;
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            Focus();
            foreach (var control in Parent.Controls.OfType<ArrowKeyPictureBox>())
            {
                if (!ReferenceEquals(control, this)) control.IsCurrentMoveTarget = false;
            }
            IsCurrentMoveTarget = true;
        }
        bool _isMoveTarget = false;

        public bool IsCurrentMoveTarget
        {
            get => _isMoveTarget;
            set
            {
                if (!Equals(_isMoveTarget, value))
                {
                    _isMoveTarget = value;
                    Refresh();
                }
            }
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var rect = new Rectangle(
                e.ClipRectangle.Location, 
                new Size(
                    (int)(e.ClipRectangle.Width - Pens.Red.Width),
                    (int)(e.ClipRectangle.Height - Pens.Red.Width)));
            e.Graphics.DrawRectangle(Pens.Red, rect);
            //e.Graphics.FillRectangle(Brushes.Red, e.ClipRectangle);
        }
    }

    delegate void CollisionDetectedEventHandler(Object sender, CollisionDetectedEventArgs e);
    class CollisionDetectedEventArgs : CancelEventArgs
    {
        public CollisionDetectedEventArgs(Control control)
        {
            Control = control;
        }
        public Control Control { get; }
    }
}
