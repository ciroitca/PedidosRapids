using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;


namespace PedidosRapids.Vista
{
    
    public partial class login : Window
    {
        public login()
        {
            InitializeComponent();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void btnMinimizar_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void btnCerrar_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("¿Está seguro que desea salir?", "SALIR", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                this.Close();
            }
          
        }

        private void IniciarSesion_Click(object sender, RoutedEventArgs e)
        {
            // lógica de validación de usuario
            string usuario = txtUsuario.Text;
            string contraseña = txtContra.Password;

            // validación 
            if (usuario == "admin" && contraseña == "1234") // Usuario administrador
            {
                Main main = new Main();
                main.Show();
                this.Close();
            }
            /*
             * 
             * else if (usuario == "empleado" && contraseña == "1234") // Usuario empleado
            {
                Empleado empleado = new Empleado();
                empleado.Show();
                this.Close();
            }
            */
            else
            {
                MessageBox.Show("Usuario o contraseña incorrecta.");
            }
        }
    }
}


