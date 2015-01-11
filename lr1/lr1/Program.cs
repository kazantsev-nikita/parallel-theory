using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace lr1
{
    public class Program
    {
        static void Main(string[] args)
        {
            int threadsCount = 2;
            int queueSize = 10;
            Thread[] producers;
            Thread[] consumers;
            Random random = new Random();
            int size = 100;
            int[] source = new int[size];
            int[] destination = new int[size];
            int sourceSum = 0;
            int destinationSum = 0;

            for (int i = 0; i < size; i++)
            {
                source[i] = random.Next(size);
                sourceSum += source[i];
            }
            Console.WriteLine("source Sum:" + sourceSum);
            Console.ReadKey();

            Counter readCounter = new Counter();
            Counter writeCounter = new Counter();
            BlockingQueue blockingQueue = new BlockingQueue(queueSize);

            producers = new Thread[threadsCount];
            for (int i = 0; i < threadsCount; i++)
            {
                producers[i] = new Thread(delegate() { ProducerJob(source, readCounter, blockingQueue); });
                producers[i].Start();
            }

            consumers = new Thread[threadsCount];
            for (int i = 0; i < threadsCount; i++)
            {
                consumers[i] = new Thread(delegate() { ConsumerJob(destination, writeCounter, blockingQueue); });
                consumers[i].Start();
            }

            for (int i = 0; i < threadsCount; i++)
            {
                producers[i].Join();
            }

            for (int i = 0; i < threadsCount; i++)
            {
                consumers[i].Join();
            }

            for (int i = 0; i < destination.Length; i++) 
            {
                destinationSum += destination[i];
            }

            Console.WriteLine("destination Sum:" + destinationSum);
            Console.ReadKey();
        }

        static void ProducerJob(int[] source, Counter counter, BlockingQueue blockingQueue)
        {
            while (true) {
                lock (counter) 
                { 
                    if (counter.getCounterValue() >= source.Length) 
                    {
                        break;
                    }
                    try
                    {
                        blockingQueue.Produce(source[counter.getCounterValue()]);
                        counter.Increment();
                    }
                    catch (Exception ex)
                    {
                        File.AppendAllText(@"C:\inetpub\wwwroot\debug.txt", ex.Message + " " + ex.StackTrace + Environment.NewLine);
                    }
                }
            }
        }

        static void ConsumerJob(int[] destination, Counter counter, BlockingQueue blockingQueue)
        {
            while (true)
            {
                lock (counter)
                {
                    if (counter.getCounterValue() >= destination.Length)
                    {
                        break;
                    }
                    try
                    {
                        destination[counter.getCounterValue()] = blockingQueue.Consume();
                        counter.Increment();
                    }
                    catch (Exception ex)
                    {
                        File.AppendAllText(@"C:\inetpub\wwwroot\debug.txt", ex.Message + " " + ex.StackTrace + Environment.NewLine);
                    }
                }
            }
        }

        class BlockingQueue
        {
            private readonly Queue queue = new Queue();
            private readonly int maxSize;
            public BlockingQueue(int maxSize) { this.maxSize = maxSize; }

            public void Produce(int item)
            {
                lock (queue)
                {
                    while (queue.Count >= maxSize)
                    {
                        Monitor.Wait(queue);
                    }
                    queue.Enqueue(item);
                    
                    if (queue.Count == 1)
                    {
                        Monitor.PulseAll(queue);
                    }
                }
            }

            public int Consume()
            {
                lock (queue)
                {
                    while (queue.Count == 0)
                    {
                        Monitor.Wait(queue);
                    }
                    
                    if (queue.Count == maxSize - 1)
                    {
                        Monitor.PulseAll(queue);
                    }
                    return Convert.ToInt32(queue.Dequeue());
                }
            }
        }

        class Counter
        {
            public int counter { get; set; }

            public int getCounterValue()
            {
                return counter;
            }

            public void Increment()
            {
                counter++;
            }

        }
    }
}