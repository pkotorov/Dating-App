namespace DatingApp.Web.ViewModels.Photos
{
    using DatingApp.Data.Models;
    using DatingApp.Services.Mapping;

    public class PhotoViewModel : IMapFrom<Photo>
    {
        public string Id { get; set; }

        public string Url { get; set; }

        public bool IsMain { get; set; }
    }
}
