﻿using Learning;
using Learning.Threads;
using Learning.Threads.BusinessRules;
using System;

namespace EntityFrameworkTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var playing = new StopTask();

            playing.Process();

            Console.ReadKey();
        }
    }
}