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

        public bool HasErrored() {
            return hasErrored;
        }


        // error types
        public void UnknownCharacterError(int line, char chr) { Error(line, "", "Unknown Character: '" + chr + "'."); }
        public void UnterminatedStringError(int line, int startLine) { Error(line, "", $"String not terminated. String started at line: {startLine}"); }
        public void MultilineStringError(int line) { Error(line, "", "String's cannot be multi-line."); }
    }
}