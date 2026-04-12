// Generate Id:abcb5332-19fa-4db6-be92-85bf4922b622
using UnityEngine;

namespace MsbFramework.UI
{
	public partial class UILogin : QFramework.IController
	{
		
		public  TMPro.TMP_InputField InputAcc;
		
		public TMPro.TMP_InputField InputPas;
		
		public UnityEngine.UI.Button BtnLogin;
		
		QFramework.IArchitecture QFramework.IBelongToArchitecture.GetArchitecture()=>MsbFramework.GameMainApp.Interface;
	}
}
