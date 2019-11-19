using System;

namespace DotnetCoreCertificateBuildpack
{
    public class CertificateLoadException : Exception
    {
    	
        public CertificateLoadException()
        {
    		
        }
        public CertificateLoadException(string message) : base(message)
        {
    		
        }

        public CertificateLoadException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}