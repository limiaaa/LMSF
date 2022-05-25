using UnityEngine;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using LayaGame;

public enum DisType
{
    Exception,
    Disconnect,
}

public class SocketClient
{
    private TcpClient client = null;
    private NetworkStream outStream = null;
    private MemoryStream memStream;
    private BinaryReader reader;

    private const int MAX_READ = 8192;
    private byte[] byteBuffer = new byte[MAX_READ];
    //判断是否与服务器断开连接，如果断开则进行一次断线重连（只进行一次断线重连）
    public static bool isLostConnection = false;

    // Use this for initialization
    public SocketClient()
    {
    }

    /// <summary>
    /// 注册代理
    /// </summary>
    public void OnRegister()
    {
        memStream = new MemoryStream();
        reader = new BinaryReader(memStream);
    }

    /// <summary>
    /// 移除代理
    /// </summary>
    public void OnRemove()
    {
        //this.Close();
        if (reader != null)
        {
            reader.Close();
            reader = null;
        }
        if (memStream != null)
        {
            memStream.Close();
            memStream = null;
        }
        this.Close();
    }

    /// <summary>
    /// 连接服务器
    /// </summary>
    void ConnectServer(string host, int port)
    {
        client = null;
        try
        {
            IPAddress[] address = Dns.GetHostAddresses(host);
            if (address.Length == 0)
            {
                //Util.LogError("host invalid");
                return;
            }
            if (address[0].AddressFamily == AddressFamily.InterNetworkV6)
            {
                client = new TcpClient(AddressFamily.InterNetworkV6);
            }
            else
            {
                client = new TcpClient(AddressFamily.InterNetwork);
            }
            client.SendTimeout = 1000;
            client.ReceiveTimeout = 1000;
            client.NoDelay = true;
            //Util.LogWarning("开始连接服务器");
            client.BeginConnect(host, port, new AsyncCallback(OnConnect), null);
        }
        catch (Exception e)
        {
            Close();
            //Util.LogError(e.Message);
        }
    }

    /// <summary>
    /// 连接上服务器
    /// </summary>
    void OnConnect(IAsyncResult asr)
    {
        //Util.LogWarning("准备连接登录服务器");
        OnRegister();
        try
        {
            isLostConnection = false;
            outStream = client.GetStream();
            client.GetStream().BeginRead(byteBuffer, 0, MAX_READ, new AsyncCallback(OnRead), null);
            //Util.LogWarning("连接上服务器");
            //NetworkManager.AddEvent(Protocal.Connect, new ByteBuffer());
        }
        catch (Exception ex)
        {
            //NetworkManager.AddEvent(Protocal.ConnectOutTime, null);
            //Util.LogWarning("登录服务器异常：：" + ex.Message);
            return;
        }
    }

    /// <summary>
    /// 写数据
    /// </summary>
    void WriteMessage(byte[] message)
    {
        MemoryStream ms = null;
        using (ms = new MemoryStream())
        {
            ms.Position = 0;
            BinaryWriter writer = new BinaryWriter(ms);
            ushort msgprelen = (ushort)message.Length;

            if (AppConst.EncryptABBol)
            {
                ByteBuffer buffer = new ByteBuffer(message);
                int mainId = buffer.ReadInt();
                byte[] messageBuffer = buffer.ReadBytes();
                long curTime = 0;
                string MD5ByteValue = MD5Utils.Md5(ByteUtils.byteTostring(messageBuffer) + curTime.ToString());
                byte[] MD5Change = CryptUtils.EncryptAES(MD5ByteValue, AppConst.AESEncryptKey, AppConst.AESEncryptIV);
                int MD5Changelen = MD5Change.Length;
                int md5Len = MD5ByteValue.Length;
                ushort msglen = (ushort)(messageBuffer.Length + MD5Change.Length + 24);
                writer.Write(msglen);
                writer.Write(mainId);
                writer.Write(messageBuffer.Length);
                writer.Write(messageBuffer);

                writer.Write(curTime);
                writer.Write(MD5Changelen);
                writer.Write(MD5Change);
                writer.Write(md5Len);
            }
            else
            {
                writer.Write(msgprelen);
                writer.Write(message);
            }
            writer.Flush();
            if (client != null && client.Connected)
            {
                byte[] payload = ms.ToArray();
                outStream.BeginWrite(payload, 0, payload.Length, new AsyncCallback(OnWrite), null);
            }
            else
            {
                //Util.LogError("client.connected----->>====");
                //TODO
                //发送数据失败，断线重连
                //Util.CallMethod("Network", "SendMessageError", "1");
            }
        }
    }

    /// <summary>
    /// 读取消息
    /// </summary>
    void OnRead(IAsyncResult asr)
    {
        int bytesRead = 0;
        try
        {
            if (client == null || !client.Connected)
                return;

            lock (client.GetStream())
            {         //读取字节流到缓冲区
                bytesRead = client.GetStream().EndRead(asr);
            }
            if (bytesRead < 1)
            {       
                         //包尺寸有问题，断线处理
                OnDisconnected(DisType.Disconnect, "bytesRead < 1");
                return;
            }

            OnReceive(byteBuffer, bytesRead);   //分析数据包内容，抛给逻辑层
            lock (client.GetStream())
            {         //分析完，再次监听服务器发过来的新消息
                Array.Clear(byteBuffer, 0, byteBuffer.Length);   //清空数组
                client.GetStream().BeginRead(byteBuffer, 0, MAX_READ, new AsyncCallback(OnRead), null);
            }
        }
        catch (Exception ex)
        {
            Debug.Log("xxxxxx "+ ex.Message.ToString());
            OnDisconnected(DisType.Exception, ex.Message);
        }
    }

    /// <summary>
    /// 丢失链接
    /// </summary>
    void OnDisconnected(DisType dis, string msg)
    {
        Close();   //关掉客户端链接
        int protocal = dis == DisType.Exception ?
        Protocal.Exception : Protocal.Disconnect;

        ByteBuffer buffer = new ByteBuffer();
        buffer.WriteShort((ushort)protocal);
        //NetworkManager.AddEvent(protocal, buffer);
        //Util.LogError("Connection was closed by the server:>" + msg + " Distype:>" + dis);
        if (isLostConnection == false)
        {
            //监测到断线，进行重新连接一次
            isLostConnection = true;
            //Util.CallMethod("GlobalListener", "LostConnectionToReConnect");
        }
    }

    /// <summary>
    /// 打印字节
    /// </summary>
    /// <param name="bytes"></param>
    void PrintBytes(byte[] bytes)
    {
        string returnStr = string.Empty;
        for (int i = 0; i < bytes.Length; i++)
        {
            //16进制两位数
            returnStr += byteBuffer[i].ToString("X2");
        }
        //Util.LogError(returnStr);
    }

    /// <summary>
    /// 向链接写入数据流
    /// </summary>
    void OnWrite(IAsyncResult r)
    {
        try
        {
            outStream.EndWrite(r);
        }
        catch (Exception ex)
        {
            //Util.LogError("OnWrite--->>>" + ex.Message);
        }
    }

    /// <summary>
    /// 接收到消息
    /// </summary>
    void OnReceive(byte[] bytes, int length)
    {
        memStream.Seek(0, SeekOrigin.End);
        memStream.Write(bytes, 0, length);
        //Reset to beginning
        memStream.Seek(0, SeekOrigin.Begin);
        while (RemainingBytes() > 2)
        {
            ushort messageLen = reader.ReadUInt16();
            if (RemainingBytes() >= messageLen)
            {
                MemoryStream ms = new MemoryStream();
                BinaryWriter writer = new BinaryWriter(ms);
                writer.Write(reader.ReadBytes(messageLen));
                ms.Seek(0, SeekOrigin.Begin);
                OnReceivedMessage(ms);
            }
            else
            {
                //Back up the position two bytes
                memStream.Position = memStream.Position - 2;
                break;
            }
        }
        //Create a new stream with any leftover bytes
        byte[] leftover = reader.ReadBytes((int)RemainingBytes());
        memStream.SetLength(0);     //Clear
        memStream.Write(leftover, 0, leftover.Length);
    }

    /// <summary>
    /// 剩余的字节
    /// </summary>
    private long RemainingBytes()
    {
        return memStream.Length - memStream.Position;
    }

    /// <summary>
    /// 接收到消息
    /// </summary>
    /// <param name="ms"></param>
    void OnReceivedMessage(MemoryStream ms)
    {
        BinaryReader r = new BinaryReader(ms);
        byte[] message = r.ReadBytes((int)(ms.Length - ms.Position));
        //int msglen = message.Length;

        ByteBuffer buffer = new ByteBuffer(message);
        //int mainId = buffer.ReadShort();
        int mainId = buffer.ReadInt();
        //NetworkManager.AddEvent(mainId, buffer);

    }

    /// <summary>
    /// 会话发送
    /// </summary>
    void SessionSend(byte[] bytes)
    {
        WriteMessage(bytes);
    }


    /// <summary>
    /// 关闭链接
    /// </summary>
    public void Close()
    {
        if (client != null)
        {
            if (client.Connected) client.Close();
            client = null;
        }
    }

    /// <summary>
    /// 发送连接请求
    /// </summary>
    public void SendConnect()
    {
        ConnectServer(AppConst.SocketAddress, AppConst.SocketPort);
    }
    /// <summary>
    /// 发送带有参数的连接请求
    /// </summary>
    /// <param name="ipStr"></param>
    /// <param name="port"></param>
    public void SendConnectIP(string ipStr, int port)
    {
        ConnectServer(ipStr, port);
    }

    /// <summary>
    /// 发送消息
    /// </summary>
    public void SendMessage(ByteBuffer buffer)
    {
        SessionSend(buffer.ToBytes());
        buffer.Close();
    }
}
