using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Xamarin.Forms;
using Ejercicio2Parcial3.Models;
using SQLite;
using System.Linq;
using System.Collections.Generic;
using Ejercicio2Parcial3.Views;
using System.Threading.Tasks;
using Plugin.Media.Abstractions;
using static Xamarin.Essentials.Permissions;
using Plugin.Media;
using Xamarin.Essentials;
using Rg.Plugins.Popup.Services;

namespace Ejercicio2Parcial3.ViewModels
{
    public class ActualizarRegistroAlumnoViewModel : INotifyPropertyChanged
    {
        private Alumnos _alumnoSeleccionado;
        private MediaFile _photo;
        private ImageSource _foto;


        public ImageSource Foto
        {
            get { return _foto; }
            set
            {
                if (_foto != value)
                {
                    _foto = value;
                    OnPropertyChanged(nameof(Foto));
                }
            }
        }

        public string Nombres
        {
            get { return _nombres; }
            set
            {
                _nombres = value;
                OnPropertyChanged();
            }
        }
        private string _nombres;

        public string Apellidos
        {
            get { return _apellidos; }
            set
            {
                _apellidos = value;
                OnPropertyChanged();
            }
        }
        private string _apellidos;

        public string Sexo
        {
            get { return _sexo; }
            set
            {
                _sexo = value;
                OnPropertyChanged();
            }
        }
        private string _sexo;


        public string Direccion
        {
            get { return _direccion; }
            set
            {
                _direccion = value;
                OnPropertyChanged();
            }
        }
        private string _direccion;



        public ICommand TomarFotoCommand { get; set; }
        public ICommand GuardarCommand { get; set; }
        public ICommand CancelarCommand { get; set; }
        public ICommand VerImagenCommand { get; set; }


        public event PropertyChangedEventHandler PropertyChanged;

        public ActualizarRegistroAlumnoViewModel()
        {
            TomarFotoCommand = new Command(ActualizarFoto);
            GuardarCommand = new Command(Guardar);
            CancelarCommand = new Command(Cancelar);
            VerImagenCommand = new Command(VerImagen);



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

        public async void CargarDatosAlumno()
        {
            using (SQLiteConnection connection = new SQLiteConnection(App.DatabasePath))
            {
                connection.CreateTable<Alumnos>();

                // Usar el ID del AlumnoSeleccionado para buscar los detalles en la base de datos
                var alumnoFromDatabase = connection.Table<Alumnos>().FirstOrDefault(a => a.Id == AlumnoSeleccionado.Id);

                if (alumnoFromDatabase != null)
                {
                    // Convertir la cadena de Base64 a bytes
                    byte[] imageBytes = Convert.FromBase64String(alumnoFromDatabase.Imagen);
                    // Crear un ImageSource desde los bytes de la imagen
                    Foto = ImageSource.FromStream(() => new MemoryStream(imageBytes));

                    Nombres = alumnoFromDatabase.Nombres;
                    Apellidos = alumnoFromDatabase.Apellidos;
                    Sexo = alumnoFromDatabase.Sexo;
                    Direccion = alumnoFromDatabase.Direccion;
                }
            }
        }
        public async void ActualizarFoto()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.Camera>();
            }

            if (status == PermissionStatus.Granted)
            {
                if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
                {
                    return;
                }

                _photo = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                {
                    Directory = "Fotos",
                    Name = "miFoto.jpg"
                });

                if (_photo != null)
                {
                    Foto = ImageSource.FromStream(() => _photo.GetStream());
                }
            }
            else
            {
                
            }

        }

        private async void VerImagen()
        {

            if (Foto == null)
            {
                Application.Current.MainPage.DisplayAlert("Sin fotografía", "La foto aún no ha sido agregada, tome una y podrá verla.", "Aceptar");
                return;
            }


            await PopupNavigation.Instance.PushAsync(new VerImagenPopup(Foto));

        }

        public async void Guardar()
        {

            if (string.IsNullOrEmpty(Nombres) || string.IsNullOrEmpty(Apellidos) || string.IsNullOrEmpty(Direccion) || Foto == null)
            {
                // Mostrar mensaje de error
                Application.Current.MainPage.DisplayAlert("Campos vacíos", "Todos los campos son obligatorios, asegúrese de llenarlos y tomar una foto.", "Aceptar");
                return;
            }

            if (AlumnoSeleccionado != null)
            {
                // Obtén el alumno seleccionado de la base de datos
                using (SQLiteConnection connection = new SQLiteConnection(App.DatabasePath))
                {
                    connection.CreateTable<Alumnos>();
                    var alumnoFromDatabase = connection.Table<Alumnos>().FirstOrDefault(a => a.Id == AlumnoSeleccionado.Id);

                    if (alumnoFromDatabase != null)
                    {
                        // Actualiza los valores con los datos ingresados en la vista
                        alumnoFromDatabase.Nombres = Nombres;
                        alumnoFromDatabase.Apellidos = Apellidos;
                        alumnoFromDatabase.Sexo = Sexo;
                        alumnoFromDatabase.Direccion = Direccion;

                        // Si se ha tomado una nueva foto, convierte y guarda la imagen en Base64
                        if (_photo != null)
                        {
                            alumnoFromDatabase.Imagen = ConvertToBase64(_photo);
                        }

                        // Actualiza el registro en la base de datos
                        connection.Update(alumnoFromDatabase);
                    }
                }

                // Vuelve a la página anterior
                await Application.Current.MainPage.Navigation.PopAsync();
            }
        }


        


        public async void Cancelar()
        {
            await Application.Current.MainPage.Navigation.PopAsync();

        }

        private string ConvertToBase64(MediaFile photo)
        {
            if (photo != null)
            {
                using (MemoryStream memory = new MemoryStream())
                {
                    Stream stream = photo.GetStream();
                    stream.CopyTo(memory);
                    byte[] fotobyte = memory.ToArray();
                    return Convert.ToBase64String(fotobyte);
                }
            }
            return null;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
