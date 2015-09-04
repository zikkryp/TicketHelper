using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Storage;
using Windows.UI.Popups;

namespace TicketHelper.Models
{
    public class HelperClient
    {
        private CookieContainer cookies;
        private Storage storage;

        public HelperClient()
        {
            storage = new Storage();
        }

        public async Task<AuthenticationResult> AuthenticateAsync(string username, string password)
        {
            HttpWebResponse response = null;
            StreamReader reader = null;
            Stream stream = null;
            string postString = string.Format("auth_user={0}&auth_pass={1}", username, password);
            byte[] buffer = Encoding.UTF8.GetBytes(postString);

            try
            {
                this.cookies = new CookieContainer();
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("http://crm.kromtech.net/");
                request.CookieContainer = cookies;
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                stream = await request.GetRequestStreamAsync();
                await stream.WriteAsync(buffer, 0, buffer.Length);
                response = (HttpWebResponse)await request.GetResponseAsync();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    reader = new StreamReader(response.GetResponseStream());
                    string content = reader.ReadToEnd();

                    if (content.Contains("Personal Page"))
                    {
                        User user = new User();
                        string filter = "Your asterisk:";
                        int index = content.IndexOf("username:");
                        string idString = content.Substring(content.IndexOf(filter) + filter.Length, 100);
                        user.Id = Convert.ToInt32(idString.Substring(idString.IndexOf("<td>"), 10).Split('>')[1].Split('<')[0]);
                        user.Username = content.Substring(index, 30).Split('"')[1];

                        storage.SaveCookies(cookies);

                        return new AuthenticationResult(user, AuthenticationStatus.Success);
                    }
                }

                return new AuthenticationResult(null, AuthenticationStatus.Failed);
            }
            catch
            {
                return new AuthenticationResult(null, AuthenticationStatus.ConnectionError);
            }
            finally
            {
                if (response != null)
                {
                    response.Dispose();
                    response = null;
                }

                if (stream != null)
                {
                    stream.Dispose();
                    stream = null;
                }

                if (reader != null)
                {
                    reader.Dispose();
                    reader = null;
                }
            }
        }

        //bool fail = false;

        public async Task SendTicket(Ticket ticket)
        {
            ticket.IsProcessing = true;
            ticket.SuccessVisibility = Windows.UI.Xaml.Visibility.Collapsed;
            ticket.ErrorVisibility = Windows.UI.Xaml.Visibility.Collapsed;

            HttpWebResponse response = null;
            StreamReader reader = null;
            Stream stream = null;

            try
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("http://crm.kromtech.net/?APP=003&MOD=tickets&action=respond_form");
                this.cookies = await storage.ReadCookies();
                request.CookieContainer = this.cookies;
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                stream = await request.GetRequestStreamAsync();
                string postString = string.Format("message_srv=kromtech.com&message_from=00077&message_to={0}&message_subject={1}&cval_value_Product={2}&cval_value_Language=English&cval_value_Issue=Technical&cval_value_Technical Problem_7977=Other technical&message={3}&send=1", ticket.Address, ticket.Subject, ticket.Product, ticket.Content);
                byte[] buffer = Encoding.UTF8.GetBytes(postString);
                await stream.WriteAsync(buffer, 0, buffer.Length);
                //string content = string.Format("{0}\n{1}\n{2}", ticket.Product, ticket.Subject, ticket.TypeId);
                //await new MessageDialog(content).ShowAsync();

                //await Task.Delay(3000);

                //if (fail)
                //{
                //    fail = false;
                //    throw new Exception("Error");
                //}

                //ticket.Id = ticket.Address.GetHashCode() > 0 ? ticket.Address.GetHashCode() : ticket.Address.GetHashCode() * -1;

                //fail = true;

                //ticket was sent
                response = (HttpWebResponse)await request.GetResponseAsync();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    reader = new StreamReader(response.GetResponseStream());
                    string content = await reader.ReadToEndAsync();
                    ticket.Id = Convert.ToInt32(content.Substring(content.IndexOf("ticket_id="), 30).Split('=')[1].Split('\'')[0]);
                    ticket.IsSent = true;
                    ticket.SuccessVisibility = Windows.UI.Xaml.Visibility.Visible;
                }

                ticket.ErrorVisibility = Windows.UI.Xaml.Visibility.Collapsed;
                ticket.IsSent = true;
            }
            catch (Exception)
            {
                ticket.IsSent = false;
            }
            finally
            {
                ticket.IsProcessing = false;

                if (response != null)
                {
                    response.Dispose();
                    response = null;
                }

                if (reader != null)
                {
                    reader.Dispose();
                    reader = null;
                }

                if (stream != null)
                {
                    stream.Dispose();
                    stream = null;
                }
            }
        }

        public async Task<IEnumerable<Report>> GetReports()
        {
            StorageFile file = await ApplicationData.Current.LocalFolder.CreateFileAsync("Reports.json", CreationCollisionOption.OpenIfExists);

            string jsonString = string.Empty;

            jsonString = await FileIO.ReadTextAsync(file, Windows.Storage.Streams.UnicodeEncoding.Utf8);
            
            if (jsonString.Equals(string.Empty) && jsonString == null)
            {
                HttpClient client = null;

                try
                {
                    client = new HttpClient();
                    jsonString = await client.GetStringAsync("https://dl.dropboxusercontent.com/u/64185161/Reports.json");
                    await FileIO.WriteTextAsync(file, jsonString);
                }
                catch (Exception e)
                {
                    await new MessageDialog(e.Message, "GetReports Error").ShowAsync();
                }
                finally
                {
                    if (client != null)
                    {
                        client.Dispose();
                    }
                }
            }

            List<Report> reports = new List<Report>();

            JsonObject jsonObject = JsonObject.Parse(jsonString);
            JsonArray jsonArray = jsonObject["items"].GetArray();

            foreach(JsonValue jsonValue in jsonArray)
            {
                JsonObject itemObject = jsonValue.GetObject();
                string content = string.Empty;
                string type = itemObject["type"].GetString();
                JsonArray lines = itemObject["lines"].GetArray();
                foreach(JsonValue line in lines)
                {
                    content += line.GetString() + "\n";
                }

                reports.Add(new Report(type, content));
            }

            return reports;
        }
    }
}
