using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HouseBlog.Models
{
    public class User : IdentityUser
    {
        public int TopicId { get; set; }
        public string TopicName { get; set; }
        public bool isBlocked { get; set; }
        public ICollection<Comment> Comments { get; set; }
        public ICollection<Post> Posts { get; set; }
    }
}
