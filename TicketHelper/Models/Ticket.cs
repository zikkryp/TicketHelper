using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace TicketHelper.Models
{
    public class Ticket : Notifier
    {
        private int? id;
        private string address;
        private string content;
        private int typeId = 0;
        private bool isSent = false;
        private bool isProcessing;
        private ProductEnum product;
        
        private Visibility progressVisibility = Visibility.Collapsed;
        private Visibility successVisibility = Visibility.Collapsed;
        private Visibility errorVisibility = Visibility.Collapsed;

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
        public string Subject
        {
            get
            {
                if (this.TypeId == 1)
                {
                    return "Remote Assistance";
                }

                return "Diagnostic Report";
            }
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
            }
        }

        public bool IsSent
        {
            get
            {
                return isSent;
            }

            set
            {
                isSent = value;

                if (isSent)
                {
                    SuccessVisibility = Visibility.Visible;
                    
                }
                else if (isSent == false)
                {
                    ErrorVisibility = Visibility.Visible;
                }

                NotifyPropertyChanged("IsSent");
            }
        }

        public bool IsProcessing
        {
            get
            {
                return isProcessing;
            }

            set
            {
                isProcessing = value;

                if (isProcessing)
                {
                    SuccessVisibility = Visibility.Collapsed;
                    ErrorVisibility = Visibility.Collapsed;
                    ProgressVisibility = Visibility.Visible;
                }
                else
                {
                    ProgressVisibility = Visibility.Collapsed;
                }

                NotifyPropertyChanged("IsProcessing");
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

        public Visibility ErrorVisibility
        {
            get
            {
                return errorVisibility;
            }

            set
            {
                errorVisibility = value;
                NotifyPropertyChanged("FailVisibility");
            }
        }

        public ProductEnum Product
        {
            get
            {
                return product;
            }

            set
            {
                product = value;
                NotifyPropertyChanged("Product");
            }
        }

        public int TypeId
        {
            get
            {
                return typeId;
            }

            set
            {
                typeId = value;
                NotifyPropertyChanged("TypeId");
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            Ticket ticket = obj as Ticket;

            if (ticket == null)
            {
                return false;
            }

            return Equals(ticket);
        }

        public bool Equals(Ticket ticket)
        {
            return this.Address == ticket.Address;
        }

        public override int GetHashCode()
        {
            return 1;
        }

        public override string ToString()
        {
            return this.Id.ToString();
        }
    }

    public enum ProductEnum
    {
        RA_Mac_Keeper = 0,
        RA_PC_Keeper = 1
    }
}
