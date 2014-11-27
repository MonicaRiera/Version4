using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Windows.Storage;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using TouristAppV3.Model;
using TouristAppV3.ViewModel;

namespace UnitTestTouristAppV4
{
    [TestClass]
    public class UnitTest1
    {
        OrderViewModel orderViewModel = new OrderViewModel();
        XDocument xdoxNew;
        XDocument xdocOld;
        
        [TestMethod]
        public void TestSelectedCategoryProperty()
        {
            MainViewModel mainViewModel = new MainViewModel();
            mainViewModel.SelectedCategory = new Categories();
            Assert.IsFalse(mainViewModel.SelectedCategory == null);
        }

        [TestMethod]
        public void TestToStringOverride()
        {
            MainViewModel mainViewModel = new MainViewModel();
            mainViewModel.ToString();
            Assert.IsTrue(mainViewModel.ToString() == "Testing to string");
        }

        [TestMethod]
        public void TestOrderAddedToXML()
        {
            orderViewModel.NewOrderModel = new OrderModel();
            orderViewModel.NewOrderModel.FirstName = "The dude";
            orderViewModel.NewOrderModel.LastName = "Dudeson";
            AddNewOrderModelCommand();
            
            Assert.AreSame(xdocOld, xdoxNew);
        }

        private async void AddNewOrderModelCommand()
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
                xdocOld = orderDocument;
                LoadStream.Dispose();

                XElement orderList = orderDocument.Element("orders");

                XElement order = new XElement("order");
                order.Add(new XElement("firstname", orderViewModel.NewOrderModel.FirstName));
                order.Add(new XElement("lastname", orderViewModel.NewOrderModel.LastName));
                
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
                xdoxNew = orderDocument;
                saveStream.Dispose();
            }
        }
    }

