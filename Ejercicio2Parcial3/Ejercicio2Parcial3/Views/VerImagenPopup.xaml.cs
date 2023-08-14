using Rg.Plugins.Popup.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Ejercicio2Parcial3.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class VerImagenPopup : PopupPage
    {
        public VerImagenPopup(ImageSource imagen)
        {
            InitializeComponent();
            Imagen.Source = imagen;
        }
    }
}