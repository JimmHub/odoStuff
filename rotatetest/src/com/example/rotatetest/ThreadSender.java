package com.example.rotatetest;
import java.io.DataInputStream;
import java.io.DataOutputStream;
import java.io.IOException;
import java.io.InterruptedIOException;
import java.io.ObjectInputStream;
import java.io.ObjectOutputStream;
import java.io.OutputStream;
import java.net.InetSocketAddress;
import java.net.Socket;
import java.net.SocketAddress;
import java.net.SocketException;
import java.net.UnknownHostException;
import java.nio.ByteBuffer;
import java.util.Date;
import java.util.Vector;
import java.util.logging.Logger;

import vector.RotationMatrix;
import vector.SensorPack;
import vector.SensorVector;
import vector.Vector3s;



import android.hardware.Sensor;
import android.hardware.SensorEvent;
import android.hardware.SensorEventListener;
import android.hardware.SensorManager;
import android.support.v4.util.LogWriter;
import android.util.Log;
import android.widget.EditText;
import android.widget.TextView;



public class ThreadSender implements SensorEventListener {

	Socket socket;
	OutputStream outputStream;
	DataInputStream dataInputStream;
	ObjectInputStream oInput;
	ObjectOutputStream oOutput;
	SensorManager sensorManager;
	Sensor rSensor;
	TextView text1;
	public boolean isConnected = false;
	public SensorEventListener sensorListener;
	
	String ip = " ";
	int port = 4343;
	
	int TYPEACC = 1;
	int TYPEMAGN = 2;
	int TYPEGYR = 3;
	
	SensorVector accVector;
	SensorVector magnetVector;
	
	long oldTime = 0;
	
	public ThreadSender(SensorManager manager,  TextView t1, final String ip, final int port)
	{
		sensorManager = manager;
		rSensor = sensorManager.getDefaultSensor(Sensor.TYPE_ORIENTATION);
		int timeStamp = (int)System.currentTimeMillis();
		accVector = new SensorVector(0, sensorManager.GRAVITY_EARTH, 0, 1, timeStamp);
		magnetVector = new SensorVector(sensorManager.MAGNETIC_FIELD_EARTH_MIN, sensorManager.MAGNETIC_FIELD_EARTH_MAX, sensorManager.MAGNETIC_FIELD_EARTH_MIN, 1, timeStamp);
		oldTime = new Date().getTime();
		this.ip = ip;
		this.port = port;
		//sensorManager.registerListener(this, rSensor, SensorManager.SENSOR_DELAY_FASTEST);
		/*sensorListener = new SensorEventListener() {
			
			@Override
			public void onSensorChanged(SensorEvent event) {
				Sensor sensr = event.sensor;
				int type = 0;
				int sensrtype = sensr.getType();
				switch(sensrtype)
				{
					case Sensor.TYPE_ACCELEROMETER:
					{
						type = TYPEACC;
						SensorVector vect = new SensorVector(event.values[0], event.values[1], event.values[2], type);
						accVector = vect;
					}
					break;
					case Sensor.TYPE_MAGNETIC_FIELD:
					{
						type = TYPEMAGN;
						SensorVector vect = new SensorVector(event.values[0], event.values[1], event.values[2], type);
						magnetVector = vect;
					}
					break;
					case Sensor.TYPE_GYROSCOPE:
					{
						type = TYPEGYR;
						SensorVector vect = new SensorVector(event.values[0], event.values[1], event.values[2], type);
	
					}
					break;
					
				}
				/*if(type != 0)
				{
					float[] R = new float[9];
					float[] I = new float[9];
					float[] gravity = new float[]{pack.AccelerometerVector.V1, pack.AccelerometerVector.V2, pack.AccelerometerVector.V3};
			    	float[] geomagnetic = new float[]{pack.MagnetometerVector.V1, pack.MagnetometerVector.V2, pack.MagnetometerVector.V3};
			    	
					sensorManager.getRotationMatrix(R, I, gravity, geomagnetic);
					RotationMatrix m = new RotationMatrix(R);
					SendMatrix(m);
				}
				
				
			}
			
			@Override
			public void onAccuracyChanged(Sensor sensor, int accuracy) {
				
			}
		};*/
		
		
		
		sensorManager.registerListener(this, sensorManager.getDefaultSensor(Sensor.TYPE_ACCELEROMETER), SensorManager.SENSOR_DELAY_GAME);
		sensorManager.registerListener(this, sensorManager.getDefaultSensor(Sensor.TYPE_MAGNETIC_FIELD), SensorManager.SENSOR_DELAY_GAME);
		sensorManager.registerListener(this, sensorManager.getDefaultSensor(Sensor.TYPE_GYROSCOPE), SensorManager.SENSOR_DELAY_GAME);
		
		/*new Thread()
		  {
				@Override
				public void run()
				{
					float[] R;
					float[] I;
					float[] gravity;
			    	float[] geomagnetic;
			    	
					while(true)
					{
						try
						{
							if(isConnected)
							{
								R = new float[9];
								I = new float[9];
								gravity = new float[]{accVector.V1, accVector.V2, accVector.V3};
						    	geomagnetic = new float[]{magnetVector.V1, magnetVector.V2, magnetVector.V3};
						    	
								sensorManager.getRotationMatrix(R, I, gravity, geomagnetic);
								RotationMatrix m = new RotationMatrix(R);
								SendMatrix(m);
								
							}
							//sleep(20);
						}
						catch (Exception e) 
						{
							// TODO Auto-generated catch block
							txt.setText(e.getStackTrace().toString());
						}
					}
				}
		  }.start();
		 */
		text1 = t1;
		socket = new Socket();
		try 
		{
			int n = 4;
			text1.setText("connecting...");
			new NetAsyncSender()
			{
				@Override
				protected String doInBackground(Socket... params)
				{
					Socket socket = params[0];
					DataInputStream inputStream;
					ObjectOutputStream oOutput;
					try
					{
						Log.w("connction", "connecting...");
						socket.connect(new InetSocketAddress(ip, port), 8000);
						Log.w("connction", "connected!");
					}
					catch(InterruptedIOException e)      
					{
						return "timeout";
					} catch (SocketException e) {
						// TODO Auto-generated catch block
						return e.getMessage();
					} catch (NumberFormatException e) {
						// TODO Auto-generated catch block
						return e.getMessage();
					} catch (IOException e) {
						// TODO Auto-generated catch block
						return e.getMessage();
					}
					
					
					return "OK";
			    	
				}
				
				@Override public void onPostExecute(String result)
				  {
					  try
				      {
						  Log.w("postExec", result);
						  //text1.setText(result);
						  text1.setText("connected");
						  outputStream = socket.getOutputStream();
						  dataInputStream = new DataInputStream(socket.getInputStream());
						  oOutput = new ObjectOutputStream(outputStream);
						  isConnected = true;
						  Log.w("postExec", "connection established!");
						  
						  
				      }
				      catch(Exception e)
				      {
				    	  Log.w("postExec", e.getMessage());
				      }
				  }
			}.execute(socket);
			//socket.connect(new InetSocketAddress("127.0.0.1", 4343), 3000);
			
			
		}
		catch(Exception e)
		{
			
		}
		
	
	}
	
	
	public void run()
	{
		int test = 4;
	}
	
	@Override
	public void onAccuracyChanged(Sensor sensor, int accuracy) {
		// TODO Auto-generated method stub
		
	}
	
	@Override
	public void onSensorChanged(SensorEvent event)
	{
		//Log.w("sensorChange", "changed");
		int type = 0;
		int sensrtype = event.sensor.getType();
		int timeStamp = (int)System.currentTimeMillis();
		switch(sensrtype)
		{
			case Sensor.TYPE_ACCELEROMETER:
			{
				type = TYPEACC;
				SensorVector vect = new SensorVector(event.values[0], event.values[1], event.values[2], type, timeStamp);
				//accVector = vect;
				SendVector(vect);
				long newTime = new Date().getTime();
				//clockTxt.setText((int)(newTime - oldTime));
				oldTime = newTime;
				
			}
			break;
			case Sensor.TYPE_MAGNETIC_FIELD:
			{
				type = TYPEMAGN;
				SensorVector vect = new SensorVector(event.values[0], event.values[1], event.values[2], type, timeStamp);
				SendVector(vect);
				//magnetVector = vect;
			}
			break;
			case Sensor.TYPE_GYROSCOPE:
			{
				type = TYPEGYR;
				SensorVector vect = new SensorVector(event.values[0], event.values[1], event.values[2], type, timeStamp);
				SendVector(vect);

			}
			break;
		}
	}
	
	/*public void onSensorChanged2(SensorEvent event)
	{
		if(isConnected)
		{
			try
			{
				final SensorEvent e = event;
				new NetAsyncSender()
				{
					@Override
					protected String doInBackground(Socket... params)
					{
						Socket socket = params[0];
						try {
							//text1.setText(String.valueOf(e.values[0]));
							//text2.setText(String.valueOf(e.values[1]));
							//text3.setText(String.valueOf(e.values[2]));
							oOutput.writeObject(new vector.Vector3s(e.values[0], e.values[1], e.values[2]));
							//text1.setText("sent");
						} catch (Exception e) {
							// TODO Auto-generated catch block
							txt.setText("snsr:" + e.getStackTrace().toString());
						}
						return "";
					}
					
					@Override public void onPostExecute(String result)
					  {
						  try
					      {
							  txt.setText(result);
					      }
					      catch(Exception e)
					      {
					    	  e.printStackTrace();
					      }
					  }
				}.execute(socket);
			}
			catch(Exception e1)
			{
				txt.setText(e1.getMessage());
			}
		}
		
	}
	*/
	public void SendVector(SensorVector vct)
	{
		if(isConnected)
		{
			
			
			
			
			ObjectOutputStream out = oOutput;
			try {
				//old
				//out.writeObject(vct);
				byte[] writeBuffer = this.SensorVectorToBuffer(vct);
				outputStream.write(writeBuffer, 0, writeBuffer.length);
				//Log.w("SendVector", "Length: ".concat(String.valueOf(writeBuffer.length)).concat(" Type: ").concat(String.valueOf(writeBuffer[0]))) ;
			} catch (IOException e) {
				Log.e("SendVector", e.getMessage());
				// TODO Auto-generated catch block
				
			}
		}
	}
	
	public byte[] SensorVectorToBuffer(SensorVector vect)
	{
		byte[] val0 = ByteBuffer.allocate(4).putFloat(vect.V1).array();
		byte[] val1 = ByteBuffer.allocate(4).putFloat(vect.V2).array();
		byte[] val2 = ByteBuffer.allocate(4).putFloat(vect.V3).array();
		
		byte[] res = new byte[] {
			(byte)vect.Type,
			(byte)(vect.TimeStamp >>> 24),
			(byte)(vect.TimeStamp >>> 16),
			(byte)(vect.TimeStamp >>> 8),
			(byte)(vect.TimeStamp),
			val0[0],
			val0[1],
			val0[2],
			val0[3],
			val1[0],
			val1[1],
			val1[2],
			val1[3],
			val2[0],
			val2[1],
			val2[2],
			val2[3]
		};
		
		return res;
	}
	
	public void SendVector2(final SensorVector vector)
	{
		if(isConnected)
		{
			try
			{
				new NetAsyncSender()
				{
					@Override
					protected String doInBackground(Socket... params)
					{
						try 
						{
							SensorVector send = new SensorVector(vector.V1, vector.V2, vector.V3, vector.Type, 2);
							oOutput.writeObject(send);
						} catch (Exception e) {
							// TODO Auto-generated catch block
							
						}
						return "";
					}
					
					@Override public void onPostExecute(String result)
					  {
						  try
					      {
							
					      }
					      catch(Exception e)
					      {
					    	  e.printStackTrace();
					      }
					  }
				}.execute(socket);
			}
			catch(Exception e1)
			{
				
			}
		}
	}
	
	public void SendMatrix(RotationMatrix matrix)
	{
		if(isConnected)
		{
			try
			{
				final RotationMatrix send = new RotationMatrix(matrix.Matrix);
				new NetAsyncSender()
				{
					@Override
					protected String doInBackground(Socket... params)
					{
						Socket socket = params[0];
						try 
						{
							oOutput.writeObject(send);
						} catch (Exception e) {
							// TODO Auto-generated catch block
						
						}
						return "";
					}
					
					@Override public void onPostExecute(String result)
					  {
						  /*try
					      {
							  txt.setText(result);
					      }
					      catch(Exception e)
					      {
					    	  e.printStackTrace();
					      }*/
					  }
				}.execute(socket);
			}
			catch(Exception e1)
			{
			
			}
		}
	}
	
	
	public void onResume() {
		// register this class as a listener for the orientation and
	    // accelerometer sensors
		sensorManager.registerListener(this, sensorManager.getDefaultSensor(Sensor.TYPE_ACCELEROMETER), SensorManager.SENSOR_DELAY_FASTEST);
		sensorManager.registerListener(this, sensorManager.getDefaultSensor(Sensor.TYPE_MAGNETIC_FIELD), SensorManager.SENSOR_DELAY_FASTEST);
		sensorManager.registerListener(this, sensorManager.getDefaultSensor(Sensor.TYPE_GYROSCOPE), SensorManager.SENSOR_DELAY_FASTEST);
		
	}

	public void onPause() {
	    // unregister listener
	    sensorManager.unregisterListener(this);
	    sensorManager.unregisterListener(this);
	    sensorManager.unregisterListener(this);
	}
	
	public void Disconnect()
	{
		try
		{
			text1.setText("disconnecting...");
			try {
				this.outputStream.close();
			} catch (Exception e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			}
			try {
				this.oInput.close();
			} catch (Exception e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			}
			try {
				this.oOutput.close();
			} catch (Exception e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			}
			try {
				this.socket.close();
			} catch (Exception e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			}////
			this.isConnected = false;
			text1.setText("disconnected!");
		}
		catch(Exception ex)
		{
			text1.setText("errDisconnected");
			ex.printStackTrace();
		}
	
	}
	
}
