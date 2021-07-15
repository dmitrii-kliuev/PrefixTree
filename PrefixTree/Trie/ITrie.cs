using System.Collections.Generic;
using System.Threading.Tasks;

namespace PrefixTree.Trie
{
    public interface ITrie
    {
        Task Init(string[] files);
        void Insert(string key);
        IEnumerable<string> SearchSuggestions(string start, int quantity = 0);
    }
}
