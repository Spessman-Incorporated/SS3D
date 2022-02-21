namespace SS3D.Core
{
    /// <summary>
    /// Constant strings to find command line arguments in the executable, Created this to improve readability,
    /// </summary>
    public static class CommandLineArgs
    {
        /// <summary>
        /// The "-host" arg in the executable, should be followed by a bool.
        /// </summary>
        public const string Host = "-host";
        /// <summary>
        /// The "-ip" arg in the executable, should be a string
        /// </summary>
        public const string Ip = "-ip";
        /// <summary>
        /// The "-username" arg in the executable, should be a string.
        /// This is temporary, in production use, this will not exist,
        /// and be replaced by the token, and then the server gets the
        /// username.
        /// </summary>
        public const string Username = "-username";
        /// <summary>
        /// The "-token" arg in the executable, Used to authenticate the user,
        /// in production this will be sent by the Hub to the client executable.
        /// </summary>
        public const string AccessToken = "-token";
    }
}