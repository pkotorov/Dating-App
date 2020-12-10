namespace DatingApp.Data.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class UserParams
    {
        public string CurrentUsername { get; set; }

        public string Gender { get; set; }

        public int MinAge { get; set; } = 18;

        public int MaxAge { get; set; } = 150;

        public string OrderBy { get; set; } = "lastActive";
    }
}
