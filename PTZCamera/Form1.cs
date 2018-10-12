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

namespace PTZ_Controller
{
    public partial class LoginForm : Form
    {
        public LoginForm loginForm = new LoginForm();
        public Form2 m_videoform = new Form2();

        public int nLoginID;

        // Instantiate device (camera) info into struct and dictionary
        public DEV_INFO m_devInfo = new DEV_INFO();
        public static Dictionary<int, DEV_INFO> dictDevInfo = new Dictionary<int, DEV_INFO>();
        public static Dictionary<int, DEV_INFO> dictDiscontDev = new Dictionary<int, DEV_INFO>();

        // 
        // private System.Timers.Timer timerDisconnect = new System.Timers.Timer(30000);
        private NETSDK.fDisConnect disCallback;
        private NETSDK.fMessCallBack msgcallback;
        // private System.Timers.ElapsedEventHandler reconnect;

        public LoginForm()
        {
            InitializeComponent();
            InitSDK();
            //reconnect = new System.Timers.ElapsedEventHandler(ReConnect);
            //GC.KeepAlive(reconnect);
            //timerDisconnect.Elapsed += new System.Timers.ElapsedEventHandler(reconnect);
            loginForm.Show();
        }

        bool MessCallBack(int lLoginID, string pBuf, uint dwBufLen, IntPtr dwUser)
        {
            LoginForm form = new LoginForm();
            Marshal.PtrToStructure(dwUser, form);
            return form.DealwithAlarm(lLoginID, pBuf, dwBufLen);
        }

        void DisConnectBackCallFunc(int lLoginID, string pchDVRIP, int nDVRPort, IntPtr dwUser)
        {
            if (lLoginID == m_videoform.GetLoginHandle())
            {
                m_videoform.OnDisconnct();
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

            /*
            if (dictDiscontDev.Count > 0)
            {
                timerDisconnect.Enabled = true;
                timerDisconnect.Start();
            }
            */
        }

        public int InitSDK()
        {
            //initialize
            disCallback = new NETSDK.fDisConnect(DisConnectBackCallFunc);
            GC.KeepAlive(disCallback);
            int bResult = NETSDK.H264_DVR_Init(disCallback, this.Handle);

            //the messages received in SDK from DVR which need to upload such as alarm information diary information may do through callback function
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

        private void buttonStart_Click(object sender, EventArgs e)
        {
            if (textBoxDevName.Text.Trim() != ""
               && textBoxIP.Text.Trim() != ""
               && textBoxport.Text.Trim() != ""
               && textBoxUsername.Text.Trim() != "")
            {
                H264_DVR_DEVICEINFO dvrdevInfo = new H264_DVR_DEVICEINFO();
                int nError;
                nLoginID = NETSDK.H264_DVR_Login(textBoxIP.Text.Trim(), ushort.Parse(textBoxport.Text.Trim()), textBoxUsername.Text, textBoxPassword.Text, out dvrdevInfo, out nError, SocketStyle.TCPSOCKET);
                DEV_INFO devInfo = new DEV_INFO();

                if (nLoginID > 0)
                {
                    PTZ_Controller.PTZForm ptzForm = new PTZ_Controller.PTZForm();
                    dictDevInfo.Add(devInfo.lLoginID, devInfo);
                    m_videoform.Show();
                    //ptzForm.Show();
                    // this.Close();
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

            // timerDisconnect.Stop();

            ExitSDk();

            loginForm.Close();
            //ptzForm.Close();
            m_videoform.VideoExit();
        }
    }
}
