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
        ToDoItem tdi = new ToDoItem { Key = "1", Name = "Job to do", IsComplete = false };
        static HttpClient client = new HttpClient();
        private bool isclientfilled = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            WebClient client = new WebClient();

            //Get request
            //                                                                              | GET DATA                   |
            string response = client.DownloadString("http://localhost/api/todo");

            //var o = new { name = 'w' };
            //o.name
        }
        private void BtnPost_Click(object sender, RoutedEventArgs e)
        {
            if (!isclientfilled)
            {
                client.BaseAddress = new Uri(URL);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                isclientfilled = true;
            }

            RunAsyncPost(tdi);

            var o = 1;
            
        }

        static async Task RunAsyncPost(ToDoItem product)
        {
            
            try
            {
                // Create a new product
                var url = await CreateProductAsync(product);
            }
            catch (Exception e)
            {
                //Console.WriteLine(e.Message);
            }
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

        static async Task<Uri> CreateProductAsync(ToDoItem product)
        {
            HttpResponseMessage response = await client.PostAsJsonAsync("api/todo", product).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            // Return the URI of the created resource.
            return response.Headers.Location;
        }

        static async Task<ToDoItem> UpdateProductAsync(ToDoItem product)
        {
            HttpResponseMessage response = await client.PutAsJsonAsync($"api/todo/{product.Key}", product);
            response.EnsureSuccessStatusCode();

            // Deserialize the updated product from the response body.
            product = await response.Content.ReadAsAsync<ToDoItem>();
            return product;
        }

        static async Task<HttpStatusCode> DeleteProductAsync(string key)
        {
            HttpResponseMessage response = await client.DeleteAsync($"api/products/{key}");
            return response.StatusCode;
        }

        //static async Task RunAsync(ToDoItem product)
        //{
        //    client.BaseAddress = new Uri(URL);
        //    client.DefaultRequestHeaders.Accept.Clear();
        //    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        //    try
        //    {
        //        // Create a new product
        //        var url = await CreateProductAsync(product);
                
        //        // Get the product
        //        product = await GetProductAsync(url.PathAndQuery);
        //        ShowProduct(product);

        //        // Update the product
        //        Console.WriteLine("Updating price...");
        //        product.Price = 80;
        //        await UpdateProductAsync(product);

        //        // Get the updated product
        //        product = await GetProductAsync(url.PathAndQuery);
        //        ShowProduct(product);

        //        // Delete the product
        //        var statusCode = await DeleteProductAsync(product.Key);
        //        Console.WriteLine($"Deleted (HTTP Status = {(int)statusCode})");

        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e.Message);
        //    }

        //}
    }
}
