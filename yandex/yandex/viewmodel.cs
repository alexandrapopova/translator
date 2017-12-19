using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Input;
using System.Data.SqlClient;
using System.Windows;

namespace yandex
{
    class viewmodel : INotifyPropertyChanged
    {
        private Translator tr;
        private string _origin;
        public string Origin
        {
            get { return _origin; }
            set
            {
                _origin = value;
                OnPropertyChanged("Origin");
            }
        }
        private string _translation;
        public string Translation
        {
            get { return _translation; }
            set
            {
                _translation = value;
                OnPropertyChanged("Translation");
            }
        }
        public Command Translate { get; set; } = new Command();
        public viewmodel()
        {
            Translate.Func = TrText;
        }
        //REVIEW: А зачем он async?
        private async void TrText(object parameter)
        {
            Translator tr = new Translator();
            Translation = tr.Translate(Origin);
            SqlConnectionStringBuilder c = new SqlConnectionStringBuilder();
            //REVIEW: А если у меня SQL Server по-другому называется? И БД? И вход по логину?
            c.DataSource = ".\\SQLEXPRESS";
            c.InitialCatalog = "Translator";
            c.IntegratedSecurity = true;

            using (SqlConnection dbase = new SqlConnection(c.ConnectionString))
            {
                SqlCommand comm = new SqlCommand("insert into Recent(origin, translation) values (@in, @out)", dbase);
                comm.Parameters.AddWithValue("@in", Origin);
                comm.Parameters.AddWithValue("@out", Translation);
                try
                {
                    //REVIEW: Смысл тут await городить, если всё равно синхронно используете?
                    await dbase.OpenAsync();
                    await comm.ExecuteNonQueryAsync();
                    dbase.Close();
                }
                catch
                {
                    MessageBox.Show("Database is inaccessible");
                }
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string v)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(v));
        }
    }
    class Command : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }
        public Action<object> Func { get; set; }
        public void Execute(object parameter)
        {
            Func(parameter);
        }
    }
}
