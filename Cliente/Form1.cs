// ***************************************************************** 
// Practica 07
// Jordy Mayorga
// Fecha de realizacion: 27/11/2024
// Fecha de entrega: 04/12/2024
// Resultados: 
//El cliente es capaz de conectarse al servidor y realizar diferentes operaciones, como el inicio de sesión
//y la consulta de información, usando un protocolo basado en comandos. La interfaz gráfica permite al usuario
//interactuar con el sistema, y el cliente se comunica con el servidor enviando cadenas de texto.
//Este diseño hace que el cliente sea fácil de utilizar y que el sistema sea flexible en términos
//de extensibilidad, permitiendo agregar nuevas funcionalidades sin complicar demasiado el flujo de trabajo.
// Conclusiones: 
// *En conclusión, la práctica permite comprender cómo GitHub facilita la gestión de versiones de un proyecto,
// permitiendo llevar un control de los cambios realizados en el código, colaborar de manera eficiente y revertir
// a versiones anteriores cuando sea necesario
// *Se puede concluir que, a través de esta práctica, se refuerza la utilidad de GitHub no solo como una
// herramienta de almacenamiento de código, sino también como una plataforma de integración continua, al
// integrar el código de Visual Studio y asegurar que los cambios sean monitoreados y gestionados adecuadamente.

// Recomendaciones: 
// *Es importante realizar commits frecuentes con mensajes claros y descriptivos para mantener un historial
// detallado de los cambios y facilitar el seguimiento del progreso del proyecto.
// *La integración de GitHub con Visual Studio mejora significativamente la eficiencia en el desarrollo de
// software, permitiendo una gestión de versiones más ordenada y un flujo de trabajo más ágil. Esto facilita
// la colaboración entre desarrolladores y asegura un control de cambios más preciso y organizado.
// ************************************************************************
using System;
using System.Text;
using System.Net.Sockets;
using System.Windows.Forms;
using Protocolo;

namespace Cliente
{
    public partial class FrmValidador : Form
    {
        private TcpClient remoto;
        private NetworkStream flujo;

        public FrmValidador()
        {
            InitializeComponent();
        }

        private void FrmValidador_Load(object sender, EventArgs e)
        {
            try
            {
                remoto = new TcpClient("127.0.0.1", 8080);
                flujo = remoto.GetStream();
            }
            catch (SocketException ex)
            {
                MessageBox.Show("No se pudo establecer conexión: " + ex.Message, "ERROR");
            }
            finally
            {
                flujo?.Close();
                remoto?.Close();
            }

            // Deshabilitar controles
            panPlaca.Enabled = false;
            chkLunes.Enabled = false;
            chkMartes.Enabled = false;
            chkMiercoles.Enabled = false;
            chkJueves.Enabled = false;
            chkViernes.Enabled = false;
            chkDomingo.Enabled = false;
            chkSabado.Enabled = false;
        }

        private void btnIniciar_Click(object sender, EventArgs e)
        {
            string usuario = txtUsuario.Text;
            string contraseña = txtPassword.Text;
            if (usuario == "" || contraseña == "")
            {
                MessageBox.Show("Se requiere el ingreso de usuario y contraseña", "ADVERTENCIA");
                return;
            }

            string respuesta = Protocolos.HazOperacion($"INGRESO {usuario} {contraseña}");
            if (respuesta == null)
            {
                MessageBox.Show("Hubo un error", "ERROR");
                return;
            }

            if (respuesta.Contains("OK ACCESO_CONCEDIDO"))
            {
                panPlaca.Enabled = true;
                panLogin.Enabled = false;
                MessageBox.Show("Acceso concedido", "INFORMACIÓN");
                txtModelo.Focus();
            }
            else
            {
                panPlaca.Enabled = false;
                panLogin.Enabled = true;
                MessageBox.Show("No se pudo ingresar, revise credenciales", "ERROR");
                txtUsuario.Focus();
            }
        }

        private void btnConsultar_Click(object sender, EventArgs e)
        {
            string modelo = txtModelo.Text;
            string marca = txtMarca.Text;
            string placa = txtPlaca.Text;

            string respuesta = Protocolos.HazOperacion($"CALCULO {modelo} {marca} {placa}");
            if (respuesta == null)
            {
                MessageBox.Show("Hubo un error", "ERROR");
                return;
            }

            if (respuesta.Contains("NOK"))
            {
                MessageBox.Show("Error en la solicitud.", "ERROR");
                chkLunes.Checked = false;
                chkMartes.Checked = false;
                chkMiercoles.Checked = false;
                chkJueves.Checked = false;
                chkViernes.Checked = false;
            }
            else
            {
                MessageBox.Show("Se recibió: " + respuesta, "INFORMACIÓN");
                byte resultado = Byte.Parse(respuesta.Split(' ')[2]);
                // Lógica para actualizar los checkboxes según el día
                // // Lógica para actualizar los checkboxes según el día
                
                chkLunes.Checked = (resultado & 0b00100000) != 0;
                chkMartes.Checked = (resultado & 0b00010000) != 0;
                chkMiercoles.Checked = (resultado & 0b00001000) != 0;
                chkJueves.Checked = (resultado & 0b00000100) != 0;
                chkViernes.Checked = (resultado & 0b00000010) != 0;

            }
        }

        private void btnNumConsultas_Click(object sender, EventArgs e)
        {
            string respuesta = Protocolos.HazOperacion("CONTADOR hola");
            if (respuesta == null)
            {
                MessageBox.Show("Hubo un error", "ERROR");
                return;
            }

            MessageBox.Show("Número de pedidos recibidos: " + respuesta, "INFORMACIÓN");
        }

        private void FrmValidador_FormClosing(object sender, FormClosingEventArgs e)
        {
            flujo?.Close();
            remoto?.Close();
        }
    }
}
