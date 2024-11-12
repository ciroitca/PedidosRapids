using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Runtime;
using System.Data.SqlClient;
using System.Data;


/*************** Falta un botón de cerrar sesión ***************************/


namespace PedidosRapids.Vista
{
    public partial class Main : Window
    {
        private List<Categoria> datos;
        private List<Ordenes> datosOrdenes;
        private List<Mesas> datosMesa;
        public Main()
        {
            InitializeComponent();
            CargarCategorias(); // Cargar las categorías al iniciar
            CargarOrdenes();// Cargar las ordenes al iniciar
            CargarMesas();
            MostrarValorEnTextBox();
            grdPlatos.ItemsSource = datos; // Enlazar los datos al DataGrid
            grdOrdenes.ItemsSource = datosOrdenes;// Enlazar los datos al DataGrid
            grdMesas.ItemsSource = datosMesa;
        }

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, int wMsg, int wParam, int lParam);
        private void pnlControlBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            WindowInteropHelper helper = new WindowInteropHelper(this);
            SendMessage(helper.Handle, 161, 2, 0);
        }
        private void pnlControlBar_MouseEnter(object sender, MouseEventArgs e)
        {
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        private void btnMaximize_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Normal)
                this.WindowState = WindowState.Maximized;
            else this.WindowState = WindowState.Normal;
        }
        //Boton salir, para cerrar sesion y volver al login
        private void btnSalir_Checked(object sender, RoutedEventArgs e)
        {
            login login = new login();
            MessageBoxResult result = MessageBox.Show("Esta seguro que desea cerrar sesión?","CERRAR SESION",MessageBoxButton.YesNo,MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                this.Close();
                login.Show();
            }
            else { 
                btnSalir.IsChecked = false;
            }
        }
        //muestra las opciones de platos
        private void btnPlatos_Checked(object sender, RoutedEventArgs e)
        {
            OcultarParaPlatos();
            lblPlatos.Visibility = Visibility.Visible;
            grdPlatos.Visibility = Visibility.Visible;
            btnAgregarPlatos.Visibility = Visibility.Visible;
            btnAgOrden.IsChecked = false;
        }

        private void btnMainMenu_Checked(object sender, RoutedEventArgs e)
        {
            login login = new login();
            MessageBoxResult result = MessageBox.Show("Desea volver al menu Principal", "Volver", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                MostrarMenu();
                btnMainMenu.Visibility = Visibility.Hidden;
                btnAgOrden.IsChecked= false;
                btnAgregarPlatos.IsChecked= false;
            }
            else
            {
               btnMainMenu.IsChecked = false;
            }
        }
        private void btnOrdenes_Checked(object sender, RoutedEventArgs e)
        {
            OcultarParaOrdenes();
            lblOrden.Visibility = Visibility.Visible;
            grdOrdenes.Visibility = Visibility.Visible;
            btnAgOrden.Visibility = Visibility.Visible;
            btnAgregarPlatos.IsChecked = false;
        }

        private void btnMesas_Checked(object sender, RoutedEventArgs e)
        {
            OcultarParaMesas();
            lblMesas.Visibility = Visibility.Visible;
            grdMesas.Visibility = Visibility.Visible;
        }



        private void btnAgOrden_Click(object sender, RoutedEventArgs e)
        {
            OcultarParaAgOrden();
            btnMainMenu.Visibility = Visibility.Visible;
        }

        //**********************Funciones para la navegacion de menus******************

        public void CargarMesas()
        {
            datosMesa = new List<Mesas>();
            string connectionString = "Data Source=tcp:sqlproyecto2024.database.windows.net,1433;Initial Catalog=sqlproyecto;User ID=proyecto24;Password=Proyecto-24";
            string storedProcedureName = "Listar_Ordenes";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(storedProcedureName, connection);
                command.CommandType = CommandType.StoredProcedure;

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        datosMesa.Add(new Mesas
                        {
                            Mesa = reader["Mesa"].ToString()
                            // Agrega más propiedades si es necesario
                        });
                    }

                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }

            }
        }

        public void CargarCategorias()
        {
            datos = new List<Categoria>();
            string connectionString = "Data Source=tcp:sqlproyecto2024.database.windows.net,1433;Initial Catalog=sqlproyecto;User ID=proyecto24;Password=Proyecto-24";
            string storedProcedureName = "Listar_Categoria";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(storedProcedureName, connection);
                command.CommandType = CommandType.StoredProcedure;

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        datos.Add(new Categoria
                        {
                            NombreCategoria = reader["NombreCategoria"].ToString()
                            // Agrega más propiedades si es necesario
                        });
                    }

                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }

            }
        }

        public void CargarOrdenes()
        {
            datosOrdenes = new List<Ordenes>();
            string connectionString = "Data Source=tcp:sqlproyecto2024.database.windows.net,1433;Initial Catalog=sqlproyecto;User ID=proyecto24;Password=Proyecto-24";
            string storedProcedureName = "Listar_Ordenes";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(storedProcedureName, connection);
                command.CommandType = CommandType.StoredProcedure;

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        datosOrdenes.Add(new Ordenes
                        {
                            Orden = reader["NombreCategoria"].ToString()
                            // Agrega más propiedades si es necesario
                        });
                    }

                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }

            }
        }


        private void MostrarValorEnTextBox()
        {
            string connectionString = "Data Source=tcp:sqlproyecto2024.database.windows.net,1433;Initial Catalog=sqlproyecto;User ID=proyecto24;Password=Proyecto-24";
            string query = "SELECT Usuario FROM Usuario WHERE Id_Usuario = 1"; // Ajusta el query según tus necesidades

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        // Asume que el TextBox se llama txtUsuario
                        txtUserEd.Text = reader["Usuario"].ToString();
                    }

                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }


        private void mostrarMenuAdmin()
        {
            lblAdmin.Visibility = Visibility.Visible;
            btnAdminBebida.Visibility = Visibility.Visible;
            btnAdminEm.Visibility = Visibility.Visible;
            btnAdminUsers.Visibility = Visibility.Visible;
            btnMainMenu.Visibility = Visibility.Visible;
            OcultarParaAgOrden();
            OcultarParaMesas();
            OcultarParaPlatos();
            OcultarParaOrdenes();
        }
        private void MostrarEditUser()
        {
            btnMainMenu.Visibility = Visibility.Visible;
            lblNPassEd.Visibility = Visibility.Visible;
            lblNUserEdit.Visibility = Visibility.Visible;
            lblEdUser.Visibility = Visibility.Visible;
            lblNUserName.Visibility = Visibility.Visible;
            txtUserEd.Visibility = Visibility.Visible;
            txtNUserName.Visibility = Visibility.Visible;
            txtNPass.Visibility = Visibility.Visible;
            btnEditUser.Visibility = Visibility.Visible;
            OcultarParaAgOrden();
            OcultarParaMesas();
            OcultarParaPlatos();
            OcultarParaOrdenes();
        }

        //Funcion para ocultar todo excepto lo que se debe mostrar para ordenes
        private void OcultarParaPlatos()
        {
            lblOrden.Visibility = Visibility.Hidden;
            grdOrdenes.Visibility = Visibility.Hidden;
            btnAgOrden.Visibility = Visibility.Hidden;
            lblMesas.Visibility = Visibility.Hidden;
            grdMesas.Visibility = Visibility.Hidden;
        }
        //Funcion para ocultar todo excepto lo que se debe mostrar para Platos
        private void OcultarParaOrdenes()
        {
            lblMesas.Visibility = Visibility.Hidden;
            grdMesas.Visibility = Visibility.Hidden;
            lblPlatos.Visibility = Visibility.Hidden;
            grdPlatos.Visibility = Visibility.Hidden;
            btnAgregarPlatos.Visibility = Visibility.Hidden;
        }

        private void OcultarParaMesas()
        {
            lblOrden.Visibility = Visibility.Hidden;
            grdOrdenes.Visibility = Visibility.Hidden;
            btnAgOrden.Visibility = Visibility.Hidden;
            lblPlatos.Visibility = Visibility.Hidden;
            grdPlatos.Visibility = Visibility.Hidden;
            btnAgregarPlatos.Visibility = Visibility.Hidden;
        }

        private void MostrarMenu()
        {
            lblOrden.Visibility = Visibility.Hidden;
            grdOrdenes.Visibility = Visibility.Hidden;
            btnAgOrden.Visibility = Visibility.Hidden;
            lblPlatos.Visibility = Visibility.Hidden;
            grdPlatos.Visibility = Visibility.Hidden;
            btnAgregarPlatos.Visibility = Visibility.Hidden;
            lblMesas.Visibility = Visibility.Hidden;
            grdMesas.Visibility = Visibility.Hidden;
            btnAgOrden.Visibility = Visibility.Visible;
            btnCambiarUsuario.Visibility = Visibility.Visible;
            btnMesas.Visibility = Visibility.Visible;
            btnPlatos.Visibility = Visibility.Visible;
            btnSalir.Visibility = Visibility.Visible;
            btnUser.Visibility = Visibility.Visible;
            btnOrdenes.Visibility = Visibility.Visible;
            btnAgOrden.Visibility = Visibility.Hidden;
            btnMainMenu.IsChecked = false;
            btnMainMenu.Visibility = Visibility.Hidden;
            lblNPassEd.Visibility = Visibility.Hidden;
            lblNUserEdit.Visibility = Visibility.Hidden;
            lblEdUser.Visibility = Visibility.Hidden;
            lblNUserName.Visibility = Visibility.Hidden;
            txtUserEd.Visibility = Visibility.Hidden;
            txtNUserName.Visibility = Visibility.Hidden;
            txtNPass.Visibility = Visibility.Hidden;
            lblAdmin.Visibility = Visibility.Hidden;
            btnAdminBebida.Visibility = Visibility.Hidden;
            btnAdminEm.Visibility = Visibility.Hidden;
            btnAdminUsers.Visibility = Visibility.Hidden;
            btnMainMenu.Visibility = Visibility.Hidden;
            btnEditUser.Visibility = Visibility.Hidden;
        }

        private void OcultarParaAgOrden()
        {
            lblOrden.Visibility = Visibility.Hidden;
            grdOrdenes.Visibility = Visibility.Hidden;
            btnAgOrden.Visibility = Visibility.Hidden;
            lblPlatos.Visibility = Visibility.Hidden;
            grdPlatos.Visibility = Visibility.Hidden;
            btnAgregarPlatos.Visibility = Visibility.Hidden;
            lblMesas.Visibility = Visibility.Hidden;
            grdMesas.Visibility = Visibility.Hidden;
            btnAgOrden.Visibility = Visibility.Hidden;
            btnCambiarUsuario.Visibility = Visibility.Hidden;
            btnMesas.Visibility = Visibility.Hidden;
            btnPlatos.Visibility = Visibility.Hidden;
            btnSalir.Visibility = Visibility.Hidden;
            btnUser.Visibility = Visibility.Hidden;
            btnOrdenes.Visibility = Visibility.Hidden;
        }

        public class Categoria
        {
            public string NombreCategoria { get; set; }
            public string NombrePlatillo { get; set; }
            public string TiempoPreparacion { get; set; }
            public string Descripcion { get; set; }

        }

        public class Ordenes
        {
            public string Orden { get; set; }
            public string Mesa { get; set; }
            public string Estado { get; set; }
            public string hora_Pedido { get; set; }
            public string total { get; set; }
        }

        public class Mesas
        {
            public string Mesa { get; set; }
        }

        //
        //
        //

        /* ACTIONES DE BOTONES DEL GRID */

        //
        //
        //

        //  BOTON AGREGAR -- VENTANA: PLATOS

        private void btnAgregarPlatos_click(object sender, RoutedEventArgs e)
        {
           


        }

        private void btnUser_Checked(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Desea editar este usuario?","Editar", MessageBoxButton.YesNo,MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                MostrarEditUser();
                btnUser.IsChecked = false;
            }
            else
            {
                btnUser.IsChecked = false;
                mostrarMenuAdmin();
            }
        }

        private void btnEditUser_Checked(object sender, RoutedEventArgs e)
        {
            MostrarEditUser();
            btnUser.IsChecked = false;

            string connectionString = "Data Source=tcp:sqlproyecto2024.database.windows.net,1433;Initial Catalog=sqlproyecto;User ID=proyecto24;Password=Proyecto-24";
            string query = "UPDATE Usuario SET Usuario = @Usuario, Password = @Password WHERE Id_Usuario = 1";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Usuario", txtNUserName.Text);
                command.Parameters.AddWithValue("@Password", txtNPass.Text);

                try
                {
                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Usuario Actualizado Corectamente");
                    }
                    else
                    {
                        MessageBox.Show("No se encontró el Usuario con el ID especificado.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }

            }
        }
    }

}


