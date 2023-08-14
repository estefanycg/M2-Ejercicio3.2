using Ejercicio2Parcial3.Models;
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
    public partial class ListaRegistrosAlumnos : ContentPage

    {

        private Alumnos _selectedAlumno;

        public ListaRegistrosAlumnos()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (BindingContext is ListaRegistrosAlumnosViewModel viewModel)
            {
                viewModel.CargarAlumnos(); 
            }
        }


        private void OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item != null)
            {
                _selectedAlumno = e.Item as Alumnos;
                if (BindingContext is ListaRegistrosAlumnosViewModel viewModel)
                {
                    viewModel.AlumnoSeleccionado = _selectedAlumno;
                }
            }
        }


    }
}