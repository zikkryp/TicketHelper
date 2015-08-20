using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Security.Credentials;
using Windows.Storage;
using Windows.UI.Popups;

namespace TicketHelper.Models
{
    public class Storage
    {
        #region Password Vault

        private PasswordVault vault;

        public Storage()
        {
            vault = new PasswordVault();
        }

        public void SaveCredentials(string username, string password)
        {
            UpdateCredentials(username, password);

            PasswordCredential credential = new PasswordCredential("Application", username, password);
            vault.Add(credential);
        }

        public PasswordCredential GetCredentials()
        {
            try
            {
                PasswordCredential credentials = vault.FindAllByResource("Application").FirstOrDefault();

                return credentials;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void RemoveCredentials()
        {
            try
            {
                PasswordCredential credentials = vault.FindAllByResource("Application").FirstOrDefault();

                vault.Remove(credentials);
            }
            catch (Exception)
            {

            }
        }

        private void UpdateCredentials(string username, string password)
        {
            PasswordCredential credentials = GetCredentials();

            if (credentials != null)
            {
                credentials.RetrievePassword();

                if (!username.Equals(credentials.UserName))
                {
                    RemoveCredentials();
                }
            }
        }

        #endregion

        #region CookieSerializer

        private void Serialize(CookieCollection cookies, Stream stream)
        {
            DataContractSerializer formatter = new DataContractSerializer(typeof(List<Cookie>));

            List<Cookie> cookieList = new List<Cookie>();

            for (var enumerator = cookies.GetEnumerator(); enumerator.MoveNext();)
            {
                var cookie = enumerator.Current as Cookie;

                if (cookie == null)
                {
                    continue;
                }

                cookieList.Add(cookie);
            }

            formatter.WriteObject(stream, cookieList);
        }

        private CookieContainer Deserialize(Stream stream, Uri uri)
        {
            List<Cookie> cookies = new List<Cookie>();
            CookieContainer container = new CookieContainer();
            DataContractSerializer formatter = new DataContractSerializer(typeof(List<Cookie>));
            cookies = (List<Cookie>)formatter.ReadObject(stream);
            CookieCollection cookieco = new CookieCollection();

            foreach (Cookie cookie in cookies)
            {
                cookieco.Add(cookie);
            }

            container.Add(uri, cookieco);

            return container;
        }

        public async void SaveCookies(CookieContainer cookie)
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            StorageFile sampleFile = await localFolder.CreateFileAsync("Cookies.xml", CreationCollisionOption.ReplaceExisting);
            StorageStreamTransaction transaction = null;

            try
            {
                transaction = await sampleFile.OpenTransactedWriteAsync();
                Serialize(cookie.GetCookies(new Uri("http://crm.kromtech.net/")), transaction.Stream.AsStream());
                await transaction.CommitAsync();
            }
            finally
            {
                if (transaction != null)
                {
                    transaction.Dispose();
                    transaction = null;
                }
            }
        }

        public async Task<CookieContainer> ReadCookies()
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            StorageFile sampleFile = null;
            Stream stream = null;

            try
            {
                sampleFile = await localFolder.GetFileAsync("Cookies.xml");
                stream = await sampleFile.OpenStreamForReadAsync();

                return Deserialize(stream, new Uri("http://crm.kromtech.net/"));
            }
            catch
            {
                return null;
            }
            finally
            {
                if (stream != null)
                {
                    stream.Dispose();
                    stream = null;
                }
            }
        }

        #endregion

        #region Serialize User && History

        public async Task SaveHistory(History history)
        {
            StorageFile file = await ApplicationData.Current.LocalFolder.CreateFileAsync("History.json", CreationCollisionOption.OpenIfExists);

            string contents = await SerializeHistory(history);

            await FileIO.WriteTextAsync(file, contents);
        }

        public async Task<History> GetHistory()
        {
            try
            {
                StorageFile file = await ApplicationData.Current.LocalFolder.CreateFileAsync("History.json", CreationCollisionOption.OpenIfExists);
                string contents = await FileIO.ReadTextAsync(file, Windows.Storage.Streams.UnicodeEncoding.Utf8);

                if (contents != null && contents != string.Empty)
                {
                    return Deserialize<History>(contents);
                }

            }
            catch(Exception e)
            {
                await new MessageDialog(e.Message, "Get History").ShowAsync();
            }

            return new History();
        }

        private async Task<string> SerializeHistory(History history)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                var serializer = new DataContractJsonSerializer(typeof(History));
                serializer.WriteObject(stream, history);
                stream.Position = 0;

                using (StreamReader reader = new StreamReader(stream))
                {
                    return await reader.ReadToEndAsync();
                }
            }
        }

        public async Task SaveUser(User user)
        {
            StorageFile file = await ApplicationData.Current.LocalFolder.CreateFileAsync("User.json", CreationCollisionOption.OpenIfExists);
            string contents = await Serialize<User>(user);
            await FileIO.WriteTextAsync(file, contents);
        }

        public async Task<User> GetUser()
        {
            try
            {
                StorageFile file = await ApplicationData.Current.LocalFolder.CreateFileAsync("User.json", CreationCollisionOption.OpenIfExists);

                string contents = await FileIO.ReadTextAsync(file, Windows.Storage.Streams.UnicodeEncoding.Utf8);

                if (contents!= null && contents != string.Empty)
                {
                    return Deserialize<User>(contents);
                }
            }
            catch(Exception e)
            {
                await new MessageDialog(e.Message, "Get User").ShowAsync();
            }

            return null;
        }

        private async Task<string> Serialize<T>(T item)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                var serializer = new DataContractJsonSerializer(typeof(T));
                serializer.WriteObject(stream, item);
                stream.Position = 0;

                using (StreamReader reader = new StreamReader(stream))
                {
                    return await reader.ReadToEndAsync();
                }
            }
        }

        private User DeserializeUser(string jsonString)
        {
            var bytes = Encoding.UTF8.GetBytes(jsonString);

            using (MemoryStream stream = new MemoryStream(bytes))
            {
                var serializer = new DataContractJsonSerializer(typeof(User));

                return (User)serializer.ReadObject(stream);
            }
        }

        private T Deserialize<T>(string jsonString)
        {
            var bytes = Encoding.UTF8.GetBytes(jsonString);

            using (MemoryStream stream = new MemoryStream(bytes))
            {
                var serializer = new DataContractJsonSerializer(typeof(T));

                return (T)serializer.ReadObject(stream);
            }
        }

        #endregion
    }
}
