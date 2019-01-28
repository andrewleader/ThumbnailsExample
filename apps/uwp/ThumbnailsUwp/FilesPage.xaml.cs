using graph_tutorial;
using Microsoft.Toolkit.Services.MicrosoftGraph;
using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace ThumbnailsUwp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FilesPage : Page
    {
        public FilesPage()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            await RefreshAsync();

            base.OnNavigatedTo(e);
        }

        private void ShowNotification(string message)
        {
            // Get the main page that contains the InAppNotification
            var mainPage = (Window.Current.Content as Frame).Content as MainPage;

            // Get the notification control
            var notification = mainPage.FindName("Notification") as InAppNotification;

            notification.Show(message);
        }

        private async void ButtonRefresh_Click(object sender, RoutedEventArgs e)
        {
            RemoteAdaptiveCardControl.ClearCache();

            await RefreshAsync();
        }

        private async Task RefreshAsync()
        {
            RecentFilesGridView.ItemsSource = null;

            // Get the Graph client from the service
            var graphClient = MicrosoftGraphService.Instance.GraphProvider;

            try
            {
                // Get the recent files
                var files = await graphClient.Me.Drive.Recent().Request().GetAsync();

                RecentFilesGridView.ItemsSource = files;
            }
            catch (Microsoft.Graph.ServiceException ex)
            {
                ShowNotification($"Exception getting events: {ex.Message}");
            }
        }
    }
}
