using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace CDV_Viewer.Properties;

[CompilerGenerated]
[GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "15.7.0.0")]
internal sealed class Settings : ApplicationSettingsBase
{
	private static Settings defaultInstance = (Settings)SettingsBase.Synchronized(new Settings());

	public static Settings Default => defaultInstance;

	[UserScopedSetting]
	[DebuggerNonUserCode]
	[DefaultSettingValue("50")]
	public int ZoomMin
	{
		get
		{
			return (int)this["ZoomMin"];
		}
		set
		{
			this["ZoomMin"] = value;
		}
	}

	[UserScopedSetting]
	[DebuggerNonUserCode]
	[DefaultSettingValue("0.1")]
	public double ZoomMax
	{
		get
		{
			return (double)this["ZoomMax"];
		}
		set
		{
			this["ZoomMax"] = value;
		}
	}

	[UserScopedSetting]
	[DebuggerNonUserCode]
	[DefaultSettingValue("13")]
	public int NbVoies
	{
		get
		{
			return (int)this["NbVoies"];
		}
		set
		{
			this["NbVoies"] = value;
		}
	}

	[UserScopedSetting]
	[DebuggerNonUserCode]
	[DefaultSettingValue("10000")]
	public int EchelleX
	{
		get
		{
			return (int)this["EchelleX"];
		}
		set
		{
			this["EchelleX"] = value;
		}
	}

	[UserScopedSetting]
	[DebuggerNonUserCode]
	[DefaultSettingValue("7")]
	public int FontSize
	{
		get
		{
			return (int)this["FontSize"];
		}
		set
		{
			this["FontSize"] = value;
		}
	}

	[UserScopedSetting]
	[DebuggerNonUserCode]
	[DefaultSettingValue("500")]
	public int Margin
	{
		get
		{
			return (int)this["Margin"];
		}
		set
		{
			this["Margin"] = value;
		}
	}

	[UserScopedSetting]
	[DebuggerNonUserCode]
	[DefaultSettingValue("150")]
	public int LongueurVoieSecondaire
	{
		get
		{
			return (int)this["LongueurVoieSecondaire"];
		}
		set
		{
			this["LongueurVoieSecondaire"] = value;
		}
	}

	[UserScopedSetting]
	[DebuggerNonUserCode]
	[DefaultSettingValue("20")]
	public int ZoomVisibleCircuits
	{
		get
		{
			return (int)this["ZoomVisibleCircuits"];
		}
		set
		{
			this["ZoomVisibleCircuits"] = value;
		}
	}

	[UserScopedSetting]
	[DebuggerNonUserCode]
	[DefaultSettingValue("10")]
	public int ZoomTexteVisibleCircuits
	{
		get
		{
			return (int)this["ZoomTexteVisibleCircuits"];
		}
		set
		{
			this["ZoomTexteVisibleCircuits"] = value;
		}
	}

	[UserScopedSetting]
	[DebuggerNonUserCode]
	[DefaultSettingValue("12")]
	public int ZoomVisibleJoints
	{
		get
		{
			return (int)this["ZoomVisibleJoints"];
		}
		set
		{
			this["ZoomVisibleJoints"] = value;
		}
	}

	[UserScopedSetting]
	[DebuggerNonUserCode]
	[DefaultSettingValue("7")]
	public int ZoomTexteVisibleJoints
	{
		get
		{
			return (int)this["ZoomTexteVisibleJoints"];
		}
		set
		{
			this["ZoomTexteVisibleJoints"] = value;
		}
	}

	[UserScopedSetting]
	[DebuggerNonUserCode]
	[DefaultSettingValue("40")]
	public int ZoomTexteVisibleJonctions
	{
		get
		{
			return (int)this["ZoomTexteVisibleJonctions"];
		}
		set
		{
			this["ZoomTexteVisibleJonctions"] = value;
		}
	}

	[UserScopedSetting]
	[DebuggerNonUserCode]
	[DefaultSettingValue("5")]
	public int ZoomVisibleBalises
	{
		get
		{
			return (int)this["ZoomVisibleBalises"];
		}
		set
		{
			this["ZoomVisibleBalises"] = value;
		}
	}

	[UserScopedSetting]
	[DebuggerNonUserCode]
	[DefaultSettingValue("7")]
	public int OrdreVoie
	{
		get
		{
			return (int)this["OrdreVoie"];
		}
		set
		{
			this["OrdreVoie"] = value;
		}
	}

	[UserScopedSetting]
	[DebuggerNonUserCode]
	[DefaultSettingValue("6")]
	public int OrdreVoieSecondaire
	{
		get
		{
			return (int)this["OrdreVoieSecondaire"];
		}
		set
		{
			this["OrdreVoieSecondaire"] = value;
		}
	}

	[UserScopedSetting]
	[DebuggerNonUserCode]
	[DefaultSettingValue("5")]
	public int OrdreJonction
	{
		get
		{
			return (int)this["OrdreJonction"];
		}
		set
		{
			this["OrdreJonction"] = value;
		}
	}

	[UserScopedSetting]
	[DebuggerNonUserCode]
	[DefaultSettingValue("3")]
	public int OrdreNoeud
	{
		get
		{
			return (int)this["OrdreNoeud"];
		}
		set
		{
			this["OrdreNoeud"] = value;
		}
	}

	[UserScopedSetting]
	[DebuggerNonUserCode]
	[DefaultSettingValue("4")]
	public int OrdreCircuit
	{
		get
		{
			return (int)this["OrdreCircuit"];
		}
		set
		{
			this["OrdreCircuit"] = value;
		}
	}

	[UserScopedSetting]
	[DebuggerNonUserCode]
	[DefaultSettingValue("2")]
	public int OrdreJoint
	{
		get
		{
			return (int)this["OrdreJoint"];
		}
		set
		{
			this["OrdreJoint"] = value;
		}
	}

	[UserScopedSetting]
	[DebuggerNonUserCode]
	[DefaultSettingValue("1")]
	public int OrdreBalise
	{
		get
		{
			return (int)this["OrdreBalise"];
		}
		set
		{
			this["OrdreBalise"] = value;
		}
	}

	[UserScopedSetting]
	[DebuggerNonUserCode]
	[DefaultSettingValue("Arial")]
	public string Font
	{
		get
		{
			return (string)this["Font"];
		}
		set
		{
			this["Font"] = value;
		}
	}

	[UserScopedSetting]
	[DebuggerNonUserCode]
	[DefaultSettingValue("20")]
	public int SizeVoieAdjacente
	{
		get
		{
			return (int)this["SizeVoieAdjacente"];
		}
		set
		{
			this["SizeVoieAdjacente"] = value;
		}
	}

	[UserScopedSetting]
	[DebuggerNonUserCode]
	[DefaultSettingValue("Normal")]
	public string CdVDrawingMode
	{
		get
		{
			return (string)this["CdVDrawingMode"];
		}
		set
		{
			this["CdVDrawingMode"] = value;
		}
	}

	private void SettingChangingEventHandler(object sender, SettingChangingEventArgs e)
	{
	}

	private void SettingsSavingEventHandler(object sender, CancelEventArgs e)
	{
	}
}
