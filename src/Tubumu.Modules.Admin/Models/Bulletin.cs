using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Tubumu.Modules.Admin.Models
{
    [Serializable]
    public class Bulletin
    {
        public Guid BulletinId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime? PublishDate { get; set; }
        public bool IsShow { get; set; }
    }
}
