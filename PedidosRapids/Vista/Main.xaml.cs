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
        private List<Platillo> datosPlatillos;
        private List<Empleado> datosEmpleados;

        public Main(string userRole)

        {
            InitializeComponent();

            // Configuración basada en el rol
            if (userRole == "Empleado")
            {
                // Oculta botones, menús u otros elementos exclusivos del administrador

               
                btnUser.Foreground = new SolidColorBrush(Colors.Black);
                btnUser.IsEnabled = false;
                btnAgregarMesa.Foreground = new SolidColorBrush(Colors.Black);
                btnAgregarMesa.IsEnabled = false;
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
            CargarEmpleados();
            MostrarValorEnTextBox();
            CargarDatosBebidas();
            CargarMunicipios();
            CargarPlatillos();


            // Enlazar datos a DataGrids
            grdPlatos1.ItemsSource = datos;
            grdOrdenes1.ItemsSource = datosOrdenes;
            grdMesas1.ItemsSource = datosMesa;
            grdBebidas1.ItemsSource = datosBebidas;
            grdPlatos1.ItemsSource = datosPlatillos;
            grdAdUsers.ItemsSource = datosUsuarios;


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
            try
            {
                // Validación del ID
                if (!int.TryParse(txtId_Bebida.Text, out int idBebida))
                {
                    MostrarError("El ID debe ser un número válido.");
                    btnAggBebidaABD.IsChecked = false;
                    return;
                }

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

                // Validación del nombre
                if (string.IsNullOrWhiteSpace(txtNameBebida1.Text))
                {
                    MostrarError("El nombre de la bebida es requerido.");
                    btnAggBebidaABD.IsChecked = false;
                    return;
                }

                string connectionString = "Data Source=tcp:sqlproyecto2024.database.windows.net,1433;Initial Catalog=sqlproyecto;User ID=proyecto24;Password=Proyecto-24";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Primero verificamos si el ID ya existe
                    string checkQuery = "SELECT COUNT(*) FROM dbo.Producto WHERE Id_Producto = @Id";
                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, connection))
                    {
                        checkCmd.Parameters.AddWithValue("@Id", idBebida);
                        int count = (int)checkCmd.ExecuteScalar();
                        if (count > 0)
                        {
                            MostrarError($"Ya existe un producto con el ID {idBebida}");
                            return;
                        }
                    }

                    // Si no existe, procedemos a insertar
                    string storedProcedure = "Agregar_Bebida";
                    using (SqlCommand command = new SqlCommand(storedProcedure, connection))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@Id_Producto", idBebida);
                        command.Parameters.AddWithValue("@NombreBebida", txtNameBebida1.Text.Trim());
                        command.Parameters.AddWithValue("@Alcoholica", rdSiBebida1.IsChecked == true);
                        command.Parameters.AddWithValue("@Precio", precio);
                        command.Parameters.AddWithValue("@Cantidad", cantidad);

                        command.ExecuteNonQuery();
                    }

                    MessageBox.Show("Bebida agregada con éxito", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                    LimpiarCampos();
                    CargarBebidas();
                }
            }
            catch (SqlException sqlEx)
            {
                MostrarError($"Error de base de datos: {sqlEx.Message}\nNúmero de error: {sqlEx.Number}");
            }
            catch (Exception ex)
            {
                MostrarError($"Error al agregar la bebida: {ex.Message}");
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
                command.Parameters.AddWithValue("@Rol", txtRol.Text);

                try
                {
                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Usuario agregado con exito");
                        CargarUsuarios();
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
            if (grdAdUsers.SelectedItem is User selectedUser)
            {
                var result = MessageBox.Show($"¿Estás seguro de que deseas eliminar al usuario '{selectedUser.Usuario}'?",
                                     "Confirmación",
                                     MessageBoxButton.YesNo,
                                     MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                string connectionString = "Data Source=tcp:sqlproyecto2024.database.windows.net,1433;Initial Catalog=sqlproyecto;User ID=proyecto24;Password=Proyecto-24";
                string storedProcedure = "Eliminar_Usuario";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(storedProcedure, connection);
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@Id_Usuario", selectedUser.Id_Usuario);

                    try
                    {
                        connection.Open();
                        command.ExecuteNonQuery();
                    
                        
                        MessageBox.Show("Operación completada.", "Resultado", MessageBoxButton.OK, MessageBoxImage.Information);
                    
                        CargarUsuarios();
                    }
                    catch (SqlException ex)
                    {
                        MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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

        private void btnAggEmpleadoBD_Checked(object sender, RoutedEventArgs e)
{
        MessageBox.Show("Botón presionado");
    string connectionString = "Data Source=tcp:sqlproyecto2024.database.windows.net,1433;Initial Catalog=sqlproyecto;User ID=proyecto24;Password=Proyecto-24";
    string storedProcedure = "Agregar_Empleado";

    using (SqlConnection connection = new SqlConnection(connectionString))
    {
        SqlCommand command = new SqlCommand(storedProcedure, connection);
        command.CommandType = System.Data.CommandType.StoredProcedure;

        
        command.Parameters.AddWithValue("@Nombre", txtNombre.Text);
        command.Parameters.AddWithValue("@Apellido", txtApellido.Text);
        command.Parameters.AddWithValue("@Salario", decimal.Parse(txtSalario.Text));
        command.Parameters.AddWithValue("@Estado_Laboral", txtEstadoLaboral.Text);
        command.Parameters.AddWithValue("@Email", txtEmail.Text);
        command.Parameters.AddWithValue("@Direccion", txtDireccion.Text);

        try
                {
                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Usuario agregado con exito");
                        CargarEmpleados();
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
            btnAggEmpleadoBD.IsChecked = false;
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
            btnEditarBebida.Visibility = Visibility.Visible;


        }


        //**********************Funciones para la navegacion de menus******************

        private void btnAdminBebida_Checked(object sender, RoutedEventArgs e)
        {
            btnAdminBebida.IsChecked = false;
            OcultarFormAgregarBebida();
            OcultarParaBebidas();
            lblBebida1s.Visibility = Visibility.Visible;
            grdBebidas1.Visibility = Visibility.Visible;
            btnEditarBebida.Visibility = Visibility.Visible;
            btnEliminarBebida.Visibility = Visibility.Visible;
            btnAggBebida.Visibility = Visibility.Visible;

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
            LimpiarFormulario();
            MostrarAggEmpleado();

            // Configurar el formulario para nuevo empleado
            txtIdPersona.Text = string.Empty;
            txtIdPersona.IsEnabled = false; // Deshabilitar el ID porque será autogenerado
            btnGuardar.Tag = "nuevo"; // Marcar que es un nuevo empleado
        }

        private void btnVolverUsuarios_Checked(object sender, RoutedEventArgs e)
        {
            VolverAtrasUsuario();
        }

        private void btnVolverEmpleados_Checked(object sender, RoutedEventArgs e)
        {
            VolverAtrasEmpleado();
        }

        private void btnEditEmpleado_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                // Verificar que haya un empleado seleccionado
                if (grdEmpleados.SelectedItem == null)
                {
                    MessageBox.Show("Por favor, seleccione un empleado para editar.", "Aviso",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                // Obtener el empleado seleccionado del DataGrid
                var empleado = (Empleado)grdEmpleados.SelectedItem;

                // Mostrar el formulario
                MostrarAggEmpleado();

                // Cargar los datos en los campos
                txtIdPersona.Text = empleado.Id_Persona.ToString();
                txtNombre.Text = empleado.Nombre;
                txtApellido.Text = empleado.Apellido;
                txtSalario.Text = empleado.Salario.ToString();
                txtEstadoLaboral.Text = empleado.Estado_Laboral;
                txtEmail.Text = empleado.Email;
                txtDireccion.Text = empleado.Direccion;
                cmbMunicipio.SelectedValue = empleado.Id_Municipio;
                txtTelefono.Text = empleado.Telefono;

                // Deshabilitar el ID ya que no debe modificarse
                txtIdPersona.IsEnabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar los datos del empleado: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void btnEditEmpleadoBD_Checked(object sender, RoutedEventArgs e)
        {
            if (grdEmpleados.SelectedItem is Empleado selectedEmpleado)
            {
                // Validar campos requeridos
                if (string.IsNullOrWhiteSpace(txtNombre.Text) ||
                    string.IsNullOrWhiteSpace(txtApellido.Text) ||
                    string.IsNullOrWhiteSpace(txtSalario.Text) ||
                    string.IsNullOrWhiteSpace(txtEstadoLaboral.Text))
                {
                    MessageBox.Show("Por favor, complete todos los campos obligatorios.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Validar que el salario sea un número válido
                if (!decimal.TryParse(txtSalario.Text, out decimal salario))
                {
                    MessageBox.Show("El salario debe ser un valor numérico válido.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                string connectionString = "Data Source=tcp:sqlproyecto2024.database.windows.net,1433;Initial Catalog=sqlproyecto;User ID=proyecto24;Password=Proyecto-24";
                string storedProcedure = "EditarEmpleado"; // Nombre corregido del procedimiento

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(storedProcedure, connection);
                    command.CommandType = CommandType.StoredProcedure;

                    // Agregar parámetros según el procedimiento almacenado
                    command.Parameters.AddWithValue("@Id_Persona", selectedEmpleado.Id_Persona);
                    command.Parameters.AddWithValue("@Nombre", txtNombre.Text.Trim());
                    command.Parameters.AddWithValue("@Apellido", txtApellido.Text.Trim());
                    command.Parameters.AddWithValue("@Email", string.IsNullOrWhiteSpace(txtEmail.Text) ? (object)DBNull.Value : (object)txtEmail.Text.Trim());
                    command.Parameters.AddWithValue("@Direccion", string.IsNullOrWhiteSpace(txtDireccion.Text) ? (object)DBNull.Value : (object)txtDireccion.Text.Trim());
                    command.Parameters.AddWithValue("@Id_Municipio", cmbMunicipio.SelectedValue ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Telefono", string.IsNullOrWhiteSpace(txtTelefono.Text) ? (object)DBNull.Value : (object)txtTelefono.Text.Trim());
                    command.Parameters.AddWithValue("@Salario", salario);
                    command.Parameters.AddWithValue("@Estado_Laboral", txtEstadoLaboral.Text.Trim());

                    try
                    {
                        connection.Open();

                        // Usamos ExecuteReader porque el procedimiento retorna un mensaje
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                reader.Read();
                                string mensaje = reader["Mensaje"].ToString();
                                MessageBox.Show(mensaje, "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                                CargarEmpleados(); // Actualizar la grilla
                            }
                        }
                    }
                    catch (SqlException ex)
                    {
                        string mensajeError;

                        // Manejar los errores específicos del procedimiento
                        if (ex.Class == 16) // Severidad 16 usada en el RAISERROR del procedimiento
                        {
                            mensajeError = ex.Message;
                        }
                        else
                        {
                            switch (ex.Number)
                            {
                                case 547: // Violación de clave foránea
                                    mensajeError = "Error en la relación de datos. Verifique que el municipio seleccionado sea válido.";
                                    break;
                                case 50000: // Errores personalizados
                                    mensajeError = ex.Message;
                                    break;
                                default:
                                    mensajeError = $"Error en la base de datos: {ex.Message}";
                                    break;
                            }
                        }

                        MessageBox.Show(mensajeError, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error inesperado: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Por favor, selecciona un empleado para editar.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            btnEditEmpleadoBD.IsChecked = false;
        }




        private void btnAggUser_Checked(object sender, RoutedEventArgs e)
        {
            MostrarAggUser();
        }

        private void btnEliminarEmpleado_Checked(object sender, RoutedEventArgs e)
        {
            if (grdEmpleados.SelectedItem is Empleado selectedEmpleado)
            {
                var result = MessageBox.Show($"¿Estás seguro de que deseas eliminar al empleado '{selectedEmpleado.Nombre} {selectedEmpleado.Apellido}'?",
                                           "Confirmación",
                                           MessageBoxButton.YesNo,
                                           MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    string connectionString = "Data Source=tcp:sqlproyecto2024.database.windows.net,1433;Initial Catalog=sqlproyecto;User ID=proyecto24;Password=Proyecto-24";
                    string storedProcedure = "EliminarEmpleado"; // Corregido el nombre del procedimiento

                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        SqlCommand command = new SqlCommand(storedProcedure, connection);
                        command.CommandType = CommandType.StoredProcedure;

                        // Agregamos el parámetro requerido por el procedimiento
                        command.Parameters.AddWithValue("@Id_Persona", selectedEmpleado.Id_Persona);

                        try
                        {
                            connection.Open();

                            // Usamos ExecuteReader porque el procedimiento retorna un mensaje
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                if (reader.HasRows)
                                {
                                    reader.Read();
                                    string mensaje = reader["Mensaje"].ToString();
                                    MessageBox.Show(mensaje, "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                                    CargarEmpleados(); // Actualizamos la grilla
                                }
                            }
                        }
                        catch (SqlException ex)
                        {
                            string mensajeError;

                            // El procedimiento usa RAISERROR, así que manejamos los mensajes personalizados
                            if (ex.Class == 16) // Severidad 16 usada en el RAISERROR del procedimiento
                            {
                                mensajeError = ex.Message;
                            }
                            else
                            {
                                switch (ex.Number)
                                {
                                    case 547: // Violación de clave foránea
                                        mensajeError = "No se puede eliminar el empleado porque tiene registros relacionados.";
                                        break;
                                    case 50000: // Errores personalizados
                                        mensajeError = ex.Message;
                                        break;
                                    default:
                                        mensajeError = $"Error en la base de datos: {ex.Message}";
                                        break;
                                }
                            }

                            MessageBox.Show(mensajeError, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error inesperado: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Por favor, selecciona un empleado para eliminar.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            btnEliminarEmpleado.IsChecked = false;
        }        

        // Método para mostrar el formulario en modo edición
        private void MostrarFormularioEdicion(Empleado empleado)
        {
            lblTitulo.Content = "Editar Empleado";

            // Mostrar todos los controles
            lblTitulo.Visibility = Visibility.Visible;
            lblIdPersona.Visibility = Visibility.Visible;
            txtIdPersona.Visibility = Visibility.Visible;
            // ... (hacer visible todos los demás controles)
            stackBotones.Visibility = Visibility.Visible;

            // Cargar los datos del empleado en los controles
            txtIdPersona.Text = empleado.Id_Persona.ToString();
            txtNombre.Text = empleado.Nombre;
            txtApellido.Text = empleado.Apellido;
            txtSalario.Text = empleado.Salario.ToString();
            txtEstadoLaboral.Text = empleado.Estado_Laboral;
            txtEmail.Text = empleado.Email;
            txtDireccion.Text = empleado.Direccion;

            // Cargar municipios y seleccionar el del empleado
            CargarMunicipios();
            cmbMunicipio.SelectedValue = empleado.Id_Municipio;

            // Cargar el teléfono del empleado
            CargarTelefonoEmpleado(empleado.Id_Persona);
        }

        // Método para cargar el teléfono del empleado
        private void CargarTelefonoEmpleado(int idPersona)
        {
            string connectionString = "Data Source=tcp:sqlproyecto2024.database.windows.net,1433;Initial Catalog=sqlproyecto;User ID=proyecto24;Password=Proyecto-24";
            string query = "SELECT Telefono FROM Telefono WHERE Id_Persona = @Id_Persona";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Id_Persona", idPersona);
                    connection.Open();

                    object result = command.ExecuteScalar();
                    txtTelefono.Text = result != null ? result.ToString() : string.Empty;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar teléfono: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Método para limpiar el formulario
        private void LimpiarFormulario()
        {
            txtIdPersona.Clear();
            txtNombre.Clear();
            txtApellido.Clear();
            txtSalario.Clear();
            txtEstadoLaboral.Clear();
            txtEmail.Clear();
            txtDireccion.Clear();
            txtTelefono.Clear();
            cmbMunicipio.SelectedIndex = -1;
        }

        // Método para ocultar el formulario
        private void OcultarFormulario()
        {
            OcultarEmpleado();
            lblTitulo.Visibility = Visibility.Hidden;
            lblIdPersona.Visibility = Visibility.Hidden;
            txtIdPersona.Visibility = Visibility.Hidden;
            lblNombre.Visibility = Visibility.Hidden;
            txtNombre.Visibility = Visibility.Hidden;
            lblApellido.Visibility = Visibility.Hidden;
            txtApellido.Visibility = Visibility.Hidden;
            lblSalario.Visibility = Visibility.Hidden;
            txtSalario.Visibility = Visibility.Hidden;
            lblEstadoLaboral.Visibility = Visibility.Hidden;
            txtEstadoLaboral.Visibility = Visibility.Hidden;
            lblEmail.Visibility = Visibility.Hidden;
            txtEmail.Visibility = Visibility.Hidden;
            lblDireccion.Visibility = Visibility.Hidden;
            txtDireccion.Visibility = Visibility.Hidden;
            lblMunicipio.Visibility = Visibility.Hidden;
            cmbMunicipio.Visibility = Visibility.Hidden;
            lblTelefono.Visibility = Visibility.Hidden;
            txtTelefono.Visibility = Visibility.Hidden;
            btnGuardar.Visibility = Visibility.Hidden;
        }

        // Manejador del botón Cancelar
        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            LimpiarFormulario();
            OcultarFormulario();
        }


        public void CargarMesas()
        {
            datosMesa = new List<Mesas>();
            string connectionString = "Data Source=tcp:sqlproyecto2024.database.windows.net,1433;Initial Catalog=sqlproyecto;User ID=proyecto24;Password=Proyecto-24";
            string storedProcedureName = "Listar_Mesas";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(storedProcedureName, connection);
                command.CommandType = CommandType.StoredProcedure;

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    var mesasUnicas = new HashSet<string>();

                    while (reader.Read())
                    {
                        string mesa = reader["Mesa"].ToString();
                        if (!mesasUnicas.Contains(mesa))
                        {
                            mesasUnicas.Add(mesa);
                            datosMesa.Add(new Mesas { Mesa = mesa });
                        }
                    }

                    reader.Close();

                    // Actualizar el DataGrid después de cargar los datos
                    grdMesas1.ItemsSource = null;
                    grdMesas1.ItemsSource = datosMesa;
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
                        Id_Usuario = reader.IsDBNull(reader.GetOrdinal("Id_Usuario")) ? 0 : Convert.ToInt32(reader["Id_Usuario"]),
                        Id_Empleado = reader.IsDBNull(reader.GetOrdinal("Id_Empleado")) ? 0 : Convert.ToInt32(reader["Id_Empleado"]),
                        Usuario = reader.IsDBNull(reader.GetOrdinal("Usuario")) ? string.Empty : reader["Usuario"].ToString(),
                        Password = reader.IsDBNull(reader.GetOrdinal("Contrasena")) ? string.Empty : reader["Contrasena"].ToString(),
                        Rol = reader.IsDBNull(reader.GetOrdinal("Rol")) ? string.Empty : reader["Rol"].ToString()
                    
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

        private void CargarMunicipios()
{
    try
    {
        string connectionString = "Data Source=tcp:sqlproyecto2024.database.windows.net,1433;Initial Catalog=sqlproyecto;User ID=proyecto24;Password=Proyecto-24";
        List<Municipio> municipios = new List<Municipio>();

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            using (SqlCommand cmd = new SqlCommand("Listar_Municipios", connection))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Municipio municipio = new Municipio
                        {
                            Nombre_Municipio = reader["Nombre_Municipio"].ToString()
                        };
                        municipios.Add(municipio);
                    }
                }
            }
        }

        // Asignar los municipios al ComboBox
        cmbMunicipio.ItemsSource = municipios;
        cmbMunicipio.DisplayMemberPath = "Nombre_Municipio";
        cmbMunicipio.SelectedValuePath = "Nombre_Municipio";
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Error al cargar los municipios: {ex.Message}", "Error", 
            MessageBoxButton.OK, MessageBoxImage.Error);
    }
}


        private void CargarEmpleados()
        {
            try
            {
                datosEmpleados = new List<Empleado>();
                string connectionString = "Data Source=tcp:sqlproyecto2024.database.windows.net,1433;Initial Catalog=sqlproyecto;User ID=proyecto24;Password=Proyecto-24";
                string storedProcedure = "ListarEmpleados";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(storedProcedure, connection);
                    command.CommandType = CommandType.StoredProcedure;

                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            float salario = 0f;
                            if (!reader.IsDBNull(reader.GetOrdinal("Salario")))
                            {
                                salario = (float)reader.GetDouble(reader.GetOrdinal("Salario"));
                            }

                            var empleado = new Empleado
                            {
                                Id_Persona = reader.IsDBNull(reader.GetOrdinal("Id_Persona")) ? 0 : Convert.ToInt32(reader["Id_Persona"]),
                                Nombre = reader.IsDBNull(reader.GetOrdinal("Nombre")) ? string.Empty : reader["Nombre"].ToString(),
                                Apellido = reader.IsDBNull(reader.GetOrdinal("Apellido")) ? string.Empty : reader["Apellido"].ToString(),
                                Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? string.Empty : reader["Email"].ToString(),
                                Direccion = reader.IsDBNull(reader.GetOrdinal("Direccion")) ? string.Empty : reader["Direccion"].ToString(),
                                Id_Municipio = reader.IsDBNull(reader.GetOrdinal("Id_Municipio")) ? 0 : Convert.ToInt32(reader["Id_Municipio"]),
                                NombreMunicipio = reader.IsDBNull(reader.GetOrdinal("NombreMunicipio")) ? string.Empty : reader["NombreMunicipio"].ToString(),
                                NombreDepartamento = reader.IsDBNull(reader.GetOrdinal("NombreDepartamento")) ? string.Empty : reader["NombreDepartamento"].ToString(),
                                Telefono = reader.IsDBNull(reader.GetOrdinal("Telefono")) ? string.Empty : reader["Telefono"].ToString(),
                                Salario = salario,
                                Estado_Laboral = reader.IsDBNull(reader.GetOrdinal("Estado_Laboral")) ? string.Empty : reader["Estado_Laboral"].ToString()
                            };
                            datosEmpleados.Add(empleado);
                        }
                    }

                    // Actualizar el DataGrid
                    grdEmpleados.ItemsSource = null;
                    grdEmpleados.ItemsSource = datosEmpleados;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar empleados: {ex.Message}\n\nStack Trace: {ex.StackTrace}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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

        private void CargarPlatillos()
        {
            // Inicializar la lista de platillos
            datosPlatillos = new List<Platillo>();
            string connectionString = "Data Source=tcp:sqlproyecto2024.database.windows.net,1433;Initial Catalog=sqlproyecto;User ID=proyecto24;Password=Proyecto-24";
            string storedProcedure = "Listar_Platillos"; // Nombre del procedimiento almacenado

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(storedProcedure, connection);
                command.CommandType = CommandType.StoredProcedure;

                try
                {
                    connection.Open(); // Abrir la conexión con la base de datos
                    SqlDataReader reader = command.ExecuteReader();

                    // Leer los datos obtenidos del procedimiento almacenado
                    while (reader.Read())
                    {
                        datosPlatillos.Add(new Platillo
                        {
                            Id_Platillo = Convert.ToInt32(reader["Id_Platillo"]),
                            Id_Categoria = reader["Id_Categoria"] != DBNull.Value ? Convert.ToInt32(reader["Id_Categoria"]) : 0,
                            Tiempo_Preparacion = reader["TiempoPreparacion"]?.ToString(),
                            Descripcion = reader["Descripcion"]?.ToString(),
                            Nombre_Platillo = reader["Nombre_Platillo"]?.ToString(),
                            Id_Producto = reader["Id_Producto"] != DBNull.Value ? Convert.ToInt32(reader["Id_Producto"]) : 0
                        });
                    }

                    reader.Close(); // Cerrar el lector de datos
                    grdPlatos1.ItemsSource = datosPlatillos; // Enlazar los datos al DataGrid
                }
                catch (Exception ex)
                {
                    // Mostrar un mensaje de error en caso de excepciones
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
            lblIdUsuarioEdit.Visibility = Visibility.Visible;
            txtIdUsuarioEdit.Visibility = Visibility.Visible;
            lblNuevoNombreUsuario.Visibility = Visibility.Visible;
            txtNuevoNombreUsuario.Visibility = Visibility.Visible;
            lblNuevaContrasenia.Visibility = Visibility.Visible;
            txtNuevaContrasenia.Visibility = Visibility.Visible;
            lblNuevoRol.Visibility = Visibility.Visible;
            txtNuevoRol.Visibility = Visibility.Visible;
            btnMainMenu.Visibility = Visibility.Visible;
            lblNPassEd1.Visibility = Visibility.Hidden;
            lblNUserEdit1.Visibility = Visibility.Hidden;
            lblEdUser1.Visibility = Visibility.Hidden;
            lblNUserName1.Visibility = Visibility.Hidden;
            txtUserEd1.Visibility = Visibility.Hidden;
            txtNUserName1.Visibility = Visibility.Hidden;
            txtNPass1.Visibility = Visibility.Hidden;
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
            btnEditarBebida.Visibility = Visibility.Hidden;
            btnEliminarMesa.Visibility = Visibility.Hidden;
            btnAgregarMesa.Visibility = Visibility.Hidden;
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
            btnEliminarMesa.Visibility = Visibility.Hidden;
            btnAgregarMesa.Visibility = Visibility.Hidden;
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
            btnEliminarMesa.Visibility = Visibility.Hidden;
            btnAgregarMesa.Visibility = Visibility.Hidden;
            btnEditarBebida.Visibility = Visibility.Hidden;
            btnEliminarMesa.Visibility = Visibility.Hidden;
            btnAgregarMesa.Visibility = Visibility.Hidden;
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
            btnEditarBebida.Visibility = Visibility.Hidden;
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
            lblTitulo.Visibility = Visibility.Hidden;
            lblIdPersona.Visibility = Visibility.Hidden;
            txtIdPersona.Visibility = Visibility.Hidden;
            lblNombre.Visibility = Visibility.Hidden;
            txtNombre.Visibility = Visibility.Hidden;
            lblApellido.Visibility = Visibility.Hidden;
            txtApellido.Visibility = Visibility.Hidden;
            lblSalario.Visibility = Visibility.Hidden;
            txtSalario.Visibility = Visibility.Hidden;
            lblEstadoLaboral.Visibility = Visibility.Hidden;
            txtEstadoLaboral.Visibility = Visibility.Hidden;
            lblEmail.Visibility = Visibility.Hidden;
            txtEmail.Visibility = Visibility.Hidden;
            lblDireccion.Visibility = Visibility.Hidden;
            txtDireccion.Visibility = Visibility.Hidden;
            btnAggEmpleadoBD.Visibility = Visibility.Hidden;
            btnEditEmpleadoBD.Visibility = Visibility.Hidden;
            lblRol.Visibility = Visibility.Hidden;
            txtRol.Visibility = Visibility.Hidden;
            lblIdUsuarioEdit.Visibility = Visibility.Hidden;
            txtIdUsuarioEdit.Visibility = Visibility.Hidden;
            lblNuevoNombreUsuario.Visibility = Visibility.Hidden;
            txtNuevoNombreUsuario.Visibility = Visibility.Hidden;
            lblNuevaContrasenia.Visibility = Visibility.Hidden;
            txtNuevaContrasenia.Visibility = Visibility.Hidden;
            lblNuevoRol.Visibility = Visibility.Hidden;
            txtNuevoRol.Visibility = Visibility.Hidden;
            btnEditarBebida.Visibility = Visibility.Hidden;
            lblMunicipio.Visibility = Visibility.Hidden;
            cmbMunicipio.Visibility = Visibility.Hidden;
            lblTelefono.Visibility = Visibility.Hidden;
            txtTelefono.Visibility = Visibility.Hidden;
            btnGuardar.Visibility = Visibility.Hidden;
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
            btnEditarBebida.Visibility = Visibility.Visible;
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
            btnAEliminarUser.Visibility = Visibility.Hidden;
        }

        private void OcultarEmpleado()
        {
            lblAdministrarEmpleados.Visibility = Visibility.Hidden;
            grdEmpleados.Visibility = Visibility.Hidden;
            btnAggEmpleado.Visibility = Visibility.Hidden;
            btnEditEmpleado.Visibility = Visibility.Hidden;
            btnEliminarEmpleado.Visibility = Visibility.Hidden;
        }

        private void MostrarAggEmpleado()
        {
            lblTitulo.Visibility = Visibility.Visible;
            lblIdPersona.Visibility = Visibility.Visible;
            txtIdPersona.Visibility= Visibility.Visible;
            lblNombre.Visibility = Visibility.Visible;
            txtNombre.Visibility = Visibility.Visible;
            lblApellido.Visibility = Visibility.Visible;
            txtApellido.Visibility = Visibility.Visible;
            lblSalario.Visibility = Visibility.Visible;
            txtSalario.Visibility= Visibility.Visible;
            lblEstadoLaboral.Visibility = Visibility.Visible;
            txtEstadoLaboral.Visibility = Visibility.Visible;
            lblEmail.Visibility = Visibility.Visible;
            txtEmail.Visibility = Visibility.Visible;
            lblDireccion.Visibility = Visibility.Visible;
            txtDireccion.Visibility = Visibility.Visible;
            lblMunicipio.Visibility = Visibility.Visible;
            cmbMunicipio.Visibility = Visibility.Visible;
            lblTelefono.Visibility = Visibility.Visible;
            txtTelefono.Visibility = Visibility.Visible;
            btnGuardar.Visibility = Visibility.Visible;
            OcultarEmpleado();
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
            lblRol.Visibility = Visibility.Visible;
            txtRol.Visibility = Visibility.Visible;
            btnAggUserBD.Visibility = Visibility.Visible;
            btnVolverUsuarios.Visibility = Visibility.Visible;
            OcultarUser();

        }

        private void VolverAtrasUsuario()
        {
            MostrarAdUsers();
            lblNuevoUser.Visibility = Visibility.Hidden;
            lblIdEmpleado.Visibility = Visibility.Hidden;
            txtIdEmpleado.Visibility = Visibility.Hidden;
            lblAggUser.Visibility = Visibility.Hidden;
            txtAggUser.Visibility = Visibility.Hidden;
            lblContrasenia.Visibility = Visibility.Hidden;
            txtContrasenia.Visibility = Visibility.Hidden;
            lblRol.Visibility = Visibility.Hidden;
            txtRol.Visibility = Visibility.Hidden;
            btnAggUserBD.Visibility = Visibility.Hidden;
            btnVolverUsuarios.Visibility = Visibility.Hidden;
        }

        private void VolverAtrasEmpleado()
        {
            MostrarAdEmpleado();
            lblTitulo.Visibility = Visibility.Hidden;
            lblIdPersona.Visibility = Visibility.Hidden;
            txtIdPersona.Visibility = Visibility.Hidden;
            lblNombre.Visibility = Visibility.Hidden;
            txtNombre.Visibility = Visibility.Hidden;
            lblApellido.Visibility = Visibility.Hidden;
            txtApellido.Visibility = Visibility.Hidden;
            lblSalario.Visibility = Visibility.Hidden;
            txtSalario.Visibility = Visibility.Hidden;
            lblEstadoLaboral.Visibility = Visibility.Hidden;
            txtEstadoLaboral.Visibility = Visibility.Hidden;
            lblEmail.Visibility = Visibility.Hidden;
            txtEmail.Visibility = Visibility.Hidden;
            lblDireccion.Visibility = Visibility.Hidden;
            txtDireccion.Visibility = Visibility.Hidden;
            btnAggEmpleadoBD.Visibility = Visibility.Hidden;
            btnVolverEmpleados.Visibility = Visibility.Hidden;
            btnEditEmpleadoBD.Visibility = Visibility.Hidden;
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
            public int Id_Usuario { get; set; }
            public int Id_Empleado { get; set; }
            public string Usuario { get; set; }
            public string Password { get; set; }
            public string Rol { get; set; }
        }

        public class Municipio
        {
            public string Nombre_Municipio { get; set; }
        }

        public class Empleado
        {
            public int Id_Persona { get; set; }
            public string Nombre { get; set; }
            public string Apellido { get; set; }
            public string Email { get; set; }
            public string Direccion { get; set; }
            public int Id_Municipio { get; set; }
            public string NombreMunicipio { get; set; }
            public string NombreDepartamento { get; set; }
            public string Telefono { get; set; }
            public float Salario { get; set; }  // Cambiado de decimal a float
            public string Estado_Laboral { get; set; }
        }

        public class Platillo
        {
            public int Id_Platillo { get; set; }
            public int Id_Categoria { get; set; }
            public string Tiempo_Preparacion { get; set; }
            public string Descripcion { get; set; }
            public string Nombre_Platillo { get; set; }
            public int Id_Producto { get; set; }
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

        // Método para insertar un platillo en la base de datos
        private void btnInsertarPlatos_Click(object sender, RoutedEventArgs e)
        {
            // Validar los datos ingresados
            if (string.IsNullOrWhiteSpace(txtCategoria.Text) ||
                string.IsNullOrWhiteSpace(txtPlatillo.Text) ||
                string.IsNullOrWhiteSpace(txtTiempo.Text) ||
                string.IsNullOrWhiteSpace(txtDescripcion.Text))
            {
                MessageBox.Show("Por favor, complete todos los campos requeridos.",
                                "Datos incompletos",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                return;
            }

            // Convertir los datos
            if (!int.TryParse(txtCategoria.Text, out int idCategoria))
            {
                MessageBox.Show("El ID de la categoría debe ser un número válido.",
                                "Error de validación",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                return;
            }

            string tiempoPreparacion = txtTiempo.Text;
            string descripcion = txtDescripcion.Text;
            string nombrePlatillo = txtPlatillo.Text;
            int Id_Producto = 0;


            // Conexión a la base de datos
            string connectionString = "Data Source=tcp:sqlproyecto2024.database.windows.net,1433;Initial Catalog=sqlproyecto;User ID=proyecto24;Password=Proyecto-24";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand("Agregar_Platillo", connection);
                command.CommandType = CommandType.StoredProcedure;

                // Configurar los parámetros del procedimiento almacenado
                command.Parameters.AddWithValue("@Id_Categoria", idCategoria);
                command.Parameters.AddWithValue("@TiempoPreparacion", tiempoPreparacion);
                command.Parameters.AddWithValue("@Descripcion", descripcion);
                command.Parameters.AddWithValue("@Nombre_Platillo", nombrePlatillo);
                command.Parameters.AddWithValue("@Id_Producto", Id_Producto);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    MessageBox.Show("Platillo insertado correctamente.",
                                    "Éxito",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Information);

                    // Limpiar los campos
                    txtCategoria.Text = "";
                    txtPlatillo.Text = "";
                    txtTiempo.Text = "";
                    txtDescripcion.Text = "";
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al insertar el platillo: {ex.Message}",
                                    "Error",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                }
            }
        }





        private void btnEditUser_Checked(object sender, RoutedEventArgs e)
        {
            MostrarEditUser();
            btnUser.IsChecked = false;

            // Validación de que el ID de usuario es un número válido
            if (string.IsNullOrEmpty(txtIdUsuarioEdit.Text) || !int.TryParse(txtIdUsuarioEdit.Text, out int idUsuario))
            {
                MessageBox.Show("Por favor, ingrese un ID de usuario válido.");
                return;
            }

                string connectionString = "Data Source=tcp:sqlproyecto2024.database.windows.net,1433;Initial Catalog=sqlproyecto;User ID=proyecto24;Password=Proyecto-24";
                string storedProcedure = "Editar_Usuario";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(storedProcedure, connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@Id_Usuario", idUsuario);
                command.Parameters.AddWithValue("@Usuario", txtNuevoNombreUsuario.Text);
                command.Parameters.AddWithValue("@Contrasena", txtNuevaContrasenia.Text);
                command.Parameters.AddWithValue("@Rol", txtNuevoRol.Text);

                try
                {
                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Usuario actualizado correctamente.");
                        txtIdUsuarioEdit.Text = "";
                        txtNuevoNombreUsuario.Text = "";
                        txtNuevaContrasenia.Text = "";
                        txtNuevoRol.Text = "";

                        CargarUsuarios();  // Opcional: Actualiza el grid de usuarios
                    }
                    else
                    {
                        MessageBox.Show("No se encontró el usuario con el ID especificado.");
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
            btnEditarBebida.Visibility = Visibility.Visible;
            OcultarFormEditBebidas();
            MostrarAdBebida();
            OcultarFormAgregarBebida();
        }

        private void btnAgregarMesa_Checked(object sender, RoutedEventArgs e)
        {
            btnAgregarMesa.IsChecked = false;
            string connectionString = "Data Source=tcp:sqlproyecto2024.database.windows.net,1433;Initial Catalog=sqlproyecto;User ID=proyecto24;Password=Proyecto-24";
            string storedProcedure = "Agregar_Mesa";

            string estadoInicial = "Disponible";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(storedProcedure, connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@Estado", estadoInicial);

                try
                {
                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Mesa agregada con éxito", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);

                        // Recargar los datos de las mesas
                        CargarMesas();

                        // Actualizar el DataGrid
                        grdMesas1.ItemsSource = null;
                        grdMesas1.ItemsSource = datosMesa;
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
                            cmd.Parameters.AddWithValue("@Id_Mesa", mesaSeleccionada.Mesa);

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

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validar campos obligatorios
                if (string.IsNullOrWhiteSpace(txtNombre.Text) ||
                    string.IsNullOrWhiteSpace(txtApellido.Text) ||
                    string.IsNullOrWhiteSpace(txtSalario.Text) ||
                    cmbMunicipio.SelectedValue == null)
                {
                    MessageBox.Show("Por favor, complete todos los campos obligatorios.",
                        "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Validar que el salario sea un número válido
                if (!decimal.TryParse(txtSalario.Text, out decimal salario))
                {
                    MessageBox.Show("El salario debe ser un número válido.",
                        "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                using (SqlConnection conn = new SqlConnection("Data Source=tcp:sqlproyecto2024.database.windows.net,1433;Initial Catalog=sqlproyecto;User ID=proyecto24;Password=Proyecto-24"))
                {
                    SqlCommand cmd;
                    if (btnGuardar.Tag?.ToString() == "nuevo")
                    {
                        // Usar el procedimiento de inserción para nuevo empleado
                        cmd = new SqlCommand("InsertarNuevoEmpleado", conn);
                    }
                    else
                    {
                        // Usar el procedimiento de edición para empleado existente
                        cmd = new SqlCommand("EditarEmpleado", conn);
                        cmd.Parameters.AddWithValue("@Id_Persona", int.Parse(txtIdPersona.Text));
                    }

                    cmd.CommandType = CommandType.StoredProcedure;

                    // Agregar los parámetros comunes
                    cmd.Parameters.AddWithValue("@Nombre", txtNombre.Text.Trim());
                    cmd.Parameters.AddWithValue("@Apellido", txtApellido.Text.Trim());
                    cmd.Parameters.AddWithValue("@Email", string.IsNullOrEmpty(txtEmail.Text) ? DBNull.Value : (object)txtEmail.Text.Trim());
                    cmd.Parameters.AddWithValue("@Direccion", string.IsNullOrEmpty(txtDireccion.Text) ? DBNull.Value : (object)txtDireccion.Text.Trim());
                    cmd.Parameters.AddWithValue("@Id_Municipio", cmbMunicipio.Text); // Enviamos el nombre del municipio
                    cmd.Parameters.AddWithValue("@Telefono", string.IsNullOrEmpty(txtTelefono.Text) ? DBNull.Value : (object)txtTelefono.Text.Trim());
                    cmd.Parameters.AddWithValue("@Salario", salario);
                    cmd.Parameters.AddWithValue("@Estado_Laboral", string.IsNullOrEmpty(txtEstadoLaboral.Text) ? DBNull.Value : (object)txtEstadoLaboral.Text.Trim());

                    conn.Open();

                    try
                    {
                        cmd.ExecuteNonQuery();
                        MessageBox.Show(btnGuardar.Tag?.ToString() == "nuevo" ?
                            "Empleado agregado exitosamente." :
                            "Empleado actualizado exitosamente.",
                            "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);

                        // Actualizar el DataGrid
                        CargarEmpleados();

                        // Limpiar y ocultar el formulario
                        LimpiarFormulario();
                        OcultarFormulario();
                        MostrarAdEmpleado();
                        btnGuardar.Tag = null; // Limpiar el tag
                    }
                    catch (SqlException sqlEx)
                    {
                        MessageBox.Show($"Error en la base de datos: {sqlEx.Message}",
                            "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar los cambios: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        // Método auxiliar para manejar valores nulos
        private object GetNullableValue(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? (object)DBNull.Value : (object)value.Trim();
        }


    }

}


