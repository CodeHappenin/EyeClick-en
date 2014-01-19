using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using gma.System.Windows;

namespace EyeClick
{
    public partial class FirstTimeRun : Form
    {
        UserActivityHook actHook;
        int step;

        public FirstTimeRun()
        {
            InitializeComponent();
        }

        private void FirstTimeRun_Load(object sender, EventArgs e)
        {
            actHook = new UserActivityHook(true, true); //Create an instance with global hooks
            actHook.OnMouseActivity += new MouseEventHandler(MouseActivity); //Currently there is no mouse activity
            actHook.Start();

            label1.Text = "Welcome to ¬¬ EyeClick, this is the quick-start guide to learn using this program. Click anywhere to start the tutorial.";
            step = 0;
        }

        public void MouseActivity(object sender, MouseEventArgs e)
        {
            if (e.Button.ToString().CompareTo("Left") == 0)
            {                
                if (step == 0)
                {
                    tableLayoutPanel1.SetColumnSpan(label1, 1);
                    label1.Text = "";
                    label2.Text = "When this tutorial is closed you should see your face on a window like shown on the image. Click anywhere to continue.";                    
                    pictureBox1.Image = EyeClick.Properties.Resources.eyeclicktutorial1;
                }
                else if (step == 1)
                {
                    
                    label2.Text = "If the camera is not connected then, a message like this will appear. Click anywhere to continue.";
                    pictureBox1.Image = EyeClick.Properties.Resources.eyeclick1;
                }
                else if (step == 2)
                {
                    label2.Text = "To perform a left-click blink with both eyes, note that the webcam must be pointing directly to your face. Click anywhere to continue.";
                    pictureBox1.Image = EyeClick.Properties.Resources.eyeclick3;
                }
                else if (step == 3)
                {
                    pictureBox1.Image = null;
                    tableLayoutPanel1.SetColumnSpan(label1, 3);
                    label1.Text = "Thats all! This tutorial will automatically dissapear after a few days. Click anywhere to finish tutorial and start using ¬¬ EyeClick.";
                    label2.Text = "";                    
                }
                else
                {
                    this.Close();
                }
                step++;
            }
        }
    }
}
