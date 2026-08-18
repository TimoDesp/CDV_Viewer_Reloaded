using System.Collections.Generic;
using CDV_Viewer.CsvBase;

namespace CDV_Viewer.Data;

public class JointsTheoriques
{
	private static List<JointTheorique> _joints = new List<JointTheorique>();

	private static JointTheorique _inc = new JointTheorique(JointType.INC, 0);

	private static JointTheorique _ji = new JointTheorique(JointType.JI, 0);

	private static JointTheorique _cc = new JointTheorique(JointType.CC, 15);

	private static JointTheorique _sv = new JointTheorique(JointType.SV, 10);

	private static JointTheorique _svac = new JointTheorique(JointType.SVAC, 10);

	public static void Add(JointTheorique joint)
	{
		_joints.Add(joint);
	}

	public static JointTheorique GetJointTheorique(JointType type)
	{
		return _joints.Find((JointTheorique joint) => joint.Type == type);
	}
}
