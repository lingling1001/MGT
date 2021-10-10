using UnityEngine;
using UnityEngine.UI;
using TMPro;

//自动生成于：9/24/2021 3:42:47 PM
	public partial class UITimeInfo
	{

		private Button m_Btn_Change;
		private TextMeshProUGUI m_Txt_Speed;
		private TextMeshProUGUI m_Txt_Time;
		private TextMeshProUGUI m_Txt_TimeReal;
		private Button m_Btn_Pause;
		private Button m_Btn_Start;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_Btn_Change = autoBindTool.GetBindComponent<Button>(0);
			m_Txt_Speed = autoBindTool.GetBindComponent<TextMeshProUGUI>(1);
			m_Txt_Time = autoBindTool.GetBindComponent<TextMeshProUGUI>(2);
			m_Txt_TimeReal = autoBindTool.GetBindComponent<TextMeshProUGUI>(3);
			m_Btn_Pause = autoBindTool.GetBindComponent<Button>(4);
			m_Btn_Start = autoBindTool.GetBindComponent<Button>(5);
		}
	}
