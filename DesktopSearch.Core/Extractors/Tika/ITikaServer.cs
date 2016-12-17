namespace DesktopSearch.Core.Extractors.Tika
{

    public interface ITikaServer
    {
        void Start();
        void Stop();

        bool IsRunning { get; }
    }
}
