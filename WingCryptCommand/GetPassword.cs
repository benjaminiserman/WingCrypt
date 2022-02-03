namespace WingCryptCommand;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

// Borrowed (with some modifications!) from https://stackoverflow.com/a/3404464, thanks Damian Leszczyński - Vash!
internal static class GetPassword
{
    public static StringBuilder Get(bool first)
    {
        if (first) Console.Write("Enter password: ");
        else Console.Write("Confirm password: ");

        var pwd = new StringBuilder();
        while (true)
        {
            ConsoleKeyInfo i = Console.ReadKey(true);
            if (i.Key == ConsoleKey.Enter)
            {
                Console.WriteLine();
                break;
            }
            else if (i.Key == ConsoleKey.Backspace)
            {
                if (pwd.Length > 0)
                {
                    pwd.Remove(pwd.Length - 1, 1);
                    Console.Write("\b \b");
                }
            }
            else if (i.KeyChar != '\u0000') // KeyChar == '\u0000' if the key pressed does not correspond to a printable character, e.g. F1, Pause-Break, etc
            {
                pwd.Append(i.KeyChar);
                Console.Write("*");
            }
        }

        return pwd;
    }
}
