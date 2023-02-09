Your post describes some problems you're having with the timer scheme and inner loop that you show in your code. But what is the purpose of this code? You state in the post that its _raison d'etre_ is:
>[...] so I can find the picturebox that I am moving with the arrow keys and checking if it collides with other pictureboxes via tags.

This "might" be considered an X-Y problem because a Timer could be considered a loose cannon for an application like this where problems of this kind could be hard to avoid. I'd like to offer an alternative for what you were trying to do to begin with. 

***
**ArrowKeyPictureBox**

Consider customizing `PictureBox` that can `MoveProgrammatically(Keys direction)` and can also keep track of whether this particular instance `IsCurrentMoveTarget`. And when it _becomes_ the current move target, it notifies all the _other_ instances that they no longer are.

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
Manage current move target, for example

        public static event CollisionDetectedEventHandler CollisionDetected;
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
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
                    BorderStyle = IsCurrentMoveTarget ?
                        BorderStyle.FixedSingle :
                        BorderStyle.None;
                }
            }
        }
    }

***
**Main Form**

What we need the main form to do is listen for Up Down Left Right and broadcast that occurrence to all of the `ArrowKeyPictureBox` instances it might hold.

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