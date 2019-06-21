using lab2_restapi_1205_taskmgmt.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace lab2_restapi_1205_taskmgmt.ViewModels
{
    public class CommentPostModel
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public bool Important { get; set; }


        public static Comment ToComment(CommentPostModel commentModel)
        {

            return new Comment
            {
                Id = commentModel.Id,
                Text = commentModel.Text,
                Important = commentModel.Important,
            };
        }
    }
}
