using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Security.Principal;
using System.ServiceModel;
using AuthenticationService;
using DataBaseAccessService;
using GameService;

namespace Host
{
    public class Program
    {
        private static void Main()
        {
            if (!new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator))
            {
                ProcessStartInfo processInfo = new ProcessStartInfo {Verb = "runas", FileName = "Host.exe"};
                try
                {
                    Process.Start(processInfo);
                }
                catch (Win32Exception)
                {
                }
                return;
            }

            var hosts = new ServiceHost[3]
            {
                new ServiceHost(typeof(CGameService)),
                new ServiceHost(typeof(CDataBaseAccessService)),
                new ServiceHost(typeof(CAuthenticationService))
            };
            Console.WriteLine("Available services:\n" +
                                  "0) Master service\n" +
                                  "1) Database access service\n" +
                                  "2) Authentication service");
            while (true)
            {
                try
                {
                    String[] words = Console.ReadLine().Split(new[] {' ', '\n'}, StringSplitOptions.RemoveEmptyEntries);
                    Int32 number;
                    switch (words[0])
                    {
                        case "open":
                            if (words[1] == "all")
                            {
                                foreach (ServiceHost serviceHost in hosts)
                                {
                                    serviceHost.Open();
                                }

                                Console.WriteLine("All hosts opened");
                                break;
                            }

                            number = Int32.Parse(words[1]);
                            if (hosts[number].State != CommunicationState.Opened)
                            {
                                hosts[number].Open();
                                Console.WriteLine($"Host {number} opened");
                            }
                            else
                            {
                                Console.WriteLine($"Host {number} already opened");
                            }

                            break;
                        case "close":
                            if (words[1] == "all")
                            {
                                foreach (ServiceHost serviceHost in hosts)
                                {
                                    serviceHost.Close();
                                }

                                Console.WriteLine("All hosts closed");
                                break;
                            }

                            number = Int32.Parse(words[1]);
                            if (hosts[number].State != CommunicationState.Closed)
                            {
                                hosts[number].Close();
                                Console.WriteLine($"Host {number} closed");
                            }
                            else
                            {
                                Console.WriteLine($"Host {number} already closed");
                            }

                            break;
                        case "check":
                            for (Int32 i = 0; i < hosts.Length; i++)
                            {
                                Console.WriteLine($"{i}) {hosts[i].State}");
                            }

                            break;
                        case "final":
                            foreach (ServiceHost serviceHost in hosts)
                            {
                                serviceHost.Close();
                            }

                            Console.WriteLine("All hosts closed");
                            return;
                        default:
                            Console.WriteLine("Unknown command");
                            break;
                    }
                }
                catch
                {
                    Console.WriteLine("Unknown or illegal command");
                }
            }
        }
    }
}
