package com.example.rotatetest;

import java.util.Dictionary;

import android.hardware.Sensor;
import android.hardware.SensorEvent;
import android.hardware.SensorEventListener;
import android.hardware.SensorManager;
import android.os.Bundle;
import android.app.Activity;
import android.view.Menu;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;
import android.widget.TextView;

public class MainActivity extends Activity {

	SensorManager sensorManager;
	Sensor rSensor;
	TextView text1;
	EditText ipText;
	EditText portText;
	ThreadSender sender;
	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.activity_main);
		sensorManager = (SensorManager) getSystemService(SENSOR_SERVICE);
		//rSensor = sensorManager.getDefaultSensor(Sensor.TYPE_ORIENTATION);
		text1 = (TextView) findViewById(R.id.textView01);
		ipText = (EditText) findViewById(R.id.ipText);
		portText = (EditText) findViewById(R.id.portText);
		
		Button button = (Button)findViewById(R.id.button1);
		button.setOnClickListener(buttonOnClickListener);
		
		Button disconnectButton = (Button)findViewById(R.id.button2);
		disconnectButton.setOnClickListener(disconnectButtonOnClickListener);
		
		sender = null;
	}

	@Override
	public boolean onCreateOptionsMenu(Menu menu) {
		// Inflate the menu; this adds items to the action bar if it is present.
		getMenuInflater().inflate(R.menu.main, menu);
		return true;
		//
	}

	
	Button.OnClickListener buttonOnClickListener
	 = new Button.OnClickListener(){

		@Override
		public void onClick(View v) {
			// TODO Auto-generated method stub
			if(sender == null || !sender.isConnected)
			{
				String ip = ipText.getText().toString();
				int port = Integer.parseInt(portText.getText().toString());
				sender = new ThreadSender(sensorManager, text1, ip, port);
			}
		}
	};
	
	Button.OnClickListener disconnectButtonOnClickListener
	 = new Button.OnClickListener(){

		@Override
		public void onClick(View arg0) {
			try
			{
				if(sender != null)
				{
					sender.Disconnect();
				}
			}
			catch(Exception ex)
			{
				
			}
			
			//sender = null;
			
		}
	};
	
	@Override
	protected void onResume() {
	    super.onResume();
	    // register this class as a listener for the orientation and
	    // accelerometer sensors
	    if(sender != null)
	    {
	    	sender.onResume();
	    }
	}

	@Override
	protected void onPause() {
	    // unregister listener
	    super.onPause();
	    if(sender != null)
	    {
	    	sender.onPause();
	    }
	}

}
