namespace WebRanging.Daemons
{
    public interface IDaemonFactory
    {
        IDaemon New(DaemonType type);
    }
}