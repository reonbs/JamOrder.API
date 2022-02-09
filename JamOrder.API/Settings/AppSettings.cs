namespace JamOrder.API.Settings
{
    public class AppSettings
    {
        public string EncryptionKey { get; set; }
        public string EncryptionSalt { get; set; }
        public int TokenExpiry { get; set; }
    }
}
