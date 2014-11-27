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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using TouristAppV3.Annotations;
using TouristAppV3.Common;
using TouristAppV3.Model;
using TouristAppV3.View;

namespace TouristAppV3.ViewModel
{
    class OrderViewModel : INotifyPropertyChanged
    {
        private Categories _selectedCategory;
        private ObservableCollection<Categories> _categoriesModels;
        private CategoryItemModel _selectedCategoryItemModel;
        private ObservableCollection<CategoryItemModel> _categoryItems;
        private ObservableCollection<OrderModel> _orderModels;
        private OrderModel _newOrderModel;
        private ICommand _addNewOrderModel;
        private string _orderType;
        private string _arrivalTimeVisibility = "Collapsed";
        private string _toFromVisibility = "Collapsed";
        private string _serviceNotAvailable = "Collapsed";
        private string _orderPlacedSuccessfully = "Collapsed";

        public string OrderPlacedSuccessfully
        {
            get { return _orderPlacedSuccessfully; }
            set
            {
                _orderPlacedSuccessfully = value; 
                OnPropertyChanged("OrderPlacedSuccessfully");
            }
        }

        public string ServiceNotAvailable
        {
            get { return _serviceNotAvailable; }
            set
            {
                _serviceNotAvailable = value;
                OnPropertyChanged("ServiceNotAvailable");
            }
        }

        public string ToFromVisibility
        {
            get { return _toFromVisibility; }
            set
            {
                _toFromVisibility = value;
                OnPropertyChanged("ToFromVisibility");
            }
        }

        public string OrderType
        {
            get
            {
                try
                {
                    if (SelectedCategoryItemModel.Category == "Restaurants")
                    {
                        return "Table";
                    }
                    else if (SelectedCategoryItemModel.Category == "Hotels and Hostals")
                    {
                        return "Room";
                    }
                    else if (SelectedCategoryItemModel.Category == "Night Life")
                    {
                        return "Drinking Table";
                    }
                    else if (SelectedCategoryItemModel.Category == "Others")
                    {
                        return "";
                    }
                    else if (SelectedCategoryItemModel.Category == "Museums")
                    {
                        return "Museum ticket";
                    }
                    else if (SelectedCategoryItemModel.Category == "Entertainment")
                    {
                        return "";
                    }
                    else
                    {
                        return "";
                    }
                }
                catch (Exception)
                {
                    
                }
                return "";
            }
            set
            {
                _orderType = value; 
                OnPropertyChanged("OrderType");
            }
           }

        public string ArrivalTimeVisibility
        {
            get { return _arrivalTimeVisibility; }
            set
            {
                _arrivalTimeVisibility = value; 
                OnPropertyChanged("ArrivalTimeVisibility");
            }
        }

        public OrderViewModel()
        {
            _addNewOrderModel = new RelayCommand(AddNewOrderModelCommand);
            _newOrderModel = new OrderModel();
            _newOrderModel.FirstName = "";
            _newOrderModel.LastName = "";
            
            LoadCategories();
            LoadCategoryItemModels();
        }

        private async void LoadCategories()
        {
            StorageFolder installationFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
            string xmlfile = @"Assets\xml\Categories.xml";
            StorageFile file = await installationFolder.GetFileAsync(xmlfile);

            Stream categoryStream = await file.OpenStreamForReadAsync();
            XDocument categoriesModelDocument = XDocument.Load(categoryStream);

            IEnumerable<XElement> categoriesModelsList = categoriesModelDocument.Descendants("category");

            CategoriesModels = new ObservableCollection<Categories>();

            foreach (XElement xElement in categoriesModelsList)
            {
                CategoriesModels.Add(new Categories()
                {
                    Name = xElement.Element("name").Value,
                    //ImageUrl = xElement.Element("imageurl").Value,
                    CategoryItems = new List<CategoryItemModel>()
                });
            }
            OnPropertyChanged("CategoriesModels");
        }
        
        private async void LoadCategoryItemModels()
        {
            StorageFile file = null;
            try
            {
                file = await Windows.Storage.ApplicationData.Current.LocalFolder.GetFileAsync("categoryitems.xml");
            }
            catch (Exception)
            {
            }

            if (file == null)
            {
                StorageFolder installationFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
                string xmlfile = @"Assets\xml\CategoryItems.xml";
                file = await installationFolder.GetFileAsync(xmlfile);
            }

            Stream categoryItemStream = await file.OpenStreamForReadAsync();
            XDocument categoryItemDocument = XDocument.Load(categoryItemStream);

            IEnumerable<XElement> categoryItemList = categoryItemDocument.Descendants("categoryitem");

            foreach (XElement xElement in categoryItemList)
            {
                CategoryItemModel ci = new CategoryItemModel();
                ci.Name = xElement.Element("name").Value;
                ci.Category = xElement.Element("category").Value;
                ci.Adress = xElement.Element("adress").Value;
                ci.Email = xElement.Element("email").Value;
                ci.ImageUrl = xElement.Element("imageurl").Value;
                ci.Phone = xElement.Element("phone").Value;
                ci.Web = xElement.Element("web").Value;

                foreach (Categories categoriesModel in CategoriesModels)
                {
                    if (categoriesModel.Name.Equals(ci.Category))
                    {
                        categoriesModel.CategoryItems.Add(ci);
                    }
                }
            }
            OnPropertyChanged("CategoryItems");
        }

        private async void AddNewOrderModelCommand()
        {
            if (_newOrderModel.FirstName != "" && _newOrderModel.LastName !="")
            {
                StorageFile file = null;
                try
                {
                    file = await Windows.Storage.ApplicationData.Current.LocalFolder.GetFileAsync("orders.xml");
                }
                catch (Exception)
                {
                }

                if (file == null)
                {
                    StorageFolder installationFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
                    string xmlfile = @"Assets\xml\Orders.xml";
                    file = await installationFolder.GetFileAsync(xmlfile);
                }

                Stream LoadStream = await file.OpenStreamForReadAsync();
                XDocument orderDocument = XDocument.Load(LoadStream);
                LoadStream.Dispose();

                XElement orderList = orderDocument.Element("orders");

                XElement order = new XElement("order");
                order.Add(new XElement("firstname", NewOrderModel.FirstName));
                order.Add(new XElement("lastname", NewOrderModel.LastName));
                order.Add(new XElement("place", SelectedCategoryItemModel.Name));
                order.Add(new XElement("from", NewOrderModel.From.Date.ToString("D")));
                order.Add(new XElement("to", NewOrderModel.To.Date.ToString("D")));
                order.Add(new XElement("time", NewOrderModel.Time.ToString()));

                orderList.LastNode.AddAfterSelf(order);

                StorageFile saveFile = null;

                try
                {
                    saveFile = await Windows.Storage.ApplicationData.Current.LocalFolder.CreateFileAsync("Orders.xml");
                }
                catch { }

                if (saveFile == null)
                {
                    saveFile = await Windows.Storage.ApplicationData.Current.LocalFolder.GetFileAsync("Orders.xml");
                }

                Stream saveStream = await saveFile.OpenStreamForWriteAsync();
                orderDocument.Save(saveStream);
                saveStream.Dispose();
                _newOrderModel.FirstName = "";
                _newOrderModel.LastName = "";
                _orderPlacedSuccessfully = "Visible";
                OnPropertyChanged("OrderPlacedSuccessfully");
                OnPropertyChanged("OrderModels");
                OnPropertyChanged("NewOrderModel");
            }
            else
            {
                string error = "You need to fill in for first name and last name";
                MessageDialog dialog = new MessageDialog(error);
                dialog.ShowAsync();
            }
        }

        public Categories SelectedCategory
        {
            get { return _selectedCategory; }
            set
            {
                _selectedCategory = value;
                OnPropertyChanged("SelectedCategory");
            }
        }

        public ObservableCollection<Categories> CategoriesModels
        {
            get { return _categoriesModels; }
            set { _categoriesModels = value; }
        }

        public CategoryItemModel SelectedCategoryItemModel
        {
            get { return _selectedCategoryItemModel; }
            set
            {
                _selectedCategoryItemModel = value;
                OnPropertyChanged("SelectedCategoryItemModel");
                OnPropertyChanged("OrderType");
                _orderPlacedSuccessfully = "Collapsed";
                OnPropertyChanged("OrderPlacedSuccessfully");
                if (_selectedCategoryItemModel != null)
                {
                    if (_selectedCategoryItemModel.Category == "Others" || SelectedCategoryItemModel.Category == "Entertainment")
                    {
                        ServiceNotAvailable = "Visible"; 
                    }
                    else
                    {
                        if (SelectedCategoryItemModel.Category != "Hotels and Hostals")
                        {
                            ArrivalTimeVisibility = "Visible";  
                        }
                        else
                        {
                            ArrivalTimeVisibility = "Visible";
                            ToFromVisibility = "Visible";
                        }
                    }
                }
                else
                {
                    ArrivalTimeVisibility = "Collapsed";
                    ServiceNotAvailable = "Collapsed";
                    ToFromVisibility = "Collapsed";
                    OrderPlacedSuccessfully = "Collapsed";
                }
            }
        }

        public ObservableCollection<CategoryItemModel> CategoryItems
        {
            get { return _categoryItems; }
            set { _categoryItems = value; }
        }

        public ObservableCollection<OrderModel> OrderModels
        {
            get { return _orderModels; }
            set { _orderModels = value; }
        }

        public OrderModel NewOrderModel
        {
            get { return _newOrderModel; }
            set { _newOrderModel = value; }
        }

        public ICommand AddNewOrderModel
        {
            get { return _addNewOrderModel; }
            set { _addNewOrderModel = value; }
        }

        #region PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        internal TouristAppV3.Model.Categories Categories
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        internal TouristAppV3.Model.CategoryItemModel CategoryItemModel
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        internal TouristAppV3.Model.OrderModel OrderModel
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }
    }
}
