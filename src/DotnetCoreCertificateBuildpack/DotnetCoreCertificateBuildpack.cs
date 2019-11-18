using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;

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
            
        }

        protected override void PreStartup()
        {
            var certData = Environment.GetEnvironmentVariable("cert");
            if (certData == null)
            {
                //Console.WriteLine("Certificate not found. Set 'cert' environmental variable with cert in PFX format, Base64 encoded");
                return;
            }
            var cert = new X509Certificate2(Convert.FromBase64String(certData));
            Console.WriteLine("=== Installed Certificate into CurrentUser/My X509Store ===");
            Console.WriteLine($"Subject: {cert.Subject}");
            Console.WriteLine($"Friendly Name: {cert.FriendlyName}");
            Console.WriteLine($"Issuer: {cert.Issuer}");
            Console.WriteLine($"Thumbprint: {cert.Thumbprint}");
            Console.WriteLine($"Serial Number: {cert.SerialNumber}");
            if (IsLinux)
            {
                InstallLinux(cert);
            }
            else
            {
                InstallWindows(cert);
            }
        }

        private void InstallLinux(X509Certificate2 cert)
        {
            var myStore = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".dotnet/corefx/cryptography/x509stores/my");
            Directory.CreateDirectory(myStore);
            File.WriteAllBytes(Path.Combine(myStore, $"{cert.Thumbprint}.pfx"), Convert.FromBase64String(Environment.GetEnvironmentVariable("cert")));
        }

        private void InstallWindows(X509Certificate2 cert)
        {
            var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadWrite);
            store.Add(cert);
        }
    }
}
