namespace WorkServerSimple;

using System.Net;
using System.Net.Sockets;

public static class Program
{
    public static void Main()
    {
        using var server = new Server(new IPEndPoint(IPAddress.Any, 10000));
        server.OnClientAccepted += socket =>
        {
            _ = Task.Run(async () =>
            {
                //connection.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.NoDelay, 1);

                var buffer = new byte[4096];
                try
                {
                    while (true)
                    {
                        var read = await socket.ReceiveAsync(buffer, SocketFlags.None);
                        if (read == 0)
                        {
                            break;
                        }

                        await socket.SendAsync(buffer[..read], SocketFlags.None);
                    }
                }
                finally
                {
                    socket.Dispose();
                }
            });
        };

        server.Start();

        // TODO test TryReadToAny line
        // TODO test threads

        Console.ReadLine();
    }
}

public sealed class Server : IDisposable
{
    private readonly object sync = new();

    private readonly EndPoint endPoint;

    private readonly SocketAsyncEventArgs saea;

    private Socket? listener;

    public event Action<Socket>? OnClientAccepted;

    public Server(EndPoint endPoint)
    {
        this.endPoint = endPoint;
        saea = new SocketAsyncEventArgs();
        saea.Completed += AcceptCompleted;
    }

    public void Dispose()
    {
        Stop();
        saea.Dispose();
    }

    public void Start()
    {
        lock (sync)
        {
            if (listener is not null)
            {
                return;
            }

            try
            {
                listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                listener.Bind(endPoint);
                listener.Listen();
            }
            catch (SocketException)
            {
                listener?.Dispose();
                listener = null;
                return;
            }
        }

        StartAccept();
    }

    public void Stop()
    {
        lock (sync)
        {
            listener?.Dispose();
            listener = null;
        }
    }

    private void StartAccept()
    {
        lock (sync)
        {
            saea.AcceptSocket = null;

            if ((listener is not null) && !listener.AcceptAsync(saea))
            {
                ProcessAccept(saea);
            }
        }
    }

    private void AcceptCompleted(object? sender, SocketAsyncEventArgs e)
    {
        if (e.LastOperation == SocketAsyncOperation.Accept)
        {
            ProcessAccept(e);
        }
    }

    private void ProcessAccept(SocketAsyncEventArgs e)
    {
        if (e.SocketError == SocketError.Success)
        {
            try
            {
                OnClientAccepted?.Invoke(e.AcceptSocket!);
            }
            catch (Exception)
            {
                // ignored
            }

            StartAccept();
        }
    }
}
