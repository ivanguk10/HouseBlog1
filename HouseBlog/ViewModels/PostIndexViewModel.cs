using HouseBlog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HouseBlog.ViewModels
{
    public class PostIndexViewModel
    {
        public IEnumerable<Topic> Topics { get; set; }
        public IEnumerable<Post> Posts { get; set; }
    }
}
