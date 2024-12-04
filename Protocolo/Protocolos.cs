// ***************************************************************** 
// Practica 07
// Jordy Mayorga
// Fecha de realizacion: 27/11/2024
// Fecha de entrega: 04/12/2024
// Resultados: 
// Esta clase centraliza el procesamiento de comandos en el sistema cliente-servidor,
// actuando como intermediario entre ambas partes. Gestiona operaciones como la autenticación de usuarios,
// la validación de placas y el conteo de consultas, asegurando que los mensajes de solicitud y respuesta
// sean estructurados y coherentes. Al centralizar esta lógica, la clase facilita la extensión y
// mantenimiento del sistema, permitiendo agregar nuevos comandos de manera sencilla sin modificar
// el cliente o el servidor. Además, al utilizar clases como Pedido y Respuesta, la clase Protocolos
// asegura que la comunicación entre el cliente y el servidor sea consistente y menos propensa a
// errores de formato o interpretación.
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
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Protocolo
{
    public static class Protocolos
    {
        private static Dictionary<string, int> listadoClientes = new Dictionary<string, int>();

        public static string ResolverPedido(string mensaje, string direccionCliente)
        {
            var partes = mensaje.Split(' ');
            string comando = partes[0].ToUpper();
            string[] parametros = partes.Length > 1 ? partes.Skip(1).ToArray() : new string[0];

            string respuesta = "NOK Comando no reconocido";

            switch (comando)
            {
                case "INGRESO":
                    if (parametros.Length == 2 && parametros[0] == "root" && parametros[1] == "admin20")
                    {
                        respuesta = new Random().Next(2) == 0
                            ? "OK ACCESO_CONCEDIDO"
                            : "NOK ACCESO_NEGADO";
                    }
                    else
                    {
                        respuesta = "NOK ACCESO_NEGADO";
                    }
                    break;

                case "CALCULO":
                    if (parametros.Length == 3)
                    {
                        string placa = parametros[2];
                        if (ValidarPlaca(placa))
                        {
                            byte indicadorDia = ObtenerIndicadorDia(placa);
                            respuesta = $"OK {placa} {indicadorDia}";
                            ContadorCliente(direccionCliente);
                        }
                        else
                        {
                            respuesta = "NOK Placa no válida";
                        }
                    }
                    break;

                case "CONTADOR":
                    if (listadoClientes.ContainsKey(direccionCliente))
                    {
                        respuesta = $"OK {listadoClientes[direccionCliente]}";
                    }
                    else
                    {
                        respuesta = "NOK No hay solicitudes previas";
                    }
                    break;
            }

            return respuesta;
        }

        public static string HazOperacion(string mensaje)
        {
            // Esta función simula la comunicación, y en un caso real iría aquí la lógica de conexión
            // con el servidor para obtener la respuesta.
            return ResolverPedido(mensaje, "ClienteLocal");
        }

        private static bool ValidarPlaca(string placa)
        {
            return Regex.IsMatch(placa, @"^[A-Z]{3}[0-9]{4}$");
        }

        private static byte ObtenerIndicadorDia(string placa)
        {
            int ultimoDigito = int.Parse(placa.Substring(6, 1));
            switch (ultimoDigito)
            {
                case 1:
                    return 0b00100000; // Lunes
                case 2:
                    return 0b00100000; // Lunes
                case 3:
                    return 0b00010000; // Martes
                case 4:
                    return 0b00010000; // Martes
                case 5:
                    return 0b00001000; // Miércoles
                case 6:
                    return 0b00001000; // Miércoles
                case 7:
                    return 0b00000100; // Jueves
                case 8:
                    return 0b00000100; // Jueves
                case 9:
                    return 0b00000010; // Viernes
                case 0:
                    return 0b00000010; // Viernes
                default:
                    return 0;
            }
        }

        private static void ContadorCliente(string direccionCliente)
        {
            if (listadoClientes.ContainsKey(direccionCliente))
            {
                listadoClientes[direccionCliente]++;
            }
            else
            {
                listadoClientes[direccionCliente] = 1;
            }
        }
    }
}

