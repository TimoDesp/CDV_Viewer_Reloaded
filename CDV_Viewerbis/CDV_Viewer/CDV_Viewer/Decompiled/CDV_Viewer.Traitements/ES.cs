using System;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace CDV_Viewer.Traitements;

public class ES
{
	[DllImport("User32.dll")]
	protected static extern short GetAsyncKeyState(Keys vKey);

	public static bool GetStateKey(Keys Key)
	{
		return (GetAsyncKeyState(Key) & 0x8000) != 0;
	}

	public static bool IsValidMACAdress(string macAdress)
	{
		NetworkInterface[] allNetworkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
		for (int i = 0; i < allNetworkInterfaces.Length; i++)
		{
			if (BitConverter.ToString(allNetworkInterfaces[i].GetPhysicalAddress().GetAddressBytes()) == macAdress)
			{
				return true;
			}
		}
		return false;
	}
}
