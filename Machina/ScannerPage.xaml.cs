using Machina.Services;
using Plugin.Media.Abstractions;
using Plugin.SimpleAudioPlayer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Machina
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ScannerPage : ContentPage
    {
        private string age;
        private string sexe;
        private MediaFile file;
        private ISimpleAudioPlayer player;
        private SpeechOptions options = null;
        private ImageSource imageSource;
        private bool isFinished;
        public HttpClient client;
        public bool _IsFinished 
        {
            get => isFinished;
            set
            {
                isFinished = value;
                OnPropertyChanged();
            }
        }
        public string Age
        {
            get => age;
            set
            {
                age = value;
                OnPropertyChanged();
            }
        }
        public string Sexe
        {
            get => sexe;
            set
            {
                sexe = value;
                OnPropertyChanged();
            }
        }
        public ImageSource _ImageSource
        {
            get => imageSource;
            set
            {
                imageSource = value;
                OnPropertyChanged();
            }
        }
        public ScannerPage()
        {
            InitializeComponent();
            laserAnimation();
            Console.WriteLine("Step over the animation");
        }
        public ScannerPage(MediaFile _file)
        {
            file = _file;
            age = "XX";
            sexe = "X";
            isFinished = false;
            imageSource = ImageSource.FromStream(() => {
                var stream = file.GetStreamWithImageRotatedForExternalStorage();
                return stream;
            });
            BindingContext = this;
            InitializeComponent();
            laserAnimation();
        }

        private async Task laserAnimation()
        {
            laserImage.Opacity = 0;
            //await Task.Delay(500);
            await laserImage.FadeTo(1, 500);
            bool reachedBottom = false;
            double target;
            int counter = 0;
            while (counter < 2)
            {
                target = reachedBottom ? 0 : 340;
                PlaySound("scan.wav");
                await laserImage.TranslateTo(0, target, 1800);
                reachedBottom = !reachedBottom;
                counter++;
            }
            CognitiveServices.FaceDetect();
            stateLabel.Text = "Scan Completed";
            laserImage.IsVisible = false;
            Age = "21";
            Sexe = "M";
            _IsFinished = true;
            continueButton.Opacity = 1;
            StartSpeech();
        }

        private async void StartSpeech()
        {
            await Speak("Human found");
            await Speak("Sex " + ((sexe == "M") ? "Male" : "Female"));
            await Speak("Age");
            Speak(age + "years");
        }

        private void PlaySound(string sound)
        {
            player = Plugin.SimpleAudioPlayer.CrossSimpleAudioPlayer.Current;
            player.Load(GetStreamFromFile(sound));
            player.Play();
        }

        private Stream GetStreamFromFile(string fileName)
        {
            var assembly = typeof(App).GetTypeInfo().Assembly;
            var stream = assembly.GetManifestResourceStream("Machina." + fileName);
            return stream;
        }
        private async Task Speak(string textToSay)
        {
            if(options == null)
            {
                InitSpeaker();
            }
            await TextToSpeech.SpeakAsync(textToSay, options);
        }

        private void InitSpeaker()
        {
            options = new SpeechOptions()
            {
                Pitch = 0.05f
            };
        }

        private async void BackButton(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}