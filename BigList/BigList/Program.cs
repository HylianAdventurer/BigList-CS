using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace BigList
{
    class Program
    {
        static int[] M, N, P, Q, R;
        static int num_threads = 0;
        private static object Lock = new object();
        private static int _next;
        static int Next
        {
            get
            {
                lock(Lock)
                {
                    return _next++;
                }
            }
        }

        static void Main(string[] args)
        {
            try
            {
                setup(args);
                long time = DateTime.Now.Ticks;
                createThreads(num_threads);
                time = DateTime.Now.Ticks - time;
                Console.WriteLine("Time elapsed: " + time);
                finish();
            }
            catch(FileNotFoundException e)
            {
                Console.WriteLine("Failed to open file.");
                Console.WriteLine(e.Message);
            }
        }

        public static void createThreads(int n)
        {
            Thread[] threads = new Thread[n-1];
            for (int i = 0; i < n-1; i++)
                threads[i] = new Thread(run);
            for (int i = 0; i < n - 1; i++)
                threads[i].Start();
            run();
        }

        public static void setup(String[] args)
        {
            StreamReader mFile = null, nFile = null;
            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "-mFile":
                        mFile = new StreamReader(args[i + 1]);
                        break;
                    case "-nFile":
                        nFile = new StreamReader(args[i + 1]);
                        break;
                    case "-n":
                        num_threads = int.Parse(args[i + 1]);
                        break;

                }
            }
            if(num_threads == 0)
            {
                Console.WriteLine("Enter the number of threads: ");
                num_threads = int.Parse(Console.ReadLine());
            }
            if(mFile == null)
            {
                Console.WriteLine("Enter filename for list M: ");
                mFile = new StreamReader(Console.ReadLine());
            }
            if(nFile == null)
            {
                Console.WriteLine("Enter filename for list N: ");
                nFile = new StreamReader(Console.ReadLine());
            }

            List<int> mList = new List<int>(), nList = new List<int>();
            do
            {
                mList.Add(int.Parse(mFile.ReadLine()));
                nList.Add(int.Parse(nFile.ReadLine()));
            } while (mFile.Peek() != -1 && nFile.Peek() != -1);

            M = new int[mList.Count];
            N = new int[mList.Count];
            P = new int[mList.Count];
            Q = new int[mList.Count];
            R = new int[mList.Count];

            for(int i = 0; i < mList.Count; i++)
            {
                M[i] = mList[i];
                N[i] = nList[i];
            }

            mFile.Close();
            nFile.Close();
        }

        public static void run()
        {
            int i;
            while((i = Next) < M.Length)
            {
                P[i] = Math.Abs(M[i] - N[i]);
                Q[i] = Math.Max(M[i], N[i]);
                while (M[i] % Q[i] > 1 || N[i] % Q[i] > 1)
                    Q[i]--;
                R[i] = Math.Max(M[i], P[i]);
                while (M[i] % R[i] > 1 || P[i] % R[i] > 1)
                    R[i]--;
            }
        }

        public static void finish()
        {
            StreamWriter
                pOut = new StreamWriter("P.dat"),
                qOut = new StreamWriter("Q.dat"),
                rOut = new StreamWriter("R.dat");

            for(int i = 0; i < M.Length; i++)
            {
                pOut.WriteLine(P[i]);
                qOut.WriteLine(Q[i]);
                rOut.WriteLine(R[i]);
            }
            pOut.Close();
            qOut.Close();
            rOut.Close();
            Console.WriteLine("Finished saving lists");
        }
    }
}
