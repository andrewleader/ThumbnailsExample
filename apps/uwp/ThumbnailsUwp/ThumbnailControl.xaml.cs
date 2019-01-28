using AdaptiveCards.Rendering.Uwp;
using JsonTransformLanguage;
using Microsoft.Graph;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
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

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace ThumbnailsUwp
{
    public sealed partial class ThumbnailControl : UserControl
    {
        public ThumbnailControl()
        {
            this.InitializeComponent();
        }

        public DriveItem DriveItem
        {
            get { return (DriveItem)GetValue(DriveItemProperty); }
            set { SetValue(DriveItemProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DriveItem.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DriveItemProperty =
            DependencyProperty.Register("DriveItem", typeof(DriveItem), typeof(ThumbnailControl), new PropertyMetadata(null));
    }
}
