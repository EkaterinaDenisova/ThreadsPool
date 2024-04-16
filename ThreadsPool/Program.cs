using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

// author: Денисова Екатерина

namespace ThreadsPool
{
    public class Program
    {
        // функция для заполнения матрицы случайными числами
        static void FillMatrixRandom(int[,] matrix, Random rand)
        {
            //Random rand = new Random();
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    matrix[i, j] = rand.Next(1, 101); // генерация случайного числа от 1 до 100
                }
            }
        }

        // метод для перемножения матриц
        // матрицы matrixA и matrixB - исходные матрицы 
        // результат перемножения записывается в матрицу result
        // i, j - индекс строки и столбца элемента, значение которого вычисляется
        // countdownEvent - 
        static void MultiplyCell(object state)
        {
            /*
            int[] indexes = (int[])state;
            int[,] matrixA = (int[,])indexes[2];
            int[,] matrixB = (int[,])indexes[3];
            int[,] result = (int[,])indexes[4];
            CountdownEvent countdownEvent = (CountdownEvent)indexes[5];*/


            (int i, int j, int[,] matrixA, int[,] matrixB, int[,] result, CountdownEvent countdownEvent) = ((int, int, int[,], int[,], int[,], CountdownEvent)) state;

            //Console.WriteLine($"Поток {Thread.CurrentThread.ManagedThreadId} для элемента [{i}, {j}] запустился");
            //result[i, j] = 0;
            //object lockObject = new object();
            for (int k = 0; k < matrixA.GetLength(1); k++)
            {
                //lock (lockObject)
                //{
                    result[i, j] += matrixA[i, k] * matrixB[k, j];
                //}
            }

            //Thread.Sleep(100); 
            //Console.WriteLine($"Поток {Thread.CurrentThread.ManagedThreadId} для элемента [{i}, {j}] завершился");


            // каждый вызов Signal уменьшает значение счетчика на 1
            countdownEvent.Signal();
        }


        static void Main(string[] args)
        {

            int M = 1000; // количество строк в матрицах
            int N = 1000; // количество столбцов в матрицах
            int K = 1000; // общее количество столбцов в первой и строк во второй матрицах

            int[,] matrixA = new int[M, K];
            int[,] matrixB = new int[K, N];
            int[,] result = new int[M, N];

            // примитив синхронизации, снимающий блокировку потока в состоянии ожидания после определенного числа сигналов
            CountdownEvent countdownEvent = new CountdownEvent(matrixA.GetLength(0) * matrixB.GetLength(1));

            Random rand = new Random();
            FillMatrixRandom(matrixA, rand);
            FillMatrixRandom(matrixB, rand);

            DateTime start_time = DateTime.Now; // начало измерения времени

            for (int i = 0; i < M; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    // QueueUserWorkItem помещает метод MultiplyCell в очередь на выполнение.
                    // метод выполняется, когда становится доступен поток из пула потоков.
                    // () - объект, в котором передаются аргументы для метода MultiplyCell
                    ThreadPool.QueueUserWorkItem(MultiplyCell, ( i, j, matrixA, matrixB, result, countdownEvent));
                }
            }

            // в основном потоке вызов Wait блокируется до тех пор, пока значение счетчика не достигнет нуля
            countdownEvent.Wait();

            DateTime end_time = DateTime.Now; // конец измерения времени

            // время перемножения в миллисекундах
            int duration = (int)(end_time - start_time).TotalMilliseconds;
            Console.WriteLine($"Время перемножения матриц в миллисекундах: {duration}");

            /*
            Console.WriteLine("Матрица A:");
            for (int i = 0; i < M; i++)
            {
                for (int j = 0; j < K; j++)
                {
                    Console.Write(matrixA[i, j] + " ");
                }
                Console.WriteLine();
            }

            Console.WriteLine("Матрица B:");
            for (int i = 0; i < K; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    Console.Write(matrixB[i, j] + " ");
                }
                Console.WriteLine();
            }


            Console.WriteLine("Результат перемножения матриц:");
            for (int i = 0; i < M; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    Console.Write(result[i, j] + " ");
                }
                Console.WriteLine();
            }*/

        }
    }
}
