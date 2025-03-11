// Console template
using System;
using LanguageLexer;
class Program
{
    static void Main(string[] args)
    {
        // Here is your empty program!
        Console.Clear();
        Console.WriteLine("Creating components");
        Lexer lexer = new Lexer();

        lexer.ReadFile("aclang_program/main.acl");
        Console.WriteLine(lexer.getRawContents());
    }
}
