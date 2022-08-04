namespace SmallChat.Client
{
    public interface IServerCommandResultListener
    {
        public void Notify(object data);
    }
}