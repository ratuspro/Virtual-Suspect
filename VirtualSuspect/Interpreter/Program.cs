﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtualSuspect;
using VirtualSuspect.Utils;
using VirtualSuspect.KnowledgeBase;

namespace Interpreter
{
    class Program
    {
        static void Main(string[] args) {

            string filepath = Console.ReadLine();

            KnowledgeBaseManager kb = KnowledgeBaseParser.parseFromFile("../../../../Story/JoãoPOV.xml");
            
        }
    }
}
