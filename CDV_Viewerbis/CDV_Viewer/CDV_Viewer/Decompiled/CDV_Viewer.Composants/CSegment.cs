using System.Drawing;

namespace CDV_Viewer.Composants;

public class CSegment
{
	private CVoie _support;

	private int _pkD;

	private int _pkF;

	public CVoie Support => _support;

	public int PkD => _pkD;

	public int PkF => _pkF;

	public Point PtD => _support.GetPoint(_pkD);

	public Point PtF => _support.GetPoint(_pkF);

	public Point[] Path => _support.GetPath(_pkD, _pkF);

	public CSegment(CVoie support, int pkD, int pkF)
	{
		_support = support;
		_pkD = pkD;
		_pkF = pkF;
	}
}
