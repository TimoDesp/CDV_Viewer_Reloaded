using System.IO;

namespace CDV_Viewer.CsvBase;

public interface ICsvTable
{
	string Path { get; }

	void Load(string TempBaseDirectory);

	void LoadBinary(BinaryReader reader);

	void Link();

	void Save();

	void Clear();
}
