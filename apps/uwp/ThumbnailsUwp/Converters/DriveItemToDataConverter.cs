using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace ThumbnailsUwp.Converters
{
    public class DriveItemToDataConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is DriveItem driveItem)
            {
                dynamic answer = new ExpandoObject();
                answer.name = driveItem.Name;
                answer.lastModified = driveItem.LastModifiedDateTime.GetValueOrDefault().ToLocalTime().ToString("MMM d, yyyy");
                answer.thumbnail = driveItem.Thumbnails?.FirstOrDefault().Medium.Url ?? "http://static1.squarespace.com/static/5248a8c6e4b0a4c7037edb93/t/55ce40dce4b0fe0a5d72daa8/1439580380983/Excel2013FileIcon.png";
                return answer;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
