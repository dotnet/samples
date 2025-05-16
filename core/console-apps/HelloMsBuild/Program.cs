using System;

namespace HelloMyBuild
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Bem-vindo ao HelloMyBuild ===");
            Console.WriteLine("Por favor, digite sua mensagem personalizada e pressione Enter:");
            
            string mensagem = Console.ReadLine();

            Console.WriteLine("\n✨ Sua mensagem foi registrada com sucesso! ✨");
            Console.WriteLine($"Aqui está a sua mensagem: \"{mensagem}\"");
            Console.WriteLine("\nObrigado por usar o HelloMyBuild. Até a próxima!");
        }
    }
}
