namespace DIG.GBLXAPI
{
    public class GBLConfig
    {
        public const string StandardsDefaultPath = "data/GBLxAPI_Vocab_Default";
        public const string StandardsUserPath = "data/GBLxAPI_Vocab_User";

        public const string LrsURL = "https://trial-lrs.yetanalytics.io/xapi";

        // Fill in these fields for GBLxAPI setup.
        public string lrsUser = "b14dfc5c83e5a6e6f5cdadfdc80d3e9a";

        public string lrsPassword = "6566f1aa869ea700491b733fb548cf1f";

        public string companyURI = "https://company.com/";
        public string gameURI = "https://company.com/example-game";
        public string gameName = "AimLab";

        public GBLConfig()
        {
        }

        public GBLConfig(string lrsUser, string lrsPassword)
        {
            this.lrsUser = lrsUser;
            this.lrsPassword = lrsPassword;
        }
    }
}