using System.Collections.Generic;
using System.IO;
using System.Linq;
using CDV_Viewer.Data.SIGObjects;

namespace CDV_Viewer.CsvBase;

public class TableCircuits : BaseTable<CircuitRow>
{
	public override string Path => "signalo\\circuits.csv";

	public override string Header => "ID;NOM;TYPE;FREQUENCE;COMPENSATION;POINTS;PAS_REEL;ICC_MIN;N_FUITE_LONG_ARR;CALCUL_CONFORME;I_FUITE_MAX;DIAPHONIE_MAX";

	public override CircuitRow RowFromCsv(string[] csvFields)
	{
		return CircuitRow.FromCsv(csvFields);
	}

	public override CircuitRow RowFromBinary(BinaryReader reader)
	{
		return CircuitRow.FromBinary(reader);
	}

	public override void Link()
	{
		foreach (CircuitRow value in _items.Values)
		{
			value.SIGCircuit.UnLink();
		}
	}

	public int Create(SIGCircuit circuit)
	{
		int result = (circuit.ID = FreeId());
		CircuitRow row = new CircuitRow(circuit);
		Add(row);
		return result;
	}

	internal List<SIGCircuit> SigCircuits()
	{
		return _items.Values.Select((CircuitRow c) => c.SIGCircuit).ToList();
	}

	internal SIGCircuit SigCircuit(int id)
	{
		if (TryGetValue(id, out var row))
		{
			return row.SIGCircuit;
		}
		return null;
	}
}
