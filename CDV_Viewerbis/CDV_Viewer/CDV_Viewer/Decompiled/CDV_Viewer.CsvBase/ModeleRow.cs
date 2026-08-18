using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using CDV_Viewer.Data.SIGObjects;

namespace CDV_Viewer.CsvBase;

public class ModeleRow : BaseRow
{
	public int CIRCUIT;

	public int JOINT_E;

	public int JOINT_S;

	public string TOURNEE;

	private string POINTS;

	public long STAMP;

	internal bool updated;

	public SIGModele SIGModele { get; private set; }

	public void SetPointsAndCondos(string points)
	{
		points = points.Replace('-', ';');
		List<Point> list = new List<Point>();
		List<int> list2 = new List<int>();
		try
		{
			string[] array = points.Split(';');
			int num = array.Length - 2;
			int num2 = 0;
			int num3 = 0;
			while (num2 <= num)
			{
				bool flag = array[num2][0] != '*';
				int num4 = int.Parse(array[num2 + 1]);
				if (num4 < 0)
				{
					num4 = 0;
				}
				int num5;
				if (flag)
				{
					num5 = int.Parse(array[num2]);
					if ((num3 & 1) == 0 && num2 + 2 < num)
					{
						list2.Add(num5);
					}
				}
				else
				{
					num5 = int.Parse(array[num2].Substring(1));
				}
				list.Add(new Point(num5, num4));
				num2 += 2;
				num3++;
			}
		}
		catch
		{
			list.Clear();
			list2.Clear();
		}
		SIGModele.Points = list;
		SIGModele.Condos = list2;
	}

	public void SetPointsAndCondos(BinaryReader reader)
	{
		int num = BaseRow.ReadId(reader);
		List<Point> list = new List<Point>(num);
		List<int> list2 = new List<int>();
		for (int i = 0; i < num; i++)
		{
			int num2 = reader.ReadInt16();
			int num3 = reader.ReadInt16();
			if (num3 < 0)
			{
				list2.Add(num2);
			}
			list.Add(new Point(num2, Math.Abs(num3)));
		}
		SIGModele.Points = list;
		SIGModele.Condos = list2;
	}

	public void WritePointsAndCondos(BinaryWriter writer)
	{
		Point[] array = SIGModele.Points.ToArray();
		Dictionary<int, int> dictionary = SIGModele.Condos.ToDictionary((int c) => c);
		int value = array.Count();
		BaseRow.WriteId(writer, value);
		Point[] array2 = array;
		for (int num = 0; num < array2.Length; num++)
		{
			Point point = array2[num];
			writer.Write((short)point.X);
			int num2 = (dictionary.ContainsKey(point.X) ? point.Y : (-point.Y));
			writer.Write((short)num2);
		}
	}

	private ModeleRow()
	{
	}

	internal ModeleRow(int id)
	{
		_id = id;
	}

	internal void SetId(int id)
	{
		_id = id;
		SIGModele.ID = id;
	}

	public static ModeleRow FromCsv(string[] fields)
	{
		int id = BaseRow.GetId(fields);
		ModeleRow modeleRow = new ModeleRow(id)
		{
			CIRCUIT = Convert.ToInt32(fields[1]),
			JOINT_E = Convert.ToInt32(fields[2]),
			JOINT_S = Convert.ToInt32(fields[3]),
			TOURNEE = fields[4],
			POINTS = fields[5]
		};
		if (fields.Length > 6)
		{
			modeleRow.STAMP = Convert.ToInt64(fields[6]);
		}
		modeleRow.SIGModele = new SIGModele(id);
		modeleRow.SetPointsAndCondos(modeleRow.POINTS);
		return modeleRow;
	}

	public static ModeleRow FromBinary(BinaryReader reader)
	{
		int id = BaseRow.ReadId(reader);
		ModeleRow modeleRow = new ModeleRow(id);
		modeleRow.CIRCUIT = BaseRow.ReadId(reader);
		modeleRow.JOINT_E = BaseRow.ReadId(reader);
		modeleRow.JOINT_S = BaseRow.ReadId(reader);
		modeleRow.TOURNEE = "";
		modeleRow.POINTS = "";
		modeleRow.STAMP = reader.ReadInt64();
		modeleRow.SIGModele = new SIGModele(id);
		modeleRow.SetPointsAndCondos(reader);
		return modeleRow;
	}

	public override string ToCsv()
	{
		return base.ID.ToString() + ";" + CIRCUIT + ";" + JOINT_E + ";" + JOINT_S + ";" + TOURNEE + ";" + POINTS + ";" + STAMP;
	}

	public override void ToBinaryStream(BinaryWriter writer)
	{
		BaseRow.WriteId(writer, base.ID);
		BaseRow.WriteId(writer, CIRCUIT);
		BaseRow.WriteId(writer, JOINT_E);
		BaseRow.WriteId(writer, JOINT_S);
		writer.Write(STAMP);
		WritePointsAndCondos(writer);
	}
}
