using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace DotnetCoreCertificateBuildpack
{
    public abstract class BuildpackBase
    {
        public bool IsLinux => RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
        /// <summary>
        /// Determines if the buildpack is compatible and should be applied to the application being staged 
        /// </summary>
        /// <param name="buildPath">Directory path to the application</param>
        /// <returns>True if buildpack should be applied, otherwise false</returns>
        protected abstract bool Detect(string buildPath);

        /// <summary>
        /// 
        /// </summary>
        protected virtual void PreStartup()
        {
        }

        /// <summary>
        /// Logic to apply when buildpack is ran.
        /// Note that for <see cref="SupplyBuildpack"/> this will correspond to "bin/supply" lifecycle event, while for <see cref="FinalBuildpack"/> it will be invoked on "bin/finalize"
        /// </summary>
        /// <param name="buildPath">Directory path to the application</param>
        /// <param name="cachePath">Location the buildpack can use to store assets during the build process</param>
        /// <param name="depsPath">Directory where dependencies provided by all buildpacks are installed. New dependencies introduced by current buildpack should be stored inside subfolder named with index argument ({depsPath}/{index})</param>
        /// <param name="index">Number that represents the ordinal position of the buildpack</param>
        protected abstract void Apply(string buildPath, string cachePath, string depsPath, int index);

        /// <summary>
        /// Entry point into the buildpack. Should be called from Main method with args
        /// </summary>
        /// <param name="args">Args array passed into Main method</param>
        /// <returns>Status return code</returns>
        public int Run(string[] args)
        {
            return DoRun(args);
        }

        protected virtual int DoRun(string[] args)
        {
            var command = args[0];
            switch (command)
            {
                case "detect":
                    return Detect(args[1]) ? 2 : 1;
                case "prestartup":
                    PreStartup();
                    break;
            }

            return 0;
        }

        protected void DoApply(string buildPath, string cachePath, string depsPath, int index)
        {
            var isPreStartOverriden = GetType().GetMethod(nameof(PreStartup), BindingFlags.Instance | BindingFlags.NonPublic).DeclaringType != typeof(BuildpackBase);
            var buildpackDepsDir = Path.Combine(depsPath, index.ToString());
            Directory.CreateDirectory(buildpackDepsDir);
            if (isPreStartOverriden)
            {
                var profiled = Path.Combine(buildPath, ".profile.d");
                Directory.CreateDirectory(profiled);
                // copy buildpack to deps dir so we can invoke it as part of startup
                foreach(var file in Directory.EnumerateFiles(Path.GetDirectoryName(GetType().Assembly.Location)))
                {
                    File.Copy(file, Path.Combine(buildpackDepsDir, Path.GetFileName(file)), true);
                }

                var extension = !IsLinux ? ".exe" : string.Empty;
                var prestartCommand = $"{this.GetType().Assembly.GetName().Name}{extension} prestartup";
                // write startup shell script to call buildpack prestart lifecycle event in deps dir
                if (IsLinux)
                {
                    File.WriteAllText(Path.Combine(profiled,"startup.sh"), $"#!/bin/bash\n$DEPS_DIR/{index}/{prestartCommand}");
                }
                else
                {
                    File.WriteAllText(Path.Combine(profiled,"startup.bat"),$@"%DEPS_DIR%\{index}\{prestartCommand}");
                }
                    
            }
            Apply(buildPath, cachePath, depsPath, index);
        }
    }
}