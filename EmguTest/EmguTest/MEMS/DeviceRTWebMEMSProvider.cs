﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Threading;

namespace EmguTest.MEMS
{
    class DeviceRTWebMEMSProvider : MEMSProvider
    {
        public DeviceRTWebMEMSProvider(int port, Logger.ILogger logger)
        {
            this.Port = port;
            this.Logger = logger;

            this.ServerSocket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            this.StartServerRoutine();
            this.LastRecSet = new MEMSReadingsSet3f();
        }

        public int? Port { get; set; }
        public bool IsConnected { get; protected set; }
        public Socket ServerSocket { get; protected set; }
        protected bool ShouldStop { get; set; }
        public Socket ClientSocket { get; protected set; }
        public Logger.ILogger Logger { get; set; }
        protected MEMSReadingsSet3f LastRecSet { get; set; }
        protected bool LastRecIsGiven { get; set; }

        protected const int RecMessageSize = (3 * sizeof(float) + sizeof(int) + sizeof(byte));

        protected void StartServerRoutine()
        {
            this.ServerRoutineThread = new Thread(this.MainServerRoutine);
        }

        protected Thread ServerRoutineThread { get; set; }
        public bool IsThreadRunning
        {
            get
            {
                if (this.ServerRoutineThread == null)
                {
                    return false;
                }
                if (this.ServerRoutineThread.ThreadState == ThreadState.Running)
                {
                    return true;
                }
                if (this.ServerRoutineThread.ThreadState == ThreadState.WaitSleepJoin)
                {
                    return true;
                }

                return false;
            }
        }

        public void StopThread()
        {
            this.ShouldStop = true;
        }

        protected void MainServerRoutine()
        {
            while (!this.ShouldStop)
            {
                //TODO: fix this to async
                this.ClientSocket = this.ServerSocket.Accept();

                try
                {
                    byte[] recBuffer = new byte[RecMessageSize];
                    while (this.ClientSocket.Connected)
                    {
                        var count = this.ClientSocket.Receive(recBuffer);
                        if (count == RecMessageSize)
                        {
                            var readType = recBuffer[0];
                            var readingVect = recBuffer.Skip(1).ToArray();
                        }
                    }
                }
                catch
                {
                }
            }
        }

        protected MEMSReadingsSet3f ConvertToMEMSReadingsSet3f(byte[] buffer)
        {
            if (buffer.Length != RecMessageSize)
            {
                return null;
            }

            MEMSReadingsSet3f res = new MEMSReadingsSet3f();

            //res.AccVector3f = this.GetNextAccVector3f();
            //res.MagnetVector3f = this.GetNextMagnetVector3f();
            //res.GyroVector3f = this.GetNextGyroVector3f();

            if (res.IsNotEmpty())
            {
                res.TimeStampI = Convert.ToInt64(((double)res.AccVector3f.TimeStampI + (double)res.MagnetVector3f.TimeStampI + (double)res.GyroVector3f.TimeStampI) / 3);
            }
            else
            {
                res.TimeStampI = 0;
            }

            return res;
        }

        protected ReadingsVector3f Get3fReadings(byte[] bufferPart)
        {
            ReadingsVector3f res = new ReadingsVector3f();
            try
            {
                int ptr = 0;
                if (bufferPart.Length <= (sizeof(float) * 3 + sizeof(int)))
                {
                    res.IsEmpty = true;
                }
                else
                {
                    res.TimeStampI = Convert.ToInt64(this.ReadingsBytesToUInt(bufferPart.Take(sizeof(int)).ToArray()));
                    ptr += sizeof(int);

                    res.Values[0] = this.ReadingsBytesToFloat(bufferPart.Skip(ptr).Take(sizeof(float)).ToArray());
                    ptr += sizeof(float);

                    res.Values[1] = this.ReadingsBytesToFloat(bufferPart.Skip(ptr).Take(sizeof(float)).ToArray());
                    ptr += sizeof(float);

                    res.Values[2] = this.ReadingsBytesToFloat(bufferPart.Skip(ptr).Take(sizeof(float)).ToArray());
                    ptr += sizeof(float);
                }
            }
            catch
            {
                res.IsEmpty = true;
            }
            return res;
        }

        protected int GetReadingVectorSize()
        {
            return sizeof(int) + 3 * sizeof(float);
        }

        protected float ReadingsBytesToFloat(byte[] value)
        {
            return BitConverter.ToSingle(value.Reverse().ToArray(), 0);
        }

        protected uint ReadingsBytesToUInt(byte[] value)
        {
            return BitConverter.ToUInt32(value.Reverse().ToArray(), 0);
        }

        protected ReadingsVector3f GetNextAccVector3f(byte[] buffer)
        {
            return this.Get3fReadings(buffer.Take(GetReadingVectorSize()).ToArray());
        }

        protected ReadingsVector3f GetNextMagnetVector3f(byte[] buffer)
        {
            return this.Get3fReadings(buffer.Skip(GetReadingVectorSize()).Take(GetReadingVectorSize()).ToArray());
        }

        protected ReadingsVector3f GetNextGyroVector3f(byte[] buffer)
        {
            return this.Get3fReadings(buffer.Skip(GetReadingVectorSize()).Take(GetReadingVectorSize()).ToArray());
        }

        public override MEMSReadingsSet3f GetCurrentReadingsSet()
        {
            return this.LastRecSet;
        }

        public override MEMSReadingsSet3f GetNextReadingsSet()
        {
            while (this.LastRecIsGiven)
            {
                if (!this.IsThreadRunning)
                {
                    throw new Exception("Thread is not running");
                }

                if (!this.IsConnected)
                {
                    throw new Exception("Client is not connected");
                }
            }

            this.LastRecIsGiven = true;
            return this.LastRecSet;
        }

        public const byte READINGS_TYPE_ACC = 0x01;
        public const byte READINGS_TYPE_MAGNET = 0x02;
        public const byte READINGS_TYPE_GYRO = 0x03;
    }
}
