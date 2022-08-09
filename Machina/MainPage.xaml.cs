using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Machina
{
    public partial class MainPage : ContentPage
    {
        private bool isFirstLoanch;
        public MainPage()
        {
            InitializeComponent();
            isFirstLoanch = true;
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (!isFirstLoanch)
                animationView.PlayAnimation();
        }
        private async void StartButton(object sender, EventArgs e)
        {
            await startButtonClickedAsync();
        }

        private async Task startButtonClickedAsync()
        {
            await CrossMedia.Current.Initialize();
            animationView.StopAnimation();

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await DisplayAlert("Error", ":( No camera available.", "OK");
                return;
            }

            var file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
            {
                Directory = "Sample",
                Name = DateTime.Now.Ticks.ToString() + "picture.jpg"
            });

            if (file == null)
            {
                await DisplayAlert("Error", "No file", "OK");
                return;
            }

            isFirstLoanch = false;
            await Navigation.PushAsync(new ScannerPage(file));
            File.Delete(file.Path);
        }
    }
}
