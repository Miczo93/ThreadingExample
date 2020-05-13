using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace ThreadingExample
{
    public delegate void DelegateCallback(int i);
    class Program
    {
        static int Total = 0;
        static readonly object _lock = new object();

        private static void Main()
        {
            //Paste functions here
            Console.ReadLine();
        }

        #region Threading Functions
        private static void ThreadSimple()
        {
            Program p = new Program();
            Thread newThread = new Thread(p.SimulateWork);
            newThread.Start();
            Console.WriteLine("Simple Finish");
        }
        private static void ThreadWithParameter()
        {
            Console.WriteLine("Please enter the number: ");
            object value = Console.ReadLine();
            ParameterizedThreadStart parameterizedThreadStart = new ParameterizedThreadStart(ObjectTypeClass.FunWithPara);
            Thread newThread = new Thread(parameterizedThreadStart);
            newThread.Start(value);
            Console.WriteLine("Parameter Finish");
        }

        private static void ThreadWithParameterSimplified()
        {
            Console.WriteLine("Please enter the number: ");
            object value = Console.ReadLine();
            Thread newThread = new Thread(ObjectTypeClass.FunWithPara);
            newThread.Start(value);
            Console.WriteLine("Parameter Simplified Finish");
        }

        private static void ThreadWithReturnValue()
        {
            object value = null;
            Console.WriteLine("Please enter the number: ");
            object value2 = Console.ReadLine();
            Thread newThread = new Thread(() => { value = ObjectTypeClass.FunReturn(value2); });
            newThread.Start();
            Console.WriteLine("Finish Parameter Function 2");
        }

        private static void ThreadWithSaveType()
        {
            Console.WriteLine("Please enter the number: ");
            int value = Convert.ToInt32(Console.ReadLine());
            SaveType saveType = new SaveType(value);
            Thread newThread = new Thread(saveType.FunWithPara);
            newThread.Start();
            Console.WriteLine("Type Save Finish");
        }

        private static void ThreadWithDelegate()
        {
            Console.WriteLine("Please enter the number: ");
            int value = Convert.ToInt32(Console.ReadLine());
            DelegateCallback delegateCallback = new DelegateCallback(Print);
            SaveType saveType = new SaveType(value, delegateCallback);
            Thread newThread = new Thread(saveType.FunWithParaWithDelegate);
            newThread.Start();
            Console.WriteLine("Delegate Finished");
        }

        private static void ThreadWithJoins()
        {
            Console.WriteLine("Thread With Joins starts");
            Thread T1 = new Thread(SimulatedFunctions1);
            T1.Start();
            Thread T2 = new Thread(SimulatedFunctions2);
            T2.Start();
            if (T1.Join(1000))
            {
                Console.WriteLine("Thread 1 ended");
            }
            else
            {
                Console.WriteLine("Thread 1 got time out after 1 second");
            }
            T2.Join();
            Console.WriteLine("Thread 2 ended");

            for (int i = 1; i <= 10; i++)
            {
                if (T1.IsAlive)
                {
                    Console.WriteLine("Thread 1 is still waiting");
                    Thread.Sleep(500);
                }
                else
                {
                    Console.WriteLine("Thread 1 ended");
                    break;
                }
            }
            Console.WriteLine("Thread With Joins ends");
        }

        private static void ProtectedResourcesNotGood()
        {
            Console.WriteLine("Resourced not protected");
            ProtectedResourcesMain(AddOneMillionNotSave);
        }

        private static void ProtectedResourcesInterlocked()
        {
            Console.WriteLine("Resourced protected");
            ProtectedResourcesMain(AddOneMillionAtomic);
        }

        private static void ProtectedResourcesMain(ThreadStart TS)
        {
            Thread T1 = new Thread(TS);
            Thread T2 = new Thread(TS);
            Thread T3 = new Thread(TS);
            T1.Start();
            T2.Start();
            T3.Start();
            T1.Join();
            T2.Join();
            T3.Join();
            Console.WriteLine("Total = " + Total);
        }
        #endregion Threading Functions

        #region Other Functions
        private void SimulateWork()
        {
            Console.WriteLine("Start simulated work");
            Thread.Sleep(5000);
            Console.WriteLine("End simulated work");
        }
        private static void Print(int i)
        {
            Console.WriteLine("Printing: " + i);
        }

        private static void SimulatedFunctions1()
        {
            Console.WriteLine("Thread 1 starts");
            Thread.Sleep(5000);
            Console.WriteLine("Thread 1 is ending");
        }
        private static void SimulatedFunctions2()
        {
            Console.WriteLine("Thread 2 starts");
        }
        #endregion Other Functions

        #region Locking

        private static void AddOneMillionNotSave()
        {
            for (int i = 1; i <= 1000000; i++)
            {
                Total++;
            }
        }
        private static void AddOneMillionAtomic()
        {
            for (int i = 1; i <= 1000000; i++)
            {
                Interlocked.Increment(ref Total);//faster
            }
        }

        private static void AddOneMillionLock()
        {
            for (int i = 1; i <= 1000000; i++)
            {
                lock (_lock)
                {
                    Total++;
                }
            }
        }

        private static void AddOneMillionMonitor()//to samo co lock
        {
            for (int i = 1; i <= 1000000; i++)
            {
                Monitor.Enter(_lock);
                try
                {
                    Total++;
                }
                finally
                {
                    Monitor.Exit(_lock);
                }
            }
        }

        private static void AddOneMillion()//to samo co lock w C#4
        {
            for (int i = 1; i <= 1000000; i++)
            {
                bool lockTaken = false;
                Monitor.Enter(_lock, ref lockTaken);
                try
                {
                    Total++;
                }
                finally
                {
                    if (lockTaken)
                        Monitor.Exit(_lock);
                }
            }
        }
        #endregion Locking
    }

    class ObjectTypeClass
    {
        public static void FunWithPara(object n)
        {
            if (int.TryParse(n.ToString(), out int number))
            {
                Console.WriteLine("My number is: " + n);
            }
        }

        public static int FunReturn(object n)
        {
            if (int.TryParse(n.ToString(), out int number))
            {
                number = +5;
                Console.WriteLine("The return number is: " + number);
                return number;
            }
            Console.WriteLine("Parameter is not a number");
            return 0;
        }
    }

    class SaveType
    {
        private int _target;
        private DelegateCallback _delegateCallback;

        public SaveType(int target)
        {
            _target = target;
        }

        public SaveType(int target, DelegateCallback delegateCallback)
        {
            _target = target;
            _delegateCallback = delegateCallback;
        }
        public void FunWithPara()
        {
            Console.WriteLine("My number is: " + _target);
        }

        public void FunWithParaWithDelegate()
        {
            int sum = 0;
            for (int i = 1; i < _target; i++)
            {
                sum += i;
            }
            _delegateCallback?.Invoke(sum);
        }
    }
}
