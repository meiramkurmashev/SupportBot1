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
using System.Collections.ObjectModel;
using System.ComponentModel;



namespace SupportBot1
{
    public class TelegramUser : INotifyPropertyChanged, IEquatable<TelegramUser>//2 интерфейса: 1-оповещает внешних агентов,а 2-за сравнивание двух пользователей
    {
        public TelegramUser(string Nickname, long ChatId)
        {
            this.nick = Nickname;
            this.id = ChatId;
            Messages = new ObservableCollection<string>();

        }
        private string nick;
        public string Nick
        {
            get { return this.nick; }
            set
            {
                this.nick = value;// ?
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.Nick)));//событие -> присваивание

            }
        }
        private long id;
        public long Id
        {
            get { return this.id; }
            set
            {
                this.id = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.Id)));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;//оповещение внешних агентов->операции выше [getы и setы]
        public bool Equals(TelegramUser other) => other.Id == this.id;//метод добавления сообщения в коллекцию к существующему юзеру
        public ObservableCollection<string> Messages { get; set; }//коллекция сообщений
        public void AddMessage(string Text) => Messages.Add(Text);//метод добавления сообщения
    }
}
