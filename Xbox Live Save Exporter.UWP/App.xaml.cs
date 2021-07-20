using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Globalization;
using Windows.Management.Deployment;
using Windows.Services.Store;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Xbox_Live_Save_Exporter.UWP
{
    /// <summary>
    /// Fournit un comportement spécifique à l'application afin de compléter la classe Application par défaut.
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// Initialise l'objet d'application de singleton.  Il s'agit de la première ligne du code créé
        /// à être exécutée. Elle correspond donc à l'équivalent logique de main() ou WinMain().
        /// </summary>
        public App()
        {
            InitializeComponent();
            Suspending += OnSuspending;

            AppCenter.Start("1c72429f-5884-4ad7-9f8d-b185dcf5eff1",
                   typeof(Analytics), typeof(Crashes));
        }

        private async Task InitializeAsync()
        {
            // Check update
            var package = Package.Current;
            var availabilityResult = await package.CheckUpdateAvailabilityAsync();

            if (availabilityResult.Availability == PackageUpdateAvailability.Available || availabilityResult.Availability == PackageUpdateAvailability.Required)
            {
                var res = ResourceLoader.GetForCurrentView();

                var dialog = new MessageDialog(
                    res.GetString("UpdateAvailableDialog"),
                    res.GetString("UpdateAvailable"));

                dialog.Commands.Add(new UICommand(
                    res.GetString("Yes"),
                    new UICommandInvokedHandler(async (command) =>
                    {
                        // https://docs.microsoft.com/en-us/windows/uwp/launch-resume/launch-store-app
                        await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-windows-store://pdp/?PFN=17470KatyCorp.XboxLiveSaveExporter"));
                    })));

                dialog.Commands.Add(new UICommand(res.GetString("No")));

                await dialog.ShowAsync();
            }

            // Check permission
            if (!await Permission.HavePermission())
                await Permission.AskPermissionAsync();
        }

        /// <summary>
        /// Invoqué lorsque l'application est lancée normalement par l'utilisateur final.  D'autres points d'entrée
        /// seront utilisés par exemple au moment du lancement de l'application pour l'ouverture d'un fichier spécifique.
        /// </summary>
        /// <param name="e">Détails concernant la requête et le processus de lancement.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            // Set size
            var minSize = new Size(250, 200);
            var prefSize = new Size(400, 450);

            ApplicationView.GetForCurrentView().SetPreferredMinSize(minSize);
            ApplicationView.PreferredLaunchViewSize = prefSize;
#if DEBUG
            //ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
#endif

#if DEBUG
            // (Debug) Set language
            //ApplicationLanguages.PrimaryLanguageOverride = "pt";
#endif
            _ = InitializeAsync();

            Frame rootFrame = Window.Current.Content as Frame;

            // Ne répétez pas l'initialisation de l'application lorsque la fenêtre comporte déjà du contenu,
            // assurez-vous juste que la fenêtre est active
            if (rootFrame == null)
            {
                // Créez un Frame utilisable comme contexte de navigation et naviguez jusqu'à la première page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: chargez l'état de l'application précédemment suspendue
                }

                // Placez le frame dans la fenêtre active
                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    // Quand la pile de navigation n'est pas restaurée, accédez à la première page,
                    // puis configurez la nouvelle page en transmettant les informations requises en tant que
                    // paramètre
                    rootFrame.Navigate(typeof(MainPage), e.Arguments);
                }
                // Vérifiez que la fenêtre actuelle est active
                Window.Current.Activate();
            }
        }

        /// <summary>
        /// Appelé lorsque la navigation vers une page donnée échoue
        /// </summary>
        /// <param name="sender">Frame à l'origine de l'échec de navigation.</param>
        /// <param name="e">Détails relatifs à l'échec de navigation</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Appelé lorsque l'exécution de l'application est suspendue.  L'état de l'application est enregistré
        /// sans savoir si l'application pourra se fermer ou reprendre sans endommager
        /// le contenu de la mémoire.
        /// </summary>
        /// <param name="sender">Source de la requête de suspension.</param>
        /// <param name="e">Détails de la requête de suspension.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: enregistrez l'état de l'application et arrêtez toute activité en arrière-plan
            deferral.Complete();
        }
    }
}
