using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace PrefixTree
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var tt = new TrieTests();
            await tt.Test();
            Console.ReadKey();
        }
    }

    public class TrieTests
    {
        public async Task Test()
        {
            // Input keys (use only 'a' 
            // through 'z' and lower case)
            string[] keys = { "the", "a", "there", "answer", "any", "by", "bye", "their" };

            const string present = "Present in trie";
            const string absent = "Not present in trie";

            var trie = new Trie();

            var files = new string[] {
                @"c:\LocalCode\PrefixTree\PrefixTree\bin\Debug\net5.0\wordforms\wordforms_1_54042.txt",
                @"c:\LocalCode\PrefixTree\PrefixTree\bin\Debug\net5.0\wordforms\wordforms.txt"
            };


            var counter = 0;
            foreach (var file in files)
            {
                var buff = new byte[1024];
                using var fs = File.OpenRead(file);
                using var sr = new StreamReader(fs, Encoding.UTF8);
                var line = string.Empty;

                while ((line = await sr.ReadLineAsync()) != null)
                {
                    var endIndex = line.IndexOf("(");
                    var word = line.Substring(0, endIndex)
                        .Replace("\'", "")
                        .Replace("`", "")
                        .Replace("\"", "");
                    //Console.WriteLine(word);
                    trie.Insert(word);
                    counter++;

                    if (counter % 10_000 == 0)
                    {
                        Console.WriteLine(counter);
                    }

                    if (counter >= 100)
                    {
                        break;
                    }
                }
            }


            // Construct trie
            //for (var i = 0; i < keys.Length; i++)
            //    trie.Insert(keys[i]);

            // Search for different keys
            //if (trie.Search("the"))
            //    Console.WriteLine("the --- " + present);
            //else Console.WriteLine("the --- " + absent);

            //if (trie.Search("these"))
            //    Console.WriteLine("these --- " + present);
            //else Console.WriteLine("these --- " + absent);

            //if (trie.Search("their"))
            //    Console.WriteLine("their --- " + present);
            //else Console.WriteLine("their --- " + absent);

            //if (trie.Search("thaw"))
            //    Console.WriteLine("thaw --- " + present);
            //else Console.WriteLine("thaw --- " + absent);

            var words = trie.GetWords();
            foreach (var word in words)
            {
                Console.WriteLine(word);
            }

            Console.WriteLine("-------абажу-------");
            foreach (var word in trie.SearchSuggestions("абажу", 10))
            {
                Console.WriteLine(word);
            }

            //Console.WriteLine("-------a-------");
            //foreach (var word in trie.SearchSuggestions("a", 20))
            //{
            //    Console.WriteLine(word);
            //}

            //Console.WriteLine("-------b-------");
            //foreach (var word in trie.SearchSuggestions("b", 20))
            //{
            //    Console.WriteLine(word);
            //}

            await Task.CompletedTask;
        }
    }

    public class Trie
    {
        private const string ALPHABET_RU = "абвгдеёжзийклмнопрстуфхцчшщъыьэюя-";
        private const string ALPHABET_EN = "abcdefghijklmnopqrstuvwxyz";

        private static readonly string Alphabet = ALPHABET_RU;
        private static readonly int ALPHABET_SIZE = Alphabet.Length;

        private readonly TrieNode _root;

        public Trie()
        {
            _root = new TrieNode();
        }

        private class TrieNode
        {
            public readonly TrieNode[] Children = new TrieNode[ALPHABET_SIZE];

            public bool IsEndOfWord;

            public TrieNode()
            {
                IsEndOfWord = false;
                for (var i = 0; i < ALPHABET_SIZE; i++)
                {
                    Children[i] = null;
                }
            }
        }

        public void Insert(string key)
        {
            var node = _root;

            for (var level = 0; level < key.Length; level++)
            {
                var index = Alphabet.IndexOf(key[level]);
                //var index = key[level] - Alphabet[0];
                //Console.WriteLine((int)key[level]);
                //Console.WriteLine((int)Alphabet[0]);
                node.Children[index] ??= new TrieNode();

                node = node.Children[index];
            }

            node.IsEndOfWord = true;
        }

        public bool Search(string key)
        {
            var node = _root;

            for (var level = 0; level < key.Length; level++)
            {
                var index = Alphabet.IndexOf(key[level]);
                //var index = key[level] - Alphabet[0];

                if (node.Children[index] == null)
                {
                    return false;
                }

                node = node.Children[index];
            }

            return node != null && node.IsEndOfWord;
        }

        public IEnumerable<string> GetWords(int quantity = 0)
        {
            const string str = "";
            var words = new List<string>();
            GetWords(_root, str, words, quantity);

            return words;
        }

        void GetWords(TrieNode node, string str, ICollection<string> words, int quantity = 0)
        {
            if (quantity != 0 && words.Count == quantity) return;

            if (node.IsEndOfWord)
            {
                words.Add(str);
            }

            for (var i = 0; i < ALPHABET_SIZE; i++)
            {
                if (node.Children[i] != null)
                {
                    var ch = Alphabet[i];
                    //var newStr = str + (char)(i + Alphabet[0]);
                    var newStr = str + ch;
                    GetWords(node.Children[i], newStr, words, quantity);
                }
            }
        }

        public IEnumerable<string> SearchSuggestions(string start, int quantity = 0)
        {
            var suggestions = new List<string>();
            if (start == null) return suggestions;

            var node = _root;

            for (var level = 0; level < start.Length; level++)
            {
                var index = start[level] - Alphabet[0];

                if (node.Children[index] != null)
                {
                    node = node.Children[index];
                }
            }

            GetWords(node, start, suggestions, quantity);

            return suggestions;
        }
    }
}
