using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace EDCFinal
{
    public partial class PopulateDB : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Context.User.Identity.GetUserId() != "190d22ec-44a8-4bf4-9579-0365a6b5af3e")
            {
                Response.Redirect("/Publib");
                return;
            }
        }
    }
}