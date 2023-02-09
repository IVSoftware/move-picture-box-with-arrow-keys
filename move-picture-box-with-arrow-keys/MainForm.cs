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
                                // Suppress OS tabbing to prevent loss of focus.
                                return true;
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
                    richTextBox.SelectionFont = new Font(richTextBox.Font, FontStyle.Bold);
                    richTextBox.SelectionColor = e.Cancel ? Color.Green : Color.Red;
                    richTextBox.AppendText($"{Environment.NewLine}Collision{Environment.NewLine}");
                    richTextBox.SelectionFont = new Font(richTextBox.Font, FontStyle.Regular);
                    richTextBox.SelectionColor = Color.Black;

                    List<string> builder = new List<string>();
                    builder.Add($"Moving: {control.Name}");
                    builder.Add($"@ {control.Bounds}");
                    builder.Add($"Collided with: {e.Control.Name}");
                    builder.Add($"@ {e.Control.Bounds}");
                    richTextBox.AppendText($"{string.Join(Environment.NewLine, builder)}{Environment.NewLine}");
                    richTextBox.ScrollToCaret();
                    _prevWarning = control.Bounds;
                }
            }
        }
        Rectangle _prevWarning = new Rectangle();
    }
    class ArrowKeyPictureBox : PictureBox
    {
        public ArrowKeyPictureBox() 
        {
            LostFocus += (sender, e) =>Refresh();
            GotFocus += (sender, e) =>Refresh();
            MouseDown += (sender, e) =>
            {
                Focus();
                foreach (var control in Parent.Controls.OfType<ArrowKeyPictureBox>())
                {
                    if (!ReferenceEquals(control, this)) control.IsCurrentMoveTarget = false;
                }
                IsCurrentMoveTarget = true;
            };
            Paint += (sender, e) =>
            {
                if (Focused && IsCurrentMoveTarget) using (var pen = new Pen(Color.Fuchsia))
                    {
                        var rect = new Rectangle(
                            e.ClipRectangle.Location,
                            new Size(
                                (int)(e.ClipRectangle.Width - pen.Width),
                                (int)(e.ClipRectangle.Height - pen.Width)));
                        e.Graphics.DrawRectangle(pen, rect);
                    }
            };
        }

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
                        Top = preview.Y;
                        break;
                    case Keys.Down:
                        if (Bottom >= Parent.Bottom)
                        {
                            return;
                        }
                        preview.Y += INCREMENT;
                        if (detectCollision())
                        {
                            return;
                        }
                        Top = preview.Y;
                        break;
                    case Keys.Left:
                        if (Left <= 0)
                        {
                            return;
                        }
                        preview.X -= INCREMENT;
                        if (detectCollision())
                        {
                            return;
                        }
                        Left = preview.X;
                        break;
                    case Keys.Right:
                        if (Right >= Parent.Right)
                        {
                            return;
                        }
                        preview.X += INCREMENT;
                        if (detectCollision())
                        {
                            return;
                        }
                        Left = preview.X;
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

        public bool IsCurrentMoveTarget
        {
            get => _isCurrentMoveTarget;
            set
            {
                if (!Equals(_isCurrentMoveTarget, value))
                {
                    _isCurrentMoveTarget = value;
                    Refresh();
                }
            }
        }
        bool _isCurrentMoveTarget = false;
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
