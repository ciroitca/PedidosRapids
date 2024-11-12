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
        private List<Bebidas> datosBebidas;
        public Main()
        {
            InitializeComponent();
            CargarCategorias(); // Cargar las categorías al iniciar
            CargarOrdenes();// Cargar las ordenes al iniciar
            CargarMesas();
            CargarBebidas();
            MostrarValorEnTextBox();
            grdPlatos1.ItemsSource = datos; // Enlazar los datos al DataGrid
            grdOrdenes1.ItemsSource = datosOrdenes;// Enlazar los datos al DataGrid
            grdMesas1.ItemsSource = datosMesa;
            grdBebidas1.ItemsSource = datosBebidas;
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
            lblPlatos1.Visibility = Visibility.Visible;
            grdPlatos1.Visibility = Visibility.Visible;
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
            lblOrden1.Visibility = Visibility.Visible;
            grdOrdenes1.Visibility = Visibility.Visible;
            btnAgOrden.Visibility = Visibility.Visible;
            btnAgregarPlatos.IsChecked = false;
        }

        private void btnMesas_Checked(object sender, RoutedEventArgs e)
        {
            OcultarParaMesas();
            lblMesas1.Visibility = Visibility.Visible;
            grdMesas1.Visibility = Visibility.Visible;
        }



        private void btnAgOrden_Click(object sender, RoutedEventArgs e)
        {
            OcultarParaAgOrden();
            btnMainMenu.Visibility = Visibility.Visible;
        }

        private void btnUser_Checked(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Desea editar este usuario?", "Editar", MessageBoxButton.YesNo, MessageBoxImage.Question);
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

        private void btnAggBebidaABD_Checked(object sender, RoutedEventArgs e)
        {
            string connectionString = "Data Source=tcp:sqlproyecto2024.database.windows.net,1433;Initial Catalog=sqlproyecto;User ID=proyecto24;Password=Proyecto-24";
            string storedProcedure = "Agregar_Bebida";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(storedProcedure, connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@Id_Producto", int.Parse(txtId_Bebida.Text));
                command.Parameters.AddWithValue("@NombreBebida", txtNameBebida1.Text);
                command.Parameters.AddWithValue("@Alcoholica", rdSiBebida1.IsChecked == true ? "True" : "False");
                command.Parameters.AddWithValue("@Precio", float.Parse(txtPriceBebida.Text));
                command.Parameters.AddWithValue("@Cantidad", int.Parse(txtCantBebida.Text));
                

                try
                {
                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Bebidas Agregadas con exito");
                    }
                    else
                    {
                        MessageBox.Show("No se pudo agregar la bebida.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }

            btnAggBebidaABD.IsChecked = false;
        }

        private void btnAggBebida1_Checked(object sender, RoutedEventArgs e)
        {
            MostrarAggBebida();
        }

        //**********************Funciones para la navegacion de menus******************

        private void btnAdminBebida_Checked(object sender, RoutedEventArgs e)
        {
            MostrarAdBebida();
        }

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

        private void CargarBebidas()
        {
            datosBebidas = new List<Bebidas>();
            string connectionString = "Data Source=tcp:sqlproyecto2024.database.windows.net,1433;Initial Catalog=sqlproyecto;User ID=proyecto24;Password=Proyecto-24";
            string storedProcedureName = "Listar_Bebidas";

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
                        datosBebidas.Add(new Bebidas
                        {
                            NombreBebidas = reader["NombreBebidas"].ToString()
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
                        txtUserEd1.Text = reader["Usuario"].ToString();
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
            lblAdmin1.Visibility = Visibility.Visible;
            btnAdminBebida.Visibility = Visibility.Visible;
            btnAdminEm.Visibility = Visibility.Visible;
            btnAdminUsers.Visibility = Visibility.Visible;
            btnMainMenu.Visibility = Visibility.Visible;
            lblAdmin1.Visibility = Visibility.Hidden;
            OcultarParaAgOrden();
            OcultarParaMesas();
            OcultarParaPlatos();
            OcultarParaOrdenes();
        }
        private void MostrarEditUser()
        {
            btnMainMenu.Visibility = Visibility.Visible;
            lblNPassEd1.Visibility = Visibility.Visible;
            lblNUserEdit1.Visibility = Visibility.Visible;
            lblEdUser1.Visibility = Visibility.Visible;
            lblNUserName1.Visibility = Visibility.Visible;
            txtUserEd1.Visibility = Visibility.Visible;
            txtNUserName1.Visibility = Visibility.Visible;
            txtNPass1.Visibility = Visibility.Visible;
            btnEditUser.Visibility = Visibility.Visible;
            OcultarParaAgOrden();
            OcultarParaMesas();
            OcultarParaPlatos();
            OcultarParaOrdenes();
        }

        //Funcion para ocultar todo excepto lo que se debe mostrar para ordenes
        private void OcultarParaPlatos()
        {
            lblOrden1.Visibility = Visibility.Hidden;
            grdOrdenes1.Visibility = Visibility.Hidden;
            btnAgOrden.Visibility = Visibility.Hidden;
            lblMesas1.Visibility = Visibility.Hidden;
            grdMesas1.Visibility = Visibility.Hidden;
        }
        //Funcion para ocultar todo excepto lo que se debe mostrar para Platos
        private void OcultarParaOrdenes()
        {
            lblMesas1.Visibility = Visibility.Hidden;
            grdMesas1.Visibility = Visibility.Hidden;
            lblPlatos1.Visibility = Visibility.Hidden;
            grdPlatos1.Visibility = Visibility.Hidden;
            btnAgregarPlatos.Visibility = Visibility.Hidden;
        }

        private void OcultarParaMesas()
        {
            lblOrden1.Visibility = Visibility.Hidden;
            grdOrdenes1.Visibility = Visibility.Hidden;
            btnAgOrden.Visibility = Visibility.Hidden;
            lblPlatos1.Visibility = Visibility.Hidden;
            grdPlatos1.Visibility = Visibility.Hidden;
            btnAgregarPlatos.Visibility = Visibility.Hidden;
        }

        private void MostrarMenu()
        {
            lblOrden1.Visibility = Visibility.Hidden;
            grdOrdenes1.Visibility = Visibility.Hidden;
            btnAgOrden.Visibility = Visibility.Hidden;
            lblPlatos1.Visibility = Visibility.Hidden;
            grdPlatos1.Visibility = Visibility.Hidden;
            btnAgregarPlatos.Visibility = Visibility.Hidden;
            lblMesas1.Visibility = Visibility.Hidden;
            grdMesas1.Visibility = Visibility.Hidden;
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
            lblNPassEd1.Visibility = Visibility.Hidden;
            lblNUserEdit1.Visibility = Visibility.Hidden;
            lblEdUser1.Visibility = Visibility.Hidden;
            lblNUserName1.Visibility = Visibility.Hidden;
            txtUserEd1.Visibility = Visibility.Hidden;
            txtNUserName1.Visibility = Visibility.Hidden;
            txtNPass1.Visibility = Visibility.Hidden;
            lblAdmin1.Visibility = Visibility.Hidden;
            btnAdminBebida.Visibility = Visibility.Hidden;
            btnAdminEm.Visibility = Visibility.Hidden;
            btnAdminUsers.Visibility = Visibility.Hidden;
            btnMainMenu.Visibility = Visibility.Hidden;
            btnEditUser.Visibility = Visibility.Hidden;
            lblAdminBebidas1.Visibility = Visibility.Hidden;
            grdBebidas1.Visibility = Visibility.Hidden;
            btnEliminarBebida.Visibility = Visibility.Hidden;
            btnEditBebida.Visibility = Visibility.Hidden;
            btnAggBebida.Visibility = Visibility.Hidden;
            lblAggBebida1.Visibility = Visibility.Hidden;
            lblNombreBebida.Visibility = Visibility.Hidden;
            txtNameBebida1.Visibility = Visibility.Hidden;
            lblAlcoholicaB.Visibility = Visibility.Hidden;
            rdSiBebida1.Visibility = Visibility.Hidden;
            rdNoBebida1.Visibility = Visibility.Hidden;
            lblPrecioBebida.Visibility = Visibility.Hidden;
            txtPriceBebida.Visibility = Visibility.Hidden;
            lblCantidadBebida.Visibility = Visibility.Hidden;
            txtCantBebida.Visibility = Visibility.Hidden;
            btnAggBebidaABD.Visibility = Visibility.Hidden;
            lblId_Bebida.Visibility = Visibility.Hidden;
            txtId_Bebida.Visibility = Visibility.Hidden;
        }

        private void OcultarMenu()
        {  StPanel.Visibility = Visibility.Hidden; }

        private void OcultarParaAgOrden()
        {
            lblOrden1.Visibility = Visibility.Hidden;
            grdOrdenes1.Visibility = Visibility.Hidden;
            btnAgOrden.Visibility = Visibility.Hidden;
            lblPlatos1.Visibility = Visibility.Hidden;
            grdPlatos1.Visibility = Visibility.Hidden;
            btnAgregarPlatos.Visibility = Visibility.Hidden;
            lblMesas1.Visibility = Visibility.Hidden;
            grdMesas1.Visibility = Visibility.Hidden;
            btnAgOrden.Visibility = Visibility.Hidden;
            btnCambiarUsuario.Visibility = Visibility.Hidden;
            btnMesas.Visibility = Visibility.Hidden;
            btnPlatos.Visibility = Visibility.Hidden;
            btnSalir.Visibility = Visibility.Hidden;
            btnUser.Visibility = Visibility.Hidden;
            btnOrdenes.Visibility = Visibility.Hidden;
        }

        private void MostrarAdBebida()
        {
            lblAdminBebidas1.Visibility = Visibility.Visible;
            grdBebidas1.Visibility = Visibility.Visible;
            btnEliminarBebida.Visibility = Visibility.Visible;
            btnEditBebida.Visibility = Visibility.Visible;
            btnAggBebida.Visibility= Visibility.Visible;
            OcultarParaOrdenes();
        }

        private void MostrarAggBebida()
        {
            lblAggBebida1.Visibility = Visibility.Visible;
            lblNombreBebida.Visibility = Visibility.Visible;
            txtNameBebida1.Visibility = Visibility.Visible;
            lblAlcoholicaB.Visibility = Visibility.Visible;
            rdSiBebida1.Visibility = Visibility.Visible;
            rdNoBebida1.Visibility = Visibility.Visible;
            lblPrecioBebida.Visibility = Visibility.Visible;
            txtPriceBebida.Visibility = Visibility.Visible;
            lblCantidadBebida.Visibility = Visibility.Visible;
            txtCantBebida.Visibility = Visibility.Visible;
            btnAggBebida.Visibility= Visibility.Hidden;
        }

        private void AgregarBebida()
        {
            lblAggBebida1.Visibility = Visibility.Visible;
            lblNombreBebida.Visibility = Visibility.Visible;
            txtNameBebida1.Visibility = Visibility.Visible;
            lblAlcoholicaB.Visibility = Visibility.Visible;
            rdSiBebida1.Visibility = Visibility.Visible;
            rdNoBebida1.Visibility = Visibility.Visible;
            lblPrecioBebida.Visibility = Visibility.Visible;
            txtPriceBebida.Visibility = Visibility.Visible;
            lblCantidadBebida.Visibility = Visibility.Visible;
            txtCantBebida.Visibility = Visibility.Visible;
            lblAdminBebidas1.Visibility = Visibility.Hidden;
            grdBebidas1.Visibility = Visibility.Hidden;
            btnEliminarBebida.Visibility = Visibility.Hidden;
            btnEditBebida.Visibility = Visibility.Hidden;
            btnAggBebida.Visibility = Visibility.Hidden;
            btnAdminEm.Visibility = Visibility.Hidden;
            btnAdminUsers.Visibility = Visibility.Hidden;
            btnAdminBebida.Visibility= Visibility.Hidden;
            btnAggBebidaABD.Visibility = Visibility.Visible;
            lblId_Bebida.Visibility = Visibility.Visible;
            txtId_Bebida.Visibility = Visibility.Visible;
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

        public class Bebidas
        {
            public string NombreBebidas { get; set; }
            public string Alcoholica { get; set; }
            public string Id_Producto { get; set; }

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
            OcultarParaPlatos();
            OcultarMenu();
            grdPlatos1.Visibility = Visibility.Hidden;
            btnAgregarPlatos.Visibility = Visibility.Hidden;
            btnVolverPlatos.Visibility = Visibility.Visible;
            lblCategoria.Visibility = Visibility.Visible;
            txtCategoria.Visibility = Visibility.Visible;
            lblDescripcion.Visibility = Visibility.Visible;
            txtDescripcion.Visibility = Visibility.Visible;
            lblPlatillo.Visibility = Visibility.Visible;
            txtPlatillo.Visibility = Visibility.Visible;
            lblTiempo.Visibility = Visibility.Visible;
            txtTiempo.Visibility = Visibility.Visible;
            btnInsertarPlatos.Visibility = Visibility.Visible;


        }

        private void btnVolPlatos_Click(object sender, RoutedEventArgs e)
        {
            OcultarParaPlatos();
            lblPlatos1.Visibility = Visibility.Visible;
            grdPlatos1.Visibility = Visibility.Visible;
            btnAgregarPlatos.Visibility = Visibility.Visible;
            btnVolverPlatos.Visibility = Visibility.Hidden;
            btnAgOrden.IsChecked = false;
            OcultarMenu();
            StPanel.Visibility = Visibility.Visible;
            lblCategoria.Visibility = Visibility.Hidden;
            txtCategoria.Visibility = Visibility.Hidden;
            lblDescripcion.Visibility = Visibility.Hidden;
            txtDescripcion.Visibility = Visibility.Hidden;
            lblPlatillo.Visibility = Visibility.Hidden;
            txtPlatillo.Visibility = Visibility.Hidden;
            lblTiempo.Visibility = Visibility.Hidden;
            txtTiempo.Visibility = Visibility.Hidden;  
            btnInsertarPlatos.Visibility =(Visibility) Visibility.Hidden;
            

        }

        private void btnInsertarPlatos_click(object sender, RoutedEventArgs e)
        { 
            //BOTON INSERTAR PLATOS ACÁ, AGREGAR LA LOGICA        
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
                command.Parameters.AddWithValue("@Usuario", txtNUserName1.Text);
                command.Parameters.AddWithValue("@Password", txtNPass1.Text);

                try
                {
                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Usuario Actualizado Corectamente");
                        txtNUserName1.Text = "";
                        txtNPass1.Text = "";
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

        private void btnAggBebida_Checked(object sender, RoutedEventArgs e)
        {
            AgregarBebida();
        }
    }

}


