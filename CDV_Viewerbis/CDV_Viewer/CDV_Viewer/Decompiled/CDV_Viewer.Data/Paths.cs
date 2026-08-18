using System.IO;
using System.Windows.Forms;

namespace CDV_Viewer.Data;

public static class Paths
{
	private static string ApplicationFolder = Path.GetDirectoryName(Application.ExecutablePath);

	public static string Cle = ApplicationFolder + "\\keys\\key";

	public static string DataFolder = ApplicationFolder + "\\data";

	public static string TempDataFolder = DataFolder + "\\temp";

	public static string DescriptionFile = TempDataFolder + "\\description.txt";

	public static string Schema = TempDataFolder + "\\bdd.xml";

	public static string TVoiesTimon = TempDataFolder + "\\topo\\voies_timon.csv";

	public static string TLignes = TempDataFolder + "\\topo\\lignes.csv";

	public static string TVoies = TempDataFolder + "\\topo\\voies.csv";

	public static string TPosVoies = TempDataFolder + "\\topo\\pos_voies.csv";

	public static string TBranches = TempDataFolder + "\\topo\\branches.csv";

	public static string TBalises = TempDataFolder + "\\topo\\balises.csv";

	public static string TCircuits = TempDataFolder + "\\signalo\\circuits.csv";

	public static string TJoints = TempDataFolder + "\\signalo\\joints.csv";

	public static string TJointsCircuits = TempDataFolder + "\\signalo\\joints_circuits.csv";

	public static string TModeles = TempDataFolder + "\\signalo\\modeles.csv";

	public static string ExportMGV_TLignes = "\\T_LIGNE.CSV";

	public static string ExportMGV_TVoies = "\\T_VOIE.CSV";

	public static string ExportMGV_TNoeuds = "\\T_NDV.CSV";

	public static string ExportMGV_TBranches = "\\T_BR_NDV.CSV";

	public static string ExportMGV_TCircuits = "\\T_CDV.CSV";

	public static string ExportMGV_TJoints = "\\T_JOINT.CSV";

	public static string ExportMGV_TSegments = "\\T_SEG_CDV.CSV";

	public static string ExportMGV_TModeles = "\\T_MODEL_CDV.CSV";

	public static string ExportUltrapot = "\\LISTE_CDV.CSV";

	public static string ExportTIMON_CDV = "\\T_CDVS_TIMON.CSV";

	public static string ExportTIMON_MODELES = "\\T_MODEL_CDV_TIMON.CSV";

	public static string ExportTIMON_SEGMENTS = "\\T_SEG_CDVS_TIMON.CSV";

	public static string ExportLoc = "\\LOC.CSV";

	public static string CDirectory = ApplicationFolder + "\\config";

	public static string CGraphique = CDirectory + "\\graph.xml";

	public static string CCouleurs = CDirectory + "\\colors.xml";

	public static string CEtudes = CDirectory + "\\etudes.ini";

	public static string CStart = CDirectory + "\\start.txt";

	public static string CSchema = CDirectory + "\\bdd.xml";

	public static string PDirectory = ApplicationFolder + "\\preferences";

	public static string PAffichage = PDirectory + "\\affichage.xml";

	public static string LocFolder = "D:\\Outils_SOL\\LocNG";

	public static string LocExe = LocFolder + "\\LOC_NG1_B.exe";

	public static string LocLastPos = LocFolder + "\\JDB\\LastPos.txt";

	public static string LocLignesFolder = LocFolder + "\\LIGTXT";

	public static string HelpFolder = ApplicationFolder + "\\help";

	public static string HelpXml = HelpFolder + "\\help.xml";
}
