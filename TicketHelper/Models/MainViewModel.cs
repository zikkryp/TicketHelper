using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using TicketHelper.Common;
using TicketHelper.Utils;
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
        protected Storage storage;
        protected Page page;
        private string address;
        private string content;
        
        private History history;
        private History displayedHistory;
        private string validationColor = "Black";
        private string validationMessage;
        private User user;

        private TicketRepository repository;
        private IEnumerable<Language> languages;
        private int selectedProduct = 0;
        private int selectedLanguage = 0;
        private int selectedType = 0;

        protected HelperClient client;

        public MainViewModel(Page page)
        {
            Repository = new TicketRepository();

            this.page = page;
            this.username = string.Empty;
            this.password = string.Empty;
            
            this.storage = new Storage();
            this.client = new HelperClient();
            this.history = new History();
            this.History.Items = new List<Ticket>();
            this.displayedHistory = new History();
            DisplayedHistory.Items = new List<Ticket>();
            TryGetCredentials();
            GetTicketRepository();
            GetUser();
            GetHistory();
        }

        #region Commands

        public ICommand SignOutCommand
        {
            get
            {
                signOutCommand = new RelayCommand(
                    () => this.TrySignOut(),
                    () => this.CanSignOut());

                return signOutCommand;
            }
        }

        public ICommand SendCommand
        {
            get
            {
                sendCommand = new RelayCommand(
                    () => this.TrySend(),
                    () => this.CanSend());

                return sendCommand;
            }
        }

        public ICommand SendFailedItemsCommand
        {
            get
            {
                sendFailedItemsCommand = new RelayCommand(
                    () => this.SendFailedItems(),
                    () => this.CanSendFailedItems());

                return sendFailedItemsCommand;
            }
        }

        public ICommand CopyCommand
        {
            get
            {
                return copyCommand = new RelayCommand(
                    () => this.CopyIds(),
                    () => this.CanCopy());
            }
        }

        public ICommand ShowAllCommand
        {
            get
            {
                showAllCommand = new RelayCommand(
                    () => this.ShowAll(),
                    () => true);

                return showAllCommand;
            }
        }

        public ICommand ShowFailedCommand
        {
            get
            {
                showFailedCommand = new RelayCommand(
                    () => this.ShowFailed(),
                    () => true);

                return showFailedCommand;
            }
        }

        public ICommand ShowSentCommand
        {
            get
            {
                showSentCommand = new RelayCommand(
                    () => this.ShowSent(),
                    () => true);

                return showSentCommand;
            }
        }

        public ICommand CleanHistoryCommand
        {
            get
            {
                cleanHistoryCommand = new RelayCommand(
                    () => this.CleanHistory(),
                    () => true);

                return cleanHistoryCommand;
            }
        }

        public History DisplayedHistory
        {
            get
            {
                return displayedHistory;
            }

            set
            {
                displayedHistory = value;
                NotifyPropertyChanged("DisplayedHistory");
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
                SetContent();
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
                SetContent();
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
                if (selectedType == 1)
                {
                    Languages = repository.Reconnection.Items;
                }
                else
                {
                    Languages = repository.DiagnosticReport.Items[selectedProduct].Items;
                }
                SelectedLanguage = 0;
                SetContent();
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
                SetContent();
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

        public ICommand SaveCommand
        {
            get
            {
                saveCommand = new RelayCommand(
                    ()=> this.SaveHistory(),
                    ()=> this.CanSave());

                return saveCommand;
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

        private async void GetTicketRepository()
        {
            Repository = await TicketSource.GetTicketAsync();

            SelectedProduct = 0;
            SelectedLanguage = 0;   
        }

        private void SetContent()
        {
            //await new MessageDialog(selectedType.ToString()).ShowAsync();
            Content = Repository.GetContent(selectedProduct, selectedLanguage, selectedType);
        }

        #endregion

        #region User & Repository & Validation & Credentials

        private async void GetUser()
        {
            this.User = await this.storage.GetUser();
        }

        protected void TryGetCredentials()
        {
            PasswordCredential credentials = storage.GetCredentials();

            if (credentials != null)
            {
                credentials.RetrievePassword();
                Username = credentials.UserName;
                Password = credentials.Password;
            }
        }

        private bool ValidateEmail()
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
                this.ValidationMessage = "Address field cannot be empty!";

                return false;
            }

            if (!regex.IsMatch(this.address))
            {
                this.ValidationColor = "Red";
                this.ValidationMessage = "Invalid address format!";

                return false;
            }
            else
            {
                this.ValidationColor = "Black";
                this.ValidationMessage = string.Empty;
            }

            return true;
        }

        #endregion

        #region SignOut

        private bool CanSignOut()
        {
            return true;
        }

        private async void TrySignOut()
        {
            MessageDialog messageDialog = new MessageDialog("Do you really want to sign out?", "Ticket Helper");
            messageDialog.Commands.Add(new UICommand("Sign Out", new UICommandInvokedHandler(this.SignOut)));
            messageDialog.Commands.Add(new UICommand("Sign Out and Remove Personal Data", new UICommandInvokedHandler(this.SignOutAndRemoveData)));
            messageDialog.Commands.Add(new UICommand("NO"));
            await messageDialog.ShowAsync();
        }

        private void SignOut(IUICommand command)
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

        private void SignOutAndRemoveData(IUICommand command)
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

        private async void CopyIds()
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
            catch(Exception e)
            {
                await new MessageDialog(e.Message, "Clipboard error").ShowAsync();
            }
        }

        private bool CanCopy()
        {
            return true;
        }

        #endregion

        #region Send

        private bool CanSend()
        {
            return true;
        }

        private void TrySend()
        {
            if (!ValidateEmail())
            {
                return;
            }

            Ticket ticket = new Ticket();
            ticket.Address = this.address;
            ticket.Content = this.content;

            if (!this.history.Items.Contains(ticket))
            {
                Send(ticket);
            }
            else
            {
                this.ValidationMessage = "Ticket has already been sent!";
            }
        }

        private async void Send(Ticket ticket)
        {
            await client.SendTicket(ticket);
        }

        private void SendFailedItems()
        {
            foreach (Ticket ticket in history.Items)
            {
                if (!ticket.IsSent && !ticket.IsProcessing)
                {
                    Send(ticket);
                }
            }
        }

        private bool CanSendFailedItems()
        {
            return true;
        }

        #endregion

        #region Save and Open History

        private bool CanSave()
        {
            return true;
        }

        private async void SaveHistory()
        {
            try
            {
                await storage.SaveHistory(history);
            }
            catch(Exception e)
            {
                await new MessageDialog(e.Message).ShowAsync();
            }
        }

        private async void GetHistory()
        {
            try
            {
                this.History = await storage.GetHistory();
                ShowAll();
            }
            catch(Exception e)
            {
                await new MessageDialog(e.Message, "Get History").ShowAsync();
            }
        }

        #endregion

        #region More menu commands

        private void ShowAll()
        {
            if (this.history.Items.Count > 0)
            {
                this.DisplayedHistory.Items = new List<Ticket>(this.history.Items);
            }
        }

        private void ShowSent()
        {
            this.DisplayedHistory.Items = new List<Ticket>(this.history.Items.Where(e => e.IsSent == true));
        }

        private void ShowFailed()
        {
            this.DisplayedHistory.Items = new List<Ticket>(this.history.Items.Where(e => e.IsSent == false));
        }

        private void CleanHistory()
        {
            this.History.Items = new List<Ticket>();
            this.DisplayedHistory.Items = new List<Ticket>();
        }

        #endregion
    }
}
