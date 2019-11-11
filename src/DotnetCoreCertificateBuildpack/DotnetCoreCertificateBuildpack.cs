using System;
using System.Security.Cryptography.X509Certificates;

namespace DotnetCoreCertificateBuildpack
{
    public class DotnetCoreCertificateBuildpack : SupplyBuildpack
    {
        
        protected override bool Detect(string buildPath)
        {
            return false;
        }

        protected override void Apply(string buildPath, string cachePath, string depsPath, int index)
        {
            var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadWrite);
            var cert = new X509Certificate2(Convert.FromBase64String(Environment.GetEnvironmentVariable("cert")),"certpassword");
            store.Add(cert);
        }
    }
}
