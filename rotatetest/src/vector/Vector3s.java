package vector;

import java.io.Serializable;

public class Vector3s implements Serializable {
	/**
	 * 
	 */
	private static final long serialVersionUID = 1L;
	public float X;
	public float Y;
	public float Z;
	public Vector3s()
	{
		X = 0.f;
		Y = 0.f;
		Z = 0.f;
	}
	
	public Vector3s(float x, float y, float z)
	{
		X = x;
		Y = y;
		Z = z;
	}
}
