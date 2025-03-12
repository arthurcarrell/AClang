namespace Language {

    class ErrorHandler {

        private bool hasErrored = false;
        private static void Report(int line, string location, string message) {
            Console.Write(" On line " + line + ": " + message);
            Console.Write("\n");
        }
        public void Error(int line, string location, string message) {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("[!]");
            Console.ResetColor();
            Report(line, location, message);
            hasErrored = true;
        }

        public void Warn(int line, string location, string message) {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("[?]");
            Console.ResetColor();
            Report(line, location, message);
        }
    }
}