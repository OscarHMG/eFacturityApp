﻿using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace eFacturityApp.Popups.ViewModels
{
    public class AlertConfirmationPopupPageViewModel
    {
        [Reactive] public string Title { get; set; }
        [Reactive] public string Message { get; set; }
        [Reactive] public string InputValue { get; set; }
        [Reactive] public EnumInputType InputType { get; set; }

        public enum EnumInputType { Ok, YesNo, OkCancel, OkCancelWithInput }
        public enum EnumOutputType { Ok, Yes, No, Cancel }
        public Tuple<EnumOutputType, string> ReturnValue;
        public AlertConfirmationPopupPageViewModel(string title, string message, EnumInputType inputType)
        {
            Title = title;
            Message = message;
            InputType = inputType;

            YesCommand = new Command(async () =>
            {
                await ClosePopUp(EnumOutputType.Yes, InputValue);
            });

            NoCommand = new Command(async () =>
            {
                await ClosePopUp(EnumOutputType.No, InputValue);
            });

            OkCommand = new Command(async () =>
            {
                await ClosePopUp(EnumOutputType.Ok, InputValue);
            });

            CancelCommand = new Command(async () =>
            {
                await ClosePopUp(EnumOutputType.Cancel, InputValue);
            });
        }

        private async Task ClosePopUp(EnumOutputType outputType, string inputValue)
        {
            ReturnValue = new Tuple<EnumOutputType, string>(outputType, inputValue);
            await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PopAsync();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand YesCommand { get; protected set; }
        public ICommand NoCommand { get; protected set; }
        public ICommand OkCommand { get; protected set; }
        public ICommand CancelCommand { get; protected set; }
    }
}
