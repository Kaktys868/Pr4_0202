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

namespace Server
{
    public class Program
    {
        public static List<User> Users = new List<User>();
        public static IPAddress IpAddress;
        public static int Port;
        static void Main(string[] args)
        {
            //Users.Add(new User("admin","admin", @"C:\Users\student-a502.PERMAVIAT\"));

            Console.Write("Введите IP адрес сервера: ");
            string sIdAddress = Console.ReadLine();
            Console.Write("Введите порт: ");
            string sPort = Console.ReadLine();
            if (int.TryParse(sPort, out Port) && IPAddress.TryParse(sIdAddress, out IpAddress))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Данные успешно введены. Запускаю сервер.");
                StartServer();
            }
            Console.Read();
        }

        public static bool AutorizationUser(string login, string password)
        {
            User user = null;
            user = Users.Find(x => x.login == login && x.password == password);
            return user != null;
        }
        public static List<string> GetDirectory(string src)
        {
            List<string> FoldersFiles = new List<string>();
            if (Directory.Exists(src))
            {
                string[] dirs = Directory.GetDirectories(src);
                foreach (string dir in dirs)
                {
                    FoldersFiles.Add(dir + "\\");
                }
                string[] files = Directory.GetFiles(src);
                foreach (string file in files)
                {
                    FoldersFiles.Add(file);
                }
            }
            return FoldersFiles;
        }
        public static void StartServer()
        {
            IPEndPoint endPoint = new IPEndPoint(IpAddress, Port);
            Socket sListener = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.Tcp);
            sListener.Bind(endPoint);
            sListener.Listen(10);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Сервер запущен");
            while (true)
            {
                try
                {
                    Socket Handler = sListener.Accept();
                    string Data = null;
                    byte[] Bytes = new byte[10485760];
                    int BytesRec = Handler.Receive(Bytes);
                    Data += Encoding.UTF8.GetString(Bytes, 0, BytesRec);
                    Console.Write("Сообщение от пользователя: " + Data + "\n");
                    string Reply = "";
                    ViewModelSend ViewModelSend = JsonConvert.DeserializeObject<ViewModelSend>(Data);
                    if (ViewModelSend != null)
                    {
                        ViewModelMessage viewModelMessage;
                        string[] DataCommand = ViewModelSend.Message.Split(new string[1] { " " }, StringSplitOptions.None);
                        if (DataCommand[0] == "connect")
                        {
                            string[] DataMessage = ViewModelSend.Message.Split(new string[1] { " " }, StringSplitOptions.None);
                            if (AutorizationUser(DataMessage[1], DataMessage[2]))
                            {
                                int userId = Users.FindIndex(x => x.login == DataMessage[1] && x.password == DataMessage[2]);
                                viewModelMessage = new ViewModelMessage("authorization", userId.ToString());
                            }
                            else
                            {
                                viewModelMessage = new ViewModelMessage("message", "Неправильный логин и пароль пользователя.");
                            }
                            Reply = JsonConvert.SerializeObject(viewModelMessage);
                            byte[] message = Encoding.UTF8.GetBytes(Reply);
                            Handler.Send(message);
                        }
                        else if (DataCommand[0] == "cd")
                        {
                            if (ViewModelSend.Id != -1)
                            {
                                string[] DataMessage = ViewModelSend.Message.Split(new string[1] { " " }, StringSplitOptions.None);
                                List<string> FoldersFiles = new List<string>();
                                if (DataMessage.Length == 1)
                                {
                                    Users[ViewModelSend.Id].temp_src = Users[ViewModelSend.Id].src;
                                    FoldersFiles = GetDirectory(Users[ViewModelSend.Id].src);
                                }
                                else
                                {
                                    string cdFolder = "";
                                    for (int i = 1; i < DataMessage.Length; i++)
                                    {
                                        if (cdFolder == "") cdFolder += DataMessage[i];
                                        else cdFolder += " " + DataMessage[i];
                                    }
                                    Users[ViewModelSend.Id].temp_src = Users[ViewModelSend.Id].src + cdFolder;
                                    FoldersFiles = GetDirectory(Users[ViewModelSend.Id].temp_src);
                                }
                                if (FoldersFiles.Count == 0)
                                {
                                    viewModelMessage = new ViewModelMessage("message", "Директория пуста или не существует.");
                                }
                                else
                                {
                                    viewModelMessage = new ViewModelMessage("cd", JsonConvert.SerializeObject(FoldersFiles));
                                }
                            }
                            else
                            {
                                viewModelMessage = new ViewModelMessage("message", "Необходимо авторизоваться");
                            }
                            Reply = JsonConvert.SerializeObject(viewModelMessage);
                            byte[] message = Encoding.UTF8.GetBytes(Reply);
                            Handler.Send(message);
                        }
                        else if (DataCommand[0] == "get")
                        {
                            if (ViewModelSend.Id != -1)
                            {
                                string[] DataMessage = ViewModelSend.Message.Split(new string[1] { " " }, StringSplitOptions.None);
                                string getFile = "";
<<<<<<< HEAD
                                for (int i = 1; i < DataMessage.Length; i++)
=======
                                for (int i = 0; i < DataMessage.Length; i++)
>>>>>>> ca1901474a42e43156dabac66486da056433aaf7
                                {
                                    if (getFile == "") getFile += DataMessage[i];
                                    else getFile += " " + DataMessage[i];
                                }
<<<<<<< HEAD

                                try
                                {
                                    string filePath = Users[ViewModelSend.Id].temp_src + getFile;
                                    if (File.Exists(filePath))
                                    {
                                        byte[] byteFile = File.ReadAllBytes(filePath);
                                        FileInfoFTP fileInfo = new FileInfoFTP(byteFile, Path.GetFileName(getFile));
                                        viewModelMessage = new ViewModelMessage("get", JsonConvert.SerializeObject(fileInfo));
                                    }
                                    else
                                    {
                                        viewModelMessage = new ViewModelMessage("message", $"Файл '{filePath}' не найден");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    viewModelMessage = new ViewModelMessage("message", $"Ошибка при чтении файла: {ex.Message}");
                                }
=======
                                byte[] byteFile = File.ReadAllBytes(Users[ViewModelSend.Id].temp_src + getFile);
                                viewModelMessage = new ViewModelMessage("file", JsonConvert.SerializeObject(byteFile));
>>>>>>> ca1901474a42e43156dabac66486da056433aaf7
                            }
                            else
                            {
                                viewModelMessage = new ViewModelMessage("message", "Необходимо авторизоваться");
                            }
                            Reply = JsonConvert.SerializeObject(viewModelMessage);
                            byte[] message = Encoding.UTF8.GetBytes(Reply);
                            Handler.Send(message);
                        }
                        else
                        {
                            if (ViewModelSend.Id != -1)
                            {
                                FileInfoFTP SendFileInfo = JsonConvert.DeserializeObject<FileInfoFTP>(ViewModelSend.Message);
<<<<<<< HEAD
                                File.WriteAllBytes(Users[ViewModelSend.Id].temp_src + SendFileInfo.Name, SendFileInfo.Data);
=======
                                File.WriteAllBytes(Users[ViewModelSend.Id].temp_src + @"\" + SendFileInfo.Name, SendFileInfo.Data);
>>>>>>> ca1901474a42e43156dabac66486da056433aaf7
                                viewModelMessage = new ViewModelMessage("message", "Файл загружен");
                            }
                            else
                            {
                                viewModelMessage = new ViewModelMessage("message", "Необходимо авторизоваться");
                            }
                            Reply = JsonConvert.SerializeObject(viewModelMessage);
                            byte[] message = Encoding.UTF8.GetBytes(Reply);
                            Handler.Send(message);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Что-то случилось: " + ex.Message);
                }
            }
        }
    }
}
