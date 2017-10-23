using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;

namespace EDCFinal
{
    public partial class editROM : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string qs = "";
            try
            {
                qs = Request.QueryString["h"];
            }
            catch (Exception)
            {
                qs = "";
            }

            if (qs == null || qs.Equals("")) return;

            XmlDocument games = new System.Xml.XmlDocument();
            System.Data.SqlClient.SqlConnection sqlCon =
                new System.Data.SqlClient.SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\Nuno\\Documents\\finaldb.mdf;Integrated Security=True;Connect Timeout=30");
            System.Data.SqlClient.SqlCommand Cmd = new System.Data.SqlClient.SqlCommand();
            Cmd.CommandText = "with xmlnamespaces ('http://ect2016/UserGamesSchema' as us) SELECT UserGames.xmlContent.query('( for $game in /us:user/us:games/us:game[@filename=sql:variable(\"@rhash\")] return $game)') AS game FROM UserGames WHERE UserGames.xmlContent.exist('/us:user[@id=sql:variable(\"@userid\")]') = 1;";
            Cmd.Parameters.Add(new System.Data.SqlClient.SqlParameter()
            {
                ParameterName = "userid",
                Value = User.Identity.GetUserId()
            });
            Cmd.Parameters.Add(new System.Data.SqlClient.SqlParameter()
            {
                ParameterName = "rhash",
                Value = qs
            });

            Cmd.Connection = sqlCon;
            sqlCon.Open();
            var gamereader = Cmd.ExecuteReader();

            var gameindex = gamereader.GetOrdinal("game");
            Boolean had_results = false;
            string results = "";
            while (gamereader.Read())
            {
                results = (string)gamereader.GetValue(gameindex);
                if (results.Length != 0) had_results = true;
                //System.Diagnostics.Debug.WriteLine(results.Length); 
            }
            sqlCon.Close();

            if(had_results) games.LoadXml(results);

            foreach (XmlElement game in games)
            {
                if (!IsPostBack)
                {
                    title.Text = game.Attributes["title"].Value;
                    release.Text = game.Attributes["releasedate"].Value;
                    genre.Text = game.Attributes["genres"].Value;
                    developer.Text = HttpUtility.HtmlDecode(game.Attributes["developers"].Value);
                    publisher.Text = game.Attributes["publishers"].Value;
                    imageurl.Text = game.Attributes["imgurl"].Value;
                    image.Src = game.Attributes["imgurl"].Value;
                }
            }

        }

        protected void Unnamed_Click(object sender, EventArgs e)
        {
            string qs = "";
            try
            {
                qs = Request.QueryString["h"];
            }
            catch (Exception)
            {
                qs = "";
            }

            if (qs == null || qs.Equals("")) return;
            title.DataBind();
            updateTitle(qs, title.Text);
            updateReleaseDate(qs, release.Text);
            updateGenre(qs, genre.Text);
            updateDeveloper(qs, developer.Text);
            updatePublisher(qs, publisher.Text);
            updateImageURL(qs, imageurl.Text);


            Response.Redirect("/Library");
        }

        public void updateTitle(string qs, string newval)
        {
            XmlDocument games = new System.Xml.XmlDocument();
            System.Data.SqlClient.SqlConnection sqlCon =
                new System.Data.SqlClient.SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\Nuno\\Documents\\finaldb.mdf;Integrated Security=True;Connect Timeout=30");
            System.Data.SqlClient.SqlCommand Cmd = new System.Data.SqlClient.SqlCommand();
            Cmd.CommandText = "with xmlnamespaces ('http://ect2016/UserGamesSchema' as us) UPDATE UserGames SET xmlContent.modify('replace value of (/us:user[@id=sql:variable(\"@userid\")]/us:games/us:game[@filename=sql:variable(\"@rhash\")]/@title)[1] with sql:variable(\"@newval\") ');";
            Cmd.Parameters.Add(new System.Data.SqlClient.SqlParameter()
            {
                ParameterName = "userid",
                Value = User.Identity.GetUserId()
            });
            Cmd.Parameters.Add(new System.Data.SqlClient.SqlParameter()
            {
                ParameterName = "rhash",
                Value = qs
            });

            Cmd.Parameters.Add(new System.Data.SqlClient.SqlParameter()
            {
                ParameterName = "newval",
                Value = newval
            });

            Cmd.Connection = sqlCon;
            sqlCon.Open();
            int result = Cmd.ExecuteNonQuery();
            sqlCon.Close();
        }

        public void updateReleaseDate(string qs, string newval)
        {
            XmlDocument games = new System.Xml.XmlDocument();
            System.Data.SqlClient.SqlConnection sqlCon =
                new System.Data.SqlClient.SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\Nuno\\Documents\\finaldb.mdf;Integrated Security=True;Connect Timeout=30");
            System.Data.SqlClient.SqlCommand Cmd = new System.Data.SqlClient.SqlCommand();
            Cmd.CommandText = "with xmlnamespaces ('http://ect2016/UserGamesSchema' as us) UPDATE UserGames SET xmlContent.modify('replace value of (/us:user[@id=sql:variable(\"@userid\")]/us:games/us:game[@filename=sql:variable(\"@rhash\")]/@releasedate)[1] with sql:variable(\"@newval\") ');";
            Cmd.Parameters.Add(new System.Data.SqlClient.SqlParameter()
            {
                ParameterName = "userid",
                Value = User.Identity.GetUserId()
            });
            Cmd.Parameters.Add(new System.Data.SqlClient.SqlParameter()
            {
                ParameterName = "rhash",
                Value = qs
            });

            Cmd.Parameters.Add(new System.Data.SqlClient.SqlParameter()
            {
                ParameterName = "newval",
                Value = newval
            });

            Cmd.Connection = sqlCon;
            sqlCon.Open();
            int result = Cmd.ExecuteNonQuery();
            sqlCon.Close();
        }

        public void updateGenre(string qs, string newval)
        {
            XmlDocument games = new System.Xml.XmlDocument();
            System.Data.SqlClient.SqlConnection sqlCon =
                new System.Data.SqlClient.SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\Nuno\\Documents\\finaldb.mdf;Integrated Security=True;Connect Timeout=30");
            System.Data.SqlClient.SqlCommand Cmd = new System.Data.SqlClient.SqlCommand();
            Cmd.CommandText = "with xmlnamespaces ('http://ect2016/UserGamesSchema' as us) UPDATE UserGames SET xmlContent.modify('replace value of (/us:user[@id=sql:variable(\"@userid\")]/us:games/us:game[@filename=sql:variable(\"@rhash\")]/@genres)[1] with sql:variable(\"@newval\") ');";
            Cmd.Parameters.Add(new System.Data.SqlClient.SqlParameter()
            {
                ParameterName = "userid",
                Value = User.Identity.GetUserId()
            });
            Cmd.Parameters.Add(new System.Data.SqlClient.SqlParameter()
            {
                ParameterName = "rhash",
                Value = qs
            });

            Cmd.Parameters.Add(new System.Data.SqlClient.SqlParameter()
            {
                ParameterName = "newval",
                Value = newval
            });

            Cmd.Connection = sqlCon;
            sqlCon.Open();
            int result = Cmd.ExecuteNonQuery();
            sqlCon.Close();
        }
        public void updateDeveloper(string qs, string newval)
        {
            XmlDocument games = new System.Xml.XmlDocument();
            System.Data.SqlClient.SqlConnection sqlCon =
                new System.Data.SqlClient.SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\Nuno\\Documents\\finaldb.mdf;Integrated Security=True;Connect Timeout=30");
            System.Data.SqlClient.SqlCommand Cmd = new System.Data.SqlClient.SqlCommand();
            Cmd.CommandText = "with xmlnamespaces ('http://ect2016/UserGamesSchema' as us) UPDATE UserGames SET xmlContent.modify('replace value of (/us:user[@id=sql:variable(\"@userid\")]/us:games/us:game[@filename=sql:variable(\"@rhash\")]/@developers)[1] with sql:variable(\"@newval\") ');";
            Cmd.Parameters.Add(new System.Data.SqlClient.SqlParameter()
            {
                ParameterName = "userid",
                Value = User.Identity.GetUserId()
            });
            Cmd.Parameters.Add(new System.Data.SqlClient.SqlParameter()
            {
                ParameterName = "rhash",
                Value = qs
            });

            Cmd.Parameters.Add(new System.Data.SqlClient.SqlParameter()
            {
                ParameterName = "newval",
                Value = newval
            });

            Cmd.Connection = sqlCon;
            sqlCon.Open();
            int result = Cmd.ExecuteNonQuery();
            sqlCon.Close();
        }

        public void updatePublisher(string qs, string newval)
        {
            XmlDocument games = new System.Xml.XmlDocument();
            System.Data.SqlClient.SqlConnection sqlCon =
                new System.Data.SqlClient.SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\Nuno\\Documents\\finaldb.mdf;Integrated Security=True;Connect Timeout=30");
            System.Data.SqlClient.SqlCommand Cmd = new System.Data.SqlClient.SqlCommand();
            Cmd.CommandText = "with xmlnamespaces ('http://ect2016/UserGamesSchema' as us) UPDATE UserGames SET xmlContent.modify('replace value of (/us:user[@id=sql:variable(\"@userid\")]/us:games/us:game[@filename=sql:variable(\"@rhash\")]/@publishers)[1] with sql:variable(\"@newval\") ');";
            Cmd.Parameters.Add(new System.Data.SqlClient.SqlParameter()
            {
                ParameterName = "userid",
                Value = User.Identity.GetUserId()
            });
            Cmd.Parameters.Add(new System.Data.SqlClient.SqlParameter()
            {
                ParameterName = "rhash",
                Value = qs
            });

            Cmd.Parameters.Add(new System.Data.SqlClient.SqlParameter()
            {
                ParameterName = "newval",
                Value = newval
            });

            Cmd.Connection = sqlCon;
            sqlCon.Open();
            int result = Cmd.ExecuteNonQuery();
            sqlCon.Close();
        }


        public void updateImageURL(string qs, string newval)
        {
            XmlDocument games = new System.Xml.XmlDocument();
            System.Data.SqlClient.SqlConnection sqlCon =
                new System.Data.SqlClient.SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\Nuno\\Documents\\finaldb.mdf;Integrated Security=True;Connect Timeout=30");
            System.Data.SqlClient.SqlCommand Cmd = new System.Data.SqlClient.SqlCommand();
            Cmd.CommandText = "with xmlnamespaces ('http://ect2016/UserGamesSchema' as us) UPDATE UserGames SET xmlContent.modify('replace value of (/us:user[@id=sql:variable(\"@userid\")]/us:games/us:game[@filename=sql:variable(\"@rhash\")]/@imgurl)[1] with sql:variable(\"@newval\") ');";
            Cmd.Parameters.Add(new System.Data.SqlClient.SqlParameter()
            {
                ParameterName = "userid",
                Value = User.Identity.GetUserId()
            });
            Cmd.Parameters.Add(new System.Data.SqlClient.SqlParameter()
            {
                ParameterName = "rhash",
                Value = qs
            });

            Cmd.Parameters.Add(new System.Data.SqlClient.SqlParameter()
            {
                ParameterName = "newval",
                Value = newval
            });

            Cmd.Connection = sqlCon;
            sqlCon.Open();
            int result = Cmd.ExecuteNonQuery();
            sqlCon.Close();
        }




    }
}