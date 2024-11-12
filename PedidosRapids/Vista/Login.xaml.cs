using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
            // Obtenemos el usuario y contraseña ingresados
            string usuario = txtUsuario.Text;
            string contraseña = txtContra.Password;

         
            // Validamos las credenciales
            UsuarioInfo info = ValidarCredenciales(usuario, contraseña);

            if (info != null)

            {
                // Si las credenciales son válidas, verificamos el usuario específico
                if (info.NombreUsuario == usuario && info.ContrasenaUsuario == contraseña)
                {
                   Main admin = new Main();
                    admin.Show();
                    this.Close();
                }
            }

            else
            {
                MessageBox.Show("Credenciales no válidas", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private UsuarioInfo ValidarCredenciales(string usuario, string contraseña)
        {
            string conexionString = "Data Source=tcp:sqlproyecto2024.database.windows.net,1433;Initial Catalog=sqlproyecto;User ID=proyecto24;Password=Proyecto-24";
            UsuarioInfo usuarioInfo = null;

            using (SqlConnection conexion = new SqlConnection(conexionString))

            {
                try
                {
                    conexion.Open();
                    string consulta = "SELECT Usuario, Password FROM usuario WHERE Usuario = @usuario AND Password = @contrasena";
                    using (SqlCommand comando = new SqlCommand(consulta, conexion))
                    {
                        comando.Parameters.AddWithValue("@usuario", usuario);
                        comando.Parameters.AddWithValue("@contrasena", contraseña);

                        using (SqlDataReader reader = comando.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                usuarioInfo = new UsuarioInfo
                                {
                                    NombreUsuario = reader.GetString(0),
                                    ContrasenaUsuario = reader.GetString(1)
                                };
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error de conexión: " + ex.Message);
                }
            }

            return usuarioInfo;
        }

        public class UsuarioInfo
        {
            public string NombreUsuario { get; set; }
            public string ContrasenaUsuario { get; set; }
        }
    }
}


