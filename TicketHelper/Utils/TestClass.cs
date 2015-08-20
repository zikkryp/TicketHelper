using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketHelper.Models;
using Windows.UI.Xaml;

namespace TicketHelper.Utils
{
    public class TestClass : Notifier
    {
        private int? id;
        private bool loading = false;
        private Visibility successVisibility = Visibility.Collapsed;
        private Visibility failVisibility = Visibility.Collapsed;
        private Visibility progressVisibility = Visibility.Collapsed;
        private bool succeed = false;
        private string address;

        public int? Id
        {
            get
            {
                return id;
            }
            set
            {
                this.id = value;
                NotifyPropertyChanged("Id");
            }
        }

        public string Address
        {
            get
            {
                return address;
            }
            private set
            {
                address = value;
                NotifyPropertyChanged("Address");
            }
        }
        public ProductType ProductType { get; private set; }
        public TicketType TicketType { get; private set; }
        public string Content { get; private set; }
        public string Subject
        {
            get
            {
                if (this.TicketType == TicketType.Reconnection)
                {
                    return "Remote Assistance";
                }

                return "Diagnostic Report";
            }
        }
        public bool Loading
        {
            get
            {
                return loading;
            }

            set
            {
                loading = value;
                NotifyPropertyChanged("Loading");
            }
        }
        public Visibility SuccessVisibility
        {
            get
            {
                return successVisibility;
            }

            set
            {
                successVisibility = value;
                NotifyPropertyChanged("SuccessVisibility");
            }
        }
        public Visibility FailVisibility
        {
            get
            {
                return failVisibility;
            }

            set
            {
                failVisibility = value;
                NotifyPropertyChanged("FailVisibility");
            }
        }
        public bool Succeed
        {
            get
            {
                return succeed;
            }

            set
            {
                succeed = value;
                if (succeed)
                {
                    this.FailVisibility = Visibility.Collapsed;
                    this.SuccessVisibility = Visibility.Visible;
                }
                else
                {
                    this.FailVisibility = Visibility.Visible;
                    this.SuccessVisibility = Visibility.Collapsed;
                }

                NotifyPropertyChanged("Succeed");
            }
        }
        public Visibility ProgressVisibility
        {
            get
            {
                return progressVisibility;
            }

            set
            {
                progressVisibility = value;
                NotifyPropertyChanged("ProgressVisibility");
            }
        }
    }
}
