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
import java.util.Vector;

import android.hardware.Sensor;
import android.hardware.SensorEvent;
import android.hardware.SensorEventListener;
import android.hardware.SensorManager;
import android.widget.EditText;
import android.widget.TextView;



public class CopyOfThreadSender implements SensorEventListener {

	Socket socket;
	OutputStream outputStream;
	DataInputStream dataInputStream;
	ObjectInputStream oInput;
	ObjectOutputStream oOutput;
	SensorManager sensorManager;
	Sensor rSensor;
	TextView text1;
	TextView text2;
	TextView text3;
	EditText txt;
	public boolean isConnected = false;
	public CopyOfThreadSender(SensorManager manager,  TextView t1, TextView t2, TextView t3, EditText txt)
	{
		sensorManager = manager;
		rSensor = sensorManager.getDefaultSensor(Sensor.TYPE_ORIENTATION);
		sensorManager.registerListener(this,
		        rSensor,
		        SensorManager.SENSOR_DELAY_NORMAL);
		text1 = t1;
		text2 = t2;
		text3 = t3;
		this.txt = txt;
		socket = new Socket();
		try 
		{
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
						socket.connect(new InetSocketAddress("78.140.61.51", 4343), 3000);
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
					
					
					return "";
			    	
				}
				
				@Override public void onPostExecute(String result)
				  {
					  try
				      {
						  //text1.setText(result);
						  text1.setText("connected");
						  outputStream = socket.getOutputStream();
						  dataInputStream = new DataInputStream(socket.getInputStream());
						  oOutput = new ObjectOutputStream(outputStream);
						  isConnected = true;
				      }
				      catch(Exception e)
				      {
				    	  //txt.setText(e.getMessage());
				    	  
				      }
				  }
			}.execute(socket);
			//socket.connect(new InetSocketAddress("127.0.0.1", 4343), 3000);
			
			
		}
		catch(Exception e)
		{
			txt.setText("cng: " + e.getMessage());
		}
		
	
	}
	
	
	public void run()
	{
		
	}
	
	@Override
	public void onAccuracyChanged(Sensor sensor, int accuracy) {
		// TODO Auto-generated method stub
		
	}
	@Override
	public void onSensorChanged(SensorEvent event) {
		
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
		// TODO Auto-generated method stub
		
		
	}
	
	
	
	public void onResume() {
	    // register this class as a listener for the orientation and
	    // accelerometer sensors
	    sensorManager.registerListener(this,
	        rSensor,
	        SensorManager.SENSOR_DELAY_FASTEST);
	}

	public void onPause() {
	    // unregister listener
	    sensorManager.unregisterListener(this);
	}
	
}
