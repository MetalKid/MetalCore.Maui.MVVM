namespace MetalCore.Chores.UI.Common
{
    public static class Converters
    {
        public static Func<int, string> FromIntToString => from => from.ToString();
        public static Func<string, int> FromStringToInt => from => int.TryParse(from, out var r) ? r : 0;
    }
}
