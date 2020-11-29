namespace DatingApp.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using DatingApp.Data.Common.Models;

    public class Photo : BaseDeletableModel<string>
    {
        public Photo()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        public string Url { get; set; }

        public bool IsMain { get; set; }

        public string PublicId { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; }

        public string ApplicationUserId { get; set; }
    }
}
