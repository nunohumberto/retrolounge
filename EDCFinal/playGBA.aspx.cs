using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace EDCFinal
{
    public partial class playGBA : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string qs = "";
            try { qs = Request.QueryString["h"]; }
            catch (Exception)
            {
                return;
            }
            Page.ClientScript.RegisterStartupScript(this.GetType(), "LoadAll", "$(function() {wait(\"" + qs + "\");});", true);
            
        }
    }
}