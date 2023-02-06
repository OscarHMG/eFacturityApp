using eFacturityApp.Popups.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Xamarin.Forms;

namespace eFacturityApp.Converters
{

    /// <summary>
    /// Converter para mostrar el alert pop up de confirmacion
    /// </summary>
    public class DisplayAlertTypeToIsVisibleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if (parameter is null || !(parameter is string))
                return false;

            string[] pars = ((string)parameter).Split('|');

            if (value is AlertConfirmationPopupPageViewModel.EnumInputType && pars != null)
            {
                switch ((AlertConfirmationPopupPageViewModel.EnumInputType)value)
                {
                    case AlertConfirmationPopupPageViewModel.EnumInputType.Ok:
                        if (Array.IndexOf(pars, "OK") >= 0)
                            return true;

                        break;
                    case AlertConfirmationPopupPageViewModel.EnumInputType.YesNo:
                        if (Array.IndexOf(pars, "YESNO") >= 0)
                            return true;

                        break;
                    case AlertConfirmationPopupPageViewModel.EnumInputType.OkCancel:
                        if (Array.IndexOf(pars, "OKCANCEL") >= 0)
                            return true;

                        break;
                    case AlertConfirmationPopupPageViewModel.EnumInputType.OkCancelWithInput:
                        if (Array.IndexOf(pars, "OKCANCELWITHINPUT") >= 0)
                            return true;

                        break;
                    default:
                        return false;
                }
            }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }



    }

    /// <summary>
    /// Converter para obtener la negacion del valor true/false
    /// </summary>
    public class InverseBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => !(bool)value;
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => !(bool)value;
    }

    /// <summary>
    /// Converter para validar igualdad de valores
    /// </summary>
    public class EqualityToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = object.Equals(value, parameter);
            return val;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(bool)value)
                return parameter;

            //it's false, so don't bind it back
            throw new Exception("EqualityToBooleanConverter: It's false, I won't bind back.");
        }
    }

    public class CurrencyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //return Decimal.Parse(value.ToString()).ToString("C");
            //system.Globalization.CultureInfo.InvariantCulture
            if (value == null)
            {
                return Decimal.Parse("0", System.Globalization.CultureInfo.InvariantCulture).ToString("C");
            }
            return Decimal.Parse(value.ToString(), System.Globalization.CultureInfo.InvariantCulture).ToString("C");
            //return Decimal.Parse(value.ToString(), System.Globalization.CultureInfo.InvariantCulture).ToString("C");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string valueFromString = Regex.Replace(value.ToString(), @"\D", "");

            if (valueFromString.Length <= 0)
                return 0m;

            long valueLong;
            if (!long.TryParse(valueFromString, out valueLong))
                return 0m;

            if (valueLong <= 0)
                return 0m;

            return valueLong / 100m;
        }
    }


    
}
