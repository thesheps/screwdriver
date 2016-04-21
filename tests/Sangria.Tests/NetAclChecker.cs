using System;
using System.Diagnostics;

namespace Sangria.Tests
{
    public static class NetAclChecker
    {
        public static void AddAddress(string address)
        {
            AddAddress(address, Environment.UserDomainName, Environment.UserName);
        }

        public static void AddAddress(string address, string domain, string user)
        {
            var args = $@"http add urlacl url={address} user={domain}\{user} listen=yes";
            var psi = new ProcessStartInfo("netsh", args)
            {
                Verb = "runas",
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                UseShellExecute = true
            };

            var process = Process.Start(psi);
            process?.WaitForExit();
        }
    }
}