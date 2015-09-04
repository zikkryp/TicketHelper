using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Storage;
using Windows.UI.Popups;

namespace TicketHelper.Models
{
    public interface ITicketRepository
    {
        DiagnosticReport DiagnosticReport { get; set; }
        Reconnection Reconnection { get; set; }

        string GetContent(int productId, int languageId, int typeId);
    }

    public class TicketRepository : Notifier, ITicketRepository
    {
        private DiagnosticReport diagnosticReport;
        private Reconnection reconnection;

        public DiagnosticReport DiagnosticReport
        {
            get
            {
                return diagnosticReport;
            }

            set
            {
                diagnosticReport = value;
                NotifyPropertyChanged("DiagnosticReport");
            }
        }

        public Reconnection Reconnection
        {
            get
            {
                return reconnection;
            }

            set
            {
                reconnection = value;
                NotifyPropertyChanged("Reconnection");
            }
        }

        public string GetContent(int productId, int languageId, int typeId)
        {
            if (typeId == 1)
            {
                return Reconnection.FirstOrDefault(e => e.Id == languageId).Content;
            }

            return DiagnosticReport.FirstOrDefault(e => e.Id == productId).FirstOrDefault(e => e.Id == languageId).Content;
        }
    }

    public class Reconnection : IList<Language>
    {
        public Reconnection()
        {
            Items = new List<Language>();
        }

        public List<Language> Items { get; set; }

        #region IList Implementation

        public int Count
        {
            get
            {
                return ((IList<Language>)Items).Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return ((IList<Language>)Items).IsReadOnly;
            }
        }

        public Language this[int index]
        {
            get
            {
                return ((IList<Language>)Items)[index];
            }

            set
            {
                ((IList<Language>)Items)[index] = value;
            }
        }

        public int IndexOf(Language item)
        {
            return ((IList<Language>)Items).IndexOf(item);
        }

        public void Insert(int index, Language item)
        {
            ((IList<Language>)Items).Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            ((IList<Language>)Items).RemoveAt(index);
        }

        public void Add(Language item)
        {
            ((IList<Language>)Items).Add(item);
        }

        public void Clear()
        {
            ((IList<Language>)Items).Clear();
        }

        public bool Contains(Language item)
        {
            return ((IList<Language>)Items).Contains(item);
        }

        public void CopyTo(Language[] array, int arrayIndex)
        {
            ((IList<Language>)Items).CopyTo(array, arrayIndex);
        }

        public bool Remove(Language item)
        {
            return ((IList<Language>)Items).Remove(item);
        }

        public IEnumerator<Language> GetEnumerator()
        {
            return ((IList<Language>)Items).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IList<Language>)Items).GetEnumerator();
        }

        #endregion
    }

    public class DiagnosticReport : Notifier, IList<Product>
    {
        private List<Product> items;

        public DiagnosticReport()
        {
            Items = new List<Product>();
        }

        public List<Product> Items
        {
            get
            {
                return items;
            }
            set
            {
                items = value;
                NotifyPropertyChanged("Items");
            }
        }

        #region IList Implemantation

        public Product this[int index]
        {
            get
            {
                return ((IList<Product>)Items)[index];
            }

            set
            {
                ((IList<Product>)Items)[index] = value;
            }
        }

        public int Count
        {
            get
            {
                return ((IList<Product>)Items).Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return ((IList<Product>)Items).IsReadOnly;
            }
        }

        public void Add(Product item)
        {
            ((IList<Product>)Items).Add(item);
        }

        public void Clear()
        {
            ((IList<Product>)Items).Clear();
        }

        public bool Contains(Product item)
        {
            return ((IList<Product>)Items).Contains(item);
        }

        public void CopyTo(Product[] array, int arrayIndex)
        {
            ((IList<Product>)Items).CopyTo(array, arrayIndex);
        }

        public IEnumerator<Product> GetEnumerator()
        {
            return ((IList<Product>)Items).GetEnumerator();
        }

        public int IndexOf(Product item)
        {
            return ((IList<Product>)Items).IndexOf(item);
        }

        public void Insert(int index, Product item)
        {
            ((IList<Product>)Items).Insert(index, item);
        }

        public bool Remove(Product item)
        {
            return ((IList<Product>)Items).Remove(item);
        }

        public void RemoveAt(int index)
        {
            ((IList<Product>)Items).RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IList<Product>)Items).GetEnumerator();
        }

        #endregion
    }

    public class Product : Notifier, IList<Language>
    {
        private List<Language> items;
        public Product()
        {
            Items = new List<Language>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public List<Language> Items
        {
            get
            {
                return items;
            }
            set
            {
                items = value;
                NotifyPropertyChanged("Items");
            }
        }

        #region IList Implementation

        public int Count
        {
            get
            {
                return ((IList<Language>)Items).Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return ((IList<Language>)Items).IsReadOnly;
            }
        }

        public Language this[int index]
        {
            get
            {
                return ((IList<Language>)Items)[index];
            }

            set
            {
                ((IList<Language>)Items)[index] = value;
            }
        }

        public int IndexOf(Language item)
        {
            return ((IList<Language>)Items).IndexOf(item);
        }

        public void Insert(int index, Language item)
        {
            ((IList<Language>)Items).Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            ((IList<Language>)Items).RemoveAt(index);
        }

        public void Add(Language item)
        {
            ((IList<Language>)Items).Add(item);
        }

        public void Clear()
        {
            ((IList<Language>)Items).Clear();
        }

        public bool Contains(Language item)
        {
            return ((IList<Language>)Items).Contains(item);
        }

        public void CopyTo(Language[] array, int arrayIndex)
        {
            ((IList<Language>)Items).CopyTo(array, arrayIndex);
        }

        public bool Remove(Language item)
        {
            return ((IList<Language>)Items).Remove(item);
        }

        public IEnumerator<Language> GetEnumerator()
        {
            return ((IList<Language>)Items).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IList<Language>)Items).GetEnumerator();
        }

        #endregion

        public override string ToString()
        {
            return Name;
        }
    }

    public class Language
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }

    public class TicketSource
    {
        private TicketRepository repository;
        private static TicketSource ticketSource;

        private TicketSource()
        {
            this.repository = new TicketRepository();
        }

        public static async Task<TicketRepository> GetTicketAsync()
        {
            ticketSource = new TicketSource();

            await ticketSource.GetAsync();

            return ticketSource.repository;
        }

        private async Task GetAsync()
        {
            StorageFile file = await ApplicationData.Current.LocalFolder.CreateFileAsync("Reports.json", CreationCollisionOption.OpenIfExists);

            string jsonString = await FileIO.ReadTextAsync(file, Windows.Storage.Streams.UnicodeEncoding.Utf8);

            if (jsonString == string.Empty)
            {
                HttpClient client = null;

                try
                {
                    client = new HttpClient();
                    jsonString = await client.GetStringAsync("https://dl.dropboxusercontent.com/u/64185161/Reports.json");

                    if (jsonString != null && jsonString != string.Empty)
                    {
                        await FileIO.WriteTextAsync(file, jsonString);
                    }
                }
                catch (Exception e)
                {
                    await new MessageDialog(e.Message, "Get Reports Error").ShowAsync();
                }
                finally
                {
                    if (client != null)
                    {
                        client.Dispose();
                    }
                }
            }

            if (jsonString.Equals(string.Empty) && jsonString == null)
            {
                return;
            }

            DiagnosticReport report = new DiagnosticReport();
            Reconnection reconnection = new Reconnection();

            JsonObject jsonObject = JsonObject.Parse(jsonString);
            JsonObject ticketObject = jsonObject["ticket"].GetObject();

            JsonArray reportArray = ticketObject["reports"].GetArray();
            GetReports(report, reportArray);

            JsonObject reconnectionObject = ticketObject["reconnection"].GetObject();
            GetReconnection(reconnection, reconnectionObject);

            this.repository.Reconnection = reconnection;
            this.repository.DiagnosticReport = report;
        }

        private void GetReports(DiagnosticReport report, JsonArray jsonArray)
        {
            foreach (JsonValue itemValue in jsonArray)
            {
                Product product = new Product();

                JsonObject itemObject = itemValue.GetObject();
                product.Id = (int)itemObject["id"].GetNumber();
                product.Name = itemObject["product"].GetString();


                JsonArray itemArray = itemObject["languages"].GetArray();

                foreach (JsonValue value in itemArray)
                {
                    Language language = new Language();
                    JsonObject valueObject = value.GetObject();
                    language.Id = (int)valueObject["id"].GetNumber();
                    language.Name = valueObject["language"].GetString();
                    language.Content = valueObject["content"].GetString();
                    product.Items.Add(language);
                }

                report.Items.Add(product);
            }
        }

        private void GetReconnection(Reconnection reconnection, JsonObject jsonObject)
        {
            JsonArray jsonArray = jsonObject["languages"].GetArray();

            foreach (JsonValue itemValue in jsonArray)
            {
                Language language = new Language();
                JsonObject itemObject = itemValue.GetObject();
                language.Id = (int)itemObject["id"].GetNumber();
                language.Name = itemObject["language"].GetString();
                language.Content = itemObject["content"].GetString();

                reconnection.Items.Add(language);
            }
        }
    }
}
