using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PTZ_Controller
{
    public partial class Form2 : UserControl
    {
        int m_nIndex;   //index	
        // bool m_bRecord; //is recording or not
        bool m_bSound;  //sound is off/on

        public int m_iPlayhandle;   //play handle
        public int m_lLogin; //login handle
        public int m_iChannel; //play channel
        // public int m_iTalkhandle;

        public Form2()
        {
            InitializeComponent();
        }

        public int ConnectRealPlay(ref DEV_INFO pDev, int nChannel)
        {
            H264_DVR_CLIENTINFO playstru = new H264_DVR_CLIENTINFO();

            playstru.nChannel = nChannel;
            playstru.nStream = 0;
            playstru.nMode = 0;
            playstru.hWnd = this.Handle;
            m_iPlayhandle = NETSDK.H264_DVR_RealPlay(pDev.lLoginID, ref playstru);

            return m_iPlayhandle;
        }

        public int GetLoginHandle()
        {
            return m_lLogin;
        }

        public void OnDisconnect()
        {
            if (m_iPlayhandle > 0)
            {
                NETSDK.H264_DVR_StopRealPlay(m_iPlayhandle, (uint)this.Handle);
                m_iPlayhandle = -1;

            }
            if (m_bSound)
            {
                OnCloseSound();
            }
            m_lLogin = -1;
        }

        public bool OnOpenSound()
        {
            if (NETSDK.H264_DVR_OpenSound(m_iPlayhandle))
            {
                m_bSound = true;
                return true;
            }
            return false;
        }

        public bool OnCloseSound()
        {
            if (NETSDK.H264_DVR_CloseSound(m_iPlayhandle))
            {
                m_bSound = false;
                return true;
            }
            return false;
        }

        public void VideoExit()
        {
            
            ((Form)this.TopLevelControl).Close();
        }
    }
}
