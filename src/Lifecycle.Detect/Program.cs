using System;
using System.Linq;

namespace Lifecycle.Supply
{
    class Program
    {
        static void Main(string[] args)
        {
            var argsWithCommand = new[] {"detect"}.Union(args).ToArray();
            new DotnetCoreCertificateBuildpack.DotnetCoreCertificateBuildpack().Run(argsWithCommand);
        }
    }
}