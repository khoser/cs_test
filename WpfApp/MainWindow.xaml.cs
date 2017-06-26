using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
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
using System.Xml.Linq;
using WpfApp.Models;
using Newtonsoft.Json;

namespace WpfApp
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static String URL = "http://localhost/";
        List<ToDoItem> tdis = null;
        static HttpClient client = new HttpClient();
        private bool isclientfilled = false;

        private void CreateClient()
        {
            if (!isclientfilled)
            {
                client.BaseAddress = new Uri(URL);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                isclientfilled = true;
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            CreateClient();
            RunAsyncGet();
        }

        private void SetVisibility()
        {
            textBox.Text = "";
            List<UIElement> lu = new List<UIElement>();

            foreach (UIElement uiel in layoutGrid.Children)
            {
                if (uiel is CheckBox)
                {
                    if (((CheckBox)uiel).Tag is ToDoItem) { lu.Add(uiel); }
                }
                else if (uiel is TextBlock)
                {
                    if (((TextBlock)uiel).Tag is ToDoItem) { lu.Add(uiel); }
                    if (((TextBlock)uiel).Tag is null) { lu.Add(uiel); }
                }
            }
            foreach (UIElement l in lu)
            {
                layoutGrid.Children.Remove(l);
            }

            int i = 0;
            for (i = layoutGrid.RowDefinitions.Count; i > 1 ; i--)
            {
                layoutGrid.RowDefinitions.RemoveAt(i-1);
            }

            Style myCheckboxStyle = this.FindResource("myCheckboxStyle") as Style;

            i = 1;
            foreach (ToDoItem tdi in tdis)
            {
                RowDefinition rdfn = new RowDefinition();
                rdfn.Height = FirstRow.Height;
                layoutGrid.RowDefinitions.Add(rdfn);

                CheckBox chbx = new CheckBox();
                chbx.IsChecked = tdi.IsComplete;
                chbx.Tag = tdi;
                chbx.VerticalAlignment = VerticalAlignment.Center;
                chbx.HorizontalAlignment = HorizontalAlignment.Center;
                chbx.Checked += checkBox_Checked;
                chbx.Unchecked += checkBox_Checked;
                chbx.Style = myCheckboxStyle;
                layoutGrid.Children.Add(chbx);
                Grid.SetRow(chbx, i);
                Grid.SetColumn(chbx, 0);

                TextBlock txblk = new TextBlock();
                txblk.Text = tdi.Name;
                txblk.Tag = tdi;
                txblk.VerticalAlignment = VerticalAlignment.Center;
                if (tdi.IsComplete)
                {
                    txblk.TextDecorations = TextDecorations.Strikethrough;
                }
                txblk.MouseRightButtonDown += textBox_MouseDoubleClick;
                layoutGrid.Children.Add(txblk);
                Grid.SetRow(txblk, i);
                Grid.SetColumn(txblk, 1);

                i++;
            }

            RowDefinition rdfn1 = new RowDefinition();
            rdfn1.Height = FirstRow.Height;
            layoutGrid.RowDefinitions.Add(rdfn1);

            TextBlock txblki = new TextBlock();
            txblki.Text = "total count: " + (--i).ToString();
            txblki.VerticalAlignment = VerticalAlignment.Center;
            layoutGrid.Children.Add(txblki);
            Grid.SetRow(txblki, ++i);
            Grid.SetColumn(txblki, 1);
        }
        
        async Task RunAsyncGet()
        {
            try
            {
                var url = await GetProductsAsync(URL);
                tdis = url;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            SetVisibility();
        }

        async Task RunAsyncPost(ToDoItem product)
        {
            try
            {
                // Create a new product
                await CreateProductAsync(product);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            RunAsyncGet();
        }

        async Task RunAsyncDel(string uuid)
        {
            try
            {
                await DeleteProductAsync(uuid);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            RunAsyncGet();
        }

        async Task RunAsyncUpdate(ToDoItem product)
        {
            try
            {
                await UpdateProductAsync(product);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            RunAsyncGet();
        }

        static async Task<ToDoItem> GetProductAsync(string path)
        {
            ToDoItem product = null;
            HttpResponseMessage response = await client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                product = await response.Content.ReadAsAsync<ToDoItem>();
            }
            return product;
        }

        static async Task<List<ToDoItem>> GetProductsAsync(string path)
        {
            List<ToDoItem> product = null;
            HttpResponseMessage response = await client.GetAsync("api/todo").ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                product = await response.Content.ReadAsAsync<List<ToDoItem>>();
            }

            return product;
        }

        static async Task<Uri> CreateProductAsync(ToDoItem product)
        {
            HttpResponseMessage response = await client.PostAsJsonAsync("api/todo", product).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            // Return the URI of the created resource.
            return response.Headers.Location;
        }

        static async Task<ToDoItem> UpdateProductAsync(ToDoItem product)
        {
            HttpResponseMessage response = await client.PutAsJsonAsync("api/todo/"+product.Key, product).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            // Deserialize the updated product from the response body.
            product = await response.Content.ReadAsAsync<ToDoItem>();
            return product;
        }

        static async Task<HttpStatusCode> DeleteProductAsync(string key)
        {
            HttpResponseMessage response = await client.DeleteAsync("api/todo/"+key).ConfigureAwait(false);
            return response.StatusCode;
        }

        private void checkBox_Checked(object sender, RoutedEventArgs e)
        {
            ToDoItem tdi = (ToDoItem)((CheckBox)e.OriginalSource).Tag;
            tdi.IsComplete = (bool)((CheckBox)e.OriginalSource).IsChecked;
            RunAsyncUpdate(tdi);
            
        }

        private void textBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ToDoItem tdi = (ToDoItem)((TextBlock)e.OriginalSource).Tag;
            RunAsyncDel(tdi.Key);
            
        }

        private void textBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (textBox.Text != "")
                {
                    ToDoItem newToDoitem = new ToDoItem { Name = textBox.Text, IsComplete = (bool)checkBox.IsChecked };
                    RunAsyncPost(newToDoitem);
                }
            }
        }
    }
}
