// Generate Id:e1343ba8-54ac-445b-8823-5a85e6d77054
using UnityEngine;

namespace MsbFramework.UI
{
	public partial class UIFixedJoystick : QFramework.IController
	{
		public UnityEngine.UI.Image Background;
		
		public UnityEngine.UI.Image Handle;
		
		public UnityEngine.UI.Button Btn1;
		
		public UnityEngine.UI.Button Btn2;
		
		QFramework.IArchitecture QFramework.IBelongToArchitecture.GetArchitecture()=>MsbFramework.GameMainApp.Interface;
	}
}
