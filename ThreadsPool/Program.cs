using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace ThreadsPool
{
    public class Program
    {

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


        static void MultiplyCell(object state)
        {
            /*
            int[] indexes = (int[])state;
            int[,] matrixA = (int[,])indexes[2];
            int[,] matrixB = (int[,])indexes[3];
            int[,] result = (int[,])indexes[4];
            CountdownEvent countdownEvent = (CountdownEvent)indexes[5];*/


            (int i, int j, int[,] matrixA, int[,] matrixB, int[,] result, CountdownEvent countdownEvent) = ((int, int, int[,], int[,], int[,], CountdownEvent)) state;

            Console.WriteLine($"Поток {Thread.CurrentThread.ManagedThreadId} для элемента [{i}, {j}] запустился");
            result[i, j] = 0;
            for (int k = 0; k < matrixA.GetLength(1); k++)
            {
                result[i, j] += matrixA[i, k] * matrixB[k, j];
            }

            //Thread.Sleep(100); 
            Console.WriteLine($"Поток {Thread.CurrentThread.ManagedThreadId} для элемента [{i}, {j}] завершился");

            countdownEvent.Signal();
        }


        static void Main(string[] args)
        {

            int M = 3; // количество строк в матрицах
            int N = 4; // количество столбцов в матрицах
            int K = 2; // общее количество столбцов в первой и строк во второй матрицах

            int[,] matrixA = new int[M, K];
            int[,] matrixB = new int[K, N];
            int[,] result = new int[M, N];

            CountdownEvent countdownEvent = new CountdownEvent(matrixA.GetLength(0) * matrixB.GetLength(1));

            Random rand = new Random();
            FillMatrixRandom(matrixA, rand);
            FillMatrixRandom(matrixB, rand);


            for (int i = 0; i < M; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    ThreadPool.QueueUserWorkItem(MultiplyCell, ( i, j, matrixA, matrixB, result, countdownEvent));
                }
            }

            countdownEvent.Wait();

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
            }

        }
    }
}
