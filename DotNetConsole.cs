using System;

namespace MonadPlayground
{
    public class DotNetConsole : IConsole {
        public static readonly IConsole Default = new DotNetConsole();

        private DotNetConsole() {}

        public void Write(char s) {
            Console.Write(s);
        }

        public char Read() {
            return (char) Console.Read();
        }
    }
}
