using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PrefixTree.Trie
{
    public class TrieWithArrays : ITrie
    {
        private readonly string Alphabet;

        private readonly TrieNode _root;

        public TrieWithArrays(string alphabet)
        {
            Alphabet = alphabet;

            _root = new TrieNode(Alphabet);
        }

        public async Task Init(string[] files)
        {
            var sw = new Stopwatch();
            sw.Start();

            var counter = 0;
            foreach (var file in files)
            {
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
                    Insert(word);
                    counter++;

                    if (counter % 10_000 == 0)
                    {
                        Console.WriteLine(counter);
                    }

                    if (counter >= 10_000)
                    {
                        break;
                    }
                }
            }

            Console.WriteLine($"Elapsed: {sw.Elapsed}");

            await Task.CompletedTask;
        }


        private class TrieNode
        {
            public readonly TrieNode[] Children;
            public bool IsEndOfWord;

            public TrieNode(string alphabet)
            {
                Children = new TrieNode[alphabet.Length];
                IsEndOfWord = false;
                for (var i = 0; i < alphabet.Length; i++)
                {
                    Children[i] = null;
                }
            }
        }

        public void Insert(string key)
        {
            if (!CheckWordCharacters(key))
            {
                return;
            }

            var node = _root;

            for (var level = 0; level < key.Length; level++)
            {
                var index = Alphabet.IndexOf(key[level]);
                //var index = key[level] - Alphabet[0];
                //Console.WriteLine((int)key[level]);
                //Console.WriteLine((int)Alphabet[0]);
                node.Children[index] ??= new TrieNode(Alphabet);

                node = node.Children[index];
            }

            node.IsEndOfWord = true;
        }

        private bool CheckWordCharacters(string key)
        {
            return !Regex.Match(key, "[^а-я,-]+").Success;
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

            for (var i = 0; i < Alphabet.Length; i++)
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
            if (string.IsNullOrEmpty(start)) return suggestions;

            var node = _root;

            for (var level = 0; level < start.Length; level++)
            {
                var index = Alphabet.IndexOf(start[level]);
                if (index == -1) return suggestions;

                if (node.Children[index] != null)
                {
                    node = node.Children[index];
                }
                else
                {
                    return suggestions;
                }
            }

            GetWords(node, start, suggestions, quantity);

            return suggestions;
        }
    }
}
