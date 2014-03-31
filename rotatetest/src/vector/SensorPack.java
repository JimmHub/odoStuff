package vector;

import java.io.Serializable;


public class SensorPack implements Serializable {
	
	public SensorVector AccelerometerVector;
	public SensorVector MagnetometerVector;
	public SensorVector GyroscopeVector;
	
	public SensorPack()
	{	
	}
	
}
