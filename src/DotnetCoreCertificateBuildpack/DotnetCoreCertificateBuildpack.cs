using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace DotnetCoreCertificateBuildpack
{
    public class DotnetCoreCertificateBuildpack : SupplyBuildpack
    {
        private bool IsLinux => RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
        protected override bool Detect(string buildPath)
        {
            return false;
        }

        protected override void Apply(string buildPath, string cachePath, string depsPath, int index)
        {
            var buildpackLocation = Assembly.GetExecutingAssembly().Location;
            var buildpackDeps = Path.Combine(depsPath, index.ToString());
            foreach (var file in Directory.EnumerateFiles(buildpackLocation))
            {
                File.Copy(file, Path.Combine(buildpackDeps, Path.GetFileName(file)));
            }
            var certData = Environment.GetEnvironmentVariable("cert");
            if (certData == null)
            {
                Console.WriteLine("Certificate not found. Set 'cert' environmental variable with cert in PFX format, Base64 encoded");
                return;
            }
            var cert = new X509Certificate2(Convert.FromBase64String(certData));
            Console.WriteLine("=== Installed Certificate into CurrentUser/My X509Store ===");
            Console.WriteLine($"Subject: {cert.Subject}");
            Console.WriteLine($"Friendly Name: {cert.FriendlyName}");
            Console.WriteLine($"Issuer: {cert.Issuer}");
            Console.WriteLine($"Thumbprint: {cert.Thumbprint}");
            Console.WriteLine($"Serial Number: {cert.SerialNumber}");

            
        }

        private void InstallLinux(X509Certificate2 cert, string buildPath)
        {
            
            var profiled = Path.Combine(buildPath, ".profile.d");
            var certsDir = Path.Combine(buildPath, "certs");
            Directory.CreateDirectory(profiled);
            Directory.CreateDirectory(certsDir);
            File.WriteAllBytes(Path.Combine(certsDir, $"{cert.Thumbprint}.pfx"), Convert.FromBase64String(Environment.GetEnvironmentVariable("cert")));
            var sb = new StringBuilder();
            void line(string s) => sb.Append($"{s}\n");
            line("#!/bin/bash");
            line("mkdir ~/.dotnet");
            line("mkdir ~/.dotnet/corefx");
            line("mkdir ~/.dotnet/corefx/cryptography");
            line("mkdir ~/.dotnet/corefx/cryptography/x509stores");
            line("mkdir ~/.dotnet/corefx/cryptography/x509stores/my");
            line($"ln ~/certs/{cert.Thumbprint}.pfx ~/.dotnet/corefx/cryptography/x509stores/my/{cert.Thumbprint}.pfx");
            
            File.WriteAllText(Path.Combine(profiled,"startup.sh"), sb.ToString());
        }

        private void InstallWindows(X509Certificate2 cert)
        {
            var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadWrite);
            store.Add(cert);
        }
    }
}
