namespace LanguageLexer 
{
    class Lexer 
    {
        private string fileContents = "";

        public void ReadFile(string path) {
            fileContents = File.ReadAllText(path);
        }

        public string getRawContents() {
            return fileContents;
        }
    }
}