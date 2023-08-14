using System;
using System.Windows.Input;
using Xamarin.Forms;
using Rg.Plugins.Popup.Services;

namespace Ejercicio2Parcial3.ViewModels
{
    public class VerImagenPopupViewModel
    {
        public ICommand ClosePopupCommand { get; private set; }

        public VerImagenPopupViewModel()
        {
            ClosePopupCommand = new Command(CerrarPopup);
        }

        private async void CerrarPopup()
        {
            await PopupNavigation.Instance.PopAsync();
        }
    }
}
