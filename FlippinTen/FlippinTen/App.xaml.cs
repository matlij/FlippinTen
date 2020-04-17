using DLToolkit.Forms.Controls;
using FlippinTen.Bootstrap;
using FlippinTen.Views;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FlippinTen
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            FlowListView.Init();
            AppContainer.RegisterDependencies();

            MainPage = new NavigationPage(new MenuPage());
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
