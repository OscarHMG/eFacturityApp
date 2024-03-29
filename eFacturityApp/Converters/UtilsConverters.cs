﻿using eFacturityApp.Popups.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Threading;
using Xamarin.Forms;
using eFacturityApp.Infraestructure.Services;

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
            UserService userService = new UserService(null, null);
            var data = userService.GetUserInformationProfile2();

            string CData = "C";
            if (data == null)
            {
                CData = "C2";
            }
            else
            {
                CData = CData + ((data.NumDecimales == null) ? "2" : data.NumDecimales.GetValueOrDefault().ToString()); //By default, minimun will have 2 decimals palces
            }

            if (value == null)
            {
                return Decimal.Parse("0", System.Globalization.CultureInfo.InvariantCulture).ToString(CData);
            }
            return Decimal.Parse(value.ToString(), System.Globalization.CultureInfo.InvariantCulture).ToString(CData);
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



    public sealed class TaskCompletionNotifier<TResult> : INotifyPropertyChanged
    {
        public TaskCompletionNotifier(Task<TResult> task)
        {
            Task = task;
            if (!task.IsCompleted)
            {
                var scheduler = (SynchronizationContext.Current == null) ? TaskScheduler.Current : TaskScheduler.FromCurrentSynchronizationContext();
                task.ContinueWith(t =>
                {
                    var propertyChanged = PropertyChanged;
                    if (propertyChanged != null)
                    {
                        propertyChanged(this, new PropertyChangedEventArgs("IsCompleted"));
                        if (t.IsCanceled)
                        {
                            propertyChanged(this, new PropertyChangedEventArgs("IsCanceled"));
                        }
                        else if (t.IsFaulted)
                        {
                            propertyChanged(this, new PropertyChangedEventArgs("IsFaulted"));
                            propertyChanged(this, new PropertyChangedEventArgs("ErrorMessage"));
                        }
                        else
                        {
                            propertyChanged(this, new PropertyChangedEventArgs("IsSuccessfullyCompleted"));
                            propertyChanged(this, new PropertyChangedEventArgs("Result"));
                        }
                    }
                },
                CancellationToken.None,
                TaskContinuationOptions.ExecuteSynchronously,
                scheduler);
            }
        }

        // Gets the task being watched. This property never changes and is never <c>null</c>.
        public Task<TResult> Task { get; private set; }



        // Gets the result of the task. Returns the default value of TResult if the task has not completed successfully.
        public TResult Result { get { return (Task.Status == TaskStatus.RanToCompletion) ? Task.Result : default(TResult); } }

        // Gets whether the task has completed.
        public bool IsCompleted { get { return Task.IsCompleted; } }

        // Gets whether the task has completed successfully.
        public bool IsSuccessfullyCompleted { get { return Task.Status == TaskStatus.RanToCompletion; } }

        // Gets whether the task has been canceled.
        public bool IsCanceled { get { return Task.IsCanceled; } }

        // Gets whether the task has faulted.
        public bool IsFaulted { get { return Task.IsFaulted; } }


        public event PropertyChangedEventHandler PropertyChanged;
    }


}
