using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using TicketHelper.Models;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace TicketHelper
{
    public sealed partial class SignInPage : Page
    {
        private SignInViewModel viewModel;

        public SignInPage()
        {
            this.viewModel = new SignInViewModel(this);
            this.DataContext = viewModel;

            this.InitializeComponent();
        }
    }
}
