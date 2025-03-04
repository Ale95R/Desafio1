using System;
using System.Collections.Generic;

namespace GestionHotel
{
    public class Habitacion
    {
        public int NumeroHabitacion { get; set; }
        public string Tipo { get; set; }          
        public decimal PrecioPorNoche { get; set; }
        public bool EstaDisponible { get; set; }

        public Habitacion(int numero, string tipo, decimal precio)
        {
            NumeroHabitacion = numero;
            Tipo = tipo;
            PrecioPorNoche = precio;
            EstaDisponible = true; 
        }
    }

    public class Cliente
    {
        public string Nombre { get; set; }
        public string DocumentoIdentidad { get; set; }
        public string Telefono { get; set; }

        public Cliente(string nombre, string documento, string telefono)
        {
            Nombre = nombre;
            DocumentoIdentidad = documento;
            Telefono = telefono;
        }

        public override string ToString()
        {
            return $"{Nombre} (Doc: {DocumentoIdentidad}, Tel: {Telefono})";
        }
    }

    public class Reservacion
    {
        public Cliente Cliente { get; set; }
        public Habitacion Habitacion { get; set; }
        public int CantidadNoches { get; set; }
        public decimal MontoTotal { get; set; }

        public Reservacion(Cliente cliente, Habitacion habitacion, int noches)
        {
            Cliente = cliente;
            Habitacion = habitacion;
            CantidadNoches = noches;
            MontoTotal = habitacion.PrecioPorNoche * noches;
        }

        public override string ToString()
        {
            return $"Reservación de {Cliente.Nombre} en Habitación #{Habitacion.NumeroHabitacion}, " +
                   $"Tipo: {Habitacion.Tipo}, Noches: {CantidadNoches}, " +
                   $"Monto Total: {MontoTotal:C2}";
        }
    }

    class Program
    {
        static Habitacion[,] habitaciones = new Habitacion[5, 10];

        static List<Cliente> listaClientes = new List<Cliente>();

        static Dictionary<int, Reservacion> dicReservaciones = new Dictionary<int, Reservacion>();

        static void Main(string[] args)
        {

            InicializarHabitaciones();

            bool salir = false;
            while (!salir)
            {

                Console.WriteLine("\n");
                Console.WriteLine("\n--- SISTEMA DE GESTIÓN DE RESERVACIONES ---");
                Console.WriteLine("1. Registrar Cliente");
                Console.WriteLine("2. Mostrar Disponibilidad de Habitaciones");
                Console.WriteLine("3. Crear Reservación");
                Console.WriteLine("4. Consultar Reservaciones Activas");
                Console.WriteLine("5. Cancelar una Reservación");
                Console.WriteLine("6. Salir");
                Console.Write("Seleccione una opción: ");

                string opcionMenu = Console.ReadLine() ?? string.Empty;

                switch (opcionMenu)
                {
                    case "1":
                        RegistrarCliente();
                        break;
                    case "2":
                        MostrarDisponibilidad();
                        break;
                    case "3":
                        CrearReservacion();
                        break;
                    case "4":
                        ConsultarReservaciones();
                        break;
                    case "5":
                        CancelarReservacion();
                        break;
                    case "6":
                        salir = true;
                        break;
                    default:
                        Console.WriteLine("Opción inválida. Intente de nuevo.");
                        break;
                }
            }
        }

        static void InicializarHabitaciones()
        {

            string[] tipos = { "Sencilla", "Doble", "Suite" };
            decimal[] precios = { 50m, 80m, 120m };

            for (int piso = 0; piso < 5; piso++)
            {
                for (int habIndex = 0; habIndex < 10; habIndex++)
                {

                    int numero = (piso + 1) * 100 + (habIndex + 1);

                    string tipo = tipos[habIndex % 3];
                    decimal precio = precios[habIndex % 3];

                    habitaciones[piso, habIndex] = new Habitacion(numero, tipo, precio);
                }
            }
        }

        static void RegistrarCliente()
        {
            Console.Clear();
            Console.WriteLine("\n--- REGISTRAR CLIENTE ---");
            Console.Write("Nombre: ");
            string nombre = Console.ReadLine() ?? string.Empty;

            Console.Write("Documento de Identidad: ");
            string doc = Console.ReadLine() ?? string.Empty;

            Console.Write("Teléfono: ");
            string telefono = Console.ReadLine() ?? string.Empty;

            Cliente nuevoCliente = new Cliente(nombre, doc, telefono);
            listaClientes.Add(nuevoCliente);

            Console.WriteLine("Cliente registrado exitosamente.");
        }

        static void MostrarDisponibilidad()
        {
            Console.Clear();
            Console.WriteLine("\n--- DISPONIBILIDAD DE HABITACIONES ---");
            for (int piso = 0; piso < 5; piso++)
            {
                Console.WriteLine($"\nPiso {piso + 1}:");
                for (int habIndex = 0; habIndex < 10; habIndex++)
                {
                    var hab = habitaciones[piso, habIndex];
                    string estado = hab.EstaDisponible ? "Disponible" : "Ocupada";

                    Console.WriteLine(
                        $"{"Hab.#" + hab.NumeroHabitacion,-12}" +
                        $"{hab.Tipo,-10}" +
                        $"{estado + ",",-12}" +
                        $"{hab.PrecioPorNoche,10:C2}"
                    );
                }
            }
        }


        static void CrearReservacion()
        {
            Console.Clear();
            Console.WriteLine("\n--- CREAR RESERVACIÓN ---");

            if (listaClientes.Count == 0)
            {
                Console.WriteLine("No hay clientes registrados. Registre un cliente primero.");
                return;
            }

            Console.WriteLine("Seleccione un cliente:");
            for (int i = 0; i < listaClientes.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {listaClientes[i]}");
            }
            Console.Write("Ingrese el número del cliente: ");
            string opcionClienteStr = Console.ReadLine() ?? string.Empty;

            if (!int.TryParse(opcionClienteStr, out int indiceCliente)
               || indiceCliente < 1
               || indiceCliente > listaClientes.Count)
            {
                Console.WriteLine("Selección inválida.");
                return;
            }

            Cliente clienteSeleccionado = listaClientes[indiceCliente - 1];

            Console.WriteLine("Ingrese el número de la habitación que desea reservar:");
            Console.WriteLine("Sencillas= *01,*04,*07,*10 ($50)");
            Console.WriteLine("Doble=     *02,*05,*08     ($80)");
            Console.WriteLine("Suite=     *03,*06,*09     ($120)");
            string numHabStr = Console.ReadLine() ?? string.Empty;
            if (!int.TryParse(numHabStr, out int numHab))
            {
                Console.WriteLine("Número de habitación inválido.");
                return;
            }

            Habitacion? habitacionSeleccionada = null;
            for (int piso = 0; piso < 5; piso++)
            {
                for (int habIndex = 0; habIndex < 10; habIndex++)
                {
                    if (habitaciones[piso, habIndex].NumeroHabitacion == numHab)
                    {
                        habitacionSeleccionada = habitaciones[piso, habIndex];
                        break;
                    }
                }
                if (habitacionSeleccionada != null) break;
            }

            if (habitacionSeleccionada == null)
            {
                Console.WriteLine("La habitación ingresada no existe.");
                return;
            }
            else if (!habitacionSeleccionada.EstaDisponible)
            {
                Console.WriteLine("La habitación ya está ocupada.");
                return;
            }

            Console.Write("Ingrese la cantidad de noches: ");
            string nochesStr = Console.ReadLine() ?? string.Empty;
            if (!int.TryParse(nochesStr, out int noches) || noches <= 0)
            {
                Console.WriteLine("Cantidad de noches inválida.");
                return;
            }

            Reservacion nuevaReservacion = new Reservacion(clienteSeleccionado, habitacionSeleccionada, noches);

            dicReservaciones.Add(habitacionSeleccionada.NumeroHabitacion, nuevaReservacion);

            habitacionSeleccionada.EstaDisponible = false;

            Console.WriteLine($"Reservación creada exitosamente. Monto total: {nuevaReservacion.MontoTotal:C2}");
        }

        static void ConsultarReservaciones()
        {
            Console.Clear();
            Console.WriteLine("\n--- RESERVACIONES ACTIVAS ---");

            if (dicReservaciones.Count == 0)
            {
                Console.WriteLine("No hay reservaciones activas.");
                return;
            }

            foreach (var kvp in dicReservaciones)
            {
                Console.WriteLine(kvp.Value);
            }
        }

        static void CancelarReservacion()
        {
            Console.Clear();
            Console.WriteLine("\n--- CANCELAR RESERVACIÓN ---");
            Console.Write("Ingrese el número de la habitación que desea liberar: ");

            string numHabStr = Console.ReadLine() ?? string.Empty;
            if (!int.TryParse(numHabStr, out int numHab))
            {
                Console.WriteLine("Número de habitación inválido.");
                return;
            }

            if (dicReservaciones.TryGetValue(numHab, out Reservacion? reservacion))
            {

                dicReservaciones.Remove(numHab);

                reservacion.Habitacion.EstaDisponible = true;

                Console.WriteLine($"La reservación de {reservacion.Cliente.Nombre} en la Habitación #{numHab} ha sido cancelada.");
            }
            else
            {
                Console.WriteLine("No existe una reservación activa para ese número de habitación.");
            }
        }
    }
}
