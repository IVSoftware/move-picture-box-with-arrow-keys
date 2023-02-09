Your post describes some problems you're having with the timer scheme and inner loop that you show in your code. But what is your actual objective? You state in the post that its _raison d'etre_ is:
>[...] so I can find the picturebox that I am moving with the arrow keys and checking if it collides with other pictureboxes via tags.

This "might" be considered an [X-Y Problem](https://meta.stackexchange.com/a/66378)
 because there may be a more optimal way than a timer to achieve what you wanted to do in the first place (because a timer with a 20ms interval might present some race conditions that make it hard to start and stop predictably). Here's one alternative:

***
**ArrowKeyPictureBox**

Consider customizing `PictureBox` that can `MoveProgrammatically(Keys direction)` when Left, Right, Up or Down is pressed. It would also need to keep track of whether this _particular_ picture box is the one that's supposed to move, based on its `IsCurrentMoveTarget` property. And when it _does_ become the current move target, it notifies all the _other_ instances that they no longer are.

[![screenshot][1]][1]


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

Move method has collision detection and fires a cancellable event when imminent.

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

Determines whether this is the control to move, and whether to draw the focus rectangle.

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


***
**Main Form**

The main form employs a [MessageFilter](https://learn.microsoft.com/en-us/dotnet/api/system.windows.forms.application.addmessagefilter) for Up Down Left Right key events and broadcasts any occurrence to all of the `ArrowKeyPictureBox` instances it might hold.  

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
                    break;
            }
            return false;
        }

Display collision status:

        private void onAnyCollisionDetected(object sender, CollisionDetectedEventArgs e)
        {
            e.Cancel = !e.Control.Name.Contains("Portal");
            if (sender is Control control)
            {
                if (!_prevWarning.Equals(control.Bounds))
                {
                    List<string> builder = new List<string>();
                    builder.Add("Collision");
                    builder.Add($"Moving {control.Name} @ {control.Bounds}");
                    richTextBox.SelectionColor = e.Cancel ? Color.Green : Color.Red;
                    richTextBox.AppendText($"{string.Join(Environment.NewLine, builder)}{Environment.NewLine}");
                    _prevWarning = control.Bounds;
                }
            }
        }
        Rectangle _prevWarning = new Rectangle();
    }


  [1]: https://i.stack.imgur.com/MKawb.png