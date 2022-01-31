namespace DIG.GBLXAPI
{
    public class GBLConfig
    {
        public const string StandardsDefaultPath = "data/GBLxAPI_Vocab_Default";
        public const string StandardsUserPath = "data/GBLxAPI_Vocab_User";

        public const string LrsURL = "https://trial-lrs.yetanalytics.io/xapi";

        // Fill in these fields for GBLxAPI setup.
        public string lrsUser = "3f518c38544091afc5a76a344dbd1698";
        public string lrsPassword = "63f9ae65f99df7b9c74f6e11afc08e65";

        public string companyURI = "https://company.com/";
        public string gameURI = "https://company.com/example-game";
        public string gameName = "Example Game";

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
