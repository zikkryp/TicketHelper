using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using TicketHelper.Common;
using Windows.Security.Credentials;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace TicketHelper.Models
{
    public class MainViewModel : Notifier
    {
        private int id;
        private string username;
        private string password;

        private ICommand signOutCommand;
        private ICommand sendCommand;
        private ICommand sendFailedItemsCommand;
        private ICommand copyCommand;
        private ICommand saveCommand;
        private ICommand showAllCommand;
        private ICommand showFailedCommand;
        private ICommand showSentCommand;
        private ICommand cleanHistoryCommand;
        private User user;
        private TicketRepository repository;
        private History history;
        private IEnumerable<Language> languages;
        private string address;
        private string content;
        private string validationColor = "Black";
        private string validationMessage;
        private int selectedProduct = 0;
        private int selectedLanguage = 0;
        private int selectedType = 0;

        protected Storage storage;
        protected Page page;
        protected HelperClient client;

        public MainViewModel(Page page)
        {
            this.page = page;
            username = string.Empty;
            password = string.Empty;
            storage = new Storage();
            client = new HelperClient();
            history = new History();
            history.Items = new List<Ticket>();
            Repository = new TicketRepository();
            tryGetCredentials();
            getTicketRepository();
            getUser();
            getHistory();
        }

        #region Commands

        public ICommand SignOutCommand
        {
            get
            {
                signOutCommand = new RelayCommand(
                    () => this.trySignOut(),
                    () => this.canSignOut());

                return signOutCommand;
            }
        }

        public ICommand SendCommand
        {
            get
            {
                sendCommand = new RelayCommand(
                    () => this.trySend(),
                    () => this.canSend());

                return sendCommand;
            }
        }

        public ICommand SendFailedItemsCommand
        {
            get
            {
                sendFailedItemsCommand = new RelayCommand(
                    () => this.sendFailedItems(),
                    () => this.canSendFailedItems());

                return sendFailedItemsCommand;
            }
        }

        public ICommand CopyCommand
        {
            get
            {
                return copyCommand = new RelayCommand(
                    () => this.copyIds(),
                    () => this.canCopy());
            }
        }

        public ICommand ShowAllCommand
        {
            get
            {
                showAllCommand = new RelayCommand(
                    () => this.showAll(),
                    () => true);

                return showAllCommand;
            }
        }

        public ICommand ShowFailedCommand
        {
            get
            {
                showFailedCommand = new RelayCommand(
                    () => this.showFailed(),
                    () => true);

                return showFailedCommand;
            }
        }

        public ICommand ShowSentCommand
        {
            get
            {
                showSentCommand = new RelayCommand(
                    () => this.showSent(),
                    () => true);

                return showSentCommand;
            }
        }

        public ICommand CleanHistoryCommand
        {
            get
            {
                cleanHistoryCommand = new RelayCommand(
                    () => this.cleanHistory(),
                    () => true);

                return cleanHistoryCommand;
            }
        }

        public ICommand SaveCommand
        {
            get
            {
                saveCommand = new RelayCommand(
                    () => this.saveHistory(),
                    () => this.canSave());

                return saveCommand;
            }
        }

        #endregion

        #region Properties

        public TicketRepository Repository
        {
            get
            {
                return repository;
            }
            set
            {
                repository = value;
                NotifyPropertyChanged("Repository");
            }
        }

        public IEnumerable<Language> Languages
        {
            get
            {
                return languages;
            }

            set
            {
                languages = value;
                setContent();
                NotifyPropertyChanged("Languages");
            }
        }

        public int SelectedProduct
        {
            get
            {
                return selectedProduct;
            }

            set
            {
                selectedProduct = value;
                Languages = repository.DiagnosticReport.Items[selectedProduct].Items;
                SelectedLanguage = 0;
                setContent();
                NotifyPropertyChanged("SelectedProduct");
            }
        }

        public int SelectedType
        {
            get
            {
                return selectedType;
            }

            set
            {
                selectedType = value;

                if (Languages.Count() > 0)
                {
                    SelectedLanguage = 0;
                }

                if (selectedType == 1)
                {
                    Languages = repository.Reconnection.Items;
                }
                else
                {
                    Languages = repository.DiagnosticReport.Items[selectedProduct].Items;
                }
                SelectedLanguage = 0;
                setContent();
                NotifyPropertyChanged("SelectedType");
            }
        }

        public int SelectedLanguage
        {
            get
            {
                return selectedLanguage;
            }

            set
            {
                selectedLanguage = value;
                setContent();
                NotifyPropertyChanged("SelectedLanguage");
            }
        }

        public int Id
        {
            get
            {
                return id;
            }

            set
            {
                id = value;
                NotifyPropertyChanged("Id");
            }
        }

        public virtual string Username
        {
            get
            {
                return username;
            }

            set
            {
                username = value;
                NotifyPropertyChanged("Username");
            }
        }

        public virtual string Password
        {
            get
            {
                return password;
            }

            set
            {
                password = value;
                NotifyPropertyChanged("Password");
            }
        }

        public Array OperatingSystems
        {
            get { return Enum.GetValues(typeof(OperatingSystem)); }
        }

        public Array Types
        {
            get { return Enum.GetValues(typeof(TicketType)); }
        }

        public string Address
        {
            get
            {
                return address;
            }

            set
            {
                address = value;
                NotifyPropertyChanged("Address");
            }
        }

        public string Content
        {
            get
            {
                return content;
            }

            set
            {
                content = value;
                NotifyPropertyChanged("Content");
            }
        }

        public string Copyright
        {
            get { return string.Format("{0} © KrypApp", DateTime.Now.Year); }
        }

        public string ValidationColor
        {
            get
            {
                return validationColor;
            }
            set
            {
                validationColor = value;
                NotifyPropertyChanged("ValidationColor");
            }
        }

        public string ValidationMessage
        {
            get
            {
                return validationMessage;
            }

            set
            {
                validationMessage = value;
                NotifyPropertyChanged("ValidationMessage");
            }
        }

        public User User
        {
            get
            {
                return user;
            }

            set
            {
                user = value;
                NotifyPropertyChanged("User");
            }
        }

        public History History
        {
            get
            {
                return history;
            }

            set
            {
                history = value;
                NotifyPropertyChanged("History");
            }
        }

        #endregion

        #region Ticket & Content Initialization

        private async void getTicketRepository()
        {
            Repository = await TicketSource.GetTicketAsync();

            SelectedProduct = 0;
            SelectedLanguage = 0;
        }

        private void setContent()
        {
            Content = Repository.GetContent(selectedProduct, selectedLanguage, selectedType);

            if (selectedType == 1)
            {
                Content = content.Replace("sincerely,", string.Format("sincerely, {0}", user.Username));
            }
        }

        #endregion

        #region User & Repository & Validation & Credentials

        private async void getUser()
        {
            User = await this.storage.GetUser();
        }

        protected void tryGetCredentials()
        {
            PasswordCredential credentials = storage.GetCredentials();

            if (credentials != null)
            {
                credentials.RetrievePassword();
                Username = credentials.UserName;
                Password = credentials.Password;
            }
        }

        private bool validateEmail()
        {
            if (this.address == null)
            {
                return false;
            }

            this.Address = this.Address.Trim();

            Regex regex = new Regex(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");

            if (this.address == null)
            {
                ValidationMessage = "Address field cannot be empty!";

                return false;
            }

            if (!regex.IsMatch(this.address))
            {
                ValidationColor = "Red";
                ValidationMessage = "Invalid address format!";

                return false;
            }
            else
            {
                ValidationColor = "Black";
                ValidationMessage = string.Empty;
            }

            return true;
        }

        #endregion

        #region SignOut

        private bool canSignOut()
        {
            return true;
        }

        private async void trySignOut()
        {
            MessageDialog messageDialog = new MessageDialog("Do you really want to sign out?", "Ticket Helper");

            messageDialog.Commands.Add(new UICommand("SIGN OUT", new UICommandInvokedHandler(this.signOut)));
            messageDialog.Commands.Add(new UICommand("SIGN OUT AND REMOVE PERSONAL DATA", new UICommandInvokedHandler(this.signOutAndRemoveData)));
            messageDialog.Commands.Add(new UICommand("CANCEL"));

            await messageDialog.ShowAsync();
        }

        private void signOut(IUICommand command)
        {
            if (this.page.Frame.CanGoBack)
            {
                this.page.Frame.GoBack();
            }
            else
            {
                this.page.Frame.Navigate(typeof(SignInPage));
            }
        }

        private void signOutAndRemoveData(IUICommand command)
        {
            storage.RemoveCredentials();

            if (this.page.Frame.CanGoBack)
            {
                this.page.Frame.GoBack();
            }
            else
            {
                this.page.Frame.Navigate(typeof(SignInPage));
            }
        }

        #endregion

        #region Copy Command

        private async void copyIds()
        {
            string tmp = string.Empty;

            foreach (var ticket in history.Items)
            {
                if (ticket.IsSent)
                {
                    tmp += ticket.Id + "\n";
                }
            }

            try
            {
                Windows.ApplicationModel.DataTransfer.DataPackage dataPackage = new Windows.ApplicationModel.DataTransfer.DataPackage();
                dataPackage.RequestedOperation = Windows.ApplicationModel.DataTransfer.DataPackageOperation.Copy;
                dataPackage.SetText(tmp);
                Windows.ApplicationModel.DataTransfer.Clipboard.SetContent(dataPackage);
            }
            catch (Exception e)
            {
                await new MessageDialog(e.Message, "Clipboard error").ShowAsync();
            }
        }

        private bool canCopy()
        {
            return true;
        }

        #endregion

        #region Send

        private bool canSend()
        {
            return true;
        }

        private void trySend()
        {
            if (!validateEmail())
            {
                return;
            }

            Ticket ticket = new Ticket();
            ticket.Address = address;
            ticket.Content = content;
            ticket.Product = (ProductEnum)selectedProduct;
            ticket.TypeId = selectedType;

            history.Items.Add(ticket);
            History.Items = new List<Ticket>(history.Items);

            send(ticket);

            Address = string.Empty;
        }

        private void sendFailedItems()
        {
            foreach (Ticket ticket in history.Items)
            {
                if (!ticket.IsSent && !ticket.IsProcessing)
                {
                    send(ticket);
                }
            }
        }

        private async void send(Ticket ticket)
        {
            await client.SendTicket(ticket);
        }

        private bool canSendFailedItems()
        {
            return true;
        }

        #endregion

        #region Save and Open History

        private bool canSave()
        {
            return true;
        }

        private async void saveHistory()
        {
            try
            {
                await storage.SaveHistory(history);
            }
            catch (Exception e)
            {
                await new MessageDialog(e.Message).ShowAsync();
            }
        }

        private async void getHistory()
        {
            try
            {
                this.History = await storage.GetHistory();
                showAll();
            }
            catch (Exception e)
            {
                await new MessageDialog(e.Message, "Get History").ShowAsync();
            }
        }

        #endregion

        #region More menu commands

        private void showAll()
        {
            if (this.history.Items.Count > 0)
            {
                //this.DisplayedHistory.Items = new List<Ticket>(this.history.Items);
            }
        }

        private void showSent()
        {
            //this.DisplayedHistory.Items = new List<Ticket>(this.history.Items.Where(e => e.IsSent == true));
        }

        private void showFailed()
        {
            //this.DisplayedHistory.Items = new List<Ticket>(this.history.Items.Where(e => e.IsSent == false));
        }

        private async void cleanHistory()
        {
            if (history.Items.Count > 0)
            {
                this.History.Items = new List<Ticket>();
                //this.DisplayedHistory.Items = new List<Ticket>();
                await this.storage.CleanHistory();
            }
        }

        #endregion
    }
}
