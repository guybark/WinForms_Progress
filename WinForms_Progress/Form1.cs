using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace WinForms_Progress
{
    public partial class Form1 : Form
    {
        private MyProgressBar myProgressBar;
        private Timer timer;

        public Form1()
        {
            InitializeComponent();

            // Create some control that we'll consider to be a progress bar.
            myProgressBar = new MyProgressBar();
            myProgressBar.Location = new Point(10, 10);
            myProgressBar.Size = new Size(100, 100);
            myProgressBar.BackColor = Color.Green;
            this.Controls.Add(myProgressBar);

            myProgressBar.CurrentValue = 0;
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            // Update the value on our custom progress bar every couple of seconds.
            if (timer == null)
            {
                myProgressBar.CurrentValue = 0;

                timer = new Timer();
                timer.Interval = 2000;
                timer.Tick += Timer_Tick;

                timer.Start();
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            // Update the value of the progress bar now, limiting it to 100.
            myProgressBar.CurrentValue = Math.Min(100, myProgressBar.CurrentValue + 10);

            if (myProgressBar.CurrentValue >= 100)
            {
                timer.Stop();
                timer = null;
            }
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }

    public class MyProgressBar : Control
    {
        private int currentValue;
        public int CurrentValue 
        {
            get
            {
                return currentValue;
            }
            set
            {
                currentValue = value;

                // Let any AT listening know of the value change.

                // Note: Ultimately we expect WinForms to raise an old MSAA EVENT_OBJECT_VALUECHANGE event through NotifyWinEvent(), 
                // passing in a child id of CHILDID_SELF, (ie zero). This means the windowed control itself is raising the event, and 
                // not some windowless element beneath that control. In one of the implmentations of AccessibilityNotifyClients() in 
                // https://referencesource.microsoft.com/#System.Windows.Forms/winforms/Managed/System/WinForms/Control.cs
                // it shows NotifyWinEvent() being called with the supplied childId plus 1. So if we ultimately want NotifyWinEvent()
                // to be called with CHILDID_SELF, we must pass in -1 here.

                this.AccessibilityNotifyClients(AccessibleEvents.ValueChange, -1);
            }
        }

        protected override AccessibleObject CreateAccessibilityInstance()
        {
            return new MyProgressBarAccessibleObject(this);
        }

        public class MyProgressBarAccessibleObject : ControlAccessibleObject
        {
            private MyProgressBar owner;

            public MyProgressBarAccessibleObject(MyProgressBar owner) : base(owner)
            {
                this.owner = owner;
            }

            public override string Name
            {
                get
                {
                    return "Install"; // Some string for a test.
                }
            }

            public override AccessibleRole Role
            {
                get
                {
                    return AccessibleRole.ProgressBar;
                }
            }

            public override string Value 
            {
                get
                {
                    return owner.CurrentValue.ToString();
                }
            }
        }
    }
}
