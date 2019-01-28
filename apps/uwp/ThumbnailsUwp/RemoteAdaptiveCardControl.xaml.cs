using AdaptiveCards.Rendering.Uwp;
using JsonTransformLanguage;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
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
    public sealed partial class RemoteAdaptiveCardControl : UserControl
    {
        public RemoteAdaptiveCardControl()
        {
            this.InitializeComponent();
        }




        public object Data
        {
            get { return (object)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Data.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register("Data", typeof(object), typeof(RemoteAdaptiveCardControl), new PropertyMetadata(null, Render));




        public Uri TemplateUrl
        {
            get { return (Uri)GetValue(TemplateUrlProperty); }
            set { SetValue(TemplateUrlProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TemplateUrl.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TemplateUrlProperty =
            DependencyProperty.Register("TemplateUrl", typeof(Uri), typeof(RemoteAdaptiveCardControl), new PropertyMetadata(null, Render));



        public Uri HostConfigUrl
        {
            get { return (Uri)GetValue(HostConfigUrlProperty); }
            set { SetValue(HostConfigUrlProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HostConfigUrl.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HostConfigUrlProperty =
            DependencyProperty.Register("HostConfigUrl", typeof(Uri), typeof(RemoteAdaptiveCardControl), new PropertyMetadata(null, Render));

        private static void Render(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as RemoteAdaptiveCardControl).Render();
        }

        private int _opCode = 0;
        private async void Render()
        {
            if (TemplateUrl == null || HostConfigUrl == null || Data == null)
            {
                ContentContainer.Child = null;
                return;
            }
            try
            {
                _opCode++;
                int currOpCode = _opCode;

                JObject template = await TemplateCache.Current.GetOrLoadAsync(TemplateUrl);
                var renderer = await RendererCache.Current.GetOrLoadAsync(HostConfigUrl);

                if (_opCode != currOpCode)
                {
                    return;
                }

                JToken transformedCard = JsonTransformer.Transform(template, JObject.FromObject(Data), new Dictionary<string, JToken>());
                string cardString = transformedCard.ToString();

                var renderResult = renderer.RenderAdaptiveCardFromJsonString(cardString);
                if (renderResult.Errors.Count > 0)
                {
                    throw new Exception(string.Join("\n", renderResult.Errors.Select(i => i.Message)));
                }
                ContentContainer.Child = renderResult.FrameworkElement;
            }
            catch (Exception ex)
            {
                ContentContainer.Child = new TextBlock()
                {
                    Text = ex.ToString(),
                    TextWrapping = TextWrapping.Wrap
                };
            }
        }

        private abstract class BaseCache<T>
        {
            private Dictionary<Uri, Task<T>> _cached = new Dictionary<Uri, Task<T>>();
            private HttpClient _httpClient = new HttpClient();

            public Task<T> GetOrLoadAsync(Uri uri)
            {
                lock (_cached)
                {
                    if (_cached.TryGetValue(uri, out Task<T> value) && !value.IsFaulted)
                    {
                        return value;
                    }

                    var answer = GetAsync(uri);
                    _cached[uri] = answer;
                    return answer;
                }
            }

            private async Task<T> GetAsync(Uri uri)
            {
                string str = await _httpClient.GetStringAsync(uri);
                return CreateObject(str);
            }

            protected abstract T CreateObject(string str);
        }

        private class TemplateCache : BaseCache<JObject>
        {
            public static TemplateCache Current { get; } = new TemplateCache();

            protected override JObject CreateObject(string str)
            {
                var template = JObject.Parse(str);
                template.Remove("data");
                return template;
            }
        }

        private class RendererCache : BaseCache<AdaptiveCardRenderer>
        {
            public static RendererCache Current { get; } = new RendererCache();

            protected override AdaptiveCardRenderer CreateObject(string str)
            {
                var hostConfig = AdaptiveHostConfig.FromJsonString(str).HostConfig;
                return new AdaptiveCardRenderer()
                {
                    HostConfig = hostConfig
                };
            }
        }
    }
}
