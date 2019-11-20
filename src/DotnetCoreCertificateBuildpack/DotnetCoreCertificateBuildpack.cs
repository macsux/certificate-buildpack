using System;
using System.IO;
using System.Linq;
//using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using Steeltoe.Extensions.Configuration.CloudFoundry;
using Steeltoe.Extensions.Configuration.ConfigServer;
using MS509 = System.Security.Cryptography.X509Certificates;

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
            var config = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .AddConfigServer()
                .Build();
            
            
            var services = new CloudFoundryServicesOptions();
            config.GetSection("vcap")?.Bind(services);
            var userProvidedServicesCerts = services.ServicesList
                .Where(x => x.Credentials.ContainsKey("certificate") || (x.Tags?.Contains("certificate") ?? false ))
                .Select(x =>
                {
                    var cert = new Cert
                    {
                        Certificate = x.Credentials["certificate"].Value
                    };
                    if (x.Credentials.TryGetValue("password", out var password))
                        cert.Password = password.Value;
                    return cert;
                })
                .ToList();
            var items = config.GetSection("certificates:CurrentUser:My");
            var rawCerts = items.GetChildren()
                .Select(x =>
                {
                    var cert = new Cert();
                    x.Bind(cert);
                    return cert;
                })
                .Union(userProvidedServicesCerts)
                .Where(x => x != null);
            Console.WriteLine("<<< Installing Certificates into CurrentUser/My X509Store >>>");
            foreach(var certData in rawCerts.Where(x => !string.IsNullOrWhiteSpace(x.Certificate)))
            {
                var pem = Regex.Match(certData.Certificate, "-+BEGIN CERTIFICATE-+.+?-+END CERTIFICATE-+", RegexOptions.Singleline);
                var key = certData.Certificate != null ? Regex.Match(certData.Certificate, "-+BEGIN RSA PRIVATE KEY-+.+?-+END RSA PRIVATE KEY-+", RegexOptions.Singleline)?.Value : null;
                MS509.X509Certificate2 cert = null;
                
                try
                {
                    if (pem.Success) // pem
                    {
                        cert = ReadPem(pem.Value, key, certData.Password);
                    }
                    else if (string.IsNullOrEmpty(certData.Password)) // pfx without password
                    {
                        cert = new MS509.X509Certificate2(Convert.FromBase64String(certData.Certificate));
                    }
                    else
                    {
                        cert = new MS509.X509Certificate2(Convert.FromBase64String(certData.Certificate), certData.Password);
                    }

                    if (IsLinux)
                    {
                        InstallLinux(cert);
                    }
                    else
                    {
                        InstallWindows(cert);
                    }
                    Console.WriteLine($"Subject: {cert.Subject}");
                    if(cert.FriendlyName != null)
                        Console.WriteLine($"Friendly Name: {cert.FriendlyName}");
                    Console.WriteLine($"Issuer: {cert.Issuer}");
                    Console.WriteLine($"Thumbprint: {cert.Thumbprint}");
                    Console.WriteLine($"Serial Number: {cert.SerialNumber}");
                    Console.WriteLine("----------------");
                }
                catch (Exception e) when (e is CryptographicException || e is CertificateLoadException)
                {
                    Console.Error.WriteLine("Error loading certificate. Skipping...");
                    Console.Error.WriteLine(e);
                }
            };
        }

        private void InstallLinux(MS509.X509Certificate2 cert)
        {
            var myStore = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".dotnet/corefx/cryptography/x509stores/my");
            Directory.CreateDirectory(myStore);
            File.WriteAllBytes(Path.Combine(myStore, $"{cert.Thumbprint}.pfx"), cert.Export(MS509.X509ContentType.Pfx));
        }

        private void InstallWindows(MS509.X509Certificate2 cert)
        {
            var store = new MS509.X509Store(MS509.StoreName.My, MS509.StoreLocation.CurrentUser);
            store.Open(MS509.OpenFlags.ReadWrite);
            store.Add(cert);
        }
        
        
        
        MS509.X509Certificate2 ReadPem(string certBase64, string keyBase64, string password)
        {
            var cert = ReadCertificate(Encoding.Default.GetBytes(certBase64));
            if (cert == null)
            {
                throw new CertificateLoadException("Error loading certificate - skipped");
            }
            AsymmetricCipherKeyPair keys = null;
            if (keyBase64 != null)
            {
                try
                {
                    var keyBytes = Encoding.Default.GetBytes(keyBase64);
                    keys = ReadKeys(keyBytes, password);
                }
                catch (Exception e)
                {
                    throw new CertificateLoadException($"Error load private key for certificate with FingerPrint {cert.SerialNumber}", e);
                }
            }
            var pfxBytes = CreatePfxContainer(cert, keys);
            return new MS509.X509Certificate2(pfxBytes);
        }
        private byte[] CreatePfxContainer(X509Certificate certificate, AsymmetricCipherKeyPair keys)
        {
            var certEntry = new X509CertificateEntry(certificate);

            var pkcs12Store = new Pkcs12StoreBuilder()
                .SetUseDerEncoding(true)
                .Build();
            if (keys != null)
            {
                var keyEntry = new AsymmetricKeyEntry(keys.Private);
                pkcs12Store.SetKeyEntry("ServerInstance", keyEntry, new [] { certEntry });
            }
            using (MemoryStream stream = new MemoryStream())
            {
                pkcs12Store.Save(stream, string.Empty.ToCharArray(), new SecureRandom()); // linux will not like certs null password as they have no signature
                var bytes = stream.ToArray();
                return Pkcs12Utilities.ConvertToDefiniteLength(bytes);
                return bytes;
            }
            
        }
        
        internal AsymmetricCipherKeyPair ReadKeys(byte[] keyBytes, string password)
        {
            using (var reader = new StreamReader(new MemoryStream(keyBytes)))
            {
                return new PemReader(reader, new PasswordFinder(password)).ReadObject() as AsymmetricCipherKeyPair;
            }
        }
        internal X509Certificate ReadCertificate(byte[] certBytes)
        {
	
            using (var reader = new StreamReader(new MemoryStream(certBytes)))
            {
                return new PemReader(reader).ReadObject() as X509Certificate;
            }
	
        }
        
        public class PasswordFinder : IPasswordFinder
        {
            private string _password;
            public PasswordFinder(string password) 
            {
                _password = password;
            }
            public char[] GetPassword() => _password == null ? Array.Empty<char>() : _password.ToArray();
        }
        public class Cert
        {
            public string Certificate {get;set;}
            public string Password {get;set;}
        }

    }
}
