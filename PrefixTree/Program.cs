using PrefixTree.Trie;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrefixTree
{
    class Program
    {
        static string[] files = new string[] {
                Path.Combine(Environment.CurrentDirectory, @"wordforms\wordforms_1_54042.txt"),
                Path.Combine(Environment.CurrentDirectory, @"wordforms\wordforms.txt")
            };

        static string ALPHABET_RU = "абвгдеёжзийклмнопрстуфхцчшщъыьэюя-";
        //string ALPHABET_EN = "abcdefghijklmnopqrstuvwxyz";

        static async Task Main(string[] args)
        {
            //await Run(new Trie(ALPHABET_RU));
            await Run(new TrieWithDictionaries(ALPHABET_RU));
        }

        static async Task Run(ITrie trie)
        {
            await trie.Init(files);

            Console.InputEncoding = Encoding.Unicode;

            while (true)
            {
                Console.WriteLine("---------------------");
                Console.Write("Input: ");
                var input = Console.ReadLine();
                var suggestions = trie.SearchSuggestions(input, 10);
                Console.WriteLine($"word quantity: {suggestions.Count()}");
                Console.WriteLine("---------------------");
                foreach (var word in suggestions)
                {
                    Console.WriteLine(word);
                }
            }
        }
    }
}
