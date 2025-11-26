using Ftp_Dan_True;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class Program
    {
        public static IPAddress IpAdress;
        public static int Port;
        public static int Id = -1;
        public static bool CheckCommand(string message)
        {
            bool BCommand = false;
            string[] DataMessage = message.Split(new string[1] { " " }, StringSplitOptions.None);
            if (DataMessage.Length > 0)
            {
                string Command = DataMessage[0];
                if (Command == "connect")
                {
                    if (DataMessage.Length != 3)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Использование: connect [login] [password]\nПример: connect User1 P@sswerd");
                        BCommand = false;
                    }
                    else
                        BCommand = true;
                }
                else if (Command == "cd")
                    BCommand = true;
                else if (Command == "get")
                {
                    if (DataMessage.Length == 1)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Использование: get [NameFile]\n«Пример: get Test.txt");
                        BCommand = false;
                    }
                    else
                        BCommand = true;
                }
                else if (Command == "set")
                {
                    if (DataMessage.Length == 1)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Использование: set [NameFile]\nПример: set Test.txt");
                        BCommand = false;
                    }
                    else
                        BCommand = true;
                }
            }
            return BCommand;
        }

        public static void ConnectServer()
        {
            try
            {
                IPEndPoint endPoint = new IPEndPoint(IpAdress, Port);
                Socket socket = new Socket(
                    AddressFamily.InterNetwork,
                    SocketType.Stream,
                    ProtocolType.Tcp);
                socket.Connect(endPoint);

                if (socket.Connected)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    string message = Console.ReadLine();

                    if (CheckCommand(message))
                    {
                        ViewModelSend viewModelSend = new ViewModelSend(message, Id);

                        if (message.Split(new string[1] { " " }, StringSplitOptions.None)[0] == "set")
                        {
                            string[] DataMessage = message.Split(new string[1] { " " }, StringSplitOptions.None);

                            string NameFile = "";
                            for (int i = 1; i < DataMessage.Length; i++)
                                if (NameFile == "")
                                    NameFile += DataMessage[i];
                                else
                                    NameFile += " " + DataMessage[i];
                            if (File.Exists(NameFile))
                            {
                                FileInfo fileInfo = new FileInfo(NameFile);
                                FileInfoFTP NewFileInfo = new FileInfoFTP(File.ReadAllBytes(NameFile), fileInfo.Name);
                                viewModelSend = new ViewModelSend(JsonConvert.SerializeObject(NewFileInfo), Id);
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("Указанный файл не существует");
                            }
                        }

                        byte[] messageByte = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(viewModelSend));
                        int BytesSend = socket.Send(messageByte);
                        byte[] bytes = new byte[10485760];
                        int BytesRec = socket.Receive(bytes);
                        string messageServer = Encoding.UTF8.GetString(bytes, 0, BytesRec);
                        ViewModelMessage viewModelMessage = JsonConvert.DeserializeObject<ViewModelMessage>(messageServer);

                        if (viewModelMessage.Command == "authorization")
                            Id = int.Parse(viewModelMessage.Data);
                        else if (viewModelMessage.Command == "message")
                            Console.WriteLine(viewModelMessage.Data);
                        else if (viewModelMessage.Command == "cd")
                        {
                            List<string> FoldersFiles = new List<string>();
                            FoldersFiles = JsonConvert.DeserializeObject<List<string>>(viewModelMessage.Data);
                            foreach (string Name in FoldersFiles)
                                Console.WriteLine(Name);
                        }
<<<<<<< HEAD
                        else if (viewModelMessage.Command == "get")
=======
                        else if (viewModelMessage.Command == "file")
>>>>>>> ca1901474a42e43156dabac66486da056433aaf7
                        {
                            string[] DataMessage = viewModelSend.Message.Split(new string[1] { " " }, StringSplitOptions.None);
                            string getFile = "";
                            for (int i = 1; i < DataMessage.Length; i++)
                                if (getFile == "")
                                    getFile = DataMessage[i];
                                else
                                    getFile += " " + DataMessage[i];
<<<<<<< HEAD

                            try
                            {
                                FileInfoFTP fileInfo = JsonConvert.DeserializeObject<FileInfoFTP>(viewModelMessage.Data);
                                File.WriteAllBytes(getFile, fileInfo.Data);
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine($"Файл '{getFile}' успешно получен");
                            }
                            catch (Exception ex)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine($"Ошибка при сохранении файла: {ex.Message}");
                            }
=======
                            byte[] byteFile = JsonConvert.DeserializeObject<byte[]>(viewModelMessage.Data);
                            File.WriteAllBytes(getFile, byteFile);
>>>>>>> ca1901474a42e43156dabac66486da056433aaf7
                        }
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Подключение не удалось.");
                }
                socket.Close();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Что-то случилось: " + ex.Message);
            }

        }

        static void Main(string[] args)
        {
            Console.Write("Введите IP адрес сервера: ");
            string sIpAddress = Console.ReadLine();

            Console.Write("Введите порт: ");
            string sPort = Console.ReadLine();

            if (int.TryParse(sPort, out Port) && IPAddress.TryParse(sIpAddress, out IpAdress))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Данные успешно введены. Подключаюсь к серверу.");
                while (true)
                {
                    ConnectServer();
                }
            }
        }
    }
}
