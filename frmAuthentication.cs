using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Emgu.CV;
using PBKDF2_hashing;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;

namespace Secure_Facial_Recognition_Security_System_using_EmguCV
{
    /// <summary>
    /// Authenticates users using EMGU.CV.LBPHFaceRecognizer
    /// </summary>
    public partial class frmAuthentication : Form
    {

        #region Variables Declaration
        // Holds the current frame form capture device
        Image<Bgr, Byte> currentFrame;
        // Holds an optimized resized version of currentFrame to be used in LBPHRecogniser
        Image<Gray, Byte> result = null;
        // Holds gray scale version of the current frame form capture device
        Image<Gray, Byte> GrayImage_frame = null;
        // Instantiates FaceDetectionOperations class
        FaceDetectionOperations LBPHRecogniser = new FaceDetectionOperations();
        // Assigns capture variable
        Capture grabber;
        // Assigns an object detection using Haar feature-based cascade classifiers
        CascadeClassifier Face;
        // Instantiates MVcFont to be used in capturing frames labeling
        MCvFont font = new MCvFont(FONT.CV_FONT_HERSHEY_TRIPLEX, 0.5d, 0.5d);
        // Holds trained faces
        List<Image<Gray, Byte>> TrainingImages = new List<Image<Gray, Byte>>();
        // Checks if there is an ongoing notification
        bool IsNotifying = false;
        // Holds number of detected faces in capturing frames
        public int NumberOfFaceDetected = 0;
        // Holds recognized user's name
        string RecogniserOutputName;


        #region Notification variables
        // Determines how long txtNotification should display a text [in seconds, if tiNotification interval is set to 1000]
        int NotificationInterval;
        // Holds number of times the tiNotification timer ticks
        int NotificationCleanerTickingTimes = 0;
        // Holds current text of txtPasscodeLogin
        string CurrentNotificationText;
        #endregion
        #endregion

        #region Form Events
        public frmAuthentication()
        {

            InitializeComponent();
            // Loads Haar cascades for face detection
            Face = new CascadeClassifier(Application.StartupPath + "/Cascades/haarcascade_frontalface_default.xml");
            txtNotification.Text = "Initializing ...";
            txtNotification.ForeColor = Color.Yellow;

        }

        private void frmAuthentication_Load(object sender, EventArgs e)
        {
            // Assigns form's KeyDown event
            this.KeyDown += FrmAuthentication_KeyDown;
            this.FormClosed += FrmAuthentication_FormClosed;
            btnConnect.Enabled = true;

            // Loads AutoLogin propertyName value from the user scope settings
            chAutoLogin.Checked = (bool)Secure_Facial_Recognition_Security_System_using_EmguCV.Properties.Settings.Default["AutoLogin"];
            if (!IsNotifying)
            {
                ShowNotification("Users data loaded", 4);
            }

        }

        private void FrmAuthentication_FormClosed(object sender, FormClosedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void FrmAuthentication_KeyDown(object sender, KeyEventArgs e)
        {
            // Checks if user presses the Escape button
            // which results in reseting the notification system to its default state
            if (e.KeyCode == Keys.Escape)
            {
                if (NumberOfFaceDetected > 1)
                {
                    ResetNotification();
                    NumberOfFaceDetected = 0;
                    Application.Idle += new EventHandler(FrameGrabber);
                    return;
                }
                chAutoLogin.Checked = false;
                ResetNotification();
                Application.Idle += new EventHandler(FrameGrabber);
            }
        }
        #endregion

        #region Form Methods
        /// <summary>
        /// Validates user's passcode
        /// </summary>
        private void CheckLogin()
        {
            if (string.IsNullOrWhiteSpace(txtPasscodeLogin.Text))
            {
                ShowNotification("Invalid Input [passcode]", 6, true, true);
                return;
            }
            // Correct passcode
            if (LBPHRecogniser.CheckPasscode(RecogniserOutputName, txtPasscodeLogin.Text))
            {
                this.Hide();
                frmWelcome frm = new frmWelcome();
                frm.userName = RecogniserOutputName;
                frm.ShowDialog();
            }
            else
            // Incorrect passcode
            {
                txtPasscodeLogin.Text = "";
                ShowNotification("Passcode is incorrect!! authentication failed", 5, true, true);
            }
        }
        /// <summary>
        /// Resets the notification text and variables
        /// </summary>
        private void ResetNotification()
        {
            // Checks if notification system is in its default state
            if (txtNotification.Text != "Nothing to report.")
            {
                tiNotification.Stop();
                NotificationCleanerTickingTimes = 0;
                txtNotification.Text = "Nothing to report.";
                txtNotification.ForeColor = Color.Gray;
            }
        }
        /// <summary>
        /// Shows a notification text in a RichTextBox
        /// </summary>
        /// <param name="Msg"></param>
        /// <param name="TimePeriod"></param>
        /// <param name="Countdown"></param>
        /// <param name="IsError"></param>
        /// <remarks>Auto-clear option gets off if Countdown parameter is set to false. TimePeriod doesn't take action if countdown parameter is set to false</remarks>
        private void ShowNotification(string Msg, int TimePeriod = 3, bool Countdown = true, bool IsError = false)
        {
            IsNotifying = true;
            CurrentNotificationText = Msg;
            txtNotification.Text = CurrentNotificationText + $" [{TimePeriod}]";
            txtNotification.ForeColor = IsError ? Color.IndianRed : Color.LawnGreen;
            NotificationInterval = TimePeriod;
            NotificationCleanerTickingTimes = 0;

            // Auto-clear option is ON
            if (Countdown)
            {
                tiNotification.Start();
            }
            // Auto-clear option is OFF
            else
            {
                tiNotification.Stop();
                txtNotification.Text = CurrentNotificationText;
            }


        }

        #endregion

        #region Form Controls


        private void tiNotification_Tick(object sender, EventArgs e)
        {
            // Keeps tracking of timer's ticks, and counting down number of remaining seconds to clear notification text automatically
            NotificationCleanerTickingTimes++;
            string CalculatedTimeValue = (NotificationInterval - NotificationCleanerTickingTimes).ToString();
            txtNotification.Text = CurrentNotificationText + $" [{CalculatedTimeValue}]";

            // It is the time to clear the notification text
            if (NotificationCleanerTickingTimes == NotificationInterval)
            {
                txtNotification.Text = "";
                NotificationCleanerTickingTimes = 0;
                txtNotification.Text = "Nothing to report.";
                txtNotification.ForeColor = Color.Gray;
                IsNotifying = false;
                tiNotification.Stop();
            }
        }






        private void btnRegister_Click(object sender, EventArgs e)

        {
            // Validates inputs before taking any further actions
            if (string.IsNullOrWhiteSpace(txtUserName.Text) || string.IsNullOrWhiteSpace(txtUserName.Text))
            {
                ShowNotification("Invalid Input [name/passcode]", 6, true, true);
                return;
            }

            // Finds rectangular regions in the given frame that are likely to contain objects
            // the cascade has been trained for and returns those regions as a sequence of rectangles. 
            Rectangle[] facesDetected = Face.DetectMultiScale(GrayImage_frame, 1.2, 10, new Size(50, 50), Size.Empty);


            // Iterates through number of faces detected 
            for (int i = 0; i < facesDetected.Length; i++)
            {
                // Optimizes scale to suppress background noises resulting in better face recognition
                facesDetected[i].X += (int)(facesDetected[i].Height * 0.15);
                facesDetected[i].Y += (int)(facesDetected[i].Width * 0.1);
                facesDetected[i].Height -= (int)(facesDetected[i].Height * 0.1);
                facesDetected[i].Width -= (int)(facesDetected[i].Width * 0.2);

                // Final optimization of the current capturing frame image
                result = currentFrame.Copy(facesDetected[i]).Convert<Gray, Byte>().Resize(100, 100, Emgu.CV.CvEnum.INTER.CV_INTER_CUBIC);
                result._EqualizeHist();

            }

            // Holds encrypted byte[] array of result image
            byte[] EncryptedImageBytes = AESCryptography.Encrypt(DatabaseOperations.ImageToByte(result.ToBitmap()));

            // Instantiates DatabaseOperations class
            DatabaseOperations dtb = new DatabaseOperations();

            // Computes hash value of user's entered passcode
            byte[] PassHashValue = HashComputation.ComputeBytesHash(txtPasscodeRegister.Text, out byte[] Salt);

            // Validates first and then registers a new user
            if (dtb.ValidateDataAndRegisterUser(txtUserName.Text, PassHashValue, EncryptedImageBytes, Salt))
            {
                // Stops Frame Grabber event temporarily
                Application.Idle -= new EventHandler(FrameGrabber);
                // Reloads users' data
                LBPHRecogniser.Retrain();
                // Restarts Frame Grabber event 
                Application.Idle += new EventHandler(FrameGrabber);

                ShowNotification("Successfully registered", 3);
            }
            else
            {
                ShowNotification("Registration failed: " + DatabaseOperations.DatabaseOperationStatus, 6, true, true);
            }



        }



        private void btnConnect_Click(object sender, EventArgs e)
        {


            try
            {

                ShowNotification("Connecting to camera ...", 0, false);
                btnConnect.Enabled = false;
                Application.DoEvents();
                //Initializes the capture device
                grabber = new Capture();
                grabber.QueryFrame();
                //Initializes the FrameGraber event
                Application.Idle += new EventHandler(FrameGrabber);
                ShowNotification("Connected to camera", 3);
                gpLogin.Enabled = true;
                gpRegisteration.Enabled = true;
            }
            catch (Exception ex)
            {
                btnConnect.Enabled = true;
                ShowNotification($"Camera connection failed: {ex}", 5, false, true);
            }



        }

        private void chAutoLogin_CheckedChanged(object sender, EventArgs e)
        {

            btnLogin.Enabled = chAutoLogin.Checked ? false : true;

            // Checks if the AutoLogin option is set to ON or OFF reading the user scope settings 
            if (chAutoLogin.Checked)
            {
                Secure_Facial_Recognition_Security_System_using_EmguCV.Properties.Settings.Default["AutoLogin"] = true;

            }
            else
            {
                Secure_Facial_Recognition_Security_System_using_EmguCV.Properties.Settings.Default["AutoLogin"] = false;

            }
            Secure_Facial_Recognition_Security_System_using_EmguCV.Properties.Settings.Default.Save();


        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            CheckLogin();
        }
        #endregion

        /// <summary>
        /// Live face detection and recognition event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void FrameGrabber(object sender, EventArgs e)
        {


            //Get the current frame form capture device
            currentFrame = grabber.QueryFrame().Resize(BoxFrameGrabber.Width, BoxFrameGrabber.Height, Emgu.CV.CvEnum.INTER.CV_INTER_CUBIC);

            //Convert it to Gray scale
            if (currentFrame != null)
            {

                GrayImage_frame = currentFrame.Convert<Gray, Byte>();

                // Finds rectangular regions in the given frame that are likely to contain objects the cascade has been trained for and returns those regions as a sequence of rectangles. 
                Rectangle[] facesDetected = Face.DetectMultiScale(GrayImage_frame, 1.2, 10, new Size(50, 50), Size.Empty);
                NumberOfFaceDetected = facesDetected.Length;

                // Prevents Login or Registration when there is no face detected
                if (NumberOfFaceDetected == 0)
                {
                    btnLogin.Enabled = false;
                    btnRegister.Enabled = false;

                }
                // Prevents Login or Registration when there are more than a single face detected
                else if (NumberOfFaceDetected > 1)
                {

                    btnLogin.Enabled = false;
                    btnRegister.Enabled = false;

                    ShowNotification("MORE THAN 1 FACE DETECTED!! Only one user can be registered or login!!! \r\nPress Esc to continue face detection", 0, false, true);
                    Application.Idle -= new EventHandler(FrameGrabber);
                }
                // Allows Login or Registration
                else
                {
                    gpLogin.Enabled = true;
                    gpRegisteration.Enabled = true;
                }


                // Iterates through number of faces detected 
                for (int i = 0; i < facesDetected.Length; i++)
                {
                    // Optimizes scale to suppress background noises resulting in better face recognition
                    facesDetected[i].X += (int)(facesDetected[i].Height * 0.15);
                    facesDetected[i].Y += (int)(facesDetected[i].Width * 0.1);
                    facesDetected[i].Height -= (int)(facesDetected[i].Height * 0.1);
                    facesDetected[i].Width -= (int)(facesDetected[i].Width * 0.2);

                    // Final optimization of the current capturing frame image
                    result = currentFrame.Copy(facesDetected[i]).Convert<Gray, Byte>().Resize(100, 100, Emgu.CV.CvEnum.INTER.CV_INTER_CUBIC);
                    result._EqualizeHist();

                    // Gets detected user's name
                    RecogniserOutputName = LBPHRecogniser.Recognise(result);

                    // Detected face doesn't match with any registered user's face
                    if (RecogniserOutputName == null)
                    {
                        btnLogin.Enabled = false;
                        btnRegister.Enabled = true;
                        currentFrame.Draw(facesDetected[i], new Bgr(Color.Red), 2);
                        currentFrame.Draw("UnRegistered", ref font, new Point(facesDetected[i].X - 2, facesDetected[i].Y - 2), new Bgr(Color.Red));
                    }
                    else
                    // Detected face matches with any registered user's face
                    {
                        btnLogin.Enabled = true;
                        btnRegister.Enabled = false;
                        currentFrame.Draw(facesDetected[i], new Bgr(Color.MediumSeaGreen), 2);
                        currentFrame.Draw("Registered", ref font, new Point(facesDetected[i].X - 2, facesDetected[i].Y - 2), new Bgr(Color.LawnGreen));

                        // Prompt to login immediately after face recognition if auto-login option is ON
                        if (chAutoLogin.Checked)
                        {
                            // Stops Frame Grabber event temporarily
                            Application.Idle -= new EventHandler(FrameGrabber);
                            ShowNotification("Please Enter your passcode \n\rOR  Press Esc to cancel the Login process and disable the auto-login option", 0, false);
                        }

                    }



                }
                // Shows currentFrame to the frameGrabber box
                BoxFrameGrabber.Image = currentFrame;
            }
        }


    }

}
