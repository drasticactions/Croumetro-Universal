namespace CrouMetro.Core.Tools
{
    public class Constants
    {
        public static string CONSUMER_KEY { get; set; }

        public static string CONSUMER_SECRET { get; set; }

        #if WINDOWS_PHONE_APP
        public const string CONSUMER_KEY = "563e31ef96ea544ee3d3f0a4c5f3284f211f90b1f41a0686fdd31cc74dca0770";

        public const string CONSUMER_SECRET = "b5a7c6088e7b69127bbb4f3dfc1c2bb59e1517dbcd93ae5449e4efd0ca0e90ef";

         public const int STATUS_LIMIT = 372;

#endif
        #if WINDOWS_APP
        public const string CONSUMER_KEY = "88af26dcf4225aa781c7d58af5c6f6aa5e3a4d64f8ddfb31d86bc5a663efed61";

        public const string CONSUMER_SECRET = "bb40e79f53611518567aeb41f7423e81e630ac383d37db3f08b7702c4910f59d";

        public const int STATUS_LIMIT = 372;
#endif
    }
}