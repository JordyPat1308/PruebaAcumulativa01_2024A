
// ***************************************************************** 
// Practica 07
// Jordy Mayorga
// Fecha de realizacion: 27/11/2024
// Fecha de entrega: 04/12/2024
// Resultados: 
//El servidor implementado demuestra cómo manejar múltiples conexiones de clientes a través de TCP y
//cómo procesar solicitudes mediante un protocolo sencillo. Cada cliente realiza una conexión, envía
//comandos con parámetros y recibe respuestas. En el servidor, la responsabilidad de recibir y enviar
//datos está claramente separada de la lógica de negocios, que es gestionada por la clase Protocolo.
//Esto hace que el servidor sea eficiente en cuanto a la gestión de recursos y fácil de extender con
//nuevos tipos de comandos sin modificar la estructura principal del código.
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
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Protocolo;

namespace Servidor
{
    class Servidor
    {
        private static TcpListener escuchador;

        static void Main(string[] args)
        {
            try
            {
                escuchador = new TcpListener(IPAddress.Any, 8080);
                escuchador.Start();
                Console.WriteLine("Servidor inició en el puerto 8080...");

                while (true)
                {
                    TcpClient cliente = escuchador.AcceptTcpClient();
                    Console.WriteLine("Cliente conectado, puerto: {0}", cliente.Client.RemoteEndPoint.ToString());
                    Thread hiloCliente = new Thread(ManipuladorCliente);
                    hiloCliente.Start(cliente);
                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine("Error de socket al iniciar el servidor: " +
                    ex.Message);
            }
            finally
            {
                escuchador?.Stop();
            }
        }

        private static void ManipuladorCliente(object obj)
        {
            TcpClient cliente = (TcpClient)obj;
            NetworkStream flujo = null;
            try
            {
                flujo = cliente.GetStream();
                byte[] bufferTx;
                byte[] bufferRx = new byte[1024];
                int bytesRx;

                while ((bytesRx = flujo.Read(bufferRx, 0, bufferRx.Length)) > 0)
                {
                    string mensajeRx = Encoding.UTF8.GetString(bufferRx, 0, bytesRx);
                    string respuesta = Protocolos.ResolverPedido(mensajeRx, cliente.Client.RemoteEndPoint.ToString());

                    Console.WriteLine("Se recibio: " + mensajeRx);
                    Console.WriteLine("Se envió: " + respuesta);

                    bufferTx = Encoding.UTF8.GetBytes(respuesta);
                    flujo.Write(bufferTx, 0, bufferTx.Length);
                }

            }
            catch (SocketException ex)
            {
                Console.WriteLine("Error de socket al manejar el cliente: " + ex.Message);
            }
            finally
            {
                flujo?.Close();
                cliente?.Close();
            }
        }
    }
}

