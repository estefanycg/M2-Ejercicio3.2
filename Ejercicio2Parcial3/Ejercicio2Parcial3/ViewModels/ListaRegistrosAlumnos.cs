using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Xamarin.Forms;
using Ejercicio2Parcial3.Models;
using Ejercicio2Parcial3.Views;
using SQLite;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Ejercicio2Parcial3.ViewModels
{
    public class ListaRegistrosAlumnosViewModel
    {
        public ObservableCollection<Alumnos> AlumnosCollection { get; set; }
        public ICommand SalirCommand { get; set; }
        public ICommand IrANuevoRegistroCommand { get; set; }
        public ICommand EliminarAlumnoCommand { get; set; }
        public ICommand ActualizarRegistroCommand { get; set; }

        private Alumnos _alumnoSeleccionado;

        public ListaRegistrosAlumnosViewModel()
        {
            AlumnosCollection = new ObservableCollection<Alumnos>();
            SalirCommand = new Command(Salir);
            IrANuevoRegistroCommand = new Command(IrANuevoRegistro);
            EliminarAlumnoCommand = new Command(async () => await EliminarAlumnoAsync());
            ActualizarRegistroCommand= new Command(async () => await ActualizarRegistroAsync());

        }

        public Alumnos AlumnoSeleccionado
        {
            get { return _alumnoSeleccionado; }
            set
            {
                _alumnoSeleccionado = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        private async Task EliminarAlumnoAsync()
        {

            if (AlumnoSeleccionado == null)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "No ha seleccionado ningún registro para eliminar", "OK");
                return;
            }

            if (AlumnoSeleccionado != null)
            {
                bool respuesta = await Application.Current.MainPage.DisplayAlert("Confirmar", "¿Está seguro que desea eliminar este registro?", "Sí", "No");

                if (respuesta)
                {
                    using (SQLiteConnection connection = new SQLiteConnection(App.DatabasePath))
                    {
                        connection.CreateTable<Alumnos>();
                        connection.Delete(AlumnoSeleccionado);
                    }

                    AlumnosCollection.Remove(AlumnoSeleccionado);
                    AlumnoSeleccionado = null;
                }
            }
        }


        private async Task ActualizarRegistroAsync()
        {

            if (AlumnoSeleccionado == null)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "No ha seleccionado ningún registro para actualizar", "OK");
                return;
            }
            if (AlumnoSeleccionado != null)
            {
                int alumnoId = AlumnoSeleccionado.Id;
                ActualizarRegistroAlumnoViewModel viewModel = new ActualizarRegistroAlumnoViewModel();
                viewModel.AlumnoSeleccionado = AlumnoSeleccionado;

                ActualizarRegistroAlumno actualizarRegistroPage = new ActualizarRegistroAlumno();
                actualizarRegistroPage.BindingContext = viewModel;

                await Application.Current.MainPage.Navigation.PushAsync(actualizarRegistroPage);
            }
        }





        public void CargarAlumnos()
        {
            AlumnosCollection.Clear();
            using (SQLiteConnection connection = new SQLiteConnection(App.DatabasePath))
            {
                connection.CreateTable<Alumnos>();
                var alumnosFromDatabase = connection.Table<Alumnos>().ToList();

                foreach (var alumno in alumnosFromDatabase)
                {
                    if (!string.IsNullOrEmpty(alumno.Imagen))
                    {
                        // Convertir la cadena de Base64 a bytes
                        byte[] imageBytes = Convert.FromBase64String(alumno.Imagen);
                        // Crear un ImageSource desde los bytes de la imagen
                        alumno.Foto = ImageSource.FromStream(() => new MemoryStream(imageBytes));
                    }
                    AlumnosCollection.Add(alumno);
                }
            }
        }

        private async void IrANuevoRegistro()
        {
            await Application.Current.MainPage.Navigation.PushAsync(new RegistrosAlumnos());
        }

        private async void Salir()
        {
            bool respuesta = await Application.Current.MainPage.DisplayAlert("Confirmar", "¿Está seguro que desea salir?", "Sí", "No");

            if (respuesta)
            {
                System.Environment.Exit(0);
            }
        }
    }
}
