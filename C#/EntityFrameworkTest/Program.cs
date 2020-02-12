﻿using Learning;
using Learning.Events;
using Learning.Serialization;
using Learning.Threads;
using Learning.Threads.BusinessRules;
using Learning.TypeAndCollections;
using System;

namespace EntityFrameworkTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var playing = new MyHashSet();

            playing.Process();
            
            Console.ReadKey();
        }
    }
}
