using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TicketHelper.Common;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;

namespace TicketHelper.Models
{
    public class SignInViewModel : MainViewModel
    {
        private ICommand signInCommand;
        private bool isConnecting = false;
        private string errorMessage;
        private bool remember = false;

        public SignInViewModel(Page page) : base(page)
        {
            
        }

        public ICommand SignInCommand
        {
            get
            {
                signInCommand = new RelayCommand(
                    () => this.TrySignIn(),
                    () => this.CanSignIn());

                return signInCommand;
            }
        }

        public bool IsConnecting
        {
            get
            {
                return isConnecting;
            }

            set
            {
                isConnecting = value;
                NotifyPropertyChanged("IsConnecting");
            }
        }

        public string ErrorMessage
        {
            get
            {
                return errorMessage;
            }

            set
            {
                errorMessage = value;
                NotifyPropertyChanged("ErrorMessage");
            }
        }

        public bool Remember
        {
            get
            {
                return remember;
            }

            set
            {
                remember = value;
                NotifyPropertyChanged("Remember");
            }
        }

        private bool CanSignIn()
        {
            return true;
        }

        private void TrySignIn()
        {
            if (!this.Username.Equals(string.Empty) && !this.Password.Equals(string.Empty))
            {
                SignIn();
            }
            else
            {
                ErrorMessage = "Enter your credentials";
            }
        }

        private async void SignIn()
        {
            //26f3f9a4
            AuthenticationResult result = null;

            this.IsConnecting = true;

            try
            {
                ErrorMessage = string.Empty;
                
                result = await client.AuthenticateAsync(this.Username, this.Password);

                if (result.ResponseStatus == AuthenticationStatus.Success)
                {
                    if (remember)
                    {
                        storage.SaveCredentials(this.Username, this.Password);
                    }

                    await storage.SaveUser(result.User);

                    this.page.Frame.Navigate(typeof(MainPage));
                }
                else if (result.ResponseStatus == AuthenticationStatus.Failed)
                {
                    ErrorMessage = "Wrong Login or Password";
                }
                else if (result.ResponseStatus == AuthenticationStatus.ConnectionError)
                {
                    ErrorMessage = "Could'n connect to server";
                }
                else
                {
                    await new MessageDialog("Unknown AuthenticationResult", "Authentication Error").ShowAsync();
                }
            }
            catch(Exception e)
            {
                await new MessageDialog(e.Message).ShowAsync();
            }
            finally
            {
                this.IsConnecting = false;

                if (result.ResponseStatus == AuthenticationStatus.Success)
                {
                    GC.SuppressFinalize(this);
                }
            }
        }
    }
}
