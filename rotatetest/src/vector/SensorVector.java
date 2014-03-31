package vector;

import java.io.Serializable;

public class SensorVector implements Serializable {
	
	public float V1;
	public float V2;
	public float V3;
	
	public int TimeStamp;
	
	public int Type = 0;
	public SensorVector()
	{
		
	}
	
	public SensorVector(float v1, float v2, float v3, int type, int timeStamp)
	{
		V1 = v1;
		V2 = v2;
		V3 = v3;
		Type = type;
		this.TimeStamp = timeStamp;
	}
	
	
}
