package vector;

import java.io.Serializable;

public class RotationMatrix implements Serializable {
	public float[] Matrix; 
	public RotationMatrix()
	{
		
	}
	
	public RotationMatrix(float[] matrix)
	{
		Matrix = new float[9];
		for(int i = 0; i < 9; ++i)
		{
			Matrix[i] = matrix[i];
		}
	}
}
