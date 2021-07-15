using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PrefixTree.Trie
{
    public class TrieWithDictionaries : ITrie
    {
        private readonly string Alphabet;
        private readonly SortedDictionary<int, TrieNode> _root;

        public TrieWithDictionaries(string alphabet)
        {
            Alphabet = alphabet;

            _root = new SortedDictionary<int, TrieNode>();
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

                    if (counter >= 1_000)
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
            public readonly SortedDictionary<int, TrieNode> Children;

            public bool IsEndOfWord;

            public TrieNode()
            {
                Children = new SortedDictionary<int, TrieNode>();
                IsEndOfWord = false;
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
                if (!node.ContainsKey(key[level]))
                {
                    node.Add(key[level], new TrieNode());
                }

                if (level == key.Length - 1)
                {
                    node[key[level]].IsEndOfWord = true;
                }
                else
                {
                    node = node[key[level]].Children;
                }
            }
        }

        private bool CheckWordCharacters(string key)
        {
            return !Regex.Match(key, "[^а-я,-]+").Success;
        }

        //public bool Search(string key)
        //{
        //    var node = _root;

        //    for (var level = 0; level < key.Length; level++)
        //    {
        //        var index = Alphabet.IndexOf(key[level]);
        //        //var index = key[level] - Alphabet[0];

        //        if (node.Children[index] == null)
        //        {
        //            return false;
        //        }

        //        node = node.Children[index];
        //    }

        //    return node != null && node.IsEndOfWord;
        //}

        public IEnumerable<string> GetWords(int quantity = 0)
        {
            const string str = "";
            var words = new List<string>();
            GetWords(_root, false, str, words, quantity);

            return words;
        }

        void GetWords(SortedDictionary<int, TrieNode> node, bool isEndOfWord, string str, ICollection<string> words, int quantity = 0)
        {
            if (quantity != 0 && words.Count == quantity) return;

            if (isEndOfWord)
            {
                words.Add(str);
            }

            for (var i = 0; i < Alphabet.Length; i++)
            {
                if (node.ContainsKey(Alphabet[i]))
                {
                    var newStr = str + Alphabet[i];
                    GetWords(node[Alphabet[i]].Children, node[Alphabet[i]].IsEndOfWord, newStr, words, quantity);
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
                var key = start[level];

                if (node.ContainsKey(key))
                {
                    node = node[key].Children;
                }
                else
                {
                    return suggestions;
                }
            }

            GetWords(node, false, start, suggestions, quantity);

            return suggestions;
        }
    }
}
