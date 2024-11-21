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
using static PedidosRapids.Vista.Main;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using System.Text.RegularExpressions;


/*************** Falta un botón de cerrar sesión ***************************/


namespace PedidosRapids.Vista
{
    public partial class Main : Window
    {
        private List<Categoria> datos;
        private List<Ordenes> datosOrdenes;
        private List<Mesas> datosMesa;
        private List<Bebidas> datosBebidas;
        private List<User> datosUsuarios;
        public Main(string userRole)

        {
            InitializeComponent();

            // Configuración basada en el rol
            if (userRole == "Empleado")
            {
                // Oculta botones, menús u otros elementos exclusivos del administrador

                //btnUser.Visibility = Visibility.Collapsed;
                btnUser.Foreground = new SolidColorBrush(Colors.Gray);
                btnUser.IsEnabled = false;
                //adminButton.Visibility = Visibility.Collapsed; // 
                //adminMenu.IsEnabled = false;                   // 
            }
            else if (userRole == "Administrador")
            {
                // Opciones exclusivas del administrador (si las hay)
            }

            // Cargar datos iniciales
            CargarCategorias();
            CargarOrdenes();
            CargarMesas();
            CargarBebidas();
            CargarUsuarios();
            MostrarValorEnTextBox();
            CargarDatosBebidas();

            // Enlazar datos a DataGrids
            grdPlatos1.ItemsSource = datos;
            grdOrdenes1.ItemsSource = datosOrdenes;
            grdMesas1.ItemsSource = datosMesa;
            grdBebidas1.ItemsSource = datosBebidas;
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
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
           // Application.Current.Shutdown();
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
            OcultarFormAgregarBebida();
            OcultarParaPlatos();
            lblPlatos1.Visibility = Visibility.Visible;
            grdPlatos1.Visibility = Visibility.Visible;
            btnAgregarPlatos.Visibility = Visibility.Visible;
            btnAgOrden.IsChecked = false;
        }

        private void btnMainMenu_Checked(object sender, RoutedEventArgs e)
        {         
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
            OcultarFormAgregarBebida();
            OcultarParaPlatos();
            OcultarParaOrdenes();
            lblOrden1.Visibility = Visibility.Visible;
            grdOrdenes1.Visibility = Visibility.Visible;
            btnAgOrden.Visibility = Visibility.Visible;
            btnAgregarPlatos.IsChecked = false;
        }

        private void btnMesas_Checked(object sender, RoutedEventArgs e)
        {
            OcultarFormAgregarBebida();
            OcultarParaMesas();
            lblMesas1.Visibility = Visibility.Visible;
            grdMesas1.Visibility = Visibility.Visible;
            btnEliminarMesa.Visibility = Visibility.Visible;            
            btnAgregarMesa.Visibility = Visibility.Visible;
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

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void DecimalValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            // Permitir solo números y coma
            var textBox = (TextBox)sender;
            var fullText = textBox.Text.Insert(textBox.SelectionStart, e.Text);

            // Solo permite una coma
            if (e.Text == "," && textBox.Text.Contains(","))
            {
                e.Handled = true;
                return;
            }

            // Verifica que sea un número válido con coma
            e.Handled = !decimal.TryParse(fullText.Replace(',', '.'), out _);
        }

        private void txtPriceBebida_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Permite el uso de backspace y delete
            if (e.Key == Key.Back || e.Key == Key.Delete)
            {
                e.Handled = false;
            }
        }

        private void btnAggBebidaABD_Checked(object sender, RoutedEventArgs e)
        {
            // Validaciones para el ID y nombre...

            // Validación del precio
            if (!decimal.TryParse(txtPriceBebida.Text.Replace(',', '.'),
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture,
                out decimal precio) || precio <= 0)
            {
                MostrarError("El precio debe ser un número positivo.");
                btnAggBebidaABD.IsChecked = false;
                return;
            }

            // Validación de la cantidad
            if (!int.TryParse(txtCantBebida.Text, out int cantidad) || cantidad <= 0)
            {
                MostrarError("La cantidad debe ser un número entero positivo.");
                btnAggBebidaABD.IsChecked = false;
                return;
            }

            string connectionString = "Data Source=tcp:sqlproyecto2024.database.windows.net,1433;Initial Catalog=sqlproyecto;User ID=proyecto24;Password=Proyecto-24";
            string storedProcedure = "Agregar_Bebida";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(storedProcedure, connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@Id_Producto", txtIdBebida.Text);
                command.Parameters.AddWithValue("@NombreBebida", txtNameBebida1.Text.Trim());
                command.Parameters.AddWithValue("@Alcoholica", rdSiBebida1.IsChecked == true);
                command.Parameters.AddWithValue("@Precio", precio);
                command.Parameters.AddWithValue("@Cantidad", cantidad); // Ahora usamos la cantidad del TextBox

                try
                {
                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Bebida agregada con éxito", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                        LimpiarCampos();
                        CargarBebidas();
                    }
                    else
                    {
                        MostrarError("No se pudo agregar la bebida.");
                    }
                }
                catch (Exception ex)
                {
                    MostrarError($"Error al agregar la bebida: {ex.Message}");
                }
            }

            btnAggBebidaABD.IsChecked = false;
        }


        private void MostrarError(string mensaje)
        {
            MessageBox.Show(mensaje, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            btnAggBebidaABD.IsChecked = false;
        }

        private void LimpiarCampos()
        {
            txtId_Bebida.Text = string.Empty;
            txtNameBebida1.Text = string.Empty;
            txtPriceBebida.Text = string.Empty;
            txtCantBebida.Text = string.Empty;
            rdSiBebida1.IsChecked = false;
            rdNoBebida1.IsChecked = false;
        }

        private void btnAggUserBD_Checked(object sender, RoutedEventArgs e)
        {
            string connectionString = "Data Source=tcp:sqlproyecto2024.database.windows.net,1433;Initial Catalog=sqlproyecto;User ID=proyecto24;Password=Proyecto-24";
            string storedProcedure = "Agregar_Usuario";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(storedProcedure, connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@Id_Empleado", int.Parse(txtIdEmpleado.Text));
                command.Parameters.AddWithValue("@Usuario", txtAggUser.Text);
                command.Parameters.AddWithValue("Password", txtContrasenia.Text);

                try
                {
                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Usuario agregado con exito");
                    }
                    else
                    {
                        MessageBox.Show("No se pudo agregar el usuario.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
            btnAggUserBD.IsChecked = false;
        }

        private void btnAEliminarUser_Checked(object sender, RoutedEventArgs e)
        {
            // Verifica si hay un elemento seleccionado en el DataGrid
            if (grdAdUsers.SelectedItem is User selectedUser)
    {
        // Mostrar una confirmación al usuario
        var result = MessageBox.Show($"¿Estás seguro de que deseas eliminar al usuario '{selectedUser.Usuario}'?",
                                     "Confirmación",
                                     MessageBoxButton.YesNo,
                                     MessageBoxImage.Warning);

        if (result == MessageBoxResult.Yes)
        {
            // Ejecutar el procedimiento almacenado para eliminar al usuario
            string connectionString = "Data Source=tcp:sqlproyecto2024.database.windows.net,1433;Initial Catalog=sqlproyecto;User ID=proyecto24;Password=Proyecto-24";
            string storedProcedure = "Eliminar_Usuario";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(storedProcedure, connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Id_Usuario", selectedUser.Id_Empleado);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery(); // Ejecuta el procedimiento almacenado
                    MessageBox.Show("Usuario eliminado exitosamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Recarga los datos del DataGrid
                    CargarUsuarios();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al eliminar el usuario: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
    else
    {
        MessageBox.Show("Por favor, selecciona un usuario para eliminar.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
    }
            btnAEliminarUser.IsChecked = false;
    }
      

        private void btnAggBebida1_Checked(object sender, RoutedEventArgs e)
        {
            MostrarAggBebida();
            btnSalirMenuAggBebidas.Visibility = Visibility.Visible;

        }

        private void btnSalirAggBebidas_Checked(object sender, RoutedEventArgs e)
        {
            btnSalirMenuAggBebidas.IsChecked = false;
            OcultarParaBebidas();
            MostrarMenu();
            lblBebida1s.Visibility = Visibility.Visible;
            grdBebidas1.Visibility = Visibility.Visible;
            btnEditBebida.Visibility = Visibility.Visible;
            btnEliminarBebida.Visibility = Visibility.Visible;
            btnAggBebida.Visibility = Visibility.Visible;


        }


        //**********************Funciones para la navegacion de menus******************

        private void btnAdminBebida_Checked(object sender, RoutedEventArgs e)
        {
            btnAdminBebida.IsChecked = false;
            MostrarAdBebida();
            OcultarParaBebidas();
            btnAggBebida.Visibility = Visibility.Visible;
            lblBebida1s.Visibility = Visibility.Visible;
            grdPlatos1.Visibility = Visibility.Hidden;
              
        }

        private void btnAdminEm_Checked(object sender, RoutedEventArgs e)
        {
            MostrarAdEmpleado();
        }

        private void btnAdminUsers_Checked(object sender, RoutedEventArgs e)
        {
            MostrarAdUsers();
        }

        private void btnAggEmpleado_Checked(object sender, RoutedEventArgs e)
        {
            MostrarAggEmpleado();
        }

        private void btnEditEmpleado_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void btnAggUser_Checked(object sender, RoutedEventArgs e)
        {
            MostrarAggUser();
        }

        private void btnEliminarEmpleado_Checked(object sender, RoutedEventArgs e)
        {

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
                        var bebida = new Bebidas
                        {
                            Id_Producto = reader.GetInt32(reader.GetOrdinal("Id_Producto")),
                            Precio = Convert.ToDecimal(reader.GetValue(reader.GetOrdinal("Precio"))),
                            Existencia = reader.GetInt32(reader.GetOrdinal("Existencia")),
                            Bebida = reader.GetString(reader.GetOrdinal("Bebida")),
                            Alcoholica = reader.GetBoolean(reader.GetOrdinal("Alcoholica"))
                        };
                        datosBebidas.Add(bebida);
                    }

                    reader.Close();

                    // Actualizar el DataGrid
                    grdBebidas1.ItemsSource = null; // Limpiar el source actual
                    grdBebidas1.ItemsSource = datosBebidas;

                    // Configurar el formato de la columna de precio
                    var precioColumn = grdBebidas1.Columns.FirstOrDefault(c => c.Header.ToString() == "Precio");
                    if (precioColumn != null && precioColumn is DataGridTextColumn dtColumn)
                    {
                        (dtColumn.Binding as Binding).StringFormat = "{0:N2}";
                    }
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

        public void CargarUsuarios()
        {
            datosUsuarios = new List<User>();
            string connectionString = "Data Source=tcp:sqlproyecto2024.database.windows.net,1433;Initial Catalog=sqlproyecto;User ID=proyecto24;Password=Proyecto-24";
            string storedProcedure = "Listar_Usuarios";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(storedProcedure, connection);
                command.CommandType = CommandType.StoredProcedure;

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        datosUsuarios.Add(new User
                        {
                            Id_Empleado = Convert.ToInt32(reader["Id_Empleado"]),
                            Usuario = reader["Usuario"].ToString(),
                            Password = reader["Contrasena"].ToString()
                            // Agrega más propiedades si es necesario
                        });
                    }

                    reader.Close();
                    grdAdUsers.ItemsSource = datosUsuarios;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }

            }
        }

        private void CargarEmpleados()
        {

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
            btnAdminEm.Visibility = Visibility.Visible;
            btnAdminUsers.Visibility = Visibility.Visible;
            btnMainMenu.Visibility = Visibility.Visible;
            lblAdmin1.Visibility = Visibility.Hidden;
            OcultarParaAgOrden();
            OcultarParaMesas();
            OcultarParaPlatos();
            OcultarParaOrdenes();
            OcultarParaBebidas();
            OcultarFormEditBebidas();

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
            btnEditBebida.Visibility = Visibility.Hidden;
            OcultarParaAgOrden();
            OcultarParaMesas();
            OcultarParaPlatos();
            OcultarParaOrdenes();
            OcultarParaBebidas();
            OcultarFormEditBebidas();
        }

        //Funcion para ocultar todo excepto lo que se debe mostrar para ordenes
        private void OcultarParaPlatos()
        {
            lblOrden1.Visibility = Visibility.Hidden;
            grdOrdenes1.Visibility = Visibility.Hidden;
            btnAgOrden.Visibility = Visibility.Hidden;
            lblMesas1.Visibility = Visibility.Hidden;
            grdMesas1.Visibility = Visibility.Hidden;
            btnAggBebida.Visibility = Visibility.Hidden;
            btnEliminarBebida.Visibility = Visibility.Hidden;
            grdBebidas1.Visibility = Visibility.Hidden;
            lblBebida1s.Visibility = Visibility.Hidden;
            btnEditarBebida.Visibility = Visibility.Hidden;

        }

        private void OcultarParaBebidas()
        {
            
            btnAgregarPlatos.Visibility = Visibility.Hidden;
            lblOrden1.Visibility = Visibility.Hidden;
            grdOrdenes1.Visibility = Visibility.Hidden;
            btnAgOrden.Visibility = Visibility.Hidden;
            lblMesas1.Visibility = Visibility.Hidden;
            grdMesas1.Visibility = Visibility.Hidden;
            btnSalirMenuAggBebidas.Visibility = Visibility.Hidden;
            lblAggBebida1.Visibility = Visibility.Hidden;
            lblAdminBebidas1.Visibility = Visibility.Hidden;
            lblNombreBebida.Visibility = Visibility.Hidden;
            lblAlcoholicaB.Visibility = Visibility.Hidden;
            lblPrecioBebida.Visibility = Visibility.Hidden;
            txtNameBebida1.Visibility = Visibility.Hidden;
            rdSiBebida1.Visibility = Visibility.Hidden;
            rdNoBebida1.Visibility = Visibility.Hidden;
            txtPriceBebida.Visibility = Visibility.Hidden;
            lblCantidadBebida.Visibility = Visibility.Hidden;
            txtCantBebida.Visibility = Visibility.Hidden;
            lblId_Bebida.Visibility = Visibility.Hidden;
            txtId_Bebida.Visibility = Visibility.Hidden;
            lblOrden1.Visibility = Visibility.Hidden;
            lblPlatos1.Visibility = Visibility.Hidden;
            btnAggBebidaABD.Visibility = Visibility.Hidden;
            lblAdminBebidas1.Visibility = Visibility.Hidden;            
            OcultarFormEditBebidas();
         }


        //Funcion para ocultar todo excepto lo que se debe mostrar para Platos
        private void OcultarParaOrdenes()
        {
            btnEditarBebida.Visibility = Visibility.Hidden;
            lblMesas1.Visibility = Visibility.Hidden;
            grdMesas1.Visibility = Visibility.Hidden;
            lblPlatos1.Visibility = Visibility.Hidden;
            grdPlatos1.Visibility = Visibility.Hidden;
            btnAgregarPlatos.Visibility = Visibility.Hidden;
            btnAggBebida.Visibility = Visibility.Hidden;
            btnEliminarBebida.Visibility = Visibility.Hidden;
            grdBebidas1.Visibility = Visibility.Hidden;
            lblBebida1s.Visibility = Visibility.Hidden;
            grdBebidas1.Visibility = Visibility.Hidden;
            btnEliminarBebida.Visibility = Visibility.Hidden;
            btnAggBebida.Visibility = Visibility.Hidden;
            btnAgregarPlatos.Visibility = Visibility.Hidden;
            lblBebida1s.Visibility = Visibility.Hidden;
            lblAdminBebidas1.Visibility= Visibility.Hidden;
            OcultarFormEditBebidas();
        }

        private void OcultarParaMesas()
        {
            lblOrden1.Visibility = Visibility.Hidden;
            grdOrdenes1.Visibility = Visibility.Hidden;
            btnAgOrden.Visibility = Visibility.Hidden;
            lblPlatos1.Visibility = Visibility.Hidden;
            grdPlatos1.Visibility = Visibility.Hidden;
            btnAgregarPlatos.Visibility = Visibility.Hidden;
            btnAggBebida.Visibility = Visibility.Hidden;
            btnEliminarBebida.Visibility = Visibility.Hidden;
            grdBebidas1.Visibility = Visibility.Hidden;
            lblBebida1s.Visibility = Visibility.Hidden;
            lblAdminBebidas1.Visibility = Visibility.Hidden;
            grdBebidas1.Visibility = Visibility.Hidden;
            OcultarFormEditBebidas();
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
            btnEditBebida.Visibility = Visibility.Visible;
            lblAdministrarEmpleados.Visibility = Visibility.Hidden;
            grdEmpleados.Visibility = Visibility.Hidden;
            btnAdminEm.IsChecked = false;
            lblAdministrarUser.Visibility = Visibility.Hidden;
            grdAdUsers.Visibility = Visibility.Hidden;
            btnAdminUsers.IsChecked = false;
            btnAggEmpleado.Visibility = Visibility.Hidden;
            btnEditEmpleado.Visibility = Visibility.Hidden;
            btnAggUser.Visibility = Visibility.Hidden;
            btnEditUser.Visibility = Visibility.Hidden;
            btnEliminarEmpleado.Visibility = Visibility.Hidden;
            lblNuevoUser.Visibility = Visibility.Hidden;
            lblAggUser.Visibility = Visibility.Hidden;
            txtAggUser.Visibility = Visibility.Hidden;
            lblContrasenia.Visibility = Visibility.Hidden;
            txtContrasenia.Visibility = Visibility.Hidden;
            btnAggUserBD.Visibility = Visibility.Hidden;
            lblIdEmpleado.Visibility = Visibility.Hidden;
            txtIdEmpleado.Visibility = Visibility.Hidden;
            btnAEliminarUser.Visibility = Visibility.Hidden;
            OcultarFormEditBebidas();
        }

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
            btnAggBebida.Visibility = Visibility.Hidden;
            btnEliminarBebida.Visibility = Visibility.Hidden;
            //BOTONES DEL MENÚ
            btnPlatos.Visibility = Visibility.Hidden;
            btnUser.Visibility = Visibility.Hidden;
            btnOrdenes.Visibility = Visibility.Hidden;
            btnEditBebida.Visibility = Visibility.Hidden;
            btnSalir.Visibility = Visibility.Hidden;
            btnCambiarUsuario.Visibility = Visibility.Hidden;
            btnMesas.Visibility = Visibility.Hidden;

        }

        private void MostrarAdBebida()
        {
            lblPlatos1.Visibility = Visibility.Hidden;
            lblAdminBebidas1.Visibility = Visibility.Visible;
            grdBebidas1.Visibility = Visibility.Visible;
            btnEliminarBebida.Visibility = Visibility.Visible;
            btnEditBebida.Visibility = Visibility.Visible;
            btnAggBebida.Visibility= Visibility.Visible;
            btnAgregarPlatos.Visibility = Visibility.Hidden;
            btnAgOrden.Visibility = Visibility.Hidden;
            lblOrden1.Visibility = Visibility.Hidden;
            grdOrdenes1.Visibility = Visibility.Hidden;
            lblMesas1.Visibility = Visibility.Hidden;
            grdMesas1.Visibility = Visibility.Hidden;
            
            OcultarParaBebidas();
            OcultarFormEditBebidas();
        }

        private void MostrarAdEmpleado()
        {
            lblAdministrarEmpleados.Visibility = Visibility.Visible;
            grdEmpleados.Visibility = Visibility.Visible;
            btnAggEmpleado.Visibility = Visibility.Visible;
            btnEditEmpleado.Visibility = Visibility.Visible;
            btnEliminarEmpleado.Visibility = Visibility.Visible;
            OcultarParaEmpleado();
        }

        private void OcultarParaEmpleado()
        {
            lblAdminBebidas1.Visibility = Visibility.Hidden;
            btnAdminBebida.Visibility = Visibility.Hidden;
            btnAdminEm.Visibility = Visibility.Hidden;
            btnAdminUsers.Visibility = Visibility.Hidden;
            
        }

        private void MostrarAdUsers()
        {
            lblAdministrarUser.Visibility = Visibility.Visible;
            grdAdUsers.Visibility = Visibility.Visible;
            btnAggUser.Visibility = Visibility.Visible;
            btnAEliminarUser.Visibility = Visibility.Visible;
            OcultarParaUser();
        }

        private void OcultarParaUser()
        {
            lblAdminBebidas1.Visibility = Visibility.Hidden;
            btnAdminBebida.Visibility = Visibility.Hidden;
            btnAdminEm.Visibility = Visibility.Hidden;
            btnAdminUsers.Visibility = Visibility.Hidden;
        }

        private void OcultarUser()
        {
            lblAdministrarUser.Visibility = Visibility.Hidden;
            grdAdUsers.Visibility = Visibility.Hidden;
            btnAggUser.Visibility = Visibility.Hidden;
        }

        private void MostrarAggEmpleado()
        {

        }

        private void MostrarAggUser()
        {
            lblNuevoUser.Visibility = Visibility.Visible;
            lblIdEmpleado.Visibility = Visibility.Visible;
            txtIdEmpleado.Visibility = Visibility.Visible;
            lblAggUser.Visibility = Visibility.Visible;
            txtAggUser.Visibility = Visibility.Visible;
            lblContrasenia.Visibility = Visibility.Visible;
            txtContrasenia.Visibility = Visibility.Visible;
            btnAggUserBD.Visibility = Visibility.Visible;
            btnAEliminarUser.Visibility = Visibility.Hidden;
            OcultarUser();

        }

        private void MostrarAggBebida()
        {
            btnAdminBebida.IsChecked = false;
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
            lblBebida1s.Visibility = Visibility.Hidden;
            btnEditBebida.Visibility = Visibility.Visible;
            btnAgregarPlatos.Visibility = Visibility.Hidden;
            OcultarFormEditBebidas();
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
            btnEliminarBebida.Visibility = Visibility.Hidden;
            btnEditBebida.Visibility = Visibility.Hidden;
            btnAdminEm.Visibility = Visibility.Hidden;
            btnAdminUsers.Visibility = Visibility.Hidden;
            btnAdminBebida.Visibility= Visibility.Hidden;
            btnAggBebidaABD.Visibility = Visibility.Visible;
            lblId_Bebida.Visibility = Visibility.Visible;
            txtId_Bebida.Visibility = Visibility.Visible;
            lblBebida1s.Visibility = Visibility.Hidden;
            btnEditarBebida.Visibility = Visibility.Hidden;
            lblOrden1.Visibility = Visibility.Hidden;
            grdOrdenes1.Visibility = Visibility.Hidden;
            btnAgOrden.Visibility = Visibility.Hidden;
            btnVolverBebidas.Visibility = Visibility.Hidden;
            grdBebidas1.Visibility = Visibility.Hidden;
            //BOTONES DEL MENÚ
            btnPlatos.Visibility = Visibility.Hidden;
            btnUser.Visibility = Visibility.Hidden;
            btnOrdenes.Visibility = Visibility.Hidden;
            btnEditBebida.Visibility = Visibility.Hidden;
            btnSalir.Visibility = Visibility.Hidden;
            btnCambiarUsuario.Visibility = Visibility.Hidden;
            btnMesas.Visibility = Visibility.Hidden;
            lblAdminBebidas1.Visibility = Visibility.Hidden;
            btnEliminarBebida.Visibility = Visibility.Hidden;
            btnAggBebida.Visibility = Visibility.Hidden;
        }

        private void OcultarFormAgregarBebida()
        {
            btnAdminBebida.IsChecked = false;
            lblAggBebida1.Visibility = Visibility.Hidden;
            lblNombreBebida.Visibility = Visibility.Hidden;
            lblAlcoholicaB.Visibility = Visibility.Hidden;
            lblPrecioBebida.Visibility = Visibility.Hidden;
            txtNameBebida1.Visibility = Visibility.Hidden;
            rdSiBebida1.Visibility= Visibility.Hidden;
            rdNoBebida1.Visibility = Visibility.Hidden;
            txtPriceBebida.Visibility = Visibility.Hidden;
            lblCantidadBebida.Visibility = Visibility.Hidden;
            txtCantBebida.Visibility = Visibility.Hidden;
            lblId_Bebida.Visibility = Visibility.Hidden;
            txtId_Bebida.Visibility = Visibility.Hidden;
            btnAggBebidaABD.Visibility= Visibility.Hidden;           
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
            public string Id_Mesa { get; set; } 
        }

        public class Bebidas
        {
            public int Id_Bebida { get; set; }
            public int Id_Producto { get; set; }
            public decimal Precio { get; set; }
            public int Existencia { get; set; }
            public string Bebida { get; set; }
            public bool Alcoholica { get; set; }

        }

        public class User
        {
            public int Id_Empleado { get; set; }
            public string Usuario { get; set; }
            public string Password { get; set; }
        }

        public class Empleado
        {
            public int Id_Persona{ get; set; }
            public string Salario { get; set; }
            public string Estado_Laboal { get; set; }
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
            btnAgregarPlatos.IsChecked = false;
            OcultarParaAgOrden();
            OcultarParaMesas();
            OcultarParaOrdenes();
            OcultarParaBebidas();

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
            btnAdminBebida.Visibility = Visibility.Hidden;
            btnEditBebida.Visibility = Visibility.Hidden;
            
        }

        private void btnVolPlatos_Click(object sender, RoutedEventArgs e)
        {
            btnVolverPlatos.IsChecked = false;
            MostrarMenu();
            OcultarParaPlatos();
            lblPlatos1.Visibility = Visibility.Visible;
            grdPlatos1.Visibility = Visibility.Visible;
            btnAgregarPlatos.Visibility = Visibility.Visible;
            btnVolverPlatos.Visibility = Visibility.Hidden;
            btnAgOrden.IsChecked = false;
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
            OcultarFormEditBebidas();                     
        }

        private void OcultarFormEditBebidas()
        {
            lblEditBebidas.Visibility = Visibility.Hidden;
            lblId_BebidaEdit.Visibility = Visibility.Hidden;
            txtIdBebida.Visibility = Visibility.Hidden;
            lblId_Producto.Visibility = Visibility.Hidden;
            txtIdProducto.Visibility = Visibility.Hidden;
            lblPrecioEdit.Visibility = Visibility.Hidden;
            txtPrecio.Visibility = Visibility.Hidden;
            lblCantiadEdit.Visibility = Visibility.Hidden;
            txtCantidad.Visibility = Visibility.Hidden;
            lblBebidaEdit.Visibility = Visibility.Hidden;
            txtBebida.Visibility = Visibility.Hidden;
            lblAlcoholicaEdit.Visibility = Visibility.Hidden;
            chkAlcoholica.Visibility = Visibility.Hidden;
            
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
            btnAggBebida.IsChecked = false; 
            btnVolverBebidas.Visibility = Visibility.Visible;
            btnSalirMenuAggBebidas.Visibility = Visibility.Visible;
            AgregarBebida();
            
        }

        private async void btnEliminarBebida_Checked(object sender, RoutedEventArgs e)
        {
            btnEliminarBebida.IsChecked = false;
            // Verificar si hay una fila seleccionada
            if (grdBebidas1.SelectedItem == null)
            {
                MessageBox.Show("Por favor, seleccione una bebida para eliminar.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // Obtener la bebida seleccionada
            var bebidaSeleccionada = (Bebidas)grdBebidas1.SelectedItem;

            // Mostrar mensaje de confirmación
            var resultado = MessageBox.Show($"¿Está seguro que desea eliminar la bebida '{bebidaSeleccionada.Bebida}'?",
                                          "Confirmar eliminación",
                                          MessageBoxButton.YesNo,
                                          MessageBoxImage.Question);

            if (resultado == MessageBoxResult.Yes)
            {
                try
                {
                    // Llamar al procedmiento almacenado
                    using (SqlConnection conn = new SqlConnection("Data Source=tcp:sqlproyecto2024.database.windows.net,1433;Initial Catalog=sqlproyecto;User ID=proyecto24;Password=Proyecto-24"))
                    {
                        await conn.OpenAsync();

                        using (SqlCommand cmd = new SqlCommand("Eliminar_Bebida", conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@Id_Bebida", bebidaSeleccionada.Id_Bebida);

                            await cmd.ExecuteNonQueryAsync();
                        }
                    }

                    if (grdBebidas1.ItemsSource is List<Bebidas> lista)
                    {
                        var observableCollection = new ObservableCollection<Bebidas>(lista);
                        observableCollection.Remove(bebidaSeleccionada);
                        grdBebidas1.ItemsSource = observableCollection;
                    }
                    // O si ya es ObservableCollection
                    else if (grdBebidas1.ItemsSource is ObservableCollection<Bebidas> observable)
                    {
                        observable.Remove(bebidaSeleccionada);
                    }

                    // Actualizar el DataGrid
                    var itemsSource = (ObservableCollection<Bebidas>)grdBebidas1.ItemsSource;
                    itemsSource.Remove(bebidaSeleccionada);

                    MessageBox.Show("Bebida eliminada correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al eliminar la bebida: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async Task CargarDatosBebidas()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection("Data Source=tcp:sqlproyecto2024.database.windows.net,1433;Initial Catalog=sqlproyecto;User ID=proyecto24;Password=Proyecto-24"))
                {
                    await conn.OpenAsync();

                    using (SqlCommand cmd = new SqlCommand("Listar_Bebidas", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        var reader = await cmd.ExecuteReaderAsync();
                        var bebidas = new ObservableCollection<Bebidas>();

                        while (await reader.ReadAsync())
                        {
                            var bebida = new Bebidas
                            {
                                Id_Bebida = reader.GetInt32(reader.GetOrdinal("Id_Bebida")),
                                Id_Producto = reader.GetInt32(reader.GetOrdinal("Id_Producto")),
                                Existencia = reader.GetInt32(reader.GetOrdinal("Existencia")),
                                Bebida = reader.IsDBNull(reader.GetOrdinal("Bebida")) ? string.Empty : reader.GetString(reader.GetOrdinal("Bebida")),
                                Alcoholica = reader.GetBoolean(reader.GetOrdinal("Alcoholica"))
                            };

                            // Manejo especial para el precio
                            var precioOrdinal = reader.GetOrdinal("Precio");
                            if (!reader.IsDBNull(precioOrdinal))
                            {
                                // Intentamos obtener el valor como decimal directamente
                                bebida.Precio = reader.GetDecimal(precioOrdinal);
                            }
                            else
                            {
                                bebida.Precio = 0m; // Valor por defecto si es nulo
                            }

                            bebidas.Add(bebida);
                        }

                        grdBebidas1.ItemsSource = bebidas;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar los datos: {ex.Message}\nStack Trace: {ex.StackTrace}",
                              "Error",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
            }
        }


        private void btnEditarBebida_Checked(object sender, RoutedEventArgs e)
        {
            btnSalirMenuAggBebidas.IsChecked = false;
            btnEditarBebida.IsChecked = false;
            if (grdBebidas1.SelectedItem == null)
            {
                MessageBox.Show("Por favor, seleccione una bebida para editar.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var bebidaSeleccionada = (Bebidas)grdBebidas1.SelectedItem;
            btnEditarBebidaABD.Visibility = Visibility.Visible;
            btnVolverBebidas.Visibility = Visibility.Visible;
            lblAdminBebidas1.Visibility = Visibility.Hidden;
            lblBebida1s.Visibility = Visibility.Hidden;
            grdBebidas1.Visibility = Visibility.Hidden;
            btnEliminarBebida.Visibility = Visibility.Hidden;
            btnEditBebida.Visibility = Visibility.Visible;
            btnAggBebida.Visibility = Visibility.Hidden;
            btnEditarBebida.Visibility = Visibility.Hidden;
            // Llenar los TextBox con los datos de la bebida seleccionada
            txtIdBebida.Text = bebidaSeleccionada.Id_Bebida.ToString();
            txtIdProducto.Text = bebidaSeleccionada.Id_Producto.ToString();
            txtPrecio.Text = bebidaSeleccionada.Precio.ToString();
            txtCantidad.Text = bebidaSeleccionada.Existencia.ToString();
            txtBebida.Text = bebidaSeleccionada.Bebida;
            chkAlcoholica.IsChecked = bebidaSeleccionada.Alcoholica;

            lblEditBebidas.Visibility = Visibility.Visible;
            lblId_BebidaEdit.Visibility = Visibility.Visible;
            txtIdBebida.Visibility = Visibility.Visible;
            lblId_Producto.Visibility = Visibility.Visible;
            txtIdProducto.Visibility = Visibility.Visible;
            lblPrecioEdit.Visibility = Visibility.Visible;
            txtPrecio.Visibility = Visibility.Visible;
            lblCantiadEdit.Visibility = Visibility.Visible;
            txtCantidad.Visibility = Visibility.Visible;
            lblBebidaEdit.Visibility =Visibility.Visible;
            txtBebida.Visibility = Visibility.Visible;
            lblAlcoholicaEdit.Visibility = Visibility.Visible;
            chkAlcoholica.Visibility = Visibility.Visible;
        }

        private async void btnEditarBebidaABD_Checked(object sender, RoutedEventArgs e)
        {            
            try
            {
                // Validar los datos ingresados
                if (string.IsNullOrWhiteSpace(txtBebida.Text) ||
                    string.IsNullOrWhiteSpace(txtPrecio.Text) ||
                    string.IsNullOrWhiteSpace(txtCantidad.Text))
                {
                    MessageBox.Show("Por favor, complete todos los campos requeridos.",
                                  "Datos incompletos",
                                  MessageBoxButton.OK,
                                  MessageBoxImage.Warning);
                    return;
                }

                // Convertir los valores
                int idBebida = int.Parse(txtIdBebida.Text);
                int idProducto = int.Parse(txtIdProducto.Text);
                decimal precio = decimal.Parse(txtPrecio.Text);
                int cantidad = int.Parse(txtCantidad.Text);
                bool alcoholica = chkAlcoholica.IsChecked ?? false;

                using (SqlConnection conn = new SqlConnection("Data Source=tcp:sqlproyecto2024.database.windows.net,1433;Initial Catalog=sqlproyecto;User ID=proyecto24;Password=Proyecto-24"))
                {
                    await conn.OpenAsync();

                    using (SqlCommand cmd = new SqlCommand("Actualizar_Bebida", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@Id_Bebida", idBebida);
                        cmd.Parameters.AddWithValue("@Id_Producto", idProducto);
                        cmd.Parameters.AddWithValue("@Precio", precio);
                        cmd.Parameters.AddWithValue("@Cantidad", cantidad);
                        cmd.Parameters.AddWithValue("@Bebida", txtBebida.Text);
                        cmd.Parameters.AddWithValue("@Alcoholica", alcoholica);

                        await cmd.ExecuteNonQueryAsync();
                    }
                }

                MessageBox.Show("Bebida actualizada correctamente.",
                              "Éxito",
                              MessageBoxButton.OK,
                              MessageBoxImage.Information);

                // Volver a la vista principal y recargar datos
                MostrarMenu();
                MostrarAdBebida();
                OcultarFormAgregarBebida();
                OcultarFormEditBebidas();
                btnEditarBebidaABD.Visibility = Visibility.Hidden;
                btnVolverBebidas.Visibility = Visibility.Hidden;
                await CargarDatosBebidas();
            }
            catch (FormatException)
            {
                MessageBox.Show("Por favor, ingrese valores válidos en los campos numéricos.",
                              "Error de formato",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al actualizar la bebida: {ex.Message}",
                              "Error",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
            }
        }

        private void btnVolverBebidas_Checked(object sender, RoutedEventArgs e)
        {
            btnAggBebida.Visibility = Visibility.Visible;
            btnEditarBebidaABD.Visibility = Visibility.Hidden;
            btnVolverBebidas.Visibility = Visibility.Hidden;
            OcultarFormEditBebidas();
            MostrarAdBebida();
            OcultarFormAgregarBebida();
        }

        private void btnAgregarMesa_Checked(object sender, RoutedEventArgs e)
        {
            btnAgregarMesa.IsChecked = false;
            string connectionString = "Data Source=tcp:sqlproyecto2024.database.windows.net,1433;Initial Catalog=sqlproyecto;User ID=proyecto24;Password=Proyecto-24";
            string storedProcedure = "Agregar_Mesa";

            // Como estado inicial siempre será "Disponible" cuando se agrega una nueva mesa
            string estadoInicial = "Disponible";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(storedProcedure, connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;

                // Solo necesitamos el parámetro @Estado
                command.Parameters.AddWithValue("@Estado", estadoInicial);

                try
                {
                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Mesa agregada con éxito", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                        CargarMesas(); // Asumiendo que tienes un método para actualizar la lista de mesas
                    }
                    else
                    {
                        MessageBox.Show("No se pudo agregar la mesa.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async void btnEliminarMesa_Checked(object sender, RoutedEventArgs e)
        {
            btnEliminarMesa.IsChecked = false;
            // Verificar si hay una fila seleccionada
            if (grdMesas1.SelectedItem == null)
            {
                MessageBox.Show("Por favor, seleccione una mesa para eliminar.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // Obtener la mesa seleccionada
            var mesaSeleccionada = (Mesas)grdMesas1.SelectedItem;

            // Mostrar mensaje de confirmación
            var resultado = MessageBox.Show($"¿Está seguro que desea eliminar la mesa '{mesaSeleccionada.Mesa}'?",
                                          "Confirmar eliminación",
                                          MessageBoxButton.YesNo,
                                          MessageBoxImage.Question);

            if (resultado == MessageBoxResult.Yes)
            {
                try
                {
                    // Llamar al procedimiento almacenado
                    using (SqlConnection conn = new SqlConnection("Data Source=tcp:sqlproyecto2024.database.windows.net,1433;Initial Catalog=sqlproyecto;User ID=proyecto24;Password=Proyecto-24"))
                    {
                        await conn.OpenAsync();

                        using (SqlCommand cmd = new SqlCommand("Eliminar_Mesa", conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@Id_Mesa", mesaSeleccionada.Id_Mesa);

                            await cmd.ExecuteNonQueryAsync();
                        }
                    }

                    if (grdMesas1.ItemsSource is List<Mesas> lista)
                    {
                        var observableCollection = new ObservableCollection<Mesas>(lista);
                        observableCollection.Remove(mesaSeleccionada);
                        grdMesas1.ItemsSource = observableCollection;
                    }
                    // O si ya es ObservableCollection
                    else if (grdMesas1.ItemsSource is ObservableCollection<Mesas> observable)
                    {
                        observable.Remove(mesaSeleccionada);
                    }

                    // Actualizar el DataGrid
                    var itemsSource = (ObservableCollection<Mesas>)grdMesas1.ItemsSource;
                    itemsSource.Remove(mesaSeleccionada);

                    MessageBox.Show("Mesa eliminada correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al eliminar la mesa: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

        }
    }

}


