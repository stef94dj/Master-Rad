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

        private Guid _userId;
        public Guid UserId
        {
            get
            {
                if (_userId == Guid.Empty)
                    _userId = new Guid(User.GetObjectId());

                return _userId;
            }
        }
    }
}
