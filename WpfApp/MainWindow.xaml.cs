﻿using System;
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
            int i = 0;
            //layoutGrid.Children.Clear();
            for (i = layoutGrid.RowDefinitions.Count; i > 1 ; i--)
            {
                layoutGrid.RowDefinitions.RemoveAt(i);
            }

            i = 1;
            foreach (ToDoItem tdi in tdis)
            {
                RowDefinition rdfn = new RowDefinition();
                rdfn.Height = FirstRow.Height;
                layoutGrid.RowDefinitions.Add(rdfn);

                CheckBox chbx = new CheckBox();
                chbx.IsChecked = tdi.IsComplete;
                chbx.Tag = tdi.Key;
                chbx.VerticalAlignment = VerticalAlignment.Center;
                chbx.HorizontalAlignment = HorizontalAlignment.Center;
                layoutGrid.Children.Add(chbx);
                Grid.SetRow(chbx, i);
                Grid.SetColumn(chbx, 0);

                TextBlock txblk = new TextBlock();
                txblk.Text = tdi.Name;
                txblk.Tag = tdi.Key;
                txblk.VerticalAlignment = VerticalAlignment.Center;
                layoutGrid.Children.Add(txblk);
                Grid.SetRow(txblk, i);
                Grid.SetColumn(txblk, 1);

                i++;
            }

            RowDefinition rdfn1 = new RowDefinition();
            rdfn1.Height = FirstRow.Height;
            layoutGrid.RowDefinitions.Add(rdfn1);

            TextBlock txblki = new TextBlock();
            txblki.Text = "total count: " + i.ToString();
            txblki.VerticalAlignment = VerticalAlignment.Center;
            layoutGrid.Children.Add(txblki);
            Grid.SetRow(txblki, ++i);
            Grid.SetColumn(txblki, 1);
        }
        

        //private void Button_Click(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        List<ToDoItem> ListOfTDI = await GetProductsAsync(URL);
        //    }
        //    catch (Exception e)
        //    {
        //        //Console.WriteLine(e.Message);
        //    }
        //}
        private void BtnPost_Click(object sender, RoutedEventArgs e)
        {
            CreateClient();
            if (textBox.Text != "")
            {
                ToDoItem newToDoitem = new ToDoItem { Name = textBox.Text, IsComplete = (bool)checkBox.IsChecked };
                RunAsyncPost(newToDoitem);
            }
        }

        async Task RunAsyncGet()
        {
            try
            {
                var url = await GetProductsAsync(URL);
                tdis = url;
                SetVisibility();
            }
            catch (Exception e)
            {
                //Console.WriteLine(e.Message);
            }
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
