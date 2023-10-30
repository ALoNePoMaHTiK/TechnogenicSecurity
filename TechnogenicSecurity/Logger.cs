﻿using System;
using System.IO;

namespace TechnogenicSecurity
{
    public static class Logger
    {
        public static void Log(string message)
        {
            using (StreamWriter writer = new StreamWriter("log.txt", true))
            {
                writer.WriteLine($"[{DateTime.Now}]" + message);
            }
        }
    }
}
