using System;
using System.Linq;

namespace SteamBot.VBot
{
    public class Utilities
    {
        public bool DoesMessageStartWith(string Message, string[] Comparison)
        {
            return Comparison.Any(CommandWord => Message.StartsWith(CommandWord, StringComparison.OrdinalIgnoreCase));
        }
    }
}
