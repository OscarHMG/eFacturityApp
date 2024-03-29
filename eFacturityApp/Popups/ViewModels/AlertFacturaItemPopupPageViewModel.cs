﻿using eFacturityApp.Infraestructure;
using eFacturityApp.Infraestructure.Services;
using eFacturityApp.Services;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using static eFacturityApp.Infraestructure.ApiModels.APIModels;
using static eFacturityApp.Utils.Utility;

namespace eFacturityApp.Popups.ViewModels
{
    public class AlertFacturaItemPopupPageViewModel : ViewModelBase
    {
        [Reactive] public List<ProductoModel> ProductosServicios { get; set; } = new List<ProductoModel>();

        [Reactive] public decimal Descuento { get; set; }

        [Reactive] public decimal Precio { get; set; }
        [Reactive] public ItemFacturaModel NewItemFactura { get; set; } = new ItemFacturaModel();
        [Reactive] public ItemProforma NewItemProforma { get; set; } = new ItemProforma();
        [Reactive] public ItemLiquidacionCompraModel NewItemLiquidacionCompra { get; set; } = new ItemLiquidacionCompraModel();
        [Reactive] public decimal Cantidad { get; set; }
        [Reactive] public string ErrorMessage { get; set; } = string.Empty;
        [Reactive] public string ReturnPageTo { get; set; }
        [Reactive] public DropDown DropDownArticulos { get; set; }

        [Reactive] public ItemPicker ArticuloSelected { get; set; }
        [Reactive] public bool EnableDescuentoEntry { get; set; } = true;
        [Reactive] public bool EnablePrecioEntry { get; set; } = true;
        public ICommand LoadArticulosCatalogosCommand { get; set; }

        public ICommand CancelCommand { get; set; }
        public ICommand AddNewItemCommand { get; set; }
        public ICommand LoadUserInformationCommand { get; set; }

        PerfilUsuarioModel PerfilUsuario = null;


        [Reactive] public int NumeroDecimales { get; set; } = 2;
        public AlertFacturaItemPopupPageViewModel(INavigationService navigationService, LoaderService loader, UserService userService, ApiService apiService) : base(navigationService, loader, userService, apiService)
        {
            Descuento = 0;
            Cantidad = 1;
            LoadArticulosCatalogosCommand = new Command(() => LoadDropDowns(ProductosServicios));

            CancelCommand = new Command(async () => await NavigateBack(_navigationService));

            AddNewItemCommand = new Command(async () => await CreateNewItemFacturaAsync());

            this.WhenAnyValue(x => x.ArticuloSelected)
                .Where(x => x != null)
                .InvokeCommand(new Command(async () =>
                {
                    if (ArticuloSelected != null)
                    {
                        var item = ProductosServicios.ToList().FirstOrDefault(c => c.IdProducto == ArticuloSelected.Id);
                        if (item != null)
                        {
                            Precio = item.Precio;
                            EnablePrecioEntry = item.PrecioManual.GetValueOrDefault();
                        }
                    }
                }
                ));
            LoadUserInformationCommand = new Command(async () =>
            {
                PerfilUsuario = await userService.GetUserInformationProfile();
                if (PerfilUsuario != null)
                {
                    NumeroDecimales = PerfilUsuario.NumDecimales != null ? PerfilUsuario.NumDecimales.GetValueOrDefault() : 2;
                }
                else
                {
                    NumeroDecimales = 2;
                }
            });

        }


        public override void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);

            ProductosServicios = parameters.GetValue<List<ProductoModel>>("ListProductos");
            Descuento = parameters.GetValue<decimal>("DescuentoGlobal");
            ReturnPageTo = parameters.GetValue<string>("PageToReturn");
            if (ProductosServicios != null)
            {
                if (Descuento != 0)
                {
                    //NewItemFactura.Descuento = Descuento;
                    EnableDescuentoEntry = true;

                }

                LoadArticulosCatalogosCommand.Execute(null);
            }

            LoadUserInformationCommand.Execute(null);
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


        private void LoadDropDowns(List<ProductoModel> ListProductos)
        {

            List<ItemPicker> ItemsProductosServicios = new List<ItemPicker>();

            ListProductos.ForEach(x => ItemsProductosServicios.Add(new ItemPicker(x.IdProducto, (x.Codigo + " " + x.Nombre).ToUpper(), (x.Codigo + " " + x.Nombre).ToUpper())));

            DropDownArticulos = new DropDown(ItemsProductosServicios);
        }


        private async Task CreateNewItemFacturaAsync()
        {
            if (ArticuloSelected == null)
            {
                ErrorMessage = "Debe escoger un artículo o servicio.";
            }
            else
            {
                var item = ProductosServicios.ToList().FirstOrDefault(c => c.IdProducto == ArticuloSelected.Id);
                if (ReturnPageTo.Equals("FacturaPage"))
                {
                    NewItemFactura.IdProducto = ArticuloSelected.Id;
                    NewItemFactura.NombreProducto = ArticuloSelected.TextoLargo;
                    NewItemFactura.Precio = Precio;
                    NewItemFactura.Descuento = Descuento;
                    NewItemFactura.Cantidad =  Cantidad;
                    if (item.PrecioManual != null && item.PrecioManual.Value)
                    {
                        NewItemFactura.PrecioManual = Precio;
                    }
                    else
                    {
                        NewItemFactura.PrecioManual = null;
                    }

                    NavigationParameters NavParams = new NavigationParameters();
                    NavParams.Add("NewItemFactura", NewItemFactura);

                    await NavigateBack(_navigationService, NavParams);
                }
                else if (ReturnPageTo.Equals("ProformaPage"))
                {
                    NewItemProforma.IdProducto = ArticuloSelected.Id;
                    NewItemProforma.NombreProducto = ArticuloSelected.TextoLargo;
                    NewItemProforma.Precio = Precio;
                    NewItemProforma.Descuento = Descuento;
                    NewItemProforma.Cantidad= Cantidad;
                    if (item.PrecioManual != null && item.PrecioManual.Value)
                    {
                        NewItemProforma.PrecioManual = Precio;
                    }
                    else
                    {
                        NewItemProforma.PrecioManual = null;
                    }

                    NavigationParameters NavParams = new NavigationParameters();
                    NavParams.Add("NewItemProforma", NewItemProforma);

                    await NavigateBack(_navigationService, NavParams);

                }
                else if (ReturnPageTo.Equals("LiquidacionCompraPage"))
                {
                    NewItemLiquidacionCompra.IdProducto = ArticuloSelected.Id;
                    NewItemLiquidacionCompra.NombreProducto = ArticuloSelected.TextoLargo;
                    NewItemLiquidacionCompra.Precio = Precio;
                    NewItemLiquidacionCompra.Descuento = Descuento;
                    NewItemLiquidacionCompra.Cantidad = Cantidad;
                    if (item.PrecioManual != null && item.PrecioManual.Value)
                    {
                        NewItemLiquidacionCompra.PrecioManual = Precio;
                    }
                    else
                    {
                        NewItemLiquidacionCompra.PrecioManual = null;
                    }

                    NavigationParameters NavParams = new NavigationParameters();
                    NavParams.Add("NewItemLiquidacionCompra", NewItemLiquidacionCompra);

                    await NavigateBack(_navigationService, NavParams);
                }



                
            }
        }
    }
}
