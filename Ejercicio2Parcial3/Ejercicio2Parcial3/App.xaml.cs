using Ejercicio2Parcial3.Views;
using System;
using System.IO;
using Xamarin.Forms;

namespace Ejercicio2Parcial3
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new ListaRegistrosAlumnos());
        }

        public static string DatabasePath
        {
            get
            {
                string basePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                return Path.Combine(basePath, "AlumnosDatabase.db3");
            }
        }
    }
}
