using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad
{
    public class BaseController : Controller
    {
        public BaseController() : base() { }

        public Guid UserId
        {
            get
            {
                return new Guid(User.GetObjectId());
            }
        }
    }
}
