namespace Validation.CardService.Services
{
    public interface IBinLookupService
    {
        void InsertPattern(string binPrefix, string issuerNetwork);
        string? Lookup(string cardNumber);
    }

    internal class BinTrieNode
    {
        public Dictionary<char, BinTrieNode> Children { get; } = new();
        public string? IssuerNetwork { get; set; }
        public bool IsEndOfPattern { get; set; }
    }

    /// <summary>
    /// Trie over BIN prefixes. Insert is O(k), Lookup is O(k) where k = prefix length (max ~6),
    /// independent of how many issuer patterns are registered — this is why a trie beats a
    /// linear list of "StartsWith" checks as the issuer table grows.
    /// Uses longest-prefix-match so more specific patterns (e.g. "4111" ) can override
    /// broader ones (e.g. "4") if you ever need issuer-specific overrides.
    /// </summary>
    public class BinTrie : IBinLookupService
    {
        private readonly BinTrieNode _root = new();

        public BinTrie()
        {
            SeedKnownIssuers();
        }

        public void InsertPattern(string binPrefix, string issuerNetwork)
        {
            var node = _root;
            foreach (var ch in binPrefix)
            {
                if (!node.Children.TryGetValue(ch, out var next))
                {
                    next = new BinTrieNode();
                    node.Children[ch] = next;
                }
                node = next;
            }
            node.IsEndOfPattern = true;
            node.IssuerNetwork = issuerNetwork;
        }

        public string? Lookup(string cardNumber)
        {
            var node = _root;
            string? lastMatch = null;

            foreach (var ch in cardNumber)
            {
                if (!node.Children.TryGetValue(ch, out var next))
                    break;

                node = next;
                if (node.IsEndOfPattern)
                    lastMatch = node.IssuerNetwork;
            }

            return lastMatch;
        }

        private void SeedKnownIssuers()
        {
            InsertPattern("4", "Visa");
            InsertPattern("51", "Mastercard");
            InsertPattern("52", "Mastercard");
            InsertPattern("53", "Mastercard");
            InsertPattern("54", "Mastercard");
            InsertPattern("55", "Mastercard");
            InsertPattern("34", "American Express");
            InsertPattern("37", "American Express");
            InsertPattern("6011", "Discover");
            InsertPattern("65", "Discover");
            InsertPattern("35", "JCB");
        }
    }
}
