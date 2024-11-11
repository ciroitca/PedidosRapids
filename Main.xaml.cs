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


/*************** Falta un botón de cerrar sesión ***************************/


namespace PedidosRapids.Vista
{
    public partial class Main : Window
    {
        public Main()
        {
            InitializeComponent();
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
            MessageBoxResult result = MessageBox.Show("Esta seguro que desea cerrar sesión?", "CERRAR SESION", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                this.Close();
                login.Show();
            }
            else
            {
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
        }

        private void btnMainMenu_Checked(object sender, RoutedEventArgs e)
        {
            login login = new login();
            MessageBoxResult result = MessageBox.Show("Desea volver al menu Principal", "Volver", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                MostrarMenu();
                btnMainMenu.Visibility = Visibility.Hidden;
                btnAgOrden.Visibility = Visibility.Hidden;
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
        }

        private void btnMesas_Checked(object sender, RoutedEventArgs e)
        {
            OcultarParaMesas();
            lblMesas.Visibility = Visibility.Visible;
            grdMesas.Visibility = Visibility.Visible;
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
        }

        private void btnAgOrden_Click(object sender, RoutedEventArgs e)
        {
            OcultarParaAgOrden();
            btnMainMenu.Visibility = Visibility.Visible;
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
    }   
}


