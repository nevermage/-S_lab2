using System;
using System.IO;
using System.Xml.Serialization;
using System.Threading;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace С_sharp_lab2
{
    public partial class Form1 : Form
    {
        int c = 0;
        Storage storage = new Storage(10000);
        public List<string> NM = new List<string>();
        public List<string> data = new List<string>();
        public bool pereuch = false;
        public bool isRefilling = true;
        public bool isWorking = true;
        string fileName = "xml.xml";
        Thread thread;
        Thread threadPereuch;
        public Form1()
        {
            InitializeComponent();
            storage.storage.Add(new Product("Snikers", 15, 5, 15));
            storage.storage.Add(new Product("Bubble gum", 10, 15, 45));
            storage.storage.Add(new Product("Crisps", 20, 12, 36));
            storage.storage.Add(new Product("Sandwitch", 15, 10, 30));
            storage.storage.Add(new Product("Juice", 30, 25, 75));
            storage.storage.Add(new Product("Lemonade", 40, 30, 90));
            storage.storage.Add(new Product("Cookies", 14, 15, 45));

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            foreach(var p in storage.storage)
            {
                NM.Add(p.NameProduct);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Random rand = new Random();
            CheckForIllegalCrossThreadCalls = false;


            threadPereuch = new Thread(() => {
                isRefilling = true;

                while (isRefilling) {
                    if (pereuch == true) {
                        data.Add("Restore: ");

                        foreach (var p in storage.storage) {
                            var b = 5;
                            p.QuantityInStock += b;

                            storage.Cashbox -= p.Price *b;

                            data.Add($"Refilled {p.NameProduct} x{b}, bank: {storage.Cashbox}");
                            textBox1.AppendText($"Refilled {p.NameProduct} x{b}, bank: {storage.Cashbox}" + Environment.NewLine);
                            var xml = new XmlSerializer(data.GetType());
                            using (var file = new FileStream(fileName, FileMode.Open))
                            {
                                xml.Serialize(file, data);
                            }

                            Thread.Sleep(1000);
                        }
                        pereuch = false;
                    }
                }
            });

            thread = new Thread(() => {
               c = 1;
               isWorking = true;
               
               while (isWorking) {
                   if (pereuch == false) {
                       var pr = NM[rand.Next(0, NM.Count)];
                       int count = rand.Next(1, 8);

                       foreach (var p in storage.storage) {
                           if (p.NameProduct == pr) {
                               if (p.QuantityInStock > count) {
                                   storage.Cashbox += p.PriceForPeople * count;
                                   p.QuantityInStock -= count;
                                   
                                   data.Add($"Sold - {pr} x{count}, bank: {storage.Cashbox}");
                                   textBox1.AppendText($"Sold - {pr} x{count}, bank: {storage.Cashbox}"+ Environment.NewLine);
                                   
                                   var xml = new XmlSerializer(data.GetType());
                                   using (var file = new FileStream(fileName,FileMode.OpenOrCreate) )
                                   {
                                       xml.Serialize(file, data);
                                   }

                                   Thread.Sleep(1000);
                               } else {
                                   pereuch = true;
                               }
                           }
                       }
                   }

               }
               
            });

            if (c == 0) {
                thread.Start();
                threadPereuch.Start();
            } else {
                MessageBox.Show("Store is already open");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            c = 0;
            isWorking = false;
            isRefilling = false;
        }
    }
}
