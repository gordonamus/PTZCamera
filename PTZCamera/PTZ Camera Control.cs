using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Data;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace Test
{
	public class Stuff
	{
		// Instantiate device (camera) info into struct and dictionary
		public DEV_INFO m_devInfo = new DEV_INFO();
		public static Dictionary<int , DEV_INFO> dictDevInfo = new Dictionary<int , DEV_INFO>();
        public static Dictionary<int, DEV_INFO> dictDiscontDev = new Dictionary<int, DEV_INFO>();

        public int speed;
		
		// 
		private System.Timers.Timer timerDisconnect = new System.Timers.Timer(30000);

        private NETSDK.fDisConnect disCallback;

        private NETSDK.fMessCallBack msgcallback;

		private System.Timers.ElapsedEventHandler reconnect;

		static void Main(string[] args)
		{
			// Request user permission to access camera
			Console.Write("Connect to Camera? (Y/N)");
			string connect = Console.ReadLine();
			
			if ((connect == "Y") || (connect == "y"))
			{
				InitSDK()
				reconnect = new System.Timers.ElapsedEventHandler(ReConnect);
            	GC.KeepAlive(reconnect);
            	timerDisconnect.Elapsed += new System.Timers.ElapsedEventHandler(reconnect); 
			}


		}
	
		void DisConnectBackCallFunc(int lLoginID, string pchDVRIP, int nDVRPort, IntPtr dwUser)
		{
			for (int i = 0; i < 16; i++)
			{
				if (lLoginID == m_videoform[i].GetLoginHandle())
				{
					m_videoform[i].OnDisconnct();
				}
			}
			
			foreach (DEV_INFO devinfo in dictDevInfo.Values)
			{
				if (devinfo.lLoginID == lLoginID)
				{
					NETSDK.H264_DVR_Logout(lLoginID);
					dictDevInfo.Remove(devinfo.lLoginID);
					dictDiscontDev.Add(devinfo.lLoginID,devinfo);
					break;
				}
			}

			if ( dictDiscontDev.Count > 0 )
			{
				timerDisconnect.Enabled = true;
				timerDisconnect.Start();
			}
		}

		public int InitSDK()
		{
			//initialize
			disCallback = new NETSDK.fDisConnect(DisConnectBackCallFunc);
			GC.KeepAlive(disCallback);
			int bResult = NETSDK.H264_DVR_Init(disCallback, this.Handle);

			//the messages received in SDK from DVR which need to upload such as alarm information diary information may do through callback function
			msgcallback  = new NETSDK.fMessCallBack(MessCallBack);
			NETSDK.H264_DVR_SetDVRMessCallBack(msgcallback, this.Handle);

			NETSDK.H264_DVR_SetConnectTime(5000, 3);

			return bResult;
		}

		public bool ExitSDk()
		{
			return NETSDK.H264_DVR_Cleanup();
		}

		public int Connect(ref DEV_INFO pDev, int nChannel, int nWndIndex)
		{
			int nRet = 0;

			//if device did not login, login first
			if (pDev.lLoginID <= 0)
			{
				H264_DVR_DEVICEINFO OutDev;
				int nError = 0;
				//
				int lLogin = NETSDK.H264_DVR_Login(pDev.szIpaddress, (ushort)pDev.nPort, pDev.szUserName, pDev.szPsw, out OutDev, out nError, SocketStyle.TCPSOCKET);
				if (lLogin <= 0)
				{

					int nErr = NETSDK.H264_DVR_GetLastError();
					if (nErr == (int)SDK_RET_CODE.H264_DVR_PASSWORD_NOT_VALID)
					{
						MessageBox.Show(("Error.PwdErr"));
					}
					else
					{
						MessageBox.Show(("Error.NotFound"));
					}

					return nRet;
				}

				pDev.lLoginID = lLogin;
				NETSDK.H264_DVR_SetupAlarmChan(lLogin);
			}

			int nWnd = m_nCurIndex;
			if (nWndIndex >= 0)
			{
				nWnd = nWndIndex;
			}

			if (nWnd >= m_nTotalWnd)
			{
				return nRet;
			}

			return m_videoform[nWnd].ConnectRealPlay(ref pDev, nChannel);	
		}

		// public void PtzControl(uint dwBtn, bool dwStop)
		// {
		// 	long lPlayHandle = m_videoform[m_nCurIndex].GetHandle();
		// 	if (lPlayHandle <= 0)
		// 	{
		// 		return;
		// 	}
		// }

		DEV_INFO ReadXML()
        {
            XmlReader xml = XmlReader.Create("");
	        DEV_INFO devInfo = new DEV_INFO();
        	
	        devInfo.nPort=0;
	        devInfo.lLoginID=0;
	        devInfo.lID=0;
           
	        while(xml.ReadToFollowing("ip"))
	        {
		        //read the information from XML
		        string strIP,strUserName,strPsw,strDevName;
		        uint nPort;
		        int byChanNum=0,lID=0;

		        uint bSerialID,nSerPort;
		        string szSerIP,szSerialInfo;			
		        xml = xml.ReadSubtree();	

		        
		        strIP=xml.ReadElementString("ip2");
		      
		        strDevName= xml.ReadElementString("DEVICENAME");
		        
		        strUserName=xml.ReadElementString("username");
		       
		        nPort= Convert.ToUInt32(xml.ReadElementString("port"));
		        
		        strPsw=xml.ReadElementString("pwd");
		       
		        byChanNum= Convert.ToInt32(xml.ReadElementString("byChanNum"));
		       
		        lID= Convert.ToInt32(xml.ReadElementString("lID"));

		        
		        bSerialID= Convert.ToByte(xml.ReadElementString("bSerialID"));
		        
		        szSerIP=xml.ReadElementString("szSerIP");
		        
		        nSerPort=Convert.ToUInt32(xml.ReadElementString("nSerPort"));
		        
		        szSerialInfo=xml.ReadElementString("szSerialInfo");//����ddns��¼
	            
		        devInfo.nTotalChannel =byChanNum;
		        devInfo.nPort = (int)nPort;

		        devInfo.bSerialID= Convert.ToByte(bSerialID);		
		        devInfo.nSerPort=(int)nSerPort;
		        devInfo.szSerIP=szSerIP;		
		        devInfo.szSerialInfo=szSerialInfo;//����ddns��¼		
		        devInfo.szDevName=strDevName;
		        devInfo.szUserName=strUserName;
		        devInfo.szPsw=strPsw;
		        devInfo.szIpaddress= strIP;
		        DEV_INFO pDev = new DEV_INFO();
		        pDev = devInfo;
                IntPtr ptr = new IntPtr();
		        Marshal.StructureToPtr(pDev,ptr,false);
                pDev.lID =ptr.ToInt32();
		        m_devMap[pDev.lID] = pDev;
		        String strName;
                TreeNode node = new  TreeNode();
                node.Text = strDevName;
		        for ( int i = 0; i < byChanNum; i ++)
		        {
			        strName= String.Format("CAM {0}", i+1);
                    node.Nodes.Add(strName);
		        }
                DevTree.Nodes.Add(node);
	        }
	        return devInfo;
        }

		int DevLogin(ref DEV_INFO pdev)
        {
            if (Convert.ToBoolean(pdev.bSerialID))//
            {
                int maxDeviceNum = 100;  //
                DDNS_INFO[] pDDNSInfo = new DDNS_INFO[maxDeviceNum];
                SearchMode searchmode;
                int nReNum = 0;  //		
                searchmode.nType = (int)SearchModeType.DDNS_SERIAL;
                searchmode.szSerIP = pdev.szSerIP;
                searchmode.nSerPort = pdev.nSerPort;
                searchmode.szSerialInfo = pdev.szSerialInfo;
                bool bret = Convert.ToBoolean(NETSDK.H264_DVR_GetDDNSInfo(ref searchmode, out pDDNSInfo, maxDeviceNum, out nReNum));
                if (!bret)
                {
                    return 0;
                }
                pdev.szIpaddress=pDDNSInfo[0].IP;
                pdev.nPort = pDDNSInfo[0].MediaPort;
            }

            H264_DVR_DEVICEINFO OutDev;
            int nError = 0;
            //
            NETSDK.H264_DVR_SetConnectTime(3000, 1);//

            int lLogin = NETSDK.H264_DVR_Login(pdev.szIpaddress, Convert.ToUInt16(pdev.nPort), pdev.szUserName,
                pdev.szPsw, out OutDev,  out nError,SocketStyle.TCPSOCKET);
            if (lLogin <= 0)
            {
                int nErr = NETSDK.H264_DVR_GetLastError();
                if (nErr == (int)SDK_RET_CODE.H264_DVR_PASSWORD_NOT_VALID)
                {
                    MessageBox.Show("Error.PwdErr");
                }
                else
                {
                    MessageBox.Show("Error.NotFound");

                }
                return lLogin;
            }
            NETSDK.H264_DVR_SetupAlarmChan(lLogin);
            return lLogin;
        }

		public void SetDevInfo(ref DEV_INFO pDev)
        {
            m_devInfo = pDev;
        }

        public void ReConnect(object source, System.Timers.ElapsedEventArgs e)
        {
                
        	foreach( DEV_INFO devinfo in dictDiscontDev.Values )
            {

                H264_DVR_DEVICEINFO OutDev = new H264_DVR_DEVICEINFO();
                int nError = 0;

                int lLogin = NETSDK.H264_DVR_Login(devinfo.szIpaddress, (ushort)devinfo.nPort, devinfo.szUserName, devinfo.szPsw, out OutDev, out nError, SocketStyle.TCPSOCKET);
                if (lLogin <= 0)
                {

                    int nErr = NETSDK.H264_DVR_GetLastError();
                    if (nErr == (int)SDK_RET_CODE.H264_DVR_PASSWORD_NOT_VALID)
                    {
                        MessageBox.Show(("Password Error"));
                    }
                    else if (nErr == (int)SDK_RET_CODE.H264_DVR_LOGIN_USER_NOEXIST)
                    {
                        MessageBox.Show(("User Not Exist"));
                    }

                    return;
                }
                dictDiscontDev.Remove(devinfo.lLoginID);

                ClientDemo clientForm = new ClientDemo();

                foreach (Form form in Application.OpenForms)
                {
                    if (form.Name == "ClientDemo")
                    {
                        clientForm = (ClientDemo)form;
                        break;
                    }
                }
                DEV_INFO devAdd = new DEV_INFO();
                devAdd = devinfo;
                devAdd.lLoginID = lLogin;
           

                foreach (TreeNode node in clientForm.devForm.DevTree.Nodes)
                {
                    if (node.Name == "Device")
                    {
                        DEV_INFO dev = (DEV_INFO)node.Tag;
                        if (dev.lLoginID == devinfo.lLoginID)
                        {
                            node.Text = devAdd.szDevName;
                            node.Tag = devAdd;
                            node.Name = "Device";

                            foreach (TreeNode channelnode in node.Nodes)
                            {
                                CHANNEL_INFO chInfo = (CHANNEL_INFO)channelnode.Tag;
                                if ( chInfo.nWndIndex > -1  )
                                {
                                    clientForm.m_videoform[chInfo.nWndIndex].ConnectRealPlay(ref devAdd, chInfo.nChannelNo);
                                    Thread.Sleep(10);
                                }
                                
                            }
                            break;
                        }

                    }

                }

                dictDevInfo.Add(lLogin, devAdd);
                NETSDK.H264_DVR_SetupAlarmChan(lLogin);
            }
            if (0 == dictDiscontDev.Count)
            {
                timerDisconnect.Enabled = false;
                timerDisconnect.Stop();
            }
        }

        private void PTZControl(int nCommand, bool bStop, int nSpeed)
        {
            ClientDemo clientForm = (ClientDemo)this.Owner;
            int nCurVideoform = clientForm.m_nCurIndex;
            if (nCurVideoform >= 0)
            {
                int nPlayHandel = clientForm.m_videoform[nCurVideoform].m_iPlayhandle;
                if (nPlayHandel > 0)
                {
                    int nLoginID = clientForm.m_videoform[nCurVideoform].m_lLogin;
                    int nChannel = clientForm.m_videoform[nCurVideoform].m_iChannel;
                    NETSDK.H264_DVR_PTZControl(nLoginID, nChannel, (int)nCommand, bStop, nSpeed);
                }
            }
        }

        // 
        private void Form2_Load(Object sender, EventArgs e)
        {
            this.KeyDown += new KeyEventHandler(Form2_KeyDown);
            this.KeyUp += new KeyEventHandler(Form2_KeyUp);

            // error possibility from simultaneous key presses
        }
        void Form2_KeyDown(object sender, KeyEventArgs e)
        {
            switch(e.KeyCode)
            {
                // handle up/down/right/left
                case Keys.Up:
                    PTZControl((int)PTZ_ControlType.TILT_UP, false, speed);
                    break;
                case Keys.Down:
                    PTZControl((int)PTZ_ControlType.TILT_DOWN, false, speed);
                    break;
                case Keys.Right:
                    PTZControl((int)PTZ_ControlType.PAN_RIGHT, false, speed);
                    break;
                case Keys.Left:
                    PTZControl((int)PTZ_ControlType.PAN_LEFT, false, speed);
                    break;
                case Keys.Z:    // zoom IN
                    PTZControl((int)PTZ_ControlType.ZOOM_IN, false, speed);
                    break;
                case Keys.X:    // zoom OUT
                    PTZControl((int)PTZ_ControlType.ZOOM_OUT, false, speed);
                    break;


                // set PTZ speed from 1-8 (Low 1 to High 8)
                case (Keys.D1 || Keys.NumPad1): speed = 1; break;
                case (Keys.D2 || Keys.NumPad2): speed = 2; break;
                case (Keys.D3 || Keys.NumPad3): speed = 3; break;
                case (Keys.D4 || Keys.NumPad4): speed = 4; break;
                case (Keys.D5 || Keys.NumPad5): speed = 5; break;
                case (Keys.D6 || Keys.NumPad6): speed = 6; break;
                case (Keys.D7 || Keys.NumPad7): speed = 7; break;
                case (Keys.D8 || Keys.NumPad8): speed = 8; break;

                default: return;  // ignore other keys
            }

            // set speed dialog on Form2 to display updated speed
        }
        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            switch(e.KeyCode)
            {
                // handle up/down/right/left STOP
                case Keys.Up:
                    PTZControl((int)PTZ_ControlType.TILT_UP, true, speed);
                    break;
                case Keys.Down:
                    PTZControl((int)PTZ_ControlType.TILT_DOWN, true, speed);
                    break;
                case Keys.Right:
                    PTZControl((int)PTZ_ControlType.PAN_RIGHT, true, speed);
                    break;
                case Keys.Left:
                    PTZControl((int)PTZ_ControlType.PAN_LEFT, true, speed);
                    break;
                case Keys.Z:    // zoom IN STOP
                    PTZControl((int)PTZ_ControlType.ZOOM_IN, true, speed);
                    break;
                case Keys.X:    // zoom OUT STOP
                    PTZControl((int)PTZ_ControlType.ZOOM_OUT, true, speed);
                    break;
                default: return;    // ignore other keys
            }
        }
    }
}





//         private void buttonUp_KeyDown(object sender, KeyEventArgs e)
//         {
//             PTZControl((int)PTZ_ControlType.TILT_UP, false, 4);
//         }

//         private void buttonUp_KeyUp(object sender, KeyEventArgs e)
//         {
//             PTZControl((int)PTZ_ControlType.TILT_UP, true, 4);
//         }

//         private void buttonUp_KeyPress(object sender, EventArgs e)
//         {
//             PTZControl((int)PTZ_ControlType.TILT_UP, true, 4);
//         }

//         private void buttonDown_KeyDown(object sender, KeyEventArgs e)
//         {
//             PTZControl((int)PTZ_ControlType.TILT_DOWN, false, 4);
//         }

//         private void buttonDown_KeyUp(object sender, KeyEventArgs e)
//         {
//             PTZControl((int)PTZ_ControlType.TILT_DOWN, true, 4);
//         }

//         private void buttonDown_KeyLeave(object sender, EventArgs e)
//         {
//             PTZControl((int)PTZ_ControlType.TILT_DOWN, true, 4);
//         }

//         private void buttonLeft_KeyDown(object sender, KeyEventArgs e)
//         {
//             PTZControl((int)PTZ_ControlType.PAN_LEFT, false, 4);
//         }

//         private void buttonLeft_KeyLeave(object sender, EventArgs e)
//         {
//             PTZControl((int)PTZ_ControlType.PAN_LEFT, true, 4);
//         }

//         private void buttonLeft_KeyUp(object sender, KeyEventArgs e)
//         {
//             PTZControl((int)PTZ_ControlType.PAN_LEFT, true, 4);
//         }

//         private void buttonRight_KeyDown(object sender, KeyEventArgs e)
//         {
//             PTZControl((int)PTZ_ControlType.PAN_RIGHT, false, 4);
//         }

//         private void buttonRight_KeyLeave(object sender, EventArgs e)
//         {
//             PTZControl((int)PTZ_ControlType.PAN_RIGHT, true, 4);
//         }

//         private void buttonRight_KeyUp(object sender, KeyEventArgs e)
//         {
//             PTZControl((int)PTZ_ControlType.PAN_RIGHT, true, 4);
//         }

//         private void buttonZoomIn_KeyDown(object sender, KeyEventArgs e)
//         {
//             PTZControl((int)PTZ_ControlType.ZOOM_IN, false, 4);
//         }

//         private void buttonZoomIn_KeyLeave(object sender, EventArgs e)
//         {
//             PTZControl((int)PTZ_ControlType.ZOOM_IN, true, 4);
//         }

//         private void buttonZoomIn_KeyUp(object sender, KeyEventArgs e)
//         {
//             PTZControl((int)PTZ_ControlType.ZOOM_IN, true, 4);
//         }

//         private void buttonZoomOut_KeyDown(object sender, KeyEventArgs e)
//         {
//             PTZControl((int)PTZ_ControlType.ZOOM_OUT, false, 4);
//         }

//         private void buttonZoomOut_KeyLeave(object sender, EventArgs e)
//         {
//             PTZControl((int)PTZ_ControlType.ZOOM_OUT, true, 4);
//         }

//         private void buttonZoomOut_KeyUp(object sender, KeyEventArgs e)
//         {
//             PTZControl((int)PTZ_ControlType.ZOOM_OUT, true, 4);
//         }


// 	}
// }

//H264_DVR_Init() ///to initialize SDK
//H264_DVR_SetDVRMessCallBack()
//H264_DVR_Cleanup() ///to release all occupied resource.
//H264_DVR_Login() ///to login
//H264_DVR_Logout() ///to logout
//H264_DVR_PTZControl() ///to control PTZ
//H264_DVR_RealPlay () ///to view feed
//H264_DVR_StopRealPlay() ///to stop feed
//H264_DVR_PTZControl(long lLoginID,int nChannelNo, long lPTZCommand, bool bStop = false, long lSpeed = 4)
//H264_DVR_Init(fDisConnect cbDisConnect, unsigned long dwUser);


//PTZ control type
//typedef enum PTZ_ControlType
//{
//	TILT_UP = 0,		//UP
//	TILT_DOWN,			//DOWN
//	PAN_LEFT,			//LEFT
// 	PAN_RIGTH,			//RIGTH
// 	PAN_LEFTTOP,		//LEFT TOP 
// 	PAN_LEFTDOWN,		//LEFT DOWN
// 	PAN_RIGTHTOP,		//RIGTH TOP 
// 	PAN_RIGTHDOWN,		//RIGTH DOWN
// 	ZOOM_IN,			//ZOOM IN 
// 	ZOOM_OUT,			//ZOOM OUT 
// 	FOCUS_FAR,			//FOCUS FAR 
// 	FOCUS_NEAR,			//FOCUS NEAR 
// 	IRIS_OPEN,			//IRIS OPEN 
// 	IRIS_CLOSE,			//IRIS CLOSE 
//  EXTPTZ_OPERATION_ALARM,			//ALARM
//  EXTPTZ_LAMP_ON,					//LIGTH OPEN 
//  EXTPTZ_LAMP_OFF,				//LIGTH CLOSE 
//  EXTPTZ_POINT_SET_CONTROL,		//SET PRESET POINT
//  EXTPTZ_POINT_DEL_CONTROL,		//CLEAR PRESET POINT
//  EXTPTZ_POINT_MOVE_CONTROL,		//GOTO PRESET POINT 
//  EXTPTZ_STARTPANCRUISE,			//START PAN CRUISE			
//  EXTPTZ_STOPPANCRUISE,			//STOP PAN CRUISE	
//  EXTPTZ_SETLEFTBORDER,			//SET LEFT BORDER		
//  EXTPTZ_SETRIGHTBORDER,			//SET RIGHT BORDER	
//  EXTPTZ_STARTLINESCAN,			//START AUTO SCAN
//  EXTPTZ_CLOSELINESCAN,			//STOP AUTO SCAN 
//  EXTPTZ_ADDTOLOOP,				//ADD PRESET POINT TO CRUISE LINE
//  EXTPTZ_DELFROMLOOP,				//DEL PRESET POINT FROM CRUISE LINE	
//  EXTPTZ_POINT_LOOP_CONTROL,		//START CRUISE
//  EXTPTZ_POINT_STOP_LOOP_CONTROL,	//STOP CRUISE 
//  EXTPTZ_CLOSELOOP,				//CLEAR CRUISE LINE
//  EXTPTZ_FASTGOTO,					//FAST GOTO	
//  EXTPTZ_AUXIOPEN,					//AUX OPEN 
//  EXTPTZ_OPERATION_MENU,			//OPERATION MENU
//  EXTPTZ_REVERSECOMM,				//REVER CAMERAL 
//  EXTPTZ_OPERATION_RESET,			///< PTZ RESET 
//  EXTPTZ_TOTAL,

// };

// typedef enum SDK_RET_CODE
// {	
// 	H264_DVR_NOERROR				= 0,		//no error 
// 	H264_DVR_SUCCESS				= 1,		//success 
//    H264_DVR_SDK_NOTVALID			= -10000,	//invalid request
// 	H264_DVR_NO_INIT				= -10001,	//SDK not inited
// 	H264_DVR_ILLEGAL_PARAM			= -10002,	// illegal user parameter
// 	H264_DVR_INVALID_HANDLE			= -10003,	//handle is null
// 	H264_DVR_SDK_UNINIT_ERROR		= -10004,	//SDK clear error
// 	H264_DVR_SDK_TIMEOUT			= -10005,	//timeout 
// 	H264_DVR_SDK_MEMORY_ERROR		= -10006,	//memory error	H264_DVR_SDK_NET_ERROR			= -10007,	//network error
// 	H264_DVR_SDK_OPEN_FILE_ERROR	= -10008,	//open file fail 
// 	H264_DVR_SDK_UNKNOWNERROR		= -10009,	//unknown error
// 	H264_DVR_DEV_VER_NOMATCH		= -11000,	//version mismatch
// 	H264_DVR_ERROR_GET_DATA			= -11001,	//get data fail（including configure, user information and etc）
	

// 	H264_DVR_OPEN_CHANNEL_ERROR		= -11200,	//open channel fail 
// 	H264_DVR_CLOSE_CHANNEL_ERROR	= -11201,	//close channel fail 
// 	H264_DVR_SUB_CONNECT_ERROR		= -11202,	//open media connet fail	H264_DVR_SUB_CONNECT_SEND_ERROR	= -11203,	//media connet send data fail

// 	/// error code of user management
// 	H264_DVR_NOPOWER				= -11300,	//no power 
// 	H264_DVR_PASSWORD_NOT_VALID		= -11301,	// password not valid
// 	H264_DVR_LOGIN_USER_NOEXIST		= -11302,	// user not exist 
// 	H264_DVR_USER_LOCKED			= -11303,	// user is locked 
// 	H264_DVR_USER_IN_BLACKLIST		= -11304,	// user is in backlist
// 	H264_DVR_USER_HAS_USED			= -11305,	// user have logined	H264_DVR_USER_NOT_LOGIN			= -11305,	// no login	H264_DVR_CONNECT_DEVICE_ERROR   = -11306,	// maybe device no exist 

// 	/// error code of configure management	
// 	H264_DVR_OPT_RESTART			= -11400,	// need to restart application	H264_DVR_OPT_REBOOT				= -11401,	// need to reboot system	H264_DVR_OPT_FILE_ERROR			= -11402,	// write file fail 
// 	H264_DVR_OPT_CAPS_ERROR			= -11403,	// not support
// 	H264_DVR_OPT_VALIDATE_ERROR		= -11404,	// validate fail	H264_DVR_OPT_CONFIG_NOT_EXIST	= -11405,	// config not exist	
// 	H264_DVR_CTRL_PAUSE_ERROR		= -11500,	// pause fail 
// 	H264_DVR_SDK_NOTFOUND			= -11501,	//not found
// 	H264_DVR_CFG_NOT_ENABLE         = -11502,   //cfg not enable
// };

// H264_DVR_API long H264_DVR_GetLastError();