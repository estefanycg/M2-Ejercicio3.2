using Ejercicio2Parcial3.ViewModels;
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
    public partial class ActualizarRegistroAlumno : ContentPage
    {
        public ActualizarRegistroAlumno()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (BindingContext is ActualizarRegistroAlumnoViewModel viewModel)
            {
                viewModel.CargarDatosAlumno();
            }
        }



    }
}