using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Terminations;
using GeneticSharp.Domain;
using GeneticSharp.Domain.Fitnesses;
using GeneticSharp.Domain.Reinsertions;
/*
    Universidad Autonoma del Estado de México
    Autor: Juan Carlos Ángeles Vázquez 
    Profesor: Asrubal Lopez Chau
    
    El Algoritmo genetico desarrollado , se encargara de 
    generar una aproximacion lo mas exacta posible 
    a la palabra introducida por el usuario(Palabra con solo mayusculas y sin espacios) 
    
     
*/
namespace HolaMundoAG
{
    class Program
    {
        static void Main(string[] args)
        {
            string cadena;//cadena leida por el usuario
            int tam_cadena;//tamaño de la cadena introducida
            string cadenana = "";//cadena resultante generada por el algoritmo 
            Console.WriteLine("Introduzca la cadena");
            cadena = Console.ReadLine();
            cadena = cadena.Trim();//se eliminan espacios de la cadena introducida
            cadena = cadena.ToLower();//se pasa a minusculas todas las letras
            tam_cadena = cadena.Length;//se obtiene le tamaño de la caden 
            double[] min = new double[tam_cadena];//se generan
            double[] max = new double[tam_cadena];//rangos minimos y maximos para que trabaje el AG 
            int[] arr = new int[tam_cadena];//se generan los arreglos
            int[] arr2 = new int[tam_cadena];// de numero de digits , bits
            
            for (int i = 0; i < tam_cadena; i++)//se debe de llenar una posicion del arreglo por cada gen(cada caracter de la cadena )
            {
                min[i] = 97;//minimo -> a 
                max[i] = 122;//maximo -> z
                arr[i] = 64;
                arr2[i] = 3;
            }
            var cromosoma = new FloatingPointChromosome(min,max,arr, arr2);//se instancia la nueva cromosoma

            var aptitud = new FuncFitness((c) => {//se define la funcion de aptitud

                var fc = c as FloatingPointChromosome;
                var x = fc.ToFloatingPoints();
                string cad_obt="";
                double porcentaje;
                for (int i = 0; i < tam_cadena; i++)//para cada gen
                    cad_obt += (char)Convert.ToInt32(x[i]);//se debe de concatenar a la cadena obtenida, convirtiendo de float a int y despues
                    //casteando a caracter
                double dist = Math.Exp(-LevenshteinDistance(cadena, cad_obt, out porcentaje));//se realiza el calculo
                return 1f/1f+ dist ;//de la aptitud
            });
            var poblacion = new Population(100, 1000, cromosoma);//se crea la poblacon con minimo 100 individuos y maximo 1000
            var cruza = new UniformCrossover(0.3f);//se define como se realizara la cruza
            var mutacion = new FlipBitMutation();//como se realizara la mutacion
            var seleccion = new RouletteWheelSelection();//se elige el metodo de seleccion a utilizar
            var terminacion = new FitnessStagnationTermination(1000);//se determina el parametro de termino para el algoritmo genetico
            //var ga = new GeneticSharp
            var ga = new GeneticAlgorithm(poblacion, aptitud, seleccion, cruza, mutacion);//se crea el algoritmo genetico
            ga.Termination = terminacion;//se asigna la condicion de termino a el AG 

            var latestFitness = 0.0;//variable para guardar fitness anterior 
            ga.GenerationRan += (sender, e) =>//funcion para estar verificando como es que evoluciona el AG 
            {

                var bestchromosome = ga.BestChromosome as FloatingPointChromosome;//se obtiene la cromosoma 
                var bestFitness = bestchromosome.Fitness.Value;//se obtiene el fitness de la cromosoma

                if (bestFitness != latestFitness)//se comprueba que el fitness actual sea distinto al anterior
                {
                    latestFitness = bestFitness;//el anterior fitness se vuelve el mejor ahora, para asi comprobar que le fitness mejora
                   // var phenotype = bestchromosome.ToFloatingPoints();//se obtienen los valores que reprsenta la cromosoma

                    Console.WriteLine("Generation {0}", ga.GenerationsNumber);//se imprime el numero de generacion
                    // Console.WriteLine("X = {0}", bestchromosome.ToFloatingPoint());
                    var x = bestchromosome.ToFloatingPoints();//se obtienen los valores que reprsenta la cromosoma
                    cadenana = "";
                    for (int i = 0; i < tam_cadena; i++)//se obtiene el valor
                        cadenana += (char) Convert.ToInt32(x[i]);// que representa el fenotipo en terminos de caracteres para 
                        //formar la cadena resultante
                    
                  //  Console.WriteLine("F(x) =  {0}", (char)((int)bestFitness));
                    Console.WriteLine("Palabra Obtenida " + cadenana);//imprime la mejor aproximacion hasta ahora
                }

            };
            ga.Start();//se Inicia el AG 
            Console.WriteLine("\nEjecucion Terminada \n");
            Console.WriteLine("La Mejor Aproximacion a la solucion encontrada es :\n " + cadenana);//se imprime la mejor solucion encontrada
            Console.ReadLine();//esto es para hacer un pausa y se pueda ver el resultado tranquilamente
        }

        public static int LevenshteinDistance(string s, string t, out double porcentaje)
        {
            porcentaje = 0;

            // d es una tabla con m+1 renglones y n+1 columnas
            int costo = 0;
            int m = s.Length;
            int n = t.Length;
            int[,] d = new int[m + 1, n + 1];

            // Verifica que exista algo que comparar
            if (n == 0) return m;
            if (m == 0) return n;

            // Llena la primera columna y la primera fila.
            for (int i = 0; i <= m; d[i, 0] = i++) ;
            for (int j = 0; j <= n; d[0, j] = j++) ;


            /// recorre la matriz llenando cada unos de los pesos.
            /// i columnas, j renglones
            for (int i = 1; i <= m; i++)
            {
                // recorre para j
                for (int j = 1; j <= n; j++)
                {
                    /// si son iguales en posiciones equidistantes el peso es 0
                    /// de lo contrario el peso suma a uno.
                    costo = (s[i - 1] == t[j - 1]) ? 0 : 1;
                    d[i, j] = System.Math.Min(System.Math.Min(d[i - 1, j] + 1,  //Eliminacion
                                  d[i, j - 1] + 1),                             //Insercion 
                                  d[i - 1, j - 1] + costo);                     //Sustitucion
                }
            }

            /// Calculamos el porcentaje de cambios en la palabra.
            if (s.Length > t.Length)
                porcentaje = ((double)d[m, n] / (double)s.Length);
            else
                porcentaje = ((double)d[m, n] / (double)t.Length);
            return d[m, n];
        }
    }
}
