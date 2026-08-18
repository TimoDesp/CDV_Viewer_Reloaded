using System;
using System.Xml.Serialization;

namespace CDV_Viewer.DockControls;

[Serializable]
public class HelpPage
{
	[XmlAttribute]
	public string Nom;

	[XmlAttribute]
	public string Path;
}
