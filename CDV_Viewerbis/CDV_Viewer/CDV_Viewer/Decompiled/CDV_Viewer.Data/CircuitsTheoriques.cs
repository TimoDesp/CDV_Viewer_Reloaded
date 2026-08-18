using System.Collections.Generic;
using CDV_Viewer.CsvBase;

namespace CDV_Viewer.Data;

public static class CircuitsTheoriques
{
	private const int iccArrMaxDefaut_nc = 0;

	private const int iccArrMaxDefaut_ite = 0;

	private const int iccArrMaxDefaut_um71_1700 = 0;

	private const int iccArrMaxDefaut_um71_2300 = 0;

	private const int iccArrMaxDefaut_um71_2000 = 0;

	private const int iccArrMaxDefaut_um71_2600 = 0;

	private const int iccArrMaxDefaut_tvm300_1700 = 200;

	private const int iccArrMaxDefaut_tvm300_2300 = 200;

	private const int iccArrMaxDefaut_tvm300_2000 = 200;

	private const int iccArrMaxDefaut_tvm300_2600 = 200;

	private const int iccArrMaxDefaut_tvm430_1700 = 320;

	private const int iccArrMaxDefaut_tvm430_2300 = 320;

	private const int iccArrMaxDefaut_tvm430_2000 = 320;

	private const int iccArrMaxDefaut_tvm430_2600 = 320;

	private const int iccArrMaxDefaut_sei_1700 = 320;

	private const int iccArrMaxDefaut_sei_2300 = 320;

	private const int iccArrMaxDefaut_sei_2000 = 320;

	private const int iccArrMaxDefaut_sei_2600 = 320;

	private static List<CircuitTheorique> _circuits = new List<CircuitTheorique>();

	private static CircuitTheorique _nc = new CircuitTheorique(CircuitType.NC, 0, choixCompensation: false, CompensationType.NON, 0, 0, 0, 0, 0);

	private static CircuitTheorique _ite = new CircuitTheorique(CircuitType.ITE, 3, choixCompensation: false, CompensationType.NON, 0, 0, 0, 0, 0);

	private static CircuitTheorique _um71_1700 = new CircuitTheorique(CircuitType.UM71, 1700, choixCompensation: true, CompensationType.NON, 100, 0, 0, 0, 0);

	private static CircuitTheorique _um71_2000 = new CircuitTheorique(CircuitType.UM71, 2000, choixCompensation: true, CompensationType.NON, 100, 0, 0, 0, 0);

	private static CircuitTheorique _um71_2300 = new CircuitTheorique(CircuitType.UM71, 2300, choixCompensation: true, CompensationType.NON, 100, 0, 0, 0, 0);

	private static CircuitTheorique _um71_2600 = new CircuitTheorique(CircuitType.UM71, 2600, choixCompensation: true, CompensationType.NON, 100, 0, 0, 0, 0);

	private static CircuitTheorique _tvm300_1700 = new CircuitTheorique(CircuitType.TVM300, 1700, choixCompensation: false, CompensationType.P_CONSTANT, 100, 100, 200, 500, 200);

	private static CircuitTheorique _tvm300_2000 = new CircuitTheorique(CircuitType.TVM300, 2000, choixCompensation: false, CompensationType.P_CONSTANT, 100, 100, 200, 500, 200);

	private static CircuitTheorique _tvm300_2300 = new CircuitTheorique(CircuitType.TVM300, 2300, choixCompensation: false, CompensationType.P_CONSTANT, 100, 100, 200, 500, 200);

	private static CircuitTheorique _tvm300_2600 = new CircuitTheorique(CircuitType.TVM300, 2600, choixCompensation: false, CompensationType.P_CONSTANT, 100, 100, 200, 450, 200);

	private static CircuitTheorique _tvm430_1700 = new CircuitTheorique(CircuitType.TVM430, 1700, choixCompensation: false, CompensationType.P_VARIABLE, 60, 160, 320, 800, 320);

	private static CircuitTheorique _tvm430_2000 = new CircuitTheorique(CircuitType.TVM430, 2000, choixCompensation: false, CompensationType.P_VARIABLE, 60, 160, 320, 800, 320);

	private static CircuitTheorique _tvm430_2300 = new CircuitTheorique(CircuitType.TVM430, 2300, choixCompensation: false, CompensationType.P_VARIABLE, 80, 160, 320, 800, 320);

	private static CircuitTheorique _tvm430_2600 = new CircuitTheorique(CircuitType.TVM430, 2600, choixCompensation: false, CompensationType.P_VARIABLE, 80, 160, 320, 800, 320);

	private static CircuitTheorique _sei_1700 = new CircuitTheorique(CircuitType.SEI, 1700, choixCompensation: false, CompensationType.P_VARIABLE, 60, 160, 320, 800, 320);

	private static CircuitTheorique _sei_2000 = new CircuitTheorique(CircuitType.SEI, 2000, choixCompensation: false, CompensationType.P_VARIABLE, 60, 160, 320, 800, 320);

	private static CircuitTheorique _sei_2300 = new CircuitTheorique(CircuitType.SEI, 2300, choixCompensation: false, CompensationType.P_VARIABLE, 80, 160, 320, 800, 320);

	private static CircuitTheorique _sei_2600 = new CircuitTheorique(CircuitType.SEI, 2600, choixCompensation: false, CompensationType.P_VARIABLE, 80, 160, 320, 800, 320);

	public static void Add(CircuitTheorique circuit)
	{
		_circuits.Add(circuit);
	}
}
