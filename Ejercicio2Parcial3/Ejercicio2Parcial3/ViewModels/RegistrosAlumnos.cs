using System;
using System.IO;
using System.Windows.Input;
using System.ComponentModel;
using Ejercicio2Parcial3.Models;
using Plugin.Media;
using Plugin.Media.Abstractions;
using SQLite;
using Xamarin.Essentials;
using Xamarin.Forms;

using static Xamarin.Essentials.Permissions;
using Ejercicio2Parcial3.Views;
using Rg.Plugins.Popup.Services;

namespace Ejercicio2Parcial3.ViewModels
{
    public class RegistrosAlumnosViewModel : INotifyPropertyChanged
    {
        private MediaFile _photo;
        private ImageSource _foto;

        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public int SexoIndex { get; set; }
        public string Direccion { get; set; }

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

        public ICommand TomarFotoCommand { get; set; }
        public ICommand GuardarCommand { get; set; }
        public ICommand CancelarCommand { get; set; }
        public ICommand VerImagenCommand { get; set; }



        public RegistrosAlumnosViewModel()
        {
            TomarFotoCommand = new Command(TomarFoto);
            GuardarCommand = new Command(Guardar);
            CancelarCommand = new Command(Cancelar);
            VerImagenCommand = new Command(VerImagen);

        }

        private async void TomarFoto()
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
                // Mostrar mensaje de que se necesita el permiso de la cámara
                // Puedes usar el plugin Toast para mostrar mensajes en Android
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

        private void Guardar()
        {
            // Validar que los campos obligatorios no estén vacíos
            if (string.IsNullOrEmpty(Nombres) || string.IsNullOrEmpty(Apellidos) || string.IsNullOrEmpty(Direccion) || Foto == null)
            {
                // Mostrar mensaje de error
                Application.Current.MainPage.DisplayAlert("Campos vacíos", "Todos los campos son obligatorios, asegúrese de llenarlos y tomar una foto.", "Aceptar");
                return;
            }

            var nuevoAlumno = new Alumnos
            {
                Nombres = Nombres,
                Apellidos = Apellidos,
                Sexo = (SexoIndex == 0) ? "Femenino" : "Masculino",
                Direccion = Direccion,
                Imagen = ConvertToBase64(_photo)
            };

            if (Foto != null) // Asegúrate de que tienes una foto tomada
            {
                nuevoAlumno.Foto = Foto; // Asigna la propiedad Foto del modelo Alumnos
            }

            using (SQLiteConnection connection = new SQLiteConnection(App.DatabasePath))
            {
                connection.CreateTable<Alumnos>();
                connection.Insert(nuevoAlumno);
            }

            Application.Current.MainPage.Navigation.PopAsync();
        }



        private async void Cancelar()
        {
            bool respuesta = await Application.Current.MainPage.DisplayAlert("Confirmar", "¿Está seguro que desea limpiar los campos?", "Sí", "No");

            if (respuesta)
            {
                Nombres = string.Empty;
                Apellidos = string.Empty;
                SexoIndex = 0;
                Direccion = string.Empty;
                _photo = null; // Limpiar la foto
                Foto = null;   // Asignar Foto a null para actualizar la vista

                OnPropertyChanged(nameof(Nombres));
                OnPropertyChanged(nameof(Apellidos));
                OnPropertyChanged(nameof(SexoIndex));
                OnPropertyChanged(nameof(Direccion));
            }
        }



        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
    }
}
