// Console template
using System;
using Language;


namespace Language {
    
    class Program
    {
        // create error handler
        private static ErrorHandler errorHandler = new ErrorHandler();

        public static ErrorHandler GetErrorHandler() {
            return errorHandler;
        }

        static void Main(string[] args)
        {
            // Here is your empty program!
            Console.Clear();
            Console.WriteLine("Creating components");
            Lexer lexer = new Lexer();
            Scanner scanner = new Scanner();
            
            scanner.ReadFile("aclang_program/test.acl");
            scanner.ScanTokens();
        }
    }
}
