using System;

namespace SteamBot.VBot
{
    public class Utilities
    {
        public bool DoesMessageStartWith(string Message, string[] Comparison)
        {
            foreach (string CommandWord in Comparison)
            {
                if (Message.StartsWith(CommandWord, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
