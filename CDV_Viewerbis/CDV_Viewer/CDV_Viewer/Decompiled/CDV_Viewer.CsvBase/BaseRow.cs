using System;
using System.IO;
using System.Text;

namespace CDV_Viewer.CsvBase;

public abstract class BaseRow : IComparable<BaseRow>
{
	internal const char CSV_SEPARATOR = ';';

	protected int _id = -1;

	public int ID => _id;

	public abstract string ToCsv();

	public abstract void ToBinaryStream(BinaryWriter writer);

	protected static int GetId(string[] fields)
	{
		return Convert.ToInt32(fields[0]);
	}

	protected static int ReadId(BinaryReader reader)
	{
		int num = 0;
		int num2 = 0;
		byte b;
		do
		{
			if (num2 == 35)
			{
				throw new FormatException("Format_Bad7BitInt32");
			}
			b = reader.ReadByte();
			num |= (b & 0x7F) << num2;
			num2 += 7;
		}
		while ((b & 0x80) != 0);
		return num;
	}

	protected static void WriteId(BinaryWriter writer, int value)
	{
		uint num;
		for (num = (uint)value; num >= 128; num >>= 7)
		{
			writer.Write((byte)(num | 0x80));
		}
		writer.Write((byte)num);
	}

	protected static string ReadAsciiString(BinaryReader reader)
	{
		int count = reader.ReadByte();
		byte[] bytes = reader.ReadBytes(count);
		return Encoding.ASCII.GetString(bytes);
	}

	protected static void WriteAsciiString(BinaryWriter writer, string value)
	{
		byte[] bytes = Encoding.ASCII.GetBytes(value);
		writer.Write((byte)bytes.Length);
		writer.Write(bytes);
	}

	protected static int ReadPK(BinaryReader reader)
	{
		byte b = reader.ReadByte();
		return (reader.ReadInt16() << 8) | b;
	}

	protected static void WritePK(BinaryWriter writer, int value)
	{
		byte value2 = (byte)(value & 0xFF);
		int num = value >> 8;
		writer.Write(value2);
		writer.Write((short)num);
	}

	public int CompareTo(BaseRow other)
	{
		return ID.CompareTo(other.ID);
	}
}
