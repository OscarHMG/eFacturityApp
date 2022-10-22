using eFacturityApp.Infraestructure;
using eFacturityApp.Infraestructure.Services;
using eFacturityApp.Popups.ViewModels;
using eFacturityApp.Services;
using Prism.Navigation;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using static eFacturityApp.Infraestructure.ApiModels.APIModels;
using static eFacturityApp.Utils.Utility;
namespace eFacturityApp.ViewModels
{
    public class ProfilePageViewModel : ViewModelBase
    {
        [Reactive] public bool ContainerEditInformationProfile { get; set; } = false;

        [Reactive] public bool ConfirmationBackPromptModal { get; set; } = false;

        public ICommand EnableEditProfileInformationCommand { get; set; }

        public ICommand CancelCommand { get; set; }

        public ICommand LoadProfileInformationCommand { get; set; }

        [Reactive] public PerfilUsuarioModel Perfil { get; set; } = new PerfilUsuarioModel();

        [Reactive] public string ProfileImageAvatar { get; set; }

        public ICommand SaveCommand {get; set;}



        public ICommand TomarFotosGaleriaCommad { get; set; }

        public static string REGEX_EMAIL_PATTERN = @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z";


        public ProfilePageViewModel(INavigationService navigationService, LoaderService loader, UserService userService, ApiService apiService) : base(navigationService, loader, userService, apiService)
        {
            EnableEditProfileInformationCommand = new Command(() => {
                ContainerEditInformationProfile = true;
                ConfirmationBackPromptModal = true;
            });

            CancelCommand = new Command(()=> 
            {
                ContainerEditInformationProfile = false;
                ConfirmationBackPromptModal = false;
                LoadProfileInformationCommand.Execute(null);
            });

            LoadProfileInformationCommand = new Command(async()=> {
                try
                {
                    Perfil = await _userService.GetUserInformationProfile();

                    ProfileImageAvatar = string.IsNullOrEmpty(Perfil.RutaImagen) ? "avatar" : Perfil.RutaImagen;
                }
                catch (Exception e)
                {
                    await ShowAlert("Mi perfil", "Ocurrio un error al consultar su información de perfil ", AlertConfirmationPopupPageViewModel.EnumInputType.Ok, _navigationService);

                }
            });


            SaveCommand = new Command(async () => await SaveProfile());


            TomarFotosGaleriaCommad = new Command(async()=> await ShowGalleryAndPickPhotos());
        }


        public async Task SaveProfile()
        {
            try
            {
                if (await ValidateFields())
                {
                    var Response = await ShowYesNoAlert("Actualizar perfil", "¿Está seguro que desea actualizar su información?", _navigationService);

                    if (Response)
                    {
                        await _loaderService.Show("Actualizando su perfil..");
                        var response = await _apiService.UpdateProfileInformation(Perfil);

                        if (await HandleAPIResponse(response.statusCode, response.message, "Mi perfil", _navigationService))
                        {
                            //Update information after edited
                            var responseProfile = await _apiService.GetProfileInformation(new LoginUsuarioRequest());
                            if (responseProfile.statusCode == 200)
                            {
                                await _userService.SaveUserInformationProfile(responseProfile.data);
                                await ShowAlert("Mi Perfil", "Su información de perfil, se ha actualizado", AlertConfirmationPopupPageViewModel.EnumInputType.Ok, _navigationService);
                                LoadProfileInformationCommand.Execute(null);
                                CancelCommand.Execute(null);
                            }
                        }
                    }
                    
                }
            }
            catch (Exception err)
            {
                await ShowAlert("Mi perfil", "Ocurrio inesperado: " + err.Message, AlertConfirmationPopupPageViewModel.EnumInputType.Ok, _navigationService);

            }
        }

        private async Task<bool> ValidateFields()
        {
            string Message = null;
            if (string.IsNullOrEmpty(Perfil.NombreComercial))
            {
                Message = "Nombre comercial, es obligatorio";
            }
            else if (string.IsNullOrEmpty(Perfil.Ciudad))
            {
                Message = "Ciudad, es obligatorio";
            }
            else if (string.IsNullOrEmpty(Perfil.Telefono))
            {
                Message = "Telefono, es obligatorio";
            }
            else if (string.IsNullOrEmpty(Perfil.Movil))
            {
                Message = "Móvil, es obligatorio";
            }
            else if (string.IsNullOrEmpty(Perfil.Correo))
            {
                Message = "Correo, es obligatorio";
            }
            else if (string.IsNullOrEmpty(Perfil.DireccionDomiciaria))
            {
                Message = "Dirección de domicilio, es obligatorio";
            }
            else if (Perfil.Movil.Trim().Length != 10)
            {
                Message = "Móvil, debe contener 10 dígitos";
            }
            else if (!Regex.IsMatch(Perfil.Correo.Trim(), REGEX_EMAIL_PATTERN, RegexOptions.IgnoreCase))
            {
                Message = "Correo, tiene un formato inválido.";
            }


            if (!string.IsNullOrEmpty(Message))
            {
                await ShowAlert("Mi perfil", Message, AlertConfirmationPopupPageViewModel.EnumInputType.Ok, _navigationService);

            }

            return string.IsNullOrEmpty(Message);
        }

        public override void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);
            LoadProfileInformationCommand.Execute(null);
        }

        public override void OnNavigatedFrom(INavigationParameters parameters)
        {
            base.OnNavigatedFrom(parameters);
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
        }

        public override void Destroy()
        {
            base.Destroy();
        }


        async Task ShowGalleryAndPickPhotos()
        {
            try
            {
                var customFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.Android, new[] { "image/png", "image/jpg", "image/jpeg" } },
                    {  DevicePlatform.iOS, new[]{ "image/png", "image/jpg", "image/jpeg" } }
                });
                var options = new PickOptions
                {
                    PickerTitle = "Escoger Fotos Galería",
                    FileTypes = customFileType,
                };
                var result = await FilePicker.PickAsync(options);
                if (result != null)
                {
                    ProfileImageAvatar = result.FullPath;
                }
            }
            catch (Exception ex)
            {
                await ShowAlert("Mi perfil", "Ocurrió un error al abrir la galería", AlertConfirmationPopupPageViewModel.EnumInputType.Ok, _navigationService);

            }
        }
    }
}
