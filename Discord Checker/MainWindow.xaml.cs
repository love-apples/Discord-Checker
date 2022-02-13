using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Leaf.xNet;
using Newtonsoft.Json;

namespace Discord_Checker
{
    public class @meDicord
    {
        public string verified { get; set; }
    }
    public partial class MainWindow : Window
    {
        public static string CheckerInfo = "Discord Checker by @loveappless\n" +
                                           "Version: 1.0\n" +
                                           "-------------------------------\n\n";

        public static string SaveLogsPath = ".";
        public static int EndedThreadsCheck = 0;
        public static int AllTokensCount = 0;
        public static int AllValidTokens = 0;
        public static int UnverifValidTokens = 0;
        public static int VerifValidTokens = 0;
        public static int InvalidTokens = 0;
        public static int NitroTokens = 0;
        public static int PaymentsTokens = 0;
        public static int StartedThreads = 0;

        public static string PutDoLog = "";
        public static bool Started = false;
        
        public static bool Proxies = false;
        public static List<string> AllValidList = new List<string>();
        public static List<string> AllInvalidList = new List<string>();
        public static List<string> UnverifValidList = new List<string>();
        public static List<string> VerifValidList = new List<string>();
        public static List<string> NitroList = new List<string>();
        public static List<string> PaymentsList = new List<string>();
        public static List<string> ProxiesList = new List<string>();
        
        public static List<string> CheckProxi = new List<string>();
        public static List<string> CheckedProxi = new List<string>();
        public static int EndThreadsCheckProxi = 0;

        public MainWindow()
        {
            InitializeComponent();
        }
        
        private void UIElement_OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".txt";
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                var filename = dlg.FileName;
                PutDoTokens.Text = filename;
            }
        }
        
        private void UIElement_OnPreviewMouseDown_GetProxies(object sender, MouseButtonEventArgs e)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".txt";
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                var filename = dlg.FileName;
                PutDoProxies.Text = filename;
            }
        }

        public void CheckProxiFun(object? proxi)
        {
            var proxiCheck = (string) proxi;
            using (var request = new HttpRequest())
            {
                try
                {
                    if (proxi is null || proxi == "")
                    {
                        EndThreadsCheckProxi++;
                        return;
                    }
                    request.ConnectTimeout = 5000;
                    request.KeepAliveTimeout = 5000;
                    request.ReadWriteTimeout = 5000;
                    request.Proxy = HttpProxyClient.Parse(proxiCheck);
                    request.Get("https://discord.com/");
                    CheckedProxi.Add(proxiCheck);
                    Dispatcher.Invoke((Action) (() => { LogsBoxCheckedTokens.Text += $"Валид: {proxi}\n"; }));
                }
                catch (HttpException err)
                {
                    Dispatcher.Invoke((Action) (() => { LogsBoxCheckedTokens.Text += $"Невалид: {proxi}\n"; }));
                }
                    
            }

            EndThreadsCheckProxi++;
        }

        void GetCheckedProxiesOwnerThread(object? allProxiesObj)
        {
            var allProxies = (int) allProxiesObj;
            while (true)
            {
                // Dispatcher.Invoke((Action) (() => { LogsBox.Text = AllProxies + " " + EndThreadsCheckProxi; }));
                if (allProxies == EndThreadsCheckProxi)
                {
                    var proxiesFileCreate = new FileInfo(@$"[LA] Проверенные прокси.txt");
                    using (StreamWriter sw = proxiesFileCreate.CreateText()) 
                    {
                        foreach (var checkProxiFile in CheckedProxi)
                        {
                            sw.WriteLine(checkProxiFile);
                        }
                    }
                    Dispatcher.Invoke((Action) (() => { LogsBox.Text += "Проверенные прокси сохранены в файл '[LA] Проверенные прокси.txt'\n"; }));
                    return;
                }
            }
        }

        void GetCheckedProxies(object? filePut)
        {
            var fileProxies = (string)filePut;
            var checkProxi = File.ReadAllLines(fileProxies);
            var allProxies = checkProxi.Length;
            if (allProxies == 0)
            {
                Dispatcher.Invoke((Action) (() => { LogsBox.Text += "Прокси не найдены.\n"; }));
                return;
            }
            
            var threadCheckProxiOwner = new Thread(new ParameterizedThreadStart(GetCheckedProxiesOwnerThread)) {IsBackground = true};
            threadCheckProxiOwner.Priority = ThreadPriority.Highest;
            threadCheckProxiOwner.Start(allProxies);
            
            foreach (var proxi in checkProxi)
            {
                var threadCheckProxi = new Thread(new ParameterizedThreadStart(CheckProxiFun)) {IsBackground = true};
                threadCheckProxi.Priority = ThreadPriority.Highest;
                threadCheckProxi.Start(proxi);
            }
            
            Dispatcher.Invoke((Action) (() => { LogsBoxCheckedTokens.Text += $"---------------[{DateTime.Now}]---------------\n"; }));
        }

        void WorkThreads(object? threadTokens)
        {
            while (true)
            {
                var threads = (int) threadTokens;
                // Dispatcher.Invoke((Action) (() => { LogsBox.Text = $"{threads} {EndedThreadsCheck}"; }));
                // Thread.Sleep(100);
                // Dispatcher.Invoke((Action) (() => { LogsBox.Clear(); }));
                
                if (threads <= EndedThreadsCheck)
                {
                    var time = @$"{DateTime.Now.Hour}:{DateTime.Now.Minute}:{DateTime.Now.Second}";
                    EndedThreadsCheck = 0;
                    Dispatcher.Invoke((Action) (() => { LogsBox.Text += $"Проверка токенов завершена.\nРезультаты:\n\nВзято токенов из файла: {AllTokensCount}\nВсего валид: {AllValidTokens} шт.\nВалид + вериф: {VerifValidTokens} шт.\nС нитро: {NitroTokens} шт.\n\nВалид без вериф: {UnverifValidTokens} шт.\nСо способами оплаты: {PaymentsTokens} шт.\n\nНевалид: {InvalidTokens} шт.\n\nЛог сохранён в " + @$"{PutDoLog}" + "\n\n        "; }));

                    File.WriteAllText(@$"{PutDoLog}\[!] Результаты.txt", $"{CheckerInfo}Взято токенов из файла: {AllTokensCount}\nВсего валид: {AllValidTokens} шт.\nВалид + вериф: {VerifValidTokens} шт.\nС нитро: {NitroTokens} шт.\n\nВалид без вериф: {UnverifValidTokens} шт.\nСо способами оплаты: {PaymentsTokens} шт.\n\nНевалид: {InvalidTokens} шт.");
                    var allValid = new FileInfo(@$"{PutDoLog}\[{AllValidTokens}] Всего валид.txt");
                    using (StreamWriter sw = allValid.CreateText()) 
                    {
                        foreach (var valid in AllValidList)
                        {
                            sw.WriteLine(valid);
                        }
                    }
                    
                    var onlyVerifValid = new FileInfo(@$"{PutDoLog}\[{VerifValidTokens}] Вериф + валид.txt");
                    using (StreamWriter sw = onlyVerifValid.CreateText()) 
                    {
                        foreach (var verifValid in VerifValidList)
                        {
                            sw.WriteLine(verifValid);
                        }
                    }
                    
                    var onlyUnverifValid = new FileInfo(@$"{PutDoLog}\[{UnverifValidTokens}] !Невериф + валид.txt");
                    using (StreamWriter sw = onlyUnverifValid.CreateText()) 
                    {
                        foreach (var unverifValid in UnverifValidList)
                        {
                            sw.WriteLine(unverifValid);
                        }
                    }
                    
                    var nitroTokens = new FileInfo(@$"{PutDoLog}\[{NitroTokens}] С нитро.txt");
                    using (StreamWriter sw = nitroTokens.CreateText()) 
                    {
                        foreach (var nitro in NitroList)
                        {
                            sw.WriteLine(nitro);
                        }
                    }
                    
                    var paymentsTokens = new FileInfo(@$"{PutDoLog}\[{PaymentsTokens}] Со способами оплаты.txt");
                    using (StreamWriter sw = paymentsTokens.CreateText()) 
                    {
                        foreach (var payment in PaymentsList)
                        {
                            sw.WriteLine(payment);
                        }
                    }
                    
                    var invalidTokens = new FileInfo(@$"{PutDoLog}\[{InvalidTokens}] Всего невалид.txt");
                    using (StreamWriter sw = invalidTokens.CreateText()) 
                    {
                        foreach (var invalidT in AllInvalidList)
                        {
                            sw.WriteLine(invalidT);
                        }
                    }

                    AllTokensCount = 0;
                    AllValidTokens = 0;
                    InvalidTokens = 0;
                    VerifValidTokens = 0;
                    UnverifValidTokens = 0;
                    NitroTokens = 0;
                    PaymentsTokens = 0;
                    StartedThreads = 0;
                    Started = false;
                    
                    PutDoLog = "";
                    Proxies = false;
                    
                    AllValidList = new List<string>();
                    AllInvalidList = new List<string>();
                    UnverifValidList = new List<string>();
                    VerifValidList = new List<string>();
                    NitroList = new List<string>();
                    PaymentsList = new List<string>();
                    ProxiesList = new List<string>();
                    return;
                }
            }
        }

        public void RequestMe(object? tokenObj)
        {
            var token = (string) tokenObj;
            if (!Proxies)
            {
                using (var request = new HttpRequest())
                {
                    request.AddHeader("Authorization", token);
                    request.UserAgent =
                        "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:95.0) Gecko/20100101 Firefox/95.0";
                    try
                    {
                        var meDiscordResponse = request.Get("https://discord.com/api/v9/users/@me");
                        var jsonRespMeDicord = JsonConvert.DeserializeObject<@meDicord>(meDiscordResponse.ToString());
                        if (jsonRespMeDicord.verified == "true")
                        {
                            VerifValidList.Add(token);
                            VerifValidTokens++;
                        }
                        else
                        {
                            UnverifValidList.Add(token);
                            UnverifValidTokens++;
                        }

                        AllValidList.Add(token);
                        AllValidTokens++;
                        
                        Dispatcher.Invoke((Action) (() => { LogsBoxCheckedTokens.Text += $"Валид: {token}\n"; }));
                    }
                    catch (HttpException e)
                    {
                        AllInvalidList.Add(token);
                        InvalidTokens++;

                        Dispatcher.Invoke((Action) (() => { LogsBoxCheckedTokens.Text += $"Невалид: {token}\n"; }));
                        // Dispatcher.Invoke((Action) (() => { LogsBox.Text += $"{e.HttpStatusCode}\n"; }));
                    }

                    request.Dispose();
                    request.Close();
                    EndedThreadsCheck++;
                }
            }
            else
            {
                foreach (var proxi in ProxiesList)
                {
                    using (var request = new HttpRequest())
                    {
                        request.AddHeader("Authorization", token);
                        request.UserAgent =
                            "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:95.0) Gecko/20100101 Firefox/95.0";
                        request.Proxy = HttpProxyClient.Parse(proxi);
                        try
                        {
                            var meDiscordResponse = request.Get("https://discord.com/api/v9/users/@me");
                            var jsonRespMeDicord = JsonConvert.DeserializeObject<@meDicord>(meDiscordResponse.ToString());
                            if (jsonRespMeDicord.verified == "true")
                            {
                                VerifValidList.Add(token);
                                VerifValidTokens++;
                            }
                            else
                            {
                                UnverifValidList.Add(token);
                                UnverifValidTokens++;
                            }

                            AllValidList.Add(token);
                            AllValidTokens++;

                            request.Dispose();
                            request.Close();
                            EndedThreadsCheck++;
                            Dispatcher.Invoke((Action) (() => { LogsBoxCheckedTokens.Text += $"Валид: {token}\n"; }));
                            return;
                        }
                        catch (HttpException e)
                        {
                            if (e.HttpStatusCode.ToString() != "Unauthorized")
                            {
                                continue;
                            }
                            else
                            {
                                request.Dispose();
                                request.Close();
                                InvalidTokens++;
                                AllInvalidList.Add(token);
                                EndedThreadsCheck++;
                                Dispatcher.Invoke((Action) (() => { LogsBoxCheckedTokens.Text += $"Невалид: {token}\n"; }));
                                return;
                            }
                            
                            // Dispatcher.Invoke((Action) (() => { LogsBox.Text += $"{e.HttpStatusCode}\n"; }));
                        }
                    }
                }
            }
        }
        
        public void RequestNitro(object? tokenObj)
        {
            var token = (string) tokenObj;
            if (!Proxies)
            {
                using (var request = new HttpRequest())
                {
                    
                    request.AddHeader("Authorization", token);
                    request.UserAgent =
                        "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:95.0) Gecko/20100101 Firefox/95.0";
                    
                    try 
                    {
                        var nitroResponse = request.Get("https://discord.com/api/v9/users/@me/billing/subscriptions")
                            .ToString();
                        if (nitroResponse != "[]")
                        {
                            NitroTokens++;
                            NitroList.Add(token);
                        }
                        // Dispatcher.Invoke((Action) (() => { LogsBox.Text += $"{nitroResponse}\n"; }));
                    }
                    catch (HttpException e)
                    {
                        // Dispatcher.Invoke((Action) (() => { LogsBox.Text += $"{e}\n"; }));
                    }

                    request.Dispose();
                    request.Close();
                    EndedThreadsCheck++;
                }
            }
            else
            {
                foreach (var proxi in ProxiesList)
                {
                    using (var request = new HttpRequest())
                    {
                        request.AddHeader("Authorization", token);
                        request.UserAgent =
                            "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:95.0) Gecko/20100101 Firefox/95.0";
                        request.Proxy = HttpProxyClient.Parse(proxi);
                        try
                        {
                            var nitroResponse = request.Get("https://discord.com/api/v9/users/@me/billing/subscriptions")
                                .ToString();
                            if (nitroResponse != "[]")
                            {
                                NitroTokens++;
                                NitroList.Add(token);
                            }

                            request.Dispose();
                            request.Close();
                            EndedThreadsCheck++;
                            return;
                        }
                        catch (HttpException e)
                        {
                            if (e.HttpStatusCode.ToString() != "Unauthorized")
                            {
                                continue;
                            }
                            else
                            {
                                request.Dispose();
                                request.Close();
                                EndedThreadsCheck++;
                                return;
                            }
                            
                            // Dispatcher.Invoke((Action) (() => { LogsBox.Text += $"{e.HttpStatusCode}\n"; }));
                        }
                    }
                }
            }
        }
        
        public void RequestPayments(object? tokenObj)
        {
            var token = (string) tokenObj;
            if (!Proxies)
            {
                using (var request = new HttpRequest())
                {
                    
                    request.AddHeader("Authorization", token);
                    request.UserAgent =
                        "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:95.0) Gecko/20100101 Firefox/95.0";
                    try
                    {
                        var paymentsResponse = request
                            .Get("https://discord.com/api/v9/users/@me/billing/payment-sources").ToString();
                        if (paymentsResponse != "[]")
                        {
                            PaymentsTokens++;
                            PaymentsList.Add(token);
                        }
                        // Dispatcher.Invoke((Action) (() => { LogsBox.Text += $"{nitroResponse}\n"; }));
                    }
                    catch (HttpException e)
                    {
                        // Dispatcher.Invoke((Action) (() => { LogsBox.Text += $"{e}\n"; }));
                    }

                    request.Dispose();
                    request.Close();
                }
            }
            else
            {
                foreach (var proxi in ProxiesList)
                {
                    using (var request = new HttpRequest())
                    {
                        request.AddHeader("Authorization", token);
                        request.UserAgent =
                            "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:95.0) Gecko/20100101 Firefox/95.0";
                        request.Proxy = HttpProxyClient.Parse(proxi);
                        try
                        {
                            var paymentsResponse = request
                                .Get("https://discord.com/api/v9/users/@me/billing/payment-sources").ToString();
                            if (paymentsResponse != "[]")
                            {
                                PaymentsTokens++;
                                PaymentsList.Add(token);
                            }

                            request.Dispose();
                            request.Close();
                            EndedThreadsCheck++;
                            return;
                        }
                        catch (HttpException e)
                        {
                            if (e.HttpStatusCode.ToString() != "Unauthorized")
                            {
                                continue;
                            }
                            else
                            {
                                request.Dispose();
                                request.Close();
                                EndedThreadsCheck++;
                                return;
                            }
                            
                            // Dispatcher.Invoke((Action) (() => { LogsBox.Text += $"{e.HttpStatusCode}\n"; }));
                        }
                    }
                }
            }

            EndedThreadsCheck++;
        }
        
        private void ButtonBase_OnClick_Start(object sender, RoutedEventArgs e)
        {
            if (Started) return;

            Started = true;
            AllTokensCount = 0;
            AllValidTokens = 0;
            InvalidTokens = 0;
            VerifValidTokens = 0;
            UnverifValidTokens = 0;
            NitroTokens = 0;
            PaymentsTokens = 0;
            StartedThreads = 0;

            PutDoLog = "";
            Proxies = false;
                    
            AllValidList = new List<string>();
            UnverifValidList = new List<string>();
            VerifValidList = new List<string>();
            NitroList = new List<string>();
            PaymentsList = new List<string>();

            if (PutDoTokens.Text == "" || PutDoTokens.Text == "Нажмите, чтобы изменить путь")
            {
                LogsBox.Text += "Укажите путь до текстового документа с токенами!\n";
                Started = false;
                return;
            }
            
            if (SaveLogs.Text != "Нажмите, чтобы изменить путь (по умолчанию - в корневой папке чекера)")
                SaveLogsPath = SaveLogs.Text;
            
            var putDoTokens = PutDoTokens.Text;

            var newDate = DateTime.Now.ToString().Replace(":", "-").Replace(".", "-");

            if (!Directory.Exists(@$"{SaveLogsPath}\[LA] Results Discord Checker"))
                Directory.CreateDirectory(@$"{SaveLogsPath}\[LA] Results Discord Checker");
            
            Directory.CreateDirectory(@$"{SaveLogsPath}\[LA] Results Discord Checker\[{newDate}] Результаты");
            PutDoLog = @$"{SaveLogsPath}\[LA] Results Discord Checker\[{newDate}] Результаты";
            
            var tokensFile = File.ReadAllLines((string) putDoTokens);
            if (tokensFile.Length == 0)
            {
                LogsBox.Text += "Токенов не найдено.\n\n";
                Started = false;
                return;
            }

            var a = tokensFile.ToList();
            var tokens = a.Distinct().ToList();
            AllTokensCount = tokens.Count;
            StartedThreads = tokens.Count * 3-3;

            LogsBox.Text += "Создание новых потоков...\n\n";
            var threadOwner = new Thread(new ParameterizedThreadStart(WorkThreads)) {IsBackground = true};
            threadOwner.Start(StartedThreads);

            if (PutDoProxies.Text != "Нажмите, чтобы изменить путь (не трогайте это поле, если прокси не требуются)")
            {
                Proxies = true;
                ProxiesList = File.ReadAllLines(PutDoProxies.Text).ToList();
            }

            foreach (var token in tokens)
            {
                var threadMe = new Thread(new ParameterizedThreadStart(RequestMe)) { IsBackground = true };
                threadMe.Priority = ThreadPriority.Highest;
                threadMe.Start(token);

                var threadNitro = new Thread(new ParameterizedThreadStart(RequestNitro)) { IsBackground = true };
                threadNitro.Priority = ThreadPriority.Highest;
                threadNitro.Start(token);
                
                var threadPayments = new Thread(new ParameterizedThreadStart(RequestPayments)) { IsBackground = true };
                threadPayments.Priority = ThreadPriority.Highest;
                threadPayments.Start(token);
            }
            
            LogsBox.Text += $"---------------[{DateTime.Now}]---------------\n";
            LogsBoxCheckedTokens.Text += $"---------------[{DateTime.Now}]---------------\n";
            LogsBox.Text += $"Готово. Всего токенов/потоков {tokens.Count}.\nПроверка стартовала.\n\n";
        }

        private void SaveLogs_OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var dialog = new Microsoft.Win32.SaveFileDialog();
            dialog.InitialDirectory = SaveLogs.Text; 
            dialog.Title = "Select a Directory";
            dialog.Filter = "Directory|*.this.directory"; 
            dialog.FileName = "select";
            if (dialog.ShowDialog() == true) {
                string path = dialog.FileName;
                path = path.Replace("\\select.this.directory", "");
                path = path.Replace(".this.directory", "");
                if (!Directory.Exists(path)) {
                    Directory.CreateDirectory(path);
                }
                SaveLogs.Text = path;
            }
        }

        private void ButtonBase_OnClick_Stop(object sender, RoutedEventArgs e)
        {
            if (!Started) return;
            
            EndedThreadsCheck = StartedThreads;
        }

        private void GetProxiesBtn_GetProxies(object sender, RoutedEventArgs e)
        {
            if (PutDoProxies.Text == "Нажмите, чтобы изменить путь (не трогайте это поле, если прокси не требуются)")
            {
                LogsBox.Text += "Укажите путь до .txt файла с прокси.";
                return;
            }
            var threadGetProxies = new Thread(new ParameterizedThreadStart(GetCheckedProxies));
            threadGetProxies.Priority = ThreadPriority.Highest;
            threadGetProxies.Start(PutDoProxies.Text);
        }

        private void GetProxiesBtn_AllClear(object sender, RoutedEventArgs e)
        {
            PutDoTokens.Text = "Нажмите, чтобы изменить путь";
            SaveLogs.Text = "Нажмите, чтобы изменить путь (по умолчанию - в корневой папке чекера)";
            PutDoProxies.Text = "Нажмите, чтобы изменить путь (если прокси требуются)";
            LogsBox.Text = "";
            LogsBoxCheckedTokens.Text = "";
        }

        private void Contacts_OnClick(object sender, RoutedEventArgs e)
        {
            Process.Start("explorer.exe", "https://www.t.me/loveappless/");
        }
    }
}


// coded by @loveappless