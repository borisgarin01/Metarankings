namespace API.SettingsDtos
{
    public sealed record EmailSettings
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public bool UseSsl { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public sealed record Sender
        {
            public string Name { get; set; }
            public string Email { get; set; }
        }
    }
}
