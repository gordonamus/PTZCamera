using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;

namespace PTZ_Controller
{
    public partial class LoginForm : Form
    {
        // Construct camera login form and video feed form
        // public LoginForm loginForm = new LoginForm();
        public VideoForm m_videoform = new VideoForm();
        public PTZForm ptzForm;

        // Construct (camera) info into struct and dictionary
        public DEV_INFO devInfo = new DEV_INFO();
        public static Dictionary<int, DEV_INFO> dictDevInfo = new Dictionary<int, DEV_INFO>();  // Login ID -> Device Info
        public static Dictionary<int, DEV_INFO> dictDiscontDev = new Dictionary<int, DEV_INFO>();

        // Construct connection timer
        private System.Timers.Timer timerDisconnect = new System.Timers.Timer(30000);   // 30 second timer interval
        private System.Timers.ElapsedEventHandler reconnect;    // Time elapsed event to reconnect to camera

        // Instantiate Disconnect and Message Callback functions
        private NETSDK.fDisConnect disCallback;
        private NETSDK.fMessCallBack msgcallback;

        public LoginForm()
        {
            InitializeComponent();
            InitSDK();  // Initialize SDK
            reconnect = new System.Timers.ElapsedEventHandler(ReConnect);   // Delegate reconnect object to elapsed event handler
            GC.KeepAlive(reconnect);    // Keep the reconnect object alive from garbage collector since it is not yet referenced
            timerDisconnect.Elapsed += new System.Timers.ElapsedEventHandler(reconnect);    //  Fire the reconnect event
        }

        // Set device callback function to get camera's current state
        bool MessCallBack(int lLoginID, string pBuf, uint dwBufLen, IntPtr dwUser)
        {
            LoginForm form = new LoginForm();   // Construct new Login Form
            Marshal.PtrToStructure(dwUser, form);   // Marshal unmanaged user data into struct data type
            return form.DealwithAlarm(lLoginID, pBuf, dwBufLen);    // Return True for callback
        }

        // Set Disconnect callback function. It is to callback disconnect device; excluding device logout successfully
        void DisConnectBackCallFunc(int lLoginID, string pchDVRIP, int nDVRPort, IntPtr dwUser)
        {
            if (lLoginID == m_videoform.GetLoginHandle())
            {
                m_videoform.OnDisconnect();
            }

            foreach (DEV_INFO devinfo in dictDevInfo.Values)
            {
                if (devinfo.lLoginID == lLoginID)
                {
                    NETSDK.H264_DVR_Logout(lLoginID);
                    dictDevInfo.Remove(devinfo.lLoginID);
                    dictDiscontDev.Add(devinfo.lLoginID, devinfo);
                    break;
                }
            }

            // If device is disconnected, activate the timer to reconnect
            if (dictDiscontDev.Count > 0)
            {
                timerDisconnect.Enabled = true; // Default value is true
                timerDisconnect.Start();
            }
        }

        public int InitSDK()
        {
            VideoForm m_videoform = new VideoForm();
            // Initialize
            disCallback = new NETSDK.fDisConnect(DisConnectBackCallFunc);
            GC.KeepAlive(disCallback);  // Keep the disconnect callback object alive
            int bResult = NETSDK.H264_DVR_Init(disCallback, this.Handle);

            // The messages received in SDK from DVR, which need to upload such as alarm information diary information, may go through callback function
            msgcallback = new NETSDK.fMessCallBack(MessCallBack);
            NETSDK.H264_DVR_SetDVRMessCallBack(msgcallback, this.Handle);
            NETSDK.H264_DVR_SetConnectTime(5000, 3);

            return bResult;
        }

        public bool ExitSDk()
        {
            return NETSDK.H264_DVR_Cleanup();
        }

        public bool DealwithAlarm(int lDevcID, string pBuf, uint dwLen)
        {
            return true;
        }

        // Connect to device and display video feed
        private void buttonStart_Click(object sender, EventArgs e)
        {
            if (textBoxDevName.Text.Trim() != ""
               && textBoxIP.Text.Trim() != ""
               && textBoxport.Text.Trim() != ""
               && textBoxUsername.Text.Trim() != "")
            {
                H264_DVR_DEVICEINFO dvrdevInfo = new H264_DVR_DEVICEINFO();
                int nError;
                int nLoginID = NETSDK.H264_DVR_Login(textBoxIP.Text.Trim(), ushort.Parse(textBoxport.Text.Trim()), textBoxUsername.Text, textBoxPassword.Text, out dvrdevInfo, out nError, SocketStyle.TCPSOCKET);

                if (nLoginID > 0)
                {
                    LoginForm loginForm = new LoginForm();
                    PTZForm ptzForm = new PTZForm(this);
                    m_videoform.Show();
                    ptzForm.Show();
                    // this.Close();

                    // Save device info into dictionary
                    devInfo.szDevName = textBoxDevName.Text;
                    devInfo.lLoginID = nLoginID;
                    devInfo.nPort = Int32.Parse(textBoxport.Text.Trim());
                    devInfo.szIpaddress = textBoxIP.Text.Trim();
                    devInfo.szUserName = textBoxUsername.Text;
                    devInfo.szPsw = textBoxPassword.Text;
                    devInfo.NetDeviceInfo = dvrdevInfo;
                    dictDevInfo.Add(devInfo.lLoginID, devInfo);
                    NETSDK.H264_DVR_SetupAlarmChan(nLoginID);
                    m_videoform.ConnectRealPlay(ref devInfo, 0);
                }
                else
                {
                    MessageBox.Show("Invalid Login Info!");
                }
            }
            else
            {
                MessageBox.Show("Please input all data!");
            }
        }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            foreach (DEV_INFO devinfo in dictDevInfo.Values)
            {

                NETSDK.H264_DVR_Logout(devinfo.lLoginID);

            }

            timerDisconnect.Stop();

            ExitSDk();

            this.Close();
            ptzForm.Close();
            m_videoform.Close();
            //m_videoform.VideoExit();
        }

        // Reconnect to camera
        public void ReConnect(object source, System.Timers.ElapsedEventArgs e)
        {

            foreach (DEV_INFO devinfo in dictDiscontDev.Values)
            {

                H264_DVR_DEVICEINFO OutDev = new H264_DVR_DEVICEINFO();
                int nError = 0; // DVR out parameter 

                // Login to camera
                int lLogin = NETSDK.H264_DVR_Login(devinfo.szIpaddress, (ushort)devinfo.nPort, devinfo.szUserName, devinfo.szPsw, out OutDev, out nError, SocketStyle.TCPSOCKET);

                // If login info is invalid
                if (lLogin <= 0)
                {
                    // Error messages
                    int nErr = NETSDK.H264_DVR_GetLastError();
                    if (nErr == (int)SDK_RET_CODE.H264_DVR_PASSWORD_NOT_VALID)
                    {
                        MessageBox.Show("Password Error");
                    }
                    else if (nErr == (int)SDK_RET_CODE.H264_DVR_LOGIN_USER_NOEXIST)
                    {
                        MessageBox.Show("User Not Exist");
                    }

                    return;
                }
                dictDiscontDev.Remove(devinfo.lLoginID);    // Remove disconnected device from dictionary

                LoginForm loginForm = new LoginForm();

                DEV_INFO devAdd = new DEV_INFO();
                devAdd = devinfo;
                devAdd.lLoginID = lLogin;

                // Connect to video feed
                loginForm.m_videoform.ConnectRealPlay(ref devAdd, 1);
                Thread.Sleep(10);

                dictDevInfo.Add(lLogin, devAdd);    // Add connected device to dictionary
                NETSDK.H264_DVR_SetupAlarmChan(lLogin); // Subscribe to alarm message
            }

            // If there is no record of any disconnected devices, cease the reconnect attempt
            if (0 == dictDiscontDev.Count)
            {
                timerDisconnect.Enabled = false;
                timerDisconnect.Stop();
            }
        }
    }
}
