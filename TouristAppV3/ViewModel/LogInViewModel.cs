using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Linq;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using TouristAppV3.Annotations;
using TouristAppV3.Common;
using TouristAppV3.Model;
using TouristAppV3.View;

namespace TouristAppV3.ViewModel
{
    class LogInViewModel : INotifyPropertyChanged
    {
                private ObservableCollection<UserModel> _users;
        private UserModel _newUserModel;
        private ICommand _addNewUserCommand;
        private ICommand _logInCommand;
        private MainPage _clear;

        public LogInViewModel()
        {
            _addNewUserCommand = new RelayCommand(AddNewUserModelCommand);
            _newUserModel = new UserModel();
            _logInCommand = new RelayCommand(LogInUserModelCommand);
            LoadUsers();
        }

        private async void LoadUsers()
        {
            StorageFile file = null;
            try
            {
                file = await Windows.Storage.ApplicationData.Current.LocalFolder.GetFileAsync("Users.xml");
            }
            catch (Exception)
            {
            }

            if (file == null)
            {
                StorageFolder installationFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
                string xmlfile = @"Assets\Xml\Users.xml";
                file = await installationFolder.GetFileAsync(xmlfile);
            }

            Stream userStream = await file.OpenStreamForReadAsync();
            XDocument usersModelDocument = XDocument.Load(userStream);

            IEnumerable<XElement> usersModelList = usersModelDocument.Descendants("user");
            Users = new ObservableCollection<UserModel>();

            foreach (XElement xElement in usersModelList)
            {
                Users.Add(new UserModel()
                {
                    UserName = xElement.Element("username").Value,
                    Password = xElement.Element("password").Value
                });
            }

            OnPropertyChanged("Users");
        }

        private async void AddNewUserModelCommand()
        {
            StorageFile file = null;
            try
            {
                file = await Windows.Storage.ApplicationData.Current.LocalFolder.GetFileAsync("users.xml");
            }
            catch (Exception)
            {
            }

            if (file == null)
            {
                StorageFolder installationFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
                string xmlfile = @"Assets\Xml\Users.xml";
                file = await installationFolder.GetFileAsync(xmlfile);
            }

            Stream LoadStream = await file.OpenStreamForReadAsync();
            XDocument userDocument = XDocument.Load(LoadStream);
            LoadStream.Dispose();

            XElement userList = userDocument.Element("users");

            XElement user = new XElement("user");
            user.Add(new XElement("username", NewUserModel.UserName));
            user.Add(new XElement("password", NewUserModel.Password));

            for (int i = 0; i < 1; i++)
            {


                foreach (UserModel userModel in Users)
                {
                    if (NewUserModel.UserName == userModel.UserName)
                    {
                        string message = "This username is not avaliable";
                        MessageDialog dialog = new MessageDialog(message);
                        await dialog.ShowAsync();
                        i ++;
                        break;
                    }
                }
                if (i == 0)
                {
                    userList.LastNode.AddAfterSelf(user);

                    StorageFile saveFile = null;
                    try
                    {
                            saveFile = await Windows.Storage.ApplicationData.Current.LocalFolder.CreateFileAsync("Users.xml");
                    }
                    catch
                    {
                    }
                    if (saveFile == null)
                    {
                        saveFile = await Windows.Storage.ApplicationData.Current.LocalFolder.GetFileAsync("Users.xml");
                    }
                    Stream saveStream = await saveFile.OpenStreamForWriteAsync();
                    userDocument.Save(saveStream);
                    saveStream.Dispose();
                    OnPropertyChanged("Users");
                    LoadUsers();
                    string message = "Your account has been created";
                    MessageDialog dialog = new MessageDialog(message);
                    await dialog.ShowAsync();
                    break;
                }
            }

        }

        private async void LogInUserModelCommand()
        {
            StorageFile file = null;
            try
            {
                file = await Windows.Storage.ApplicationData.Current.LocalFolder.GetFileAsync("users.xml");
            }
            catch (Exception)
            {
            }

            if (file == null)
            {
                StorageFolder installationFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
                string xmlfile = @"Assets\Xml\Users.xml";
                file = await installationFolder.GetFileAsync(xmlfile);
            }

            Stream LoadStream = await file.OpenStreamForReadAsync();
            XDocument userDocument = XDocument.Load(LoadStream);
            LoadStream.Dispose();

            XElement userList = userDocument.Element("users");

            XElement user = new XElement("user");
            user.Add(new XElement("username", NewUserModel.UserName));
            user.Add(new XElement("password", NewUserModel.Password));

            for (int i = 0; i < 1; i++)
            {
                string message;
                MessageDialog dialog;
                foreach (UserModel userModel in Users)
                {
                    try
                    {
                        if (NewUserModel.UserName == userModel.UserName)
                        {
                            if (NewUserModel.Password == userModel.Password)
                            {
                                message = "Welcome " + NewUserModel.UserName;
                                dialog = new MessageDialog(message);
                                await dialog.ShowAsync();
                                i ++;
                                break;
                            }
                            else
                            {
                                message = "Invalid password or username";
                                dialog = new MessageDialog(message);
                                await dialog.ShowAsync();
                                i ++;
                                break;
                            }
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
                if (i == 0)
                {
                    message = "You are not registered";
                    dialog = new MessageDialog(message);
                    await dialog.ShowAsync();
                    break;                    
                }
            }
        }

        public ObservableCollection<UserModel> Users
        {
            get { return _users; }
            set { _users = value; }
        }

        public UserModel NewUserModel
        {
            get { return _newUserModel; }
            set { _newUserModel = value; }
        }

        public ICommand AddNewUserCommand
        {
            get { return _addNewUserCommand; }
            set { _addNewUserCommand = value; }
        }

        public ICommand LogInCommand
        {
            get { return _logInCommand; }
            set { _logInCommand = value; }
        }

        public MainPage Clear
        {
            get { return _clear; }
            set { _clear = value; }
        }

        public ObservableCollection<UserModel> FilteredUsers { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
