namespace Settings
{
    public sealed record EmailSettings
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public bool UseSsl { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public Sender Sender { get; set; }
    }
}
