using System;
using System.Runtime.InteropServices;

public static class vCatchStationTypeDef
{
	///////////////////////////////////////////////////////////////////////////////
	// vCatch Common Interface

	public static int vCatchStation_Version            = 2;

	//public static int vCatchStation_Action_ModuleID 	= 1;
	//public static int vCatchStation_Action_ModuleInfo	= 2;
	public static int vCatchStation_Action_Settings     = 3;
	public static int vCatchStation_Action_StartApp    = 5;
	public static int vCatchStation_Action_vCatch      = 6;
	public static int vCatchStation_Action_StopApp     = 7;
	public static int vCatchStation_Action_T3kStatus   = 10;
	public static int vCatchStation_Action_OnMSG       = 11;
	public static int vCatchStation_Action_OnRSP       = 12;

	public enum T3kStatusCode {
		T3kStatus_OnClose = 1,
		T3kStatus_NoError = 0
	};

	public static ulong MakeLongByModuleIDAndVersion(ulong id, ulong ver) {
		return ((id << 16) | ver);
	}
	public static ushort ModuleIDFromLong(ulong dw) {
		return ((ushort)(dw >> 16));
	}
	public static ushort VersionFromLong(ulong dw) {
		return ((ushort)dw);
	}

	public static ulong ModuleID_3101			 = 3101;
	public static ulong ModuleID_3201			 = 3201;
	public static ulong ModuleID_3301			 = 3301;
	public static ulong ModuleID_4001			 = 4001;


	public static int vCatchStation_TCP_PortNumber = 0x3700;


	//
	public static ulong v4001_TYPE_20x15		 = 00;
	public static ulong v4001_TYPE_4x3			 = 01;
	public static ulong v4001_TYPE_1x1			 = 02;
	// 

	///////////////////////////////////////////////////////////////////////////////
	// Interface between Midleware and Application(Game)

	public static uint WM_USER 									= 0x0400;

	// handshake between Middleware(vCatchStation) and Application(Game)
	// when Application(Game) is started, Application(Game) broadcasts that Application(Game) has started 
	// wparam: zero
	// lparam: zero
	// if vCatchStation is stared or receive the message, vCatchStation broadcasts its HWND
	// wparam: zero
	// lparam: HWND of vCatchStation
	public static string vCatchStation_WindowMessageName 		= "vCatchStation_BroadcastMessage";

	// request initialize the service to Middleware(vCatchStation)
	// wparam: id of a Modele
	// lparam: HWND of the application(game)
	// return: id and version of Module if successful
	public static uint WM_vCatchStation_Initialize_vCatch 		= (WM_USER + 0x3601);

	// request starting catching to Middleware(vCatchStation)
	// wparam: (int)type
	// lparam: HWND of the application(game)
	// return: TRUE(1) if successful
	public static uint WM_vCatchStation_Do_vCatch 				= (WM_USER + 0x3602);

	// send a command to a catch process while catching progress
	// wparam: (int)command
	// lparam: HWND of the application(game)
	// return: TRUE(1) if successful
	public static uint WM_vCatchStation_Command_vCatch       	= (WM_USER + 0x3603);
	public static uint		vCatchStation_Command_Stop             		= 0;          // stop vCatch
	public static uint 		vCatchStation_Command_SetTImeout       		= 0x00001000; // time(100ms unit) = 0~0xfff, ex) 5sec timeout = 0x1000 | (5*10) = 0x1032
	public static uint vCatchStation_CMD_SetTImeout( uint ms ) { return (vCatchStation_Command_SetTImeout | (ms >= 0x0fff ? 0x0fff : (ms / 100))); }
	public static uint vCatchStation_Command_Points_Reporter  			= 0x00002000; // time(10ms unit) = 0~0xff, ex) 100msec interval = 0x2000 | (100/10) = 0x200a
	public static uint			vCatchStation_Points_Report_Down       	= 0x00000100; // 
	public static uint			vCatchStation_Points_Report_Up         	= 0x00000200; // 
	public static uint			vCatchStation_Points_Report_Move       	= 0x00000400; // 
	public static uint			vCatchStation_Points_Report_MultiTouch 	= 0x00000800; // 
	public static ulong vCatchStation_CMD_Points_Reporter( ulong flags, ulong interval ) { return (vCatchStation_Command_Points_Reporter | flags | (interval / 10)); }

	public unsafe static int[] vCatchStation_GetMotion1x1(vCatchResult_Motion1x1 data)
	{
		int[] motion = new int[1];
		motion[0] = data.motion[0];
		return motion;
	}

	public unsafe static int[] vCatchStation_GetMotion4x3(vCatchResult_Motion4x3 data)
	{
		int[] motion = new int[12];
		for (int i=0; i<motion.Length; i++)
			motion[i] = data.motion[i];
		return motion;
	}

	public unsafe static int[] vCatchStation_GetMotion20x15(vCatchResult_Motion20x15 data)
	{
		int[] motion = new int[300];
		for (int i=0; i<motion.Length; i++)
			motion[i] = data.motion[i];
		return motion;
	}

	// request deinitialize the service to Middleware(vCatchStation)
	// wparam: zero
	// lparam: HWND of the application(game)
	// return: TRUE(1) if successful
	public static uint WM_vCatchStation_Deinitialize_vCatch		= (WM_USER + 0x3604);


	public static int CatchResult_idSensor_All  = -1;
	public static int CatchResult_idSensor_1    = 10;
	public static int CatchResult_idSensor_2    = 20;

	public enum vCatchStationStatusCode {
		No_Sensor           = 1,
		//	NG_USB              = 2,
		No_vCatchModule     = 20,
		vCatch_IsNotStarted = 21,
		vCatch_IsNotWorking = 22,
		Redid_vCatch        = 40, // DovCatch 다시 시작
		Stopped_Start            = 50,
		Stopped_ByCommand        = 50, // 사용자의 요청(vCatchStation_Command_Stop)으로 종료
		Stopped_ByTimeout        = 51, // 
		Stopped_ByWorkCompletion = 52, // 수행이 완료되어서 종료
		// Stopped_NotReady    = 62, // 센서와 준비작업에서 오류가 발생
		Stopped_NoSetting   = 63, // Setting 정보가 없음
		Stopped_LackOfData  = 64, // 센서로 부터 일부 정보를 받지 못함
		Stopped_ByOtherApp  = 65, // 다른 앱이 점유함
		No_Error = 0
	};

	public static uint WM_COPYDATA_DataType_SensorStatus = 0;
	[System.Serializable]
	public struct vCatchResult_SensorStatus
	{
		public ushort  	idModule;
		public ushort  	version;
		public uint 	msec;
		public int   	idSensor;
		public vCatchStationStatusCode code;
	}

	public static uint WM_COPYDATA_DataType_Degree = 10;
	[System.Serializable]
	public struct vCatchResult_Degree
	{
		public ushort  	idModule;
		public ushort  	version;
		public uint 	msec;
		public int   	idSensor;
		public float 	pos;   // -0.5 ~ 0.5
		public float 	width; // 0 ~ 1
	}

	public static uint WM_COPYDATA_DataType_Point = 20;
	public static int PointStatus_Down = 1;
	public static int PointStatus_Up   = 2;
	public static int PointStatus_Move = 4;
	[System.Serializable]
	public struct vCatchResult_Point
	{
		public ushort  	idModule;
		public ushort  	version;
		public uint 	msec;
		public ushort  	idPoint;
		public ushort  	status;
		public float 	posX;   // 0 ~ 1
		public float 	posY;   // 0 ~ 1
		public float 	width;  // 0 ~ 1
		public float 	height; // 0 ~ 1
	}

	public static uint WM_COPYDATA_DataType_SpeedPoint = 21;
	[System.Serializable]
	public struct vCatchResult_SpeedPoint
	{
		public ushort  	idModule;
		public ushort  	version;
		public uint 	msec;
		public ushort   idPoint;
		public ushort	status;
		public float 	posX;   // 0 ~ 1
		public float 	posY;   // 0 ~ 1
		public float 	width;  // 0 ~ 1
		public float 	height; // 0 ~ 1
		public float 	speed;  // km
	}

	public static uint WM_COPYDATA_DataType_Motion20x15	= 40;
	[System.Serializable]
	public unsafe struct vCatchResult_Motion20x15
	{
		public ushort idModule;
		public ushort version;
		public uint msec;
		public fixed int motion[300];
	}

	public static uint WM_COPYDATA_DataType_Motion4x3	= 41;
	[System.Serializable]
	public unsafe struct vCatchResult_Motion4x3
	{
		public ushort  idModule;
		public ushort  version;
		public uint msec;
		public fixed int motion[12];
	}

	public static uint WM_COPYDATA_DataType_Motion1x1	= 42;
	[System.Serializable]
	public unsafe struct vCatchResult_Motion1x1
	{
		public ushort  idModule;
		public ushort  version;
		public uint msec;
		public fixed int motion[1];
	}

	[System.Runtime.InteropServices.StructLayout(LayoutKind.Explicit)]  
	public struct vCatchResult
	{
		[System.Runtime.InteropServices.FieldOffset(0)]
		public vCatchResult_SensorStatus senser_status;
		[System.Runtime.InteropServices.FieldOffset(0)]
		public vCatchResult_Degree      resultDegree;
		[System.Runtime.InteropServices.FieldOffset(0)]
		public vCatchResult_Point       resultPoint;
		[System.Runtime.InteropServices.FieldOffset(0)]
		public vCatchResult_SpeedPoint  resultSpeedPoint;
		[System.Runtime.InteropServices.FieldOffset(0)]
		public vCatchResult_Motion20x15 resultMotion20x15;
		[System.Runtime.InteropServices.FieldOffset(0)]
		public vCatchResult_Motion4x3   resultMotion4x3;
		[System.Runtime.InteropServices.FieldOffset(0)]
		public vCatchResult_Motion1x1   resultMotion1x1;
	}
}

