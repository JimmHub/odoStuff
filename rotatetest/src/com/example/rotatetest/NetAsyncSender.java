package com.example.rotatetest;
import android.os.AsyncTask;

import java.io.DataInputStream;
import java.io.DataOutputStream;
import java.io.IOException;
import java.io.InterruptedIOException;
import java.io.ObjectOutputStream;
import java.net.InetSocketAddress;
import java.net.Socket;
import java.net.SocketAddress;
import java.net.SocketException;
import java.net.UnknownHostException;

public class NetAsyncSender extends AsyncTask<Socket, Integer, String> {
	
	protected String doInBackground(Socket... params)
	{
		/*Socket socket = params[0];
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
		}*/
		
		
		return "what";
    	
	}
	
}
