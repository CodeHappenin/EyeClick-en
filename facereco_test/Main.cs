using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;
using System.Media;

namespace EyeClick
{
    public partial class Main : Form
    {
        MyPipeline pipeline;
        public SoundPlayer player;
        FirstTimeRun firstTimeRunDialog;
        bool isRunning = false;

        public Main()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            player = new SoundPlayer(Properties.Resources.click);            
            int right = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Size.Width;

            //Properties.Settings.Default.Reset(); //Uncomment to reset the counter!
            
            //The runTimes counter is temporally disbled, now the tutorial always shows
            //if (Properties.Settings.Default.runTimes > 0)
            //{
                this.Hide();
                firstTimeRunDialog = new FirstTimeRun();
                firstTimeRunDialog.ShowDialog();
                this.Show();
                //Properties.Settings.Default.runTimes--;
                //Properties.Settings.Default.Save();
            //}
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {

        }

        private void clickAllow_Tick(object sender, EventArgs e)
        {
            clickAllow.Enabled = false;
            pipeline.canClick = true;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            isRunning = false;
            pipeline.PauseFaceLandmark(true);
            pipeline.PauseFaceLocation(true);
            pipeline.Close();
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            isRunning = true;
            pipeline = new MyPipeline(this, pictureBox1);
            pipeline.LoopFrames();
        }
    }

    class MyPipeline : UtilMPipeline
    {
        [DllImport("user32.dll")]
        static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

        [Flags]
        public enum MouseEventFlags
        {
            LEFTDOWN = 0x00000002,
            LEFTUP = 0x00000004,
            MIDDLEDOWN = 0x00000020,
            MIDDLEUP = 0x00000040,
            MOVE = 0x00000001,
            ABSOLUTE = 0x00008000,
            RIGHTDOWN = 0x00000008,
            RIGHTUP = 0x00000010
        }

        //Core variables
        ulong timeStamp;
        int faceId;
        uint fidx = 0; //Unknown variable, what does this does?

        //Statuses
        pxcmStatus locationStatus;
        pxcmStatus landmarkStatus;
        pxcmStatus attributeStatus;
        public bool takeRecoSnapshot = false;
        public bool canClick = true;

        //Form variables
        Main parent;
        string detectionConfidence;
        Bitmap lastProcessedBitmap;

        //Attribute array
        uint[] blink = new uint[2];
        
        //PXCM variables
        PXCMFaceAnalysis faceAnalysis;
        PXCMSession session;
        PXCMFaceAnalysis.Detection faceLocation;
        PXCMFaceAnalysis.Attribute faceAttributes;
        PXCMFaceAnalysis.Detection.Data faceLocationData;
        PXCMFaceAnalysis.Landmark.ProfileInfo landmarkProfile;
        PXCMFaceAnalysis.Attribute.ProfileInfo attributeProfile;

        //Face data
        PictureBox recipient; //Where the image will be drawn

        public MyPipeline(Main parent, PictureBox recipient)
        {
            lastProcessedBitmap = new Bitmap(640, 480);            

            this.recipient = recipient;
            this.parent = parent;
                        
            attributeProfile = new PXCMFaceAnalysis.Attribute.ProfileInfo();

            EnableImage(PXCMImage.ColorFormat.COLOR_FORMAT_RGB24);
            EnableFaceLocation();
        }

        public override bool OnNewFrame()
        {
            faceAnalysis = QueryFace();
            faceAnalysis.QueryFace(fidx, out faceId, out timeStamp);
            
            //Get face location
            faceLocation = (PXCMFaceAnalysis.Detection)faceAnalysis.DynamicCast(PXCMFaceAnalysis.Detection.CUID);
            locationStatus = faceLocation.QueryData(faceId, out faceLocationData);
            detectionConfidence = faceLocationData.confidence.ToString();

            //Get face attributes (smile, age group, gender, eye blink, etc)
            faceAttributes = (PXCMFaceAnalysis.Attribute)faceAnalysis.DynamicCast(PXCMFaceAnalysis.Attribute.CUID);
            faceAttributes.QueryProfile(PXCMFaceAnalysis.Attribute.Label.LABEL_EYE_CLOSED, 0, out attributeProfile);
            attributeProfile.threshold = 50; //Must be here!
            faceAttributes.SetProfile(PXCMFaceAnalysis.Attribute.Label.LABEL_EYE_CLOSED, ref attributeProfile);
            attributeStatus = faceAttributes.QueryData(PXCMFaceAnalysis.Attribute.Label.LABEL_EYE_CLOSED, faceId, out blink);

            ShowAttributesOnForm();
            
            //Do the application events
            try
            {
                Application.DoEvents(); //TODO: This should be avoided using a different thread, but how?
            }
            catch (AccessViolationException e)
            {
                //TODO: Handle exception!
            }
            return true;
        }

        public override void OnImage(PXCMImage image)
        {
            session = QuerySession();
            image.QueryBitmap(session, out lastProcessedBitmap);
            using (Graphics drawer = Graphics.FromImage(lastProcessedBitmap))
            {
                if (locationStatus != pxcmStatus.PXCM_STATUS_ITEM_UNAVAILABLE)
                {
                    drawer.DrawRectangle(new Pen(new SolidBrush(Color.Red), 1), new Rectangle(new Point((int)faceLocationData.rectangle.x, (int)faceLocationData.rectangle.y), new Size((int)faceLocationData.rectangle.w, (int)faceLocationData.rectangle.h)));
                }
            }
            //Show main image
            recipient.Image = lastProcessedBitmap;
        }

        private void ShowAttributesOnForm()
        {
            if (blink[0] == 100)
            {
                //parent.checkBox1.Checked = true;
                if (canClick)
                {
                    canClick = false;
                    mouse_event((int)(MouseEventFlags.LEFTDOWN | MouseEventFlags.LEFTUP), Cursor.Position.X, Cursor.Position.Y, 0, 0);
                    parent.player.Play();
                }
            }
            else
            {
                //parent.checkBox1.Checked = false;
                parent.clickAllow.Enabled = true;
            }
        }
    }
}
