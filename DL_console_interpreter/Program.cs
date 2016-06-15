﻿using DL_Interpreter;
using System;
using System.Collections.Generic;
using System.IO;

namespace DL_console_interpreter
{
    class Program
    {
        static void Main(string[] args)
        {
            string file;
            if (args.Length != 0) file = args[0];
            else file = Console.ReadLine();
            
            if (File.Exists(file))
            {
                var code = File.ReadAllText(file);

                System.Threading.Thread.CurrentThread.Priority = System.Threading.ThreadPriority.Highest;
                
                Interpreter.ShowError = ShowError;
                Interpreter.Write = Console.Write;

                Interpreter.Execute(
                    "a = { five: (function() => 5)() };\n" +
                    "b='speeding up a.five = ' + a.five;\n" +
                    "if(typeof(a) == 3) a.five = 3; else {}" +
                    "for ( i = 0; i < a.five; i += 1 ) ;\na['five'];"
                    );
                Interpreter.Reset();

                Library.RegisterObject("Console")
                    .AddFunction("ReadLine", Input, new string[0])
                    .AddFunction("Read", InputChar, new string[0])
                    .AddFunction("Write", Write, new[] { "object" })
                    .AddFunction("WriteLine", WriteLine, new[] { "object" })
                    .AddFunction("Clear", Clear, new string[0]);
                
                Interpreter.Execute(code);
            }
            else
                Console.WriteLine("File {0} doesn't exists", args[0]);

            if(File.Exists("_.temp.dl")) File.Delete("_.temp.dl");

            Console.WriteLine("\n\nPress any key to continue");
            Console.ReadKey();
        }

        private static void ShowError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static DL_Interpreter.Parser.Variable Clear(List<DL_Interpreter.Parser.Variable> args)
        {
            Console.Clear();
            return new DL_Interpreter.Parser.Variable();
        }

        public static DL_Interpreter.Parser.Variable Input(List<DL_Interpreter.Parser.Variable> args) =>
            new DL_Interpreter.Parser.Variable(Console.ReadLine(), "string");

        public static DL_Interpreter.Parser.Variable InputChar(List<DL_Interpreter.Parser.Variable> args)
        {
            try { return new DL_Interpreter.Parser.Variable(Console.ReadKey().KeyChar.ToString(), "string"); }
            catch(Exception ex) { ShowError(ex.Message); }

            return new DL_Interpreter.Parser.Variable();
        }

        public static DL_Interpreter.Parser.Variable Write(List<DL_Interpreter.Parser.Variable> args)
        {
            Console.Write(args[0].value);
            return new DL_Interpreter.Parser.Variable();
        }

        public static DL_Interpreter.Parser.Variable WriteLine(List<DL_Interpreter.Parser.Variable> args)
        {
            Console.WriteLine(args[0].value);
            return new DL_Interpreter.Parser.Variable();
        }
    }
}