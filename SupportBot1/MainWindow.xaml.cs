using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using Telegram.Bot;
using System.IO;
//using System.Globalization;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net;
using Microsoft.Win32.SafeHandles;


namespace SupportBot1
{
    public partial class MainWindow : Window
    {
        ObservableCollection<TelegramUser> Users;// коллекция юзеров
        TelegramBotClient bot;//заводим переменную для клиента для телеграмботАПИ
        private Telegram.Bot.Types.File tFile;
        private FileStream fs;
        

        public MainWindow()
        {

            InitializeComponent();
            Users = new ObservableCollection<TelegramUser>();//инициализация коллекции юзеров
            userList.ItemsSource = Users;//отображение к окне

            string token = "5460626969:AAGDHyC_UV8Y-opBHI-CSlC1EEk9rNlMhVA";

            bot = new TelegramBotClient(token);//   идентификация АПИ с помощью токена
            
#pragma warning disable CS8622 // Допустимость значений NULL для ссылочных типов в типе параметра не соответствует целевому объекту делегирования (возможно, из-за атрибутов допустимости значений NULL).
            bot.OnMessage += async delegate (object sender, Telegram.Bot.Args.MessageEventArgs e)//события сообщения боту
            {
                
                if(e.Message.Type == Telegram.Bot.Types.Enums.MessageType.Document)
                {
                   // string path = @"c:\temp\MyTest";
                    tFile = await bot.GetFileAsync(e.Message.Document.FileId);
                    string fExt = System.IO.Path.GetExtension(tFile.FilePath).ToLower();
                    string fileName = @"c:\temp\" + e.Message.Document.FileName + DateTime.Now.ToString("dd.MM.yyyy hh-mm-ss") + fExt;
                    
                    FileStream  fs = new FileStream(fileName, FileMode.Create);

                    await bot.DownloadFileAsync(tFile.FilePath, fs );
                    fs.Close();
                    fs.Dispose();
                    //await bot.SendTextMessageAsync(e.Message.Chat.Id, getText(233), replyMarkup: keyboardYesNo());
                    
                }

                // !  сюда в событие добавить если сообщение такое то то .... а если .. то ... !!!!
                //if (e.Message.Text.ToLower() == "/start")
                //{
                //    await bot.SendTextMessageAsync(e.Message.Chat, "Добро пожаловать! Как вас зовут?");
                //    return;
                //    //if (e.Message.Text.ToLower() == "Мейрам")
                //    //{
                //    //    await bot.SendTextMessageAsync(e.Message.Chat, "Очень приятно Мейрам, чем могу помочь?");

                //    //    return;
                //    //}
                //}
                //if (e.Message.Text.ToLower() == "директор") { await bot.SendTextMessageAsync(e.Message.Chat, "asd"); return; }
                string msg = $"{DateTime.Now}: {e.Message.Chat.FirstName} {e.Message.Chat.Id}{"+++"}{e.Message.Text}\n";//формируем текст
                //string msg1 = $"{DateTime.Now}; {e.Message.Chat.FirstName}; {e.Message.Chat.Id};{e.Message.Text};";
                File.AppendAllText("data.log", $"{msg}\n");//логируем в файл
                //File.AppendAllText("data.txt", $"{msg1}");

                //Debug.WriteLine(msg);//проверка 
                this.Dispatcher.Invoke(() =>//добавление в коллекцию
                {
                    var person = new TelegramUser(e.Message.Chat.FirstName, e.Message.Chat.Id);//создание пользователя на основе класса 
                    if (!Users.Contains(person)) Users.Add(person);//дбавление пользоатели если нет и сообщения
                    if (e.Message.Type == Telegram.Bot.Types.Enums.MessageType.Document) Users[Users.IndexOf(person)].AddMessage($"{person.Nick}:{e.Message.Document.FileName}");
                    else { Users[Users.IndexOf(person)].AddMessage($"{person.Nick}:{e.Message.Text}"); }//добавление сообщение в коллекцию сообщений
                });
            };

#pragma warning restore CS8622 // Допустимость значений NULL для ссылочных типов в типе параметра не соответствует целевому объекту делегирования (возможно, из-за атрибутов допустимости значений NULL).
            bot.StartReceiving(); //старт сервиса т.е.-онМессадж
            
            btnSendMsg.Click += delegate { SendMsg(); }; //возможность отвечать по кнопке ОТПРАВИТЬ
            txtBxSendMsg.KeyDown += (s, e) => { if (e.Key == Key.Return) { SendMsg(); } };// ОТКУДА брать текст ответа и если кнопка ИНТЕР тогда вызываем метод sendMessage();




        }

        public void SendMsg()// метод отправки
        {
            var concreteUser = Users[Users.IndexOf(userList.SelectedItem as TelegramUser)];//получаем пользователя из списка выделением его
            string responseMsg = $"Support: {txtBxSendMsg.Text}";//переменная сообщения ОТВЕТ
            concreteUser.Messages.Add(responseMsg);//добавление ОТВЕТА Суппорта в коллекцию беседы с пользователем

            bot.SendTextMessageAsync(concreteUser.Id, txtBxSendMsg.Text);//отправление сообщения текущему пользователю
            string logText = $"{DateTime.Now}: >> {concreteUser.Id} {concreteUser.Nick} {responseMsg}\n";//добавление ответа в файл лога
            File.AppendAllText("data.log", logText);// --/--
            txtBxSendMsg.Text = String.Empty;// обнуление окна текста
        }

    }
}