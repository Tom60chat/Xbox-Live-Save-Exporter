/*
 *  From : https://stackoverflow.com/questions/47888405/uwp-localization-using-one-resource
 */
using System;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Xbox_Live_Save_Exporter.UWP
{
    public class LocalizationConverter : IValueConverter
    {
        private readonly ResourceLoader _resourceLoader = ResourceLoader.GetForViewIndependentUse("/Resources");

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (parameter is string resourceId)
            {
                return _resourceLoader.GetString(resourceId);
            }
            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotSupportedException();
        }
    }
}
