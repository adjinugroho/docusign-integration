namespace docusign_integration.Helper
{
    public static class AppConstant
    {
        public const string DateFormat = "MM/dd/yyyy HH:mm:ss";
    }

    public static class TempConstant
    {
        public const string DSAuthString = "DSAuthString";
        public const string ExpireOn = "ExpireOn";
    }

    public static class CfgDocuSign
    {
        public const string Environment = "DocuSignConfig:Environment";
        public const string ImpersonatedUserId = "DocuSignConfig:ImpersonatedUserId";
        public const string APIAccountId = "DocuSignConfig:APIAccountId";
        public const string BasePath = "DocuSignConfig:BasePath";
        public const string ClientId = "DocuSignConfig:ClientId";
        public const string PrivateKeyFile = "DocuSignConfig:PrivateKeyFile";
        public const string RedirectUrl = "DocuSignConfig:RedirectUrl";
    }
}
