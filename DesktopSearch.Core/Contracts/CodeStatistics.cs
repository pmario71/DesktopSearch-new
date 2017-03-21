namespace DesktopSearch.Core.Contracts
{
    public class CodeStatistics
    {
        public int Classes { get; internal set; }
        public int Activities { get; internal set; }
        public int Interfaces { get; internal set; }
        public int Enums { get; internal set; }
        public int Structs { get; internal set; }
        public int Types { get; internal set; }
        public long APIs { get; internal set; }
    }
}