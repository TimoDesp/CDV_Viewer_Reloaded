namespace CDV_Viewer.Traitements;

public static class Calculs
{
	public static void OrderPk(ref int pk1, ref int pk2)
	{
		if (pk1 > pk2)
		{
			int num = pk2;
			pk2 = pk1;
			pk1 = num;
		}
	}

	public static int BoundedValue(int value, int min, int max)
	{
		if (value < min)
		{
			return min;
		}
		if (value > max)
		{
			return max;
		}
		return value;
	}

	public static void MinMax(int v1, int v2, out int min, out int max)
	{
		if (v1 > v2)
		{
			max = v1;
			min = v2;
		}
		else
		{
			min = v1;
			max = v2;
		}
	}

	public static void Swap(ref int v1, ref int v2)
	{
		int num = v1;
		v1 = v2;
		v2 = num;
	}
}
