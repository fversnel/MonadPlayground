namespace MonadPlayground {
    public sealed class Unit {
        public static readonly Unit Default = new Unit();

        private Unit() {}
    }
}