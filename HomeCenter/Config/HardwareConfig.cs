namespace HomeCenter.Config
{
    public class HardwareConfig : IValidator
    {
        public MiHomeConfig MiHome { get; set; }

        public MqttConfig Mqtt { get; set; }

        public HttpConfig Http { get; set; }

        public VirtualConfig Virtual { get; set; }

        public void Validate()
        {
            MiHome?.Validate();
            Mqtt?.Validate();
            Http?.Validate();
            Virtual?.Validate();
        }
    }
}
