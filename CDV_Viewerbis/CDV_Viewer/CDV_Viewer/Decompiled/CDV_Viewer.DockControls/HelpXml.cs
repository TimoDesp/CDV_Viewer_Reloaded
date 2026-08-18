using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using CDV_Viewer.Data;

namespace CDV_Viewer.DockControls;

[Serializable]
public class HelpXml
{
	public List<HelpPage> Pages;

	public static HelpXml Load()
	{
		XmlSerializer xmlSerializer = new XmlSerializer(typeof(HelpXml));
		FileStream fileStream = File.OpenRead(Paths.HelpXml);
		HelpXml result;
		try
		{
			result = (HelpXml)xmlSerializer.Deserialize(fileStream);
		}
		catch
		{
			fileStream.Close();
			fileStream.Dispose();
			throw new Exception();
		}
		fileStream.Close();
		fileStream.Dispose();
		return result;
	}

	public void Save()
	{
		XmlSerializer xmlSerializer = new XmlSerializer(typeof(HelpXml));
		FileStream fileStream = File.Create(Paths.HelpXml);
		xmlSerializer.Serialize(fileStream, this);
		fileStream.Close();
		fileStream.Dispose();
	}
}
