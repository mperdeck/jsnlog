using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NLog;

namespace EmptyWebFormsApplication
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Logger logger = LogManager.GetLogger("serverlogger");
            logger.Warn("Warn Message generated on server");

        }
    }
}