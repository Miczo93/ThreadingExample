using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace ThreadingExample
{
    class Tasking
    {

        private static string DoWork()
        {
            Console.WriteLine("Working starts.");
            Thread.Sleep(5000);
            Console.WriteLine("Working ended.");
            return "Finished";
        }

        public static async void AsyncExample()
        {
            Console.WriteLine("Non blocking example.");
            Task<string> task = new Task<string>(DoWork);
            task.Start();
            string s = await task;
            Console.WriteLine("Non blocking example is " + s);
        }
        public static void ThreadExample()
        {
            Console.WriteLine("Blocking example.");
            string s = "";
            Thread thread = new Thread(() => { s = DoWork(); });
            thread.Start();
            thread.Join();
            Console.WriteLine("Blocking example is " + s);
        }

        public static void ThreadNonBlockingExample()
        {
            Console.WriteLine("Non blocking example.");
            string s = "";
            Thread thread = new Thread(() => 
            { 
                s = DoWork();
                Console.WriteLine("Non blocking example is " + s);//do not do that
            });
            thread.Start();
        }
    }
}
