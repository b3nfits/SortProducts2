using Avt.Database;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using System.Windows.Shapes;

namespace Avt.View
{
    public class SortItem
    {
        public string Text { get; set; }
        public SortDescription Description { get; set; }
    }
    
    public partial class ProductView : Window
    {

        private readonly Database.TradeEntities entities;

        private readonly Database.Manufacturer allManufacturer;
    
        public Database.Manufacturer SelectedManufacturer { get; set; } 
        public SortItem SelectedSort { get; set; }
        public ObservableCollection<Database.Product> Products { get; set; }
        public ObservableCollection<Database.Manufacturer> Manufacturers { get; set; }
        public ObservableCollection<SortItem> SotrItems { get; set; }


        public ProductView(Database.TradeEntities entities1, Database.User user)
        {
            InitializeComponent();
            entities = entities1;
            this.allManufacturer = new Database.Manufacturer() { ID = 0, Name = "Все производители" };
            Products = new ObservableCollection<Database.Product>(entities.Products);
            Manufacturers = new ObservableCollection<Database.Manufacturer>(entities.Manufacturers);
            Manufacturers.Insert(0, allManufacturer);

            SotrItems = new ObservableCollection<SortItem>()
                {
                    new SortItem()
                    {
                        Text = "Сортировать по возрастанию цены",
                        Description = new SortDescription(){
                        PropertyName = "ProductCost",
                        Direction = ListSortDirection.Ascending }

                },
                    new SortItem()
                    {
                        Text = "Сортировать по убыванию цены",
                        Description = new SortDescription()  { 
                        PropertyName = "ProductCost", 
                        Direction = ListSortDirection.Descending
                    } }
            };

                

            DataContext = this; 
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (DataContext != null)
            {
                Owner.Show();
            }
        }

        

        private void ApplyFilter()
        {
            var searchString = tbSearch.Text.Trim().ToLower();
            

            var view = CollectionViewSource.GetDefaultView(lvProducts.ItemsSource);
            if (view == null) return;

            view.Filter = (object o) =>
            {
                var product = o as Database.Product;   
                if(product == null) return false;

                if (searchString.Length > 0)
                {
                    if (!(product.ProductDescription.ToLower().Contains(searchString) ||
                     product.ProductCategory1.Name.ToLower().Contains(searchString) ||
                     product.ProductName.ToLower().Contains(searchString) ||
                     product.ProductDescription.ToLower().Contains(searchString) ||
                     product.Provider.Name.ToLower().Contains(searchString)))
                    { 
                        return false; 
                    }
                       
                }

                if(SelectedManufacturer != null && SelectedManufacturer != allManufacturer)
                {
                    if(product.Manufacturer != SelectedManufacturer)
                    {
                        return false;
                    }
                }
                return true;
                
            };
        }

        private void ApplySotr()
        {
            var view = CollectionViewSource.GetDefaultView(lvProducts.ItemsSource);
            if (view == null) return;

            view.SortDescriptions.Clear();

            if(SelectedSort != null)
            {
                 view.SortDescriptions.Add(SelectedSort.Description);

            }

            
        }
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilter();
        }
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilter();
        }

        private void ComboBox_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            ApplySotr();
        }

        private void lvProducts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
