// Importaciones
using System;


// Métodos que vamos a usar

void printearMenu()
{
    Console.WriteLine("--------------");
    Console.WriteLine("Elige una opción:");
    Console.WriteLine("1. Ver los resultados de un equipo");
    Console.WriteLine("2. Ver los resultados de una jornada");
    Console.WriteLine("3. Ver el top de los MVPs");
    Console.WriteLine("4. Ver clasificación");
    Console.WriteLine("5. Guardar los resultados en los ficheros");
    Console.WriteLine("6. Salir");
    Console.WriteLine("--------------");

}

String[] cargarFichero()
{
    return File.ReadAllLines("acb2526.csv");
}

void escribirFichero(String[] lineas)
{
    Console.WriteLine("¿Quiéres guardar los datos en un fichero? S/N");
    String opt = Console.ReadLine();
    if(opt == "S")
    {
        List<string> aFichero = new List<string>(lineas);
        Console.WriteLine("¿Qué nombre le quieres poner al fichero?");
        String nombre = Console.ReadLine();
        File.WriteAllLines(nombre + ".txt", aFichero);
    } 
    
}

String[] conseguirEquipos()
{
    String[] lineas = cargarFichero();

    List<string> equipos = new List<String>();
    foreach (String linea in lineas)
    {
        String eq = linea.Split(",")[1];
        if (!equipos.Contains(eq) && eq != "equipo_local")
        {
            equipos.Add(eq);
        }
    }
    
    return equipos.ToArray();
}

String[] printearResultados(String equipo, bool aFichero)
{
    String[] lineas = cargarFichero();

    String path = equipo.Trim() + ".txt";
    List<string> lineasEscribir = new List<string>();

    foreach(String linea in lineas)
    {
        String[] partido = linea.Split(",");
        if(partido[1] == equipo || partido[2] == equipo) // Si el equipo introducido es local o visitante
        {
            String resultado = partido[1] + " " + partido[3] + " " + partido[2];
            Console.WriteLine(resultado);
            if (aFichero)
            {
                // Si esta opción está activa vamos a guardar en un fichero los datos
                lineasEscribir.Add(resultado);
            }
        }
    }

    if(aFichero)
    {
        File.WriteAllLines(path, lineasEscribir);
    }

    return lineasEscribir.ToArray();
    
}

void resultadosEquipo()
{

    String[] equipos = conseguirEquipos();
    Console.WriteLine("Elige uno entre los equipos de la siguiente lista: ");

    foreach (String eq in equipos) // Visualizamos la lista de equipos que hay en la liga
    {
        Console.WriteLine(eq);
    }

    String equipo = Console.ReadLine();


    if (!equipos.Contains(equipo))
    {
        Console.WriteLine("Equipo no encontrado, elige un equipo de la lista, por favor...");
        resultadosEquipo();
    }
    else
    {
        String[] resultados = printearResultados(equipo, false);
        escribirFichero(resultados);
    }
}

void printearResultadosJornada(String jornada)
{
    String[] lineas = cargarFichero();

    List<string> lst = new List<string>();

    Console.WriteLine("Has elegido la jornada: " + jornada);
    foreach(String linea in lineas)
    {
        String[] partido = linea.Split(",");
        if(partido[0] == "Jornada " + jornada)
        {
            string res = partido[1] + " " + partido[3] + " " + partido[2];
            Console.WriteLine(res);
            lst.Add(res);
        }
    }

    escribirFichero(lst.ToArray());
}


void resultadosJornada()
{
    Console.WriteLine("Introduce los datos de la jornada que quieres saber (Recuerda que solo hay 34 jornadas)");

    try
        {
            int jornada = Int32.Parse(Console.ReadLine());
            if(jornada < 35)
            {
                printearResultadosJornada(jornada.ToString());
            } else
            {
                Console.WriteLine("La jornada que has elegido no existe, elige una jornada válida por favor");
                resultadosJornada();
            }
        }
        catch (FormatException)
        {
            Console.WriteLine("Has metido un valor que no es un número");
            resultadosJornada();
        }
    
}

void mvps()
{
    String[] lineas = cargarFichero();

    // Voy a usar un array asociativo en el que la clave va a ser el nombre del jugador y el valor la cantidad de veces que ha sido
    Dictionary<String, int> jugadores = new Dictionary<string, int>();
    List<String> res = new List<string>();

    foreach(String linea in lineas)
    {
        String[] partido = linea.Split(',');
        if(jugadores.ContainsKey(partido[4]) && partido[4] != "mvp_partido")
        
            jugadores[partido[4]] += 1;
        else
            jugadores[partido[4]] = 1;

    }

    Dictionary<String, int> topTres = jugadores
             .OrderByDescending(pair => pair.Value) // Ordena por valor de mayor a menor
            .Take(3)                              // Se queda con los 3 primeros 
            .ToDictionary(pair => pair.Key, pair => pair.Value);    

    Console.WriteLine("El top 3 de MVPs es el siguiente:");
    int cont = 1;
    foreach (var par in topTres)
    {
        String act = cont + ". " + par.Key + " (" + par.Value + " apariciones)";
        res.Add(act);
        Console.WriteLine(act);
        cont++;
    }

    escribirFichero(res.ToArray());
}

void clasificacion()
{
    // Creamos el diccionario y lo inicializamos con todos los equipos a 0 victorias
    Dictionary<String, int> victorias = new Dictionary<string, int>();
    Dictionary<String, int> derrotas = new Dictionary<string, int>();

    String[] nombreEquipos = conseguirEquipos();

    foreach(String eq in nombreEquipos)
    {
        victorias[eq] = 0;
        derrotas[eq] = 0;
    }

    // Vamos jornada por jornada actualizado las victorias y las derrotas
    String[] lineas = cargarFichero();
    foreach(String partido in lineas)
    {
        String[] arrPart = partido.Split(',');
        if(arrPart[0] != "jornada")
        {
            int resLocal = Int32.Parse(arrPart[3].Split('-')[0]);
            int resVisitante = Int32.Parse(arrPart[3].Split('-')[1]);

            if(resLocal > resVisitante)
            {
                victorias[arrPart[1]] +=1;
                derrotas[arrPart[2]] +=1;
            } else
            {
                victorias[arrPart[2]] +=1;
                derrotas[arrPart[1]] +=1;
            }
        }
    }

    // Ordenamos el diccionario

    Dictionary<String, int> clasificacion = victorias
        .OrderByDescending(pair => pair.Value) // Ordena por valor de mayor a menor
        .ToDictionary(pair => pair.Key, pair => pair.Value); 

    List<String> res = new List<string>(); 

    int cont = 1;
    foreach(var par in clasificacion)
    {
        String act = cont + ". " + par.Key + " " + par.Value + "V-" + derrotas[par.Key] + "D";
        Console.WriteLine(act);
        res.Add(act);
        cont++;
    } 

    escribirFichero(res.ToArray());
    
}

void resultadosAFicheros()
{
    String[] equipos = conseguirEquipos();
    foreach(String eq in equipos)
    {
        printearResultados(eq, true);
    }


}

String opt = "";

while (opt != "6")
{
    printearMenu();
    Console.WriteLine("Elige una opción entre el 1 y el 5");

    opt = Console.ReadLine();

    switch (opt)
    {
        case "1": // Resultados de un equipo
            resultadosEquipo();
            break;
        case "2": // Resultados de una jornada
            resultadosJornada();
            break;
        case "3": // Top de MVPs
            mvps();
            break;
        case "4": // Clasificación
            clasificacion();
            break;
        case "5":
            resultadosAFicheros();
            break;
        case "6":
            Console.WriteLine("Saliendo del programa");
            break;
        default:
            Console.WriteLine("Opción no encontrada");
            break;

    }
}