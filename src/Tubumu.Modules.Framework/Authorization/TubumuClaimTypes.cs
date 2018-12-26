namespace Tubumu.Modules.Framework.Authorization
{
    public class TubumuClaimTypes
    {
        public const string Group = "g"; // 主要不要使用 group，否则会被篡改。

        public const string Permission = "p";
    }
}
