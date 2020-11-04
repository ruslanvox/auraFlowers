namespace aura.flowers.Utils
{
    public class ApplicationSettings
    {
        public GoogleReCaptcha GoogleReCaptcha { get; set; }
    }

    public class GoogleReCaptcha
    {
        public string Key { get; set; }

        public string Secret { get; set; }
    }
}