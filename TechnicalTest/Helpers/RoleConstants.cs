namespace TechnicalTest.Helpers
{
    /// <summary>
    /// Sentralisasi nama role. Menghindari "magic string" ("Admin", "Supplier")
    /// yang sebelumnya tersebar di Controller, Attribute, dan View.
    /// </summary>
    public static class RoleConstants
    {
        public const string Admin = "Admin";
        public const string Supplier = "Supplier";
    }
}
